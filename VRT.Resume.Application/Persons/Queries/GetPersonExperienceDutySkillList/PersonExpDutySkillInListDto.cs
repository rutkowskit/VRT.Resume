
namespace VRT.Resume.Application.Persons.Queries.GetPersonExperienceDutySkillList
{
    public sealed class PersonExpDutySkillInListDto
    {
        public int SkillId { get; internal set; }
        public bool IsRelevent { get; internal set; }        
        public string Name { get; internal set; }
        public string Type { get; internal set; }
    }
}