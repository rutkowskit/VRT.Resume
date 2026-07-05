using Microsoft.EntityFrameworkCore;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Pwa.Services;

public sealed class LocalProfileService(AppDbContext context)
{
    public async Task<IReadOnlyList<LocalProfileDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.UserPerson
            .AsNoTracking()
            .Include(u => u.Person)
            .OrderBy(u => u.Person.LastName)
            .ThenBy(u => u.Person.FirstName)
            .Select(u => new LocalProfileDto(
                u.UserId,
                u.Person.FirstName,
                u.Person.LastName))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var userPerson = await context.UserPerson
            .Include(u => u.Person)
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);

        if (userPerson is null)
            return false;

        var personId = userPerson.PersonId;

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var experienceIds = context.PersonExperience
                .Where(e => e.PersonId == personId)
                .Select(e => e.ExperienceId);

            var dutyIds = context.PersonExperienceDuty
                .Where(d => experienceIds.Contains(d.ExperienceId))
                .Select(d => d.DutyId);

            await context.PersonExperienceDutySkill
                .Where(ds => dutyIds.Contains(ds.DutyId))
                .ExecuteDeleteAsync(cancellationToken);

            await context.PersonExperienceDuty
                .Where(d => dutyIds.Contains(d.DutyId))
                .ExecuteDeleteAsync(cancellationToken);

            await context.PersonExperience
                .Where(e => e.PersonId == personId)
                .ExecuteDeleteAsync(cancellationToken);

            var resumeIds = context.PersonResume
                .Where(r => r.PersonId == personId)
                .Select(r => r.ResumeId);

            await context.ResumePersonSkill
                .Where(rs => resumeIds.Contains(rs.ResumeId))
                .ExecuteDeleteAsync(cancellationToken);

            await context.PersonResume
                .Where(r => r.PersonId == personId)
                .ExecuteDeleteAsync(cancellationToken);

            await context.PersonEducation
                .Where(e => e.PersonId == personId)
                .ExecuteDeleteAsync(cancellationToken);

            await context.PersonContact
                .Where(c => c.PersonId == personId)
                .ExecuteDeleteAsync(cancellationToken);

            await context.PersonSkill
                .Where(s => s.PersonId == personId)
                .ExecuteDeleteAsync(cancellationToken);

            await context.PersonImage
                .Where(i => i.PersonId == personId)
                .ExecuteDeleteAsync(cancellationToken);

            context.UserPerson.Remove(userPerson);
            context.Person.Remove(userPerson.Person);

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        return true;
    }
}