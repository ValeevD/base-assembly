namespace Base
{
    public interface ISceneState {
        int SceneIndex {get; set;}

        bool UpdateDuringPause {get;}
        bool UpdateLowerStates {get;}

        bool IsTopmost {get;}
        bool IsVisible {get;}
    }
}
