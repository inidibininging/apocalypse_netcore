using System.Linq;
using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.LivingObject;
using Apocalypse.Any.Core.Screen;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Client.Mechanics
{
    public class CorpseRemoverMechanicBehaviour : Behaviour<GameScreen>
    {
        public CorpseRemoverMechanicBehaviour(GameScreen target) : base(target)
        {
        }

        public override void Update(GameTime gameTime)
        {
            (from lo in Target.AllOfType<ILivingObject>()
                where lo.GetLivingState() == LivingObjectState.Dead
                select lo).ToList().ForEach(obj =>
            {
                obj.UnloadContent();
                Target.Remove(obj);
            });
            base.Update(gameTime);
        }
    }
}