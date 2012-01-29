namespace npantarhei.runtime.data
{
    internal interface INotifyingResource
    {
        void Notify();
        bool Wait(int milliseconds);
    }
}