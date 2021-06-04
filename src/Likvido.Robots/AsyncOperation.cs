using System;
using System.Threading.Tasks;

namespace Likvido.Robots
{
    public class AsyncOperation : IAsyncDisposable
    {
        private readonly Func<Task> _action;
        private Func<ValueTask>? _disposeAction;
        private object _lockObject = new object();
        private bool _disposed;

        public AsyncOperation(Func<Task> action, Func<ValueTask>? disposeAction)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _disposeAction = disposeAction;
        }

        public Task Run()
        {
            if (_disposed)
            {
                throw new InvalidOperationException("The object has already been dispossed");
            }
            return _action();
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                await DoDisposeAsync();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        private async ValueTask DoDisposeAsync()
        {
            if (_disposeAction == null)
            {
                return;
            }
            Func<ValueTask>? dispose = null;
            lock (_lockObject)
            {
                dispose = _disposeAction;
                _disposeAction = null;
            }

            if (dispose != null)
            {
                await dispose().ConfigureAwait(false);
            }
        }
    }
}
