namespace VRT.Resume.Application.Persons.Queries.GetPersonContacts
{
    public sealed class PersonContactVM
    {
        public int ContactId { get; internal set; }
        public string Name { get; internal set; }
        public string Value { get; internal set; }
        public string Icon { get; internal set; }
        public string Url { get; internal set; }
    }
}
