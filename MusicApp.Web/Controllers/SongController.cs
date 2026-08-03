using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MusicApp.Web.Data;
using MusicApp.Web.Helpers;
using MusicApp.Web.Models;

namespace MusicApp.Web.Controllers;

public class SongController : Controller
{
    private const int PageSize = 100;
    private static readonly TimeSpan CountCacheTtl = TimeSpan.FromMinutes(5);
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;

    public SongController(AppDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    // GET: Song?singerName=&categoryId=&page=&lastId=&seek={next|prev}
    public async Task<IActionResult> Index(
        string? singerName, int? categoryId, int? page, int? lastId, int? firstId, string? seek)
    {
        ViewBag.SingerName = singerName;
        ViewBag.CategoryId = categoryId;

        ViewBag.Categories = new SelectList(
            await _context.Categories.AsNoTracking().ToListAsync(),
            "Id", "Name", categoryId);

        string cacheKey = $"songs_count:{singerName ?? ""}|{categoryId ?? 0}";

        PaginatedList<Song> paged;
        int lastSeenIdOnPage;
        int firstIdOnPage;

        // ponytail: keyset (seek) for sequential Next/Prev -> near-constant cost.
        // Direct page-number jumps fall back to OFFSET/FETCH (acceptable for first-page nav).
        if (seek == "next" && lastId is not null)
        {
            var baseQuery = BuildQuery(singerName, categoryId);
            var keyset = baseQuery.Where(s => s.Id > lastId.Value);
            int count = await GetCachedCountAsync(baseQuery, cacheKey);
            var items = await keyset.OrderBy(s => s.Id).Take(PageSize).ToListAsync();
            paged = new PaginatedList<Song>(items, count, (page ?? 1) + 1, PageSize);
            firstIdOnPage = items.Count > 0 ? items[0].Id : lastId.Value;
            lastSeenIdOnPage = items.Count > 0 ? items[^1].Id : lastId.Value;
        }
        else if (seek == "prev" && firstId is not null)
        {
            var baseQuery = BuildQuery(singerName, categoryId);
            var keyset = baseQuery.Where(s => s.Id < firstId.Value).OrderByDescending(s => s.Id);
            int count = await GetCachedCountAsync(baseQuery, cacheKey);
            var descItems = await keyset.Take(PageSize).ToListAsync();
            var items = descItems.OrderBy(s => s.Id).ToList();
            paged = new PaginatedList<Song>(items, count, Math.Max(1, (page ?? 2) - 1), PageSize);
            firstIdOnPage = items.Count > 0 ? items[0].Id : firstId.Value;
            lastSeenIdOnPage = items.Count > 0 ? items[^1].Id : firstId.Value;
        }
        else
        {
            int pageIndex = page is null or < 1 ? 1 : page.Value;
            paged = await PaginatedList<Song>.CreateAsync(
                BuildQuery(singerName, categoryId), s => s.Id, pageIndex, PageSize,
                _cache, cacheKey, CountCacheTtl);
            firstIdOnPage = paged.Items.Count > 0 ? paged.Items[0].Id : 0;
            lastSeenIdOnPage = paged.Items.Count > 0 ? paged.Items[^1].Id : 0;
        }

        ViewBag.LastId = lastSeenIdOnPage;
        ViewBag.FirstId = firstIdOnPage;
        ViewBag.Seek = seek;

        ViewBag.SingersOnPage = paged.Items
            .Select(s => s.Singer)
            .GroupBy(s => s.Id)
            .Select(g => g.First())
            .ToList();

        return View(paged);
    }

    private async Task<int> GetCachedCountAsync(IQueryable<Song> baseQuery, string cacheKey)
    {
        if (_cache.TryGetValue(cacheKey, out int cached) && cached > 0)
            return cached;

        int count = await baseQuery.CountAsync();
        if (count > 0) _cache.Set(cacheKey, count, CountCacheTtl);
        return count;
    }

    private IQueryable<Song> BuildQuery(string? singerName, int? categoryId)
    {
        IQueryable<Song> query = _context.Songs
            .Include(s => s.Singer)
            .Include(s => s.Category)
            .AsNoTracking();

        // ponytail: filters chained via LINQ method syntax, both apply together
        if (!string.IsNullOrWhiteSpace(singerName))
            query = query.Where(s => s.Singer.Name.Contains(singerName));

        if (categoryId is not null)
            query = query.Where(s => s.CategoryId == categoryId);

        return query;
    }
}
