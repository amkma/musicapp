using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicApp.Web.Data;
using MusicApp.Web.Models;

namespace MusicApp.Web.Controllers;

public class SingerController : Controller
{
    private readonly AppDbContext _context;

    public SingerController(AppDbContext context) => _context = context;

    // GET: Singer
    public async Task<IActionResult> Index() =>
        View(await _context.Singers.ToListAsync());

    // GET: Singer/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();

        var singer = await _context.Singers
            .Include(s => s.Songs)
            .ThenInclude(s => s.Category)
            .FirstOrDefaultAsync(m => m.Id == id);

        return singer is null ? NotFound() : View(singer);
    }

    // GET: Singer/Create
    public IActionResult Create() => View();

    // POST: Singer/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,ImageUrl")] Singer singer)
    {
        if (ModelState.IsValid)
        {
            _context.Add(singer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(singer);
    }

    // GET: Singer/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();

        var singer = await _context.Singers.FindAsync(id);
        return singer is null ? NotFound() : View(singer);
    }

    // POST: Singer/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ImageUrl")] Singer singer)
    {
        if (id != singer.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(singer);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!SingerExists(singer.Id))
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
        return View(singer);
    }

    // GET: Singer/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();

        var singer = await _context.Singers.FirstOrDefaultAsync(m => m.Id == id);
        return singer is null ? NotFound() : View(singer);
    }

    // POST: Singer/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var singer = await _context.Singers.FindAsync(id);
        if (singer is not null) _context.Singers.Remove(singer);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool SingerExists(int id) =>
        _context.Singers.Any(e => e.Id == id);
}
