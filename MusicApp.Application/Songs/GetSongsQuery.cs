namespace MusicApp.Application.Songs;

public record GetSongsQuery(
    string? SingerName,
    int? CategoryId,
    int? Page,
    int? LastId,
    int? FirstId,
    string? Seek);
