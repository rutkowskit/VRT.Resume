namespace VRT.Resume.Application.Persons.Queries.GetPersonContacts
{
    public sealed class PersonContactInListVM
    {
        public int ContactId { get; internal set; }
        public required string Name { get; set; }
        public required string Value { get; set; }
    }
}