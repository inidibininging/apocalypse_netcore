using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace Apocalypse.Any.Core.Behaviour
{
    /// <summary>
    /// This class cleans every object which is marked for clean up
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CleanDeletedGameObjectsBehaviour : Behaviour<IGameObjectDictionary>
    {
        public CleanDeletedGameObjectsBehaviour(IGameObjectDictionary target) : base(target)
        {
            if (Target.Any(kv => kv.Value is CleanDeletedGameObjectsBehaviour))
                throw new InvalidOperationException("Target already has a clean delete behaviour");
        }

        public override void Update(GameTime gameTime)
        {
            var garbage = Target
                .Where(kv => (kv.Value as IDeletableGameObject)?.MarkedForDeletion == true)
                .Select(dObj => { dObj.Value.UnloadContent(); return dObj.Key; })
                .ToList();
            foreach (var key in garbage)
                Target.Remove(key);
            base.Update(gameTime);
        }
    }
}