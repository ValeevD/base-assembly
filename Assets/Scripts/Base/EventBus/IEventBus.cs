using System;
using System.Threading.Tasks;

namespace Base
{
    public interface IEventBus {
        void Subscribe<T>(ISubscriber subscriber);
        void Unsubscribe<T>(ISubscriber subscriber);
        void UnsubscribeAll(ISubscriber subscriber);
        void RaiseEvent<T>(Action<T> raisedEvent);
        Task[] RaiseEventAsync<T>(Func<T, Task> raisedEvent);
    }
}
