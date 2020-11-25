using CSharpFunctionalExtensions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application
{
    /// <summary>
    /// Template method for standard Delete handlers
    /// </summary>
    /// <typeparam name="TCommand">Command type parameter</typeparam>
    /// <typeparam name="TDomainModel">Domain model type parameter</typeparam>
    internal abstract class DeleteHandlerBase<TCommand, TDomainModel> : HandlerBase, IRequestHandler<TCommand, Result>
        where TCommand: IRequest<Result>
        where TDomainModel: class, new()
    {
        protected DeleteHandlerBase(AppDbContext context, ICurrentUserService userService)
            : base(context, userService)
        {
        }

        public async Task<Result> Handle(TCommand request, CancellationToken cancellationToken)
        {
            return await GetExistingData(request)                
                .Map(d => Remove(d))
                .Map(i => Context.SaveChangesAsync());
        }        
        protected abstract Result<TDomainModel> GetExistingData(TCommand request);                
        protected virtual Result Remove(TDomainModel entity)
        {
            Context.Remove(entity);
            return Result.Success();
        }
    }
}
