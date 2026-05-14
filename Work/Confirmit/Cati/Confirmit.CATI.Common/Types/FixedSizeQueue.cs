using System.Collections.Concurrent;

namespace Confirmit.CATI.Common.Types
{
    public class FixedSizedQueue<T>
    {
        readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();

        public int Size { get; }

        public FixedSizedQueue(int size)
        {
            Size = size;
        }

        public void Enqueue(T obj)
        {
            _queue.Enqueue(obj);

            while (_queue.Count > Size)
            {
                _queue.TryDequeue(out _);
            }
        }
        public int Count()
        {
            return _queue.Count;
        }

        public T TryPeek()
        {
            _queue.TryPeek(out var result);
            return result;
        }
    }
}
