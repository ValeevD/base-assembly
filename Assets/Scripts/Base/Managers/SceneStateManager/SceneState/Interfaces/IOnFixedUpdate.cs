namespace Base
{
    public interface IOnFixedUpdate {
        ISceneState State{get;}
        void DoUpdate();
    }
}
