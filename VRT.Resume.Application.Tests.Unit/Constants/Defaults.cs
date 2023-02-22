using System;

namespace VRT.Resume.Application;

internal static class Defaults
{        
    internal static readonly DateTime Today = new(2020, 2, 3);
    internal static readonly string UserId = "tester@testing.me";
    internal const string TestDbConnectionString = "Server=(localdb)\\mssqllocaldb;Database=Test.VRT.Resume;Trusted_Connection=True;MultipleActiveResultSets=true;";
    internal const string TestError = "Test Error Occurred";
}
