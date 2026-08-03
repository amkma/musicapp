using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicApp.Web.Data;

namespace MusicApp.Web.Controllers;

public class PerfController : Controller
{
    private const int TargetRows = 2_000_000;
    private const int BatchSize = 5000;

    private readonly AppDbContext _context;

    public PerfController(AppDbContext context) => _context = context;

    // GET: /Perf
    public async Task<IActionResult> Index()
    {
        ViewBag.CurrentSongs = await _context.Songs.AsNoTracking().CountAsync();
        ViewBag.Singers = await _context.Singers.AsNoTracking().CountAsync();
        ViewBag.Categories = await _context.Categories.AsNoTracking().CountAsync();
        ViewBag.TargetRows = TargetRows;
        return View();
    }

    // POST: /Perf/Seed
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Seed()
    {
        var existing = await _context.Songs.AsNoTracking().CountAsync();
        if (existing >= TargetRows)
        {
            ViewBag.Message = $"Skipped: Songs table already has {existing:N0} rows (target {TargetRows:N0}).";
            return RedirectToAction(nameof(Index));
        }

        var singerIds = await _context.Singers.AsNoTracking()
            .Select(s => s.Id).ToListAsync();
        var categoryIds = await _context.Categories.AsNoTracking()
            .Select(c => c.Id).ToListAsync();

        if (singerIds.Count == 0 || categoryIds.Count == 0)
        {
            ViewBag.Message = "Cannot seed: no Singers or Categories found. Apply migrations and seed first.";
            return RedirectToAction(nameof(Index));
        }

        int rowsToInsert = TargetRows - existing;
        int batches = (int)Math.Ceiling(rowsToInsert / (double)BatchSize);

        var sw = Stopwatch.StartNew();

        // ponytail: disable change tracking for bulk insert, restore after
        _context.ChangeTracker.AutoDetectChangesEnabled = false;

        try
        {
            var rand = new Random(42);
            int songId = existing + 1;

            for (int b = 0; b < batches; b++)
            {
                int take = Math.Min(BatchSize, rowsToInsert - b * BatchSize);
                var buffer = new List<Models.Song>(take);

                for (int i = 0; i < take; i++)
                {
                    buffer.Add(new Models.Song
                    {
                        Title = $"Perf Song {songId}",
                        SingerId = singerIds[songId % singerIds.Count],
                        CategoryId = categoryIds[songId % categoryIds.Count]
                    });
                    songId++;
                }

                await _context.Songs.AddRangeAsync(buffer);
                await _context.SaveChangesAsync();
                _context.ChangeTracker.Clear();
            }
        }
        finally
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = true;
        }

        sw.Stop();
        TempData["SeedMs"] = sw.ElapsedMilliseconds;
        TempData["SeedRows"] = rowsToInsert;
        return RedirectToAction(nameof(Index));
    }
}
