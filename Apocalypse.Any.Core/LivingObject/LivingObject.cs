using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing;

namespace Apocalypse.Any.Core.LivingObject
{
    /// <summary>
    /// This class describes the base functionality of a living game object with the base values such as health and its life state (dead or aliveeeeee)
    /// For now the initial values for health are described by the default values
    /// </summary>
    public class LivingObject :
        CollidableGameObject<AnimatedImage>, IHealthHolder
    {
        /// <summary>
        /// This is the initial health of a living object. This value can change in the future, due to its datatype int. For now I'm not planning of changing it in near future, but because of design reasons this should be variable (example => Health has double as data type)
        /// </summary>
        private readonly int DefaultInitialHealth = 100;

        /// <summary>
        /// This is the max health of a living object. This value can change in the future, due to its datatype int. For now I'm not planning of changing it in near future, but because of design reasons this should be variable (example => Health has double as data type)
        /// </summary>
        private readonly int DefaultMaxHealth = 200;

        /// <summary>
        /// Health representation of the game object
        /// </summary>
        public HealthBehaviour Health
        {
            get;
            set;
        }

        public LivingObject()
        {
            Health = new HealthBehaviour(DefaultInitialHealth, DefaultMaxHealth);
        }

        protected virtual bool StillAlive()
        {
            return Health > 0;
        }

        public LivingObjectState GetLivingState()
        {
            if (StillAlive())
                return LivingObjectState.Living;
            return LivingObjectState.Dead;
        }

        /// <summary>
        /// Does what it says. It sets Health to 0.
        /// </summary>
        public virtual void Die() => Health.Health = 0;
    }
}