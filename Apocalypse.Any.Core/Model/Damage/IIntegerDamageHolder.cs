namespace Apocalypse.Any.Core.Model.Damage
{
    public interface IIntegerDamageHolder<TDamageType>
        : IDamageHolder<TDamageType, int>
        where TDamageType : Damage<int>
    {
    }
}