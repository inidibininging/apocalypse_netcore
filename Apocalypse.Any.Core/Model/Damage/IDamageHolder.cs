namespace Apocalypse.Any.Core.Model.Damage
{
    /// <summary>
    /// An object that stores the damage object
    /// </summary>
    /// <typeparam name="TDamageType"></typeparam>
    /// <typeparam name="TDamageValuteType"></typeparam>
    public interface IDamageHolder<TDamageType, TDamageValuteType>
        where TDamageType : Damage<TDamageValuteType>
        where TDamageValuteType : struct
    {
        /// <summary>
        /// The damage object
        /// </summary>
        TDamageType Damage { get; }
    }
}