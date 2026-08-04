using MusicApp.Domain.Entities;
using MusicApp.Application.Interfaces;

namespace MusicApp.Application.Seed;

public record SeedResult(int InsertedRows, int ElapsedMs);

public class GeneratePerfSongsHandler(ISongRepository songRepo, ISingerRepository singerRepo, ICategoryRepository categoryRepo)
{
    private const int TargetRows = 2_000_000;
    private const int BatchSize = 5000;

    public async Task<(int Inserted, int ElapsedMs, string? Error)> HandleAsync(CancellationToken ct = default)
    {
        int existing = await songRepo.CountAsync(null, null, ct);
        if (existing >= TargetRows)
            return (0, 0, $"Skipped: Songs table already has {existing:N0} rows (target {TargetRows:N0}).");

        var singerIds = (await singerRepo.GetPageAsync(1, int.MaxValue, ct)).Select(s => s.Id).ToList();
        var categoryIds = (await categoryRepo.GetAllAsync(ct)).Select(c => c.Id).ToList();
        if (singerIds.Count == 0 || categoryIds.Count == 0)
            return (0, 0, "Cannot seed: no Singers or Categories found.");

        int rowsToInsert = TargetRows - existing;
        int batches = (int)Math.Ceiling(rowsToInsert / (double)BatchSize);
        var sw = System.Diagnostics.Stopwatch.StartNew();

        int songId = existing + 1;
        for (int b = 0; b < batches; b++)
        {
            int take = Math.Min(BatchSize, rowsToInsert - b * BatchSize);
            var buffer = new List<Song>(take);
            for (int i = 0; i < take; i++)
            {
                buffer.Add(new Song
                {
                    Title = $"Perf Song {songId}",
                    SingerId = singerIds[songId % singerIds.Count],
                    CategoryId = categoryIds[songId % categoryIds.Count]
                });
                songId++;
            }
            await songRepo.AddRangeAsync(buffer, ct);
            await songRepo.SaveChangesAsync(ct);
        }

        sw.Stop();
        return (rowsToInsert, (int)sw.ElapsedMilliseconds, null);
    }
}
