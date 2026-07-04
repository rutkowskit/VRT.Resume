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
}