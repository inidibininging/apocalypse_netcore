namespace Apocalypse.Any.Core.Behaviour
{
    /// <summary>
    /// This describes the behaviour of an object that is bound to health.
    /// For now health is an integer value.
    /// TODO: replace int with generics and make a HealthIntegerBehaviour representation
    /// </summary>
    public class HealthBehaviour //: Behaviour //todo: change name to Health #Health
    {
        public HealthBehaviour(int initialHealth, int maxHealth)//(IGameObject target, int initialHealth, int maxHealth) : base(target)
        {
            MaxHealth = maxHealth;
        }

        public int Health { get; set; } = 0; //todo: change property name to Value #Health

        protected int MaxHealth { get; set; }

        public override string ToString()
        {
            return Health.ToString();
        }

        public static implicit operator int(HealthBehaviour behaviour)
        {
            return behaviour.Health;
        }
    }
}