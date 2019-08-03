using System.Threading;

namespace System
{
    class Throttle
    {
        public Throttle(TimeSpan period) {
            _period = period;
        }

        public T Invoke<T>(Func<T> func) {
            var delta = DateTime.Now - _lastInvoke;
            if (delta < _period) {
                Thread.Sleep(_period - delta);
            }

            try {
                return func();
            } finally {
                _lastInvoke = DateTime.Now;
            }
        }

        DateTime _lastInvoke = DateTime.MinValue;
        TimeSpan _period;
    }
}