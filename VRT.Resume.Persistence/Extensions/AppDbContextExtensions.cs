using Microsoft.EntityFrameworkCore;
using VRT.Resume.Domain.Common;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Persistence
{
    public static class AppDbContextExtensions
    {
        public static void InitDatabase(this AppDbContext context)
        {
            if (context == null) return;
            context.Database.EnsureCreated();
            if (context.SkillType.Any())
                return;
            context.SeedSkillTypes();
            context.SaveChanges();
        }

        public static async Task InitDatabaseAsync(this AppDbContext context, CancellationToken cancellationToken = default)
        {
            if (context == null) return;
            await context.Database.EnsureCreatedAsync(cancellationToken);
            if (await context.SkillType.AnyAsync(cancellationToken))
                return;
            context.SeedSkillTypes();
            await context.SaveChangesAsync(cancellationToken);
        }
        private static AppDbContext SeedSkillTypes(this AppDbContext ctx)
        {
            ctx.SkillType.AddRange(Enum.GetNames(typeof(SkillTypes))
                .Select(AsSkillType)
                .Where(w => w != null)
                .Select(w => w!)
            );
            return ctx;
        }

        private static Domain.Entities.SkillType? AsSkillType(string skillName)
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
