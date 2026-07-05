using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.MssqlToSqlite;

public sealed class MssqlToSqliteExporter
{
    public async Task ExportAsync(ExportOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(options.MssqlConnectionString);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.SqlitePath);

        var sqlitePath = Path.GetFullPath(options.SqlitePath);
        Directory.CreateDirectory(Path.GetDirectoryName(sqlitePath)!);

        if (options.OverwriteSqlite && File.Exists(sqlitePath))
        {
            File.Delete(sqlitePath);
            Console.WriteLine($"Removed existing SQLite file: {sqlitePath}");
        }

        await using var source = CreateContext(options.MssqlConnectionString, sqlServer: true);
        await using var target = CreateContext(BuildSqliteConnectionString(sqlitePath), sqlServer: false);

        var personIds = await ResolvePersonIdsAsync(source, options.PersonId, cancellationToken);
        if (personIds.Count == 0)
        {
            Console.WriteLine("No persons to export.");
            return;
        }

        Console.WriteLine(options.PersonId.HasValue
            ? $"Exporting person {options.PersonId.Value} to {sqlitePath}"
            : $"Exporting {personIds.Count} person(s) to {sqlitePath}");

        await target.Database.EnsureCreatedAsync(cancellationToken);
        await target.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = OFF;", cancellationToken);

        try
        {
            await CopyLookupsAsync(source, target, personIds, cancellationToken);
            await CopyPersonGraphAsync(source, target, personIds, cancellationToken);
        }
        finally
        {
            await target.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = ON;", cancellationToken);
        }

        Console.WriteLine("Export completed.");
    }

    private static AppDbContext CreateContext(string connectionString, bool sqlServer)
    {
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        if (sqlServer)
        {
            builder.UseSqlServer(connectionString, cfg =>
            {
                cfg.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                cfg.CommandTimeout(120);
            });
        }
        else
        {
            builder.ConfigureWarnings(w => w.Ignore(SqliteEventId.SchemaConfiguredWarning));
            builder.UseSqlite(connectionString);
        }

        return new AppDbContext(builder.Options);
    }

    private static string BuildSqliteConnectionString(string path)
    {
        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = path,
            Mode = SqliteOpenMode.ReadWriteCreate,
        };
        return builder.ConnectionString;
    }

    private static async Task<IReadOnlyList<int>> ResolvePersonIdsAsync(
        AppDbContext source,
        int? personId,
        CancellationToken cancellationToken)
    {
        if (personId.HasValue)
        {
            var exists = await source.Person
                .AsNoTracking()
                .AnyAsync(p => p.PersonId == personId.Value, cancellationToken);
            if (!exists)
                throw new InvalidOperationException($"Person {personId.Value} was not found in the source database.");

            return [personId.Value];
        }

        return await source.Person
            .AsNoTracking()
            .OrderBy(p => p.PersonId)
            .Select(p => p.PersonId)
            .ToListAsync(cancellationToken);
    }

    private static async Task CopyLookupsAsync(
        AppDbContext source,
        AppDbContext target,
        IReadOnlyList<int> personIds,
        CancellationToken cancellationToken)
    {
        await CopyAllAsync(target, source.SkillType, target.SkillType, cancellationToken);

        var educationQuery = source.PersonEducation.AsNoTracking()
            .Where(e => personIds.Contains(e.PersonId));

        var degreeIds = await educationQuery
            .Where(e => e.DegreeId != null)
            .Select(e => e.DegreeId!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);
        var fieldIds = await educationQuery
            .Where(e => e.EducationFieldId != null)
            .Select(e => e.EducationFieldId!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);
        var schoolIds = await educationQuery.Select(e => e.SchoolId).Distinct().ToListAsync(cancellationToken);

        await CopyWhereAsync(target, source.Degree, target.Degree, d => degreeIds.Contains(d.DegreeId), cancellationToken);
        await CopyWhereAsync(target, source.EducationField, target.EducationField, f => fieldIds.Contains(f.EducationFieldId), cancellationToken);
        await CopyWhereAsync(target, source.School, target.School, s => schoolIds.Contains(s.SchoolId), cancellationToken);
    }

    private static async Task CopyPersonGraphAsync(
        AppDbContext source,
        AppDbContext target,
        IReadOnlyList<int> personIds,
        CancellationToken cancellationToken)
    {
        await CopyWhereAsync(target, source.Person, target.Person, p => personIds.Contains(p.PersonId), cancellationToken);
        await CopyWhereAsync(target, source.UserPerson, target.UserPerson, u => personIds.Contains(u.PersonId), cancellationToken);
        await CopyWhereAsync(target, source.PersonContact, target.PersonContact, c => personIds.Contains(c.PersonId), cancellationToken);
        await CopyWhereAsync(target, source.PersonEducation, target.PersonEducation, e => personIds.Contains(e.PersonId), cancellationToken);
        await CopyWhereAsync(target, source.PersonImage, target.PersonImage, i => personIds.Contains(i.PersonId), cancellationToken);
        await CopyWhereAsync(target, source.PersonSkill, target.PersonSkill, s => personIds.Contains(s.PersonId), cancellationToken);
        await CopyWhereAsync(target, source.PersonResume, target.PersonResume, r => personIds.Contains(r.PersonId), cancellationToken);

        var experienceIds = await source.PersonExperience.AsNoTracking()
            .Where(e => personIds.Contains(e.PersonId))
            .Select(e => e.ExperienceId)
            .ToListAsync(cancellationToken);
        await CopyWhereAsync(target, source.PersonExperience, target.PersonExperience, e => experienceIds.Contains(e.ExperienceId), cancellationToken);

        var dutyIds = await source.PersonExperienceDuty.AsNoTracking()
            .Where(d => experienceIds.Contains(d.ExperienceId))
            .Select(d => d.DutyId)
            .ToListAsync(cancellationToken);
        await CopyWhereAsync(target, source.PersonExperienceDuty, target.PersonExperienceDuty, d => dutyIds.Contains(d.DutyId), cancellationToken);

        var skillIds = await source.PersonSkill.AsNoTracking()
            .Where(s => personIds.Contains(s.PersonId))
            .Select(s => s.SkillId)
            .ToListAsync(cancellationToken);
        await CopyWhereAsync(
            target,
            source.PersonExperienceDutySkill,
            target.PersonExperienceDutySkill,
            ds => dutyIds.Contains(ds.DutyId) && skillIds.Contains(ds.SkillId),
            cancellationToken);

        var resumeIds = await source.PersonResume.AsNoTracking()
            .Where(r => personIds.Contains(r.PersonId))
            .Select(r => r.ResumeId)
            .ToListAsync(cancellationToken);
        await CopyWhereAsync(
            target,
            source.ResumePersonSkill,
            target.ResumePersonSkill,
            rs => resumeIds.Contains(rs.ResumeId) && skillIds.Contains(rs.SkillId),
            cancellationToken);
    }

    private static async Task CopyAllAsync<TEntity>(
        AppDbContext target,
        DbSet<TEntity> sourceSet,
        DbSet<TEntity> targetSet,
        CancellationToken cancellationToken)
        where TEntity : class
    {
        var rows = await sourceSet.AsNoTracking().ToListAsync(cancellationToken);
        await InsertAsync(target, targetSet, rows, typeof(TEntity).Name, cancellationToken);
    }

    private static async Task CopyWhereAsync<TEntity>(
        AppDbContext target,
        DbSet<TEntity> sourceSet,
        DbSet<TEntity> targetSet,
        System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken)
        where TEntity : class
    {
        var rows = await sourceSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
        await InsertAsync(target, targetSet, rows, typeof(TEntity).Name, cancellationToken);
    }

    private static async Task InsertAsync<TEntity>(
        AppDbContext target,
        DbSet<TEntity> targetSet,
        List<TEntity> rows,
        string label,
        CancellationToken cancellationToken)
        where TEntity : class
    {
        if (rows.Count == 0)
        {
            Console.WriteLine($"  {label}: 0");
            return;
        }

        await targetSet.AddRangeAsync(rows, cancellationToken);
        await target.SaveChangesAsync(cancellationToken);
        target.ChangeTracker.Clear();
        Console.WriteLine($"  {label}: {rows.Count}");
    }
}