namespace Base
{
    public interface IOnUpdate {
        ISceneState State{get;}
        void DoUpdate(float deltaTime);
    }
}
