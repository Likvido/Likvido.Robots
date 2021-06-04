using System;

namespace Likvido.Robots
{
    public class Operation : IDisposable
    {
        private readonly Action _action;
        private bool disposedValue;
        private object _lockObject = new object();
        private Action? _dispose;

        public Operation(Action action, Action dispose)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _dispose = dispose;
        }

        public void Run()
        {
            if (disposedValue)
            {
                throw new InvalidOperationException("The object has already been dispossed");
            }
            _action();
        }

        public void DoDispose()
        {
            if (_dispose == null)
            {
                return;
            }
            Action? dispose = null;
            lock (_lockObject)
            {
                dispose = _dispose;
                _dispose = null;
            }

            if (dispose != null)
            {
                dispose();
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DoDispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}
