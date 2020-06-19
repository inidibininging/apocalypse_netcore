using System;

namespace Apocalypse.Any.Core.Model.Damage
{
    /// <summary>
    /// A dynamic damage that can be lazy loaded or its implementation can be changed at runtime
    /// </summary>
    /// <typeparam name="TDamageValueType"></typeparam>
    public sealed class DynamicDamage<TDamageValueType> : Damage<TDamageValueType>
        where TDamageValueType : struct
    {
        private Func<TDamageValueType> InternalDamageFn { get; set; }

        public override TDamageValueType GetDamage()
        => InternalDamageFn == null ? throw new ArgumentNullException("No default declaration for dynamic damage") : InternalDamageFn();

        public DynamicDamage(Func<TDamageValueType> internalDamageFn)
        {
            InternalDamageFn = internalDamageFn ?? throw new ArgumentNullException("No default declaration for dynamic damage");
        }
    }
}