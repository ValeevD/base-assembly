using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base
{
    public class SceneStateManager : AbstractService<ISceneStateManager>, ISceneStateManager
    {
        public SceneState[] initStates;

        [SerializeField] private SceneState rootState;
        ISceneState ISceneStateManager.RootState => rootState;


        private ISceneState currentState;
        ISceneState ISceneStateManager.CurrentSceneState => currentState;


        private List<ISceneState> stateStack = new List<ISceneState>();
        private List<ISceneState> cachedStates = new List<ISceneState>();

        private bool stateStackChanged;

        public override void Start()
        {
            base.Start();

            stateStackChanged = false;

            foreach(ISceneState state in initStates)
                (this as ISceneStateManager).PushState(state);
        }

        ISceneState ISceneStateManager.GetLocalSceneState(GameObject gameObject)
        {
            ISceneState localSceneState = gameObject.GetComponentInParent<ISceneState>();

            return localSceneState == null ? rootState : localSceneState;
        }

        void ISceneStateManager.PopState(ISceneState state)
        {
            if(state != null){
                int stateIndex = stateStack.IndexOf(state);

                if(stateIndex == - 1){
                    DebugOnly.Error("Attempt to pop non-existed scene state!");
                    return;
                }

                if(currentState == state)
                    currentState = null;

                (state as ISceneStateLocal).LocalDeactivate();

                for(int i = stateIndex + 1; i < stateStack.Count; ++i){
                    stateStack[i - 1] = stateStack[i];
                    stateStack[i - 1].SceneIndex = i - 1;
                }
            }

            stateStackChanged = true;
            stateStack.RemoveAt(stateStack.Count - 1);
        }

        void ISceneStateManager.PushState(ISceneState newState)
        {
            if(newState == null){
                DebugOnly.Error("Attempt to pop non-existed scene state!");
                return;
            }

            stateStackChanged = true;
            stateStack.Add(newState);

            (newState as ISceneStateLocal).LocalActivate();
        }

        void UpdateStateStack()
        {
            if(!stateStackChanged)
                return;

            cachedStates.Clear();
            cachedStates.AddRange(stateStack);

            if(stateStack.Count == 0){
                if(currentState != null){
                    (currentState as ISceneStateLocal).LocalResignTopmost();
                    currentState = null;
                }
            }
            else {
                if(currentState != null)
                    (currentState as ISceneStateLocal).LocalResignTopmost();

                currentState = stateStack[stateStack.Count - 1];
                (currentState as ISceneStateLocal).LocalBecomeTopmost();
            }
        }

        void Update()
        {
            UpdateStateStack();

            int n = cachedStates.Count;
            bool doUpdate = true;

            float deltaTime = Time.deltaTime;

            while(--n >= 0){
                ISceneState _state = cachedStates[n];

                if(doUpdate)
                    eventBus.RaiseEvent<IOnUpdate>(
                        (x) => {
                            if(x.State != _state)
                                return;

                            x.DoUpdate(deltaTime);
                        }
                    );
                else if(_state.UpdateDuringPause)
                    eventBus.RaiseEvent<IOnUpdateDuringPause>(
                            (x) => {
                                if(x.State != _state)
                                    return;

                                x.DoUpdate(deltaTime);
                            }
                        );

                doUpdate = doUpdate && _state.UpdateLowerStates;
            }
        }
    }
}
