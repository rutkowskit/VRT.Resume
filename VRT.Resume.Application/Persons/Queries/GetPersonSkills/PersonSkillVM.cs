using VRT.Resume.Domain.Common;

namespace VRT.Resume.Application.Persons.Queries.GetPersonSkills
{
    public sealed class PersonSkillVM
    {
        public int SkillId { get; set; }        
        public SkillTypes Type 
        { 
            get=>(SkillTypes) SkillTypeId; 
            set=>SkillTypeId=(byte)value; 
        }        
        public string Name { get; set; }        
        public string Level { get; set; }
        internal byte SkillTypeId { get; set; }
    }
}
