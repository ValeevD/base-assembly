using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Base
{
    public class SceneState : AbstractService<ISceneState>, ISceneState, ISceneStateLocal
    {

        public int SceneIndex {get; set;}

        private bool isTopmost;
        public bool IsTopmost => isTopmost;

        private bool isVisible;
        public bool IsVisible => isVisible;

        [SerializeField] private bool updateDuringPause;
        [SerializeField] private bool updateLowerStates;

        public bool UpdateDuringPause => updateDuringPause;
        public bool UpdateLowerStates => updateLowerStates;

        void ISceneStateLocal.LocalBecomeTopmost()
        {
            if(isTopmost)
                return;

            isTopmost = true;
            eventBus.RaiseEvent<IOnBecomeTopmost>(
                (x) => {
                    if(x.State != (this as ISceneState))
                        return;

                    x.BecomeTopmost();
                }
            );
        }

        void ISceneStateLocal.LocalResignTopmost()
        {
            if(!isTopmost)
                return;

            isTopmost = false;
            eventBus.RaiseEvent<IOnResignTopmost>(
                (x) => {
                    if(x.State != (this as ISceneState))
                        return;

                    x.ResignTopmost();
                }
            );
        }

        void ISceneStateLocal.LocalActivate()
        {
            if(isVisible)
                return;

            gameObject.SetActive(true);

            isVisible = true;

            //Debug.Log(eventBus==null);
            eventBus.RaiseEvent<IOnActivate>(
                (x) => {
                    if(x.State != (this as ISceneState))
                        return;

                    x.Activate();
                }
            );
        }

        void ISceneStateLocal.LocalDeactivate()
        {
            if(!isVisible)
                return;

            isVisible = false;
            eventBus.RaiseEvent<IOnDeactivate>(
                (x) => {
                    if(x.State != (this as ISceneState))
                        return;

                    x.Deactivate();
                }
            );
        }
    }
}
