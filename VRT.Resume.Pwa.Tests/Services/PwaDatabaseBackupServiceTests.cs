using System.Text;
using FluentAssertions;
using VRT.Resume.Pwa.Services;

namespace VRT.Resume.Pwa.Tests.Services;

public sealed class PwaDatabaseBackupServiceTests
{
    [Fact]
    public void IsValidSqliteFile_ReturnsTrue_ForSqliteHeader()
    {
        var data = Encoding.ASCII.GetBytes("SQLite format 3\0" + new string('x', 100));
        PwaDatabaseBackupService.IsValidSqliteFile(data).Should().BeTrue();
    }

    [Fact]
    public void IsValidSqliteFile_ReturnsFalse_ForInvalidHeader()
    {
        var data = Encoding.ASCII.GetBytes("not sqlite");
        PwaDatabaseBackupService.IsValidSqliteFile(data).Should().BeFalse();
    }

    [Fact]
    public void IsValidSqliteFile_ReturnsFalse_ForEmptyFile()
    {
        PwaDatabaseBackupService.IsValidSqliteFile([]).Should().BeFalse();
    }
}