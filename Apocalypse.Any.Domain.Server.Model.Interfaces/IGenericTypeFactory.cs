namespace Apocalypse.Any.Domain.Server.Model.Interfaces
{
    public interface IGenericTypeFactory<T>
    {
        bool CanUse<TParam>(TParam parameter);

        T Create<TParam>(TParam parameter);
    }
}