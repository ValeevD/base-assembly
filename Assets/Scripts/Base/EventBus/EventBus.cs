using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Base
{
    public class EventBus : MonoInstaller, IEventBus, IDisposable
    {

        struct SubsribesLists<T>{
            public List<T> subscribers;
            public List<T> cachedList;
        }

        private Dictionary<Type, SubsribesLists<ISubscriber> > subscribers;
        private Dictionary<Type, SubsribesLists<ISubscriber> > Subscribers{
            get{
                if(subscribers == null)
                    subscribers = new Dictionary<Type, SubsribesLists<ISubscriber> >();

                return subscribers;
            }
        }

        public void Subscribe<T>(ISubscriber subscriber){
            Type subscribersType = typeof(T);
            List<ISubscriber> listOfSubscribers;

            if(!Subscribers.ContainsKey(subscribersType)){
                listOfSubscribers = new List<ISubscriber>();

                List<ISubscriber> cachedList = new List<ISubscriber>();

                Subscribers.Add(subscribersType, new SubsribesLists<ISubscriber> {subscribers = listOfSubscribers, cachedList = cachedList});
            }
            else{
                listOfSubscribers = Subscribers[subscribersType].subscribers;
            }

            if(listOfSubscribers.Contains(subscriber)){
                return;
            }

            listOfSubscribers.Add(subscriber);
        }

        public void Unsubscribe<T>(ISubscriber subscriber){
            Type subscribersType = typeof(T);

            if(!Subscribers.ContainsKey(subscribersType))
                return;

            var listOfSubscribers = Subscribers[subscribersType].subscribers;

            if(listOfSubscribers.Contains(subscriber))
                listOfSubscribers.Remove(subscriber);
        }

        public void UnsubscribeAll(ISubscriber subscriber){
            foreach(var elem in Subscribers)
                if(elem.Value.subscribers.Contains(subscriber))
                    elem.Value.subscribers.Remove(subscriber);
        }

        public void RaiseEvent<T>(Action<T> raisedEvent){
            Type subscribersType = typeof(T);

            if(!Subscribers.ContainsKey(subscribersType))
                return;

            var cachedList = Subscribers[subscribersType].cachedList;

            cachedList.Clear();
            cachedList.AddRange(Subscribers[subscribersType].subscribers);

            foreach(var elem in cachedList){
                raisedEvent.Invoke((T)elem);
            }
        }

        public Task[] RaiseEventAsync<T>(Func<T, Task> raisedEvent){
            Type subscribersType = typeof(T);

            if(!Subscribers.ContainsKey(subscribersType))
                return new Task[0];

            var cachedList = Subscribers[subscribersType].cachedList;

            cachedList.Clear();
            cachedList.AddRange(Subscribers[subscribersType].subscribers);

            Task[] tasks = new Task[cachedList.Count];

            for(int i = 0; i < cachedList.Count; ++i)
                tasks[i] = raisedEvent((T)cachedList[i]);

            return tasks;
        }

        public override void InstallBindings()
        {
            Container.Bind<IEventBus>().FromInstance(this as EventBus);
        }

        void IDisposable.Dispose()
        {
            int i = 0;
            foreach(var elem in Subscribers)
            {
                elem.Value.subscribers.Clear();
                elem.Value.cachedList.Clear();
                Debug.Log(++i);
            }

            Subscribers.Clear();
        }
    }
}
