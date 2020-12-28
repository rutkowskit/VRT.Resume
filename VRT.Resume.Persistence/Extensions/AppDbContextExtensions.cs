using System;
using System.Linq;
using VRT.Resume.Domain.Common;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Persistence
{
    public static class AppDbContextExtensions
    {
        public static void InitDatabase(this AppDbContext context)
        {
            if (context == null) return;
            if (!context.Database.EnsureCreated())
                return;
            context.SeedSkillTypes();
            context.SaveChanges();
        }
        private static AppDbContext SeedSkillTypes(this AppDbContext ctx)
        {
            ctx.SkillType.AddRange(Enum.GetNames(typeof(SkillTypes))
                .Select(skillName => AsSkillType(skillName))
                .Where(w => w != null)
            );
            return ctx;
        }

        private static Domain.Entities.SkillType AsSkillType(string skillName)
        {
            return Enum.TryParse<SkillTypes>(skillName, out var skillType)
                ? new Domain.Entities.SkillType()
                {
                    SkillTypeId = (byte)skillType,
                    Name = skillName
                }
                : null;
        }
    }
}
