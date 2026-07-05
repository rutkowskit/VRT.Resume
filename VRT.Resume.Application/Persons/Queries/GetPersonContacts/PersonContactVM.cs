namespace VRT.Resume.Application.Persons.Queries.GetPersonContacts
{
    public sealed class PersonContactVM
    {
        public int ContactId { get; internal set; }
        public required string Name { get; set; }
        public required string Value { get; set; }
        public string? Icon { get; internal set; }
        public string? Url { get; internal set; }
    }
}