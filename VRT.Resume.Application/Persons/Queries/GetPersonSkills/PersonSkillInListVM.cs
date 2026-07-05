namespace VRT.Resume.Application.Persons.Queries.GetPersonSkills
{
    public sealed class PersonSkillInListVM
    {
        public int SkillId { get; internal set; }
        public required string Type { get; set; }
        public required string Name { get; set; }
        public string? Level { get; internal set; }
    }
}