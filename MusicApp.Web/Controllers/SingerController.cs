using Microsoft.AspNetCore.Mvc;
using MusicApp.Application.Singers;
using MusicApp.Domain.Entities;

namespace MusicApp.Web.Controllers;

public class SingerController(SingerHandlers handlers) : Controller
{
    private const int PageSize = 100;

    // GET: Singer
    public async Task<IActionResult> Index(int page = 1, CancellationToken ct = default)
    {
        var paged = await handlers.ListAsync(page, PageSize, ct);
        return View(paged);
    }

    // GET: Singer/Details/5
    public async Task<IActionResult> Details(int? id, CancellationToken ct = default)
    {
        if (id is null) return NotFound();
        var singer = await handlers.DetailsAsync(id.Value, ct);
        return singer is null ? NotFound() : View(singer);
    }

    // GET: Singer/Create
    public IActionResult Create() => View();

    // POST: Singer/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,ImageUrl")] Singer singer, CancellationToken ct = default)
    {
        if (!ModelState.IsValid) return View(singer);
        await handlers.CreateAsync(singer, ct);
        return RedirectToAction(nameof(Index));
    }

    // GET: Singer/Edit/5
    public async Task<IActionResult> Edit(int? id, CancellationToken ct = default)
    {
        if (id is null) return NotFound();
        var singer = await handlers.DetailsAsync(id.Value, ct);
        return singer is null ? NotFound() : View(singer);
    }

    // POST: Singer/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ImageUrl")] Singer singer, CancellationToken ct = default)
    {
        if (id != singer.Id) return NotFound();
        if (!ModelState.IsValid) return View(singer);

        var ok = await handlers.EditAsync(singer, ct);
        return ok ? RedirectToAction(nameof(Index)) : NotFound();
    }

    // GET: Singer/Delete/5
    public async Task<IActionResult> Delete(int? id, CancellationToken ct = default)
    {
        if (id is null) return NotFound();
        var singer = await handlers.DetailsAsync(id.Value, ct);
        return singer is null ? NotFound() : View(singer);
    }

    // POST: Singer/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct = default)
    {
        var ok = await handlers.DeleteAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }
}
