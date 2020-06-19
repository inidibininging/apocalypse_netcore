namespace Apocalypse.Any.Core.Model.Damage
{
    public interface IIntegerDamageHandler<TDamageType>
        : IDamageHandler<TDamageType, int>
        where TDamageType : Damage<int>
    {
    }
}