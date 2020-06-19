namespace Apocalypse.Any.Core.Model.Damage
{
    /// <summary>
    /// Actual damage representation in the game. Value must be value type .
    /// The basic idea behind this is to push a damage to other objects, so that you can combine,alter and change every bit of the damage.
    /// I built this because I wanted to test some basic rock paper scissor / rpg elements with this junk
    /// </summary>
    /// <typeparam name="TDamageValueType"></typeparam>
    public abstract class Damage<TDamageValueType>
        where TDamageValueType : struct
    {
        public abstract TDamageValueType GetDamage();
    }
}