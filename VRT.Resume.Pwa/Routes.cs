namespace VRT.Resume.Pwa;

internal static class Routes
{
    public const string Home = "/";
    public const string About = "/about";
    public const string NotFound = "/not-found";

    internal static class Profiles
    {
        public const string List = "/profiles";
        public const string Create = "/profiles/create";
    }

    internal static class Persons
    {
        public const string Person = "/person";
    }

    internal static class Resumes
    {
        public const string Show = "/resumes/show/{ResumeId:int}";
    }
}