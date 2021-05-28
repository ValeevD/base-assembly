using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Base
{
    public class AbstractService<T> : MonoInstaller, ISubscriber where T : class
    {
        [Inject] protected IEventBus eventBus;

        protected virtual void OnDisable(){
            eventBus.UnsubscribeAll(this);
        }

        public override void InstallBindings()
        {
            Container.Bind<T>().FromInstance(this as T);
        }

    }
}
