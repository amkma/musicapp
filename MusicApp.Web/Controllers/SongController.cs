using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicApp.Web.Data;
using MusicApp.Web.Helpers;
using MusicApp.Web.Models;

namespace MusicApp.Web.Controllers;

public class SongController : Controller
{
    private const int PageSize = 5;
    private readonly AppDbContext _context;

    public SongController(AppDbContext context) => _context = context;

    // GET: Song?SingerName=&CategoryId=&page=
    public async Task<IActionResult> Index(string? singerName, int? categoryId, int? page)
    {
        ViewBag.SingerName = singerName;
        ViewBag.CategoryId = categoryId;

        ViewBag.Categories = new SelectList(
            await _context.Categories.AsNoTracking().ToListAsync(),
            "Id", "Name", categoryId);

        IQueryable<Song> query = _context.Songs
            .Include(s => s.Singer)
            .Include(s => s.Category)
            .AsNoTracking();

        // ponytail: filters chained via LINQ method syntax, both apply together
        if (!string.IsNullOrWhiteSpace(singerName))
            query = query.Where(s => s.Singer.Name.Contains(singerName));

        if (categoryId is not null)
            query = query.Where(s => s.CategoryId == categoryId);

        int pageIndex = page is null or < 1 ? 1 : page.Value;

        var paged = await PaginatedList<Song>.CreateAsync(query, pageIndex, PageSize);

        return View(paged);
    }
}
