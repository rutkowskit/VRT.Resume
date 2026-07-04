namespace VRT.Resume.Application;

internal abstract class HandlerBase
{
    private readonly ICurrentUserService _userService;

    protected HandlerBase(AppDbContext context, ICurrentUserService userService)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    protected AppDbContext Context { get; }

    protected async Task<Result<int>> GetCurrentUserPersonId()
    {
        var result = await Context.UserPerson
            .Where(u => u.UserId == _userService.UserId)
            .Select(u => u.PersonId)
            .FirstOrDefaultAsync();

        return result <= 0
            ? Result.Failure<int>(Errors.UserUnauthorized)
            : result;
    }

    protected async Task<Result<int>> GetCurrentUserPersonIdAsync(CancellationToken cancellationToken = default)
    {
        var id = await Context.UserPerson
            .AsNoTracking()
            .Where(u => u.UserId == _userService.UserId)
            .Select(u => u.PersonId)
            .FirstOrDefaultAsync(cancellationToken);

        return id <= 0 ? Result.Failure<int>(Errors.UserUnauthorized) : id;
    }
}