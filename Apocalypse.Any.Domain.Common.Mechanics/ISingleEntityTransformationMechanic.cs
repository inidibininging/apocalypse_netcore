namespace Apocalypse.Any.Domain.Common.Mechanics
{
    public interface ISingleEntityTransformationMechanic<TSource, TDestination>
    {
        TDestination Update(TSource source);
    }
}