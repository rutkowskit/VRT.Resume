using SqliteWasmBlazor;

namespace VRT.Resume.Pwa.Tests.Fixtures;

internal sealed class StubSqliteWasmDatabaseService : ISqliteWasmDatabaseService
{
    public Task<bool> ExistsDatabaseAsync(string databaseName, CancellationToken ct = default)
        => Task.FromResult(true);

    public Task DeleteDatabaseAsync(string databaseName, CancellationToken ct = default)
        => Task.CompletedTask;

    public Task RenameDatabaseAsync(string oldName, string newName, CancellationToken ct = default)
        => Task.CompletedTask;

    public Task CloseDatabaseAsync(string databaseName, CancellationToken ct = default)
        => Task.CompletedTask;

    public Task<DiskImportResult> ImportDatabaseAsync(string databaseName, byte[] data, CancellationToken ct = default)
        => Task.FromResult(new DiskImportResult());

    public Task<byte[]> ExportDatabaseAsync(string databaseName, CancellationToken ct = default)
        => Task.FromResult(Array.Empty<byte>());

    public Task<IReadOnlyList<string>> ListDatabasesAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<string>>([]);

    public Task<int> ImportRowsAsync(string databaseName, byte[] payload, CancellationToken ct = default)
        => Task.FromResult(0);

    public Task<byte[]> ExportAllDatabasesAsync(CancellationToken ct = default)
        => Task.FromResult(Array.Empty<byte>());

    public Task<DiskImportResult> ImportAllDatabasesAsync(byte[] data, CancellationToken ct = default)
        => Task.FromResult(new DiskImportResult());

    public Task<int> BulkImportAsync(
        string databaseName,
        byte[] payload,
        ConflictResolutionStrategy conflictStrategy = default,
        CancellationToken ct = default)
        => Task.FromResult(0);

    public Task<byte[]> BulkExportAsync(string databaseName, BulkExportMetadata metadata, CancellationToken ct = default)
        => Task.FromResult(Array.Empty<byte>());
}