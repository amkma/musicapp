using Microsoft.AspNetCore.Mvc;
using MusicApp.Application.Songs;
using MusicApp.Application.Seed;

namespace MusicApp.Web.Controllers;

public class PerfController(GeneratePerfSongsHandler seeder, GetSongsHandler songsHandler) : Controller
{
    private const int TargetRows = 2_000_000;

    // GET: /Perf
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        // ponytail: counts via handler paths so Web stays free of DbContext
        var page = await songsHandler.HandleAsync(new GetSongsQuery(null, null, 1, null, null, null), ct);
        ViewBag.CurrentSongs = page.TotalCount;

        // counts are derived from page metadata handlers already produce
        ViewBag.TargetRows = TargetRows;
        return View();
    }

    // POST: /Perf/Seed
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Seed(CancellationToken ct)
    {
        var (inserted, elapsedMs, error) = await seeder.HandleAsync(ct);

        if (error is not null)
        {
            TempData["SeedError"] = error;
        }
        else
        {
            TempData["SeedMs"] = elapsedMs;
            TempData["SeedRows"] = inserted;
        }

        return RedirectToAction(nameof(Index));
    }
}
