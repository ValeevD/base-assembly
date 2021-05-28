namespace Base
{
    public interface IOnBecomeTopmost {
        ISceneState State{get;}
        void BecomeTopmost();
    }
}
