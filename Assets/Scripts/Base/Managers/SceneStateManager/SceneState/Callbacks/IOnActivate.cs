namespace Base
{
    public interface IOnActivate {
        ISceneState State{get;}
        void Activate();
    }
}
