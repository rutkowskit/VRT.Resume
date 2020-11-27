using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Application.Common.Services;
using VRT.Resume.Domain;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application
{
    /// <summary>
    /// Template method for standard upsert handlers
    /// </summary>
    /// <typeparam name="TCommand">Command type parameter</typeparam>
    /// <typeparam name="TDomainModel">Domain model type parameter</typeparam>
    internal abstract class UpsertHandlerBase<TCommand, TDomainModel> : HandlerBase, IRequestHandler<TCommand, Result>
        where TCommand: IRequest<Result>
        where TDomainModel: class, new()
    {
        protected IDateTimeService DateService { get; }

        protected UpsertHandlerBase(AppDbContext context,
            ICurrentUserService userService)
            : this (context, userService, new DateTimeService())
        {            
        }

        protected UpsertHandlerBase(AppDbContext context, 
            ICurrentUserService userService, 
            IDateTimeService dateService)
            : base(context, userService)
        {
            DateService = dateService;
        }

        public async Task<Result> Handle(TCommand request, CancellationToken cancellationToken)
        {
            return await GetExistingData(request)
                .OnFailureCompensate(() => CreateNewData(request).Tap(i => Context.Add(i)))
                .Bind(i => UpdateData(i, request))
                .Map(i => Context.SaveChangesAsync());
        }

        protected abstract Result<TDomainModel> UpdateData(TDomainModel current, TCommand request);
        protected abstract Result<TDomainModel> GetExistingData(TCommand request);
        protected virtual Result<TDomainModel> CreateNewData(TCommand request)
        {
            return GetCurrentUserPersonId()
                .Map(s => 
                {
                    var result = new TDomainModel();
                    if (result is IPersonEntity person)
                        person.PersonId = s;
                    return result;
                });
        }    
        //TODO: create date time service
        protected DateTime GetCurrentDate()=> DateService.Now;        
    }
}
