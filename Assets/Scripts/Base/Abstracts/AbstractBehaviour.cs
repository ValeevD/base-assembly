using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Base
{
    public class AbstractBehaviour : MonoBehaviour, ISubscriber
    {
        //protected ServiceLocator serviceLocator = ServiceLocator.Instance;
        [Inject] private ISceneState state;
        public ISceneState State => state;

        [Inject] protected ISceneStateManager sceneStateManager;
        [Inject] protected IEventBus eventBus;

        protected virtual void OnDisable(){
            if(eventBus == null)
                return;

            eventBus.UnsubscribeAll(this);
        }

    }
}
