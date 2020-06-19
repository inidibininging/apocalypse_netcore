using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Core.Behaviour
{
    public class UpdateableBehaviourWrapper : Behaviour<IUpdateableLite>
    {
        public UpdateableBehaviourWrapper(IUpdateableLite gameTimeTimer) : base(gameTimeTimer)
        {
        }

        public override void Update(GameTime gameTime)
        {
            this.Target.Update(gameTime);
        }
    }
}