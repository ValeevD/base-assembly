using System.Collections.Generic;
using UnityEngine;

namespace Base
{
    public interface ISceneStateManager {
        ISceneState CurrentSceneState {get; }
        ISceneState RootState {get; }

        //void Init();
        ISceneState GetLocalSceneState(GameObject gameObject);

        void PushState(ISceneState newState);
        void PopState(ISceneState state=null);
    }
}
