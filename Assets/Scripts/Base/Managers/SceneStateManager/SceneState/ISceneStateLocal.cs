namespace Base
{
    public interface ISceneStateLocal {
        void LocalBecomeTopmost();
        void LocalResignTopmost();
        void LocalActivate();
        void LocalDeactivate();
    }
}
