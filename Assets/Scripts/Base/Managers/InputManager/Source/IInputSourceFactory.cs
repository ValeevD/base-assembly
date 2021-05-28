namespace Base
{
    public interface IInputSourceFactory {
        IInputSource Spawn();
        void Despawn(IInputSource source);
    }
}
