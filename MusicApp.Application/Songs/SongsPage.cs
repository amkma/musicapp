using MusicApp.Domain.Entities;

namespace MusicApp.Application.Songs;

// ponytail: page DTO returned by GetSongsHandler; no IQueryable leaks to caller
public record SongsPage(
    IReadOnlyList<Song> Items,
    int TotalCount,
    int PageIndex,
    int PageSize,
    int FirstIdOnPage,
    int LastIdOnPage)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;
}
