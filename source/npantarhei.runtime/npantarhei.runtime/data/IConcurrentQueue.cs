namespace npantarhei.runtime.data
{
    internal interface IConcurrentQueue<T> : INotifyingResource
    {
        void Enqueue(int priority, T message);
        bool TryDequeue(out T message);
    }
}