using Microsoft.Extensions.Configuration;
using VRT.Resume.MssqlToSqlite;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

if (!TryParseArgs(args, out var mssql, out var sqlite, out var personId, out var keepSqlite))
{
    PrintUsage();
    return 1;
}

mssql ??= configuration.GetConnectionString("ResumeData:mssql")
    ?? configuration["ConnectionStrings:ResumeData:mssql"];
sqlite ??= configuration["Export:SqlitePath"];

if (string.IsNullOrWhiteSpace(mssql) || string.IsNullOrWhiteSpace(sqlite))
{
    Console.Error.WriteLine("Missing SQL Server connection string or SQLite path.");
    PrintUsage();
    return 1;
}

if (!personId.HasValue && int.TryParse(configuration["Export:PersonId"], out var configuredPersonId))
    personId = configuredPersonId;

var options = new ExportOptions
{
    MssqlConnectionString = mssql,
    SqlitePath = sqlite,
    PersonId = personId,
    OverwriteSqlite = !keepSqlite,
};

try
{
    await new MssqlToSqliteExporter().ExportAsync(options);
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex.Message);
    return 1;
}

static bool TryParseArgs(
    string[] args,
    out string? mssql,
    out string? sqlite,
    out int? personId,
    out bool keepSqlite)
{
    mssql = null;
    sqlite = null;
    personId = null;
    keepSqlite = false;

    for (var i = 0; i < args.Length; i++)
    {
        switch (args[i])
        {
            case "--mssql" when i + 1 < args.Length:
                mssql = args[++i];
                break;
            case "--sqlite" when i + 1 < args.Length:
                sqlite = args[++i];
                break;
            case "--person-id" when i + 1 < args.Length && int.TryParse(args[++i], out var id):
                personId = id;
                break;
            case "--keep-sqlite":
                keepSqlite = true;
                break;
            case "--help" or "-h" or "/?":
                return false;
        }
    }

    return true;
}

static void PrintUsage()
{
    Console.WriteLine("""
        VRT.Resume MSSQL → SQLite exporter (AppDbContext schema)

        Usage:
          dotnet run --project Prototypes/VRT.Resume.MssqlToSqlite -- [options]

        Options:
          --mssql <connectionString>   SQL Server source (or appsettings / env)
          --sqlite <filePath>          SQLite target file path
          --person-id <id>             Export one person only (default: all persons)
          --keep-sqlite                Do not delete an existing SQLite file before export
          -h, --help                   Show this help

        Examples:
          dotnet run --project Prototypes/VRT.Resume.MssqlToSqlite
          dotnet run --project Prototypes/VRT.Resume.MssqlToSqlite -- --person-id 3 --sqlite C:\temp\cv.sqlite
        """);
}