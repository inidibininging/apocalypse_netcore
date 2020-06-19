namespace Apocalypse.Any.Core.Behaviour
{
    /// <summary>
    /// Describes the interface of a behaviour. A behaviour is like a decorator pattern.
    /// It should describe how a certain kind of object "behaves".
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBehaviour<T>
        where T : IUpdateableLite
    {
        void Attach(T target);
    }
}