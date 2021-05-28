namespace Base
{
    public interface IOnUpdateDuringPause {
        ISceneState State{get;}
        void DoUpdate(float deltaTime);
    }
}
