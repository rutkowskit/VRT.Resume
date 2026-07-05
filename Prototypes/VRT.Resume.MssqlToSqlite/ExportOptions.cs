namespace VRT.Resume.MssqlToSqlite;

public sealed class ExportOptions
{
    public required string MssqlConnectionString { get; init; }

    public required string SqlitePath { get; init; }

    /// <summary>When set, exports a single person graph (plus referenced lookups).</summary>
    public int? PersonId { get; init; }

    public bool OverwriteSqlite { get; init; } = true;
}