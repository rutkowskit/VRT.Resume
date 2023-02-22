namespace VRT.Resume.Application.Fixtures;

public abstract class BaseFixture : IDisposable
{
    private IServiceProvider? _serviceProvider;
    private bool _disposedValue;

    protected BaseFixture()
    {
        ServiceCollection = new ServiceCollection();            
    }

    protected IServiceCollection ServiceCollection { get; private set; }
    public IServiceProvider Services => _serviceProvider ??= BuildServiceProvider();
    private IServiceProvider BuildServiceProvider()
    {
        var result = ServiceCollection.BuildServiceProvider();
        OnServiceProviderBuid(result);        
        return result;
    }
    protected virtual Task OnServiceProviderBuid(IServiceProvider provider)
    {
        return Task.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                ServiceCollection.Clear();
            }
            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
