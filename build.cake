var target = Argument("target", "ExecuteBuild");
var configuration = Argument("configuration", "Release");
var solutionFolder = "./"; //current directory
var outputFolder = "./deploy/release";
var webProjectFolder = "./VRT.Resume.Mvc";
var webOutputFolder = "./deploy/web";

Task("Clean")
    .Does(() => {
        CleanDirectory(outputFolder);
        CleanDirectory(webOutputFolder);
    });
Task("Restore")
    .Does(() => {
        DotNetCoreRestore(solutionFolder);
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() => {
        DotNetCoreBuild(solutionFolder, new DotNetCoreBuildSettings
        {
            NoRestore = true,
            Configuration = configuration
        });
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {
        DotNetCoreTest(solutionFolder,new DotNetCoreTestSettings
        {
            NoRestore = true,
            Configuration = configuration,
            NoBuild = true
        });
    });
Task("Publish")
    .IsDependentOn("Test")
    .Does(() => {
        DotNetCorePublish(webProjectFolder, new DotNetCorePublishSettings
        {
            NoRestore = true,
            Configuration = configuration,
            NoBuild = true,
            PublishTrimmed = true,
            OutputDirectory = outputFolder
        });
    });

Task("PublishWeb")
    .IsDependentOn("Test")
    .Does(() => {
        DotNetCorePublish(solutionFolder, new DotNetCorePublishSettings
        {
            NoRestore = true,            
            Configuration = configuration,
            NoBuild = true,
            OutputDirectory = webOutputFolder
        });
    });

Task("ExecuteBuild")
    //.IsDependentOn("Publish")
    .IsDependentOn("PublishWeb");

RunTarget(target);