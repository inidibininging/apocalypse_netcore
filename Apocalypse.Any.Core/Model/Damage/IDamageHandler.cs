namespace Apocalypse.Any.Core.Model.Damage
{
    /// <summary>
    /// This interface defines an object that takes damage.
    /// </summary>
    public interface IDamageHandler<TDamageType, TDamageTypeValue>
        where TDamageType : Damage<TDamageTypeValue>
            where TDamageTypeValue : struct
    {
        /// <summary>
        /// This method is for handling the taking of damage as such
        /// </summary>
        /// <typeparam name="TDamageType">Damage type to handle</typeparam>
        /// <typeparam name="TDamageTypeValue">Prmitive value type used for damage(can be int16,int32,double etc.)</typeparam>
        /// <param name="damage"></param>
        /// <returns></returns>
        bool HandleDamage(TDamageType damage);
    }
}