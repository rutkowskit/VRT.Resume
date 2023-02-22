using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VRT.Resume.Application.Fixtures;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application;

public abstract class BaseRequestTest : IDisposable
{
    private bool _disposedValue;
    protected BaseFixture Fixture { get; }
    public IServiceScope Scope { get; }
    
    protected BaseRequestTest(BaseFixture fixture)
    {
        Fixture = fixture;
        Scope = fixture.Services.CreateScope();
    }

    protected async Task<TResponse> Send<TResponse>(IRequest<TResponse>? request)
    {
        var mediator = GetService<IMediator>(Scope);
        var result = await mediator.Send(request!);
        return result;
    }

    protected TService GetService<TService>()
        where TService : class
    {
        return GetService<TService>(Fixture.Services);
    }
    protected static TService GetService<TService>(IServiceScope scope)
        where TService : class
    {
        return GetService<TService>(scope.ServiceProvider);
    }
    protected static TService GetService<TService>(IServiceProvider services)
        where TService : class
    {
        return services.GetRequiredService<TService>();
    }
    protected IReadOnlyCollection<TEntity> Query<TEntity>(Func<IQueryable<TEntity>, IReadOnlyCollection<TEntity>> queryFunc)
        where TEntity : class
    {        
        var context = (Scope.ServiceProvider.GetService<AppDbContext>() as DbContext)!;        
        return queryFunc(context.Set<TEntity>());
    }
    protected IReadOnlyCollection<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        var context = (Scope.ServiceProvider.GetService<AppDbContext>() as DbContext)!;
        return context.Set<TEntity>().Where(predicate).ToArray();
    }

    protected async Task<TEntity> AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        var context = (Scope.ServiceProvider.GetService<AppDbContext>() as DbContext)!;
        context.Add(entity);

        await context.SaveChangesAsync();
        return entity;
    }

    protected async Task<int> CountAsync<TEntity>() where TEntity : class
    {
        var context = (Scope.ServiceProvider.GetService<AppDbContext>() as DbContext)!;
        return await context.Set<TEntity>().CountAsync();
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // Add here things to dispose
                Scope.Dispose();
            }
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    protected AppDbContext GetDbContext() => GetService<AppDbContext>(Scope);
    protected AppDbContext GetDbContext(IServiceScope scope) => GetService<AppDbContext>(scope);
}
