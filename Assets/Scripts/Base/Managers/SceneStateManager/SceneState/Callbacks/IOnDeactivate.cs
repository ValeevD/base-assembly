namespace Base
{
    public interface IOnDeactivate {
        ISceneState State{get;}
        void Deactivate();
    }
}
