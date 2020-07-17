namespace Apocalypse.Any.Domain.Common.Mechanics
{
    public interface ISingleEntityTransformationMechanic<TSource, TDestination>
    {
        TDestination Update(TSource source);
    }
    public interface ISingleEntityTransformationMechanic<T> :
        ISingleEntityTransformationMechanic<T,T>
    {
        new T Update(T source);
    }
}