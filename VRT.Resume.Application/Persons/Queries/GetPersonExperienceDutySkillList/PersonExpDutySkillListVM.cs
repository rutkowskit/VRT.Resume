
namespace VRT.Resume.Application.Persons.Queries.GetPersonExperienceDutySkillList
{
    public sealed class PersonExpDutySkillListVM
    {
        public int DutyId { get; internal set; }
        public PersonExpDutySkillInListDto[] DutySkills { get; internal set; }
    }
}