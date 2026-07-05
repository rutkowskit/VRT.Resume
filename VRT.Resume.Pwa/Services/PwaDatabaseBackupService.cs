using Microsoft.JSInterop;
using SqliteWasmBlazor;

namespace VRT.Resume.Pwa.Services;

public sealed class PwaDatabaseBackupService(
    ISqliteWasmDatabaseService databaseService,
    IJSRuntime jsRuntime)
{
    private const string SqliteHeader = "SQLite format 3\0";

    public async Task ExportAsync(CancellationToken cancellationToken = default)
    {
        var data = await databaseService.ExportDatabaseAsync(PwaDatabaseNames.FileName, cancellationToken);
        var fileName = $"vrt-resume-{DateTime.UtcNow:yyyy-MM-dd}.db";
        var base64 = Convert.ToBase64String(data);
        await jsRuntime.InvokeVoidAsync("pwaDbBackup.downloadFile", cancellationToken, fileName, base64);
    }

    public async Task ImportAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        if (!IsValidSqliteFile(data))
            throw new InvalidDataException("The selected file is not a valid SQLite database.");

        await databaseService.ImportDatabaseAsync(PwaDatabaseNames.FileName, data, cancellationToken);
    }

    internal static bool IsValidSqliteFile(byte[] data)
    {
        if (data.Length < SqliteHeader.Length)
            return false;

        return System.Text.Encoding.ASCII.GetString(data, 0, SqliteHeader.Length) == SqliteHeader;
    }
}