using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MusicApp.Application.Categories;
using MusicApp.Application.Songs;
using MusicApp.Domain.Entities;

namespace MusicApp.Web.Controllers;

public class SongController(GetSongsHandler songsHandler, CategoryHandlers categoriesHandler) : Controller
{
    // GET: Song?singerName=&categoryId=&page=&lastId=&seek={next|prev}
    public async Task<IActionResult> Index(GetSongsQuery query, CancellationToken ct)
    {
        ViewBag.SingerName = query.SingerName;
        ViewBag.CategoryId = query.CategoryId;

        var categories = await categoriesHandler.ListAsync(ct);
        ViewBag.Categories = new SelectList(categories, "Id", "Name", query.CategoryId);

        var page = await songsHandler.HandleAsync(query, ct);

        ViewBag.LastId = page.LastIdOnPage;
        ViewBag.FirstId = page.FirstIdOnPage;
        ViewBag.Seek = query.Seek;

        // ponytail: distinct singers on the current page feed the modal partial
        ViewBag.SingersOnPage = page.Items
            .Select(s => s.Singer)
            .GroupBy(s => s.Id)
            .Select(g => g.First())
            .ToList();

        return View(page);
    }
}
