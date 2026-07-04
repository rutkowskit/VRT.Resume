using CSharpFunctionalExtensions;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application
{
    internal abstract class HandlerBase
    {
        private readonly ICurrentUserService _userService;

        protected HandlerBase(AppDbContext context, ICurrentUserService userService)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }
        protected AppDbContext Context { get; }

        protected Result<int> GetCurrentUserPersonId()
        {
            var accessor = Context.GetService<ICurrentPersonIdAccessor>();
            if (accessor is not null && accessor.TryGetPersonId(out var personId))
                return personId;

            var result = Context.UserPerson
                .Where(u => u.UserId == _userService.UserId)
                .Select(u => u.PersonId)
                .FirstOrDefault();

            return result <= 0
                ? Result.Failure<int>(Errors.UserUnauthorized)
                : result;
        }
    }
}
