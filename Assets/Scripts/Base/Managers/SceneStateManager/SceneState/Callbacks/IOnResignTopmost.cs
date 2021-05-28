namespace Base
{
    public interface IOnResignTopmost {
        ISceneState State{get;}
        void ResignTopmost();
    }
}
