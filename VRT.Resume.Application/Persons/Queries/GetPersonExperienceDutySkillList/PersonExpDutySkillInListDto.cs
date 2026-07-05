
namespace VRT.Resume.Application.Persons.Queries.GetPersonExperienceDutySkillList
{
    public sealed class PersonExpDutySkillInListDto
    {
        public int SkillId { get; internal set; }
        public bool IsRelevant { get; internal set; }        
        public required string Name { get; set; }
        public required string Type { get; set; }
    }
}