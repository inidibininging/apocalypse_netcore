using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.EnemyMechanics
{
    /// <summary>
    /// Checks enemies for distance to player. If the enemy is not near the player, then no mechanic should be applied to the entity
    /// </summary>
    public class SetMechanicsStatusBasedOnDistanceToPlayerState : IState<string, IGameSectorLayerService>
    {
        //this distance should based on the max players sight
        public int DistanceToActivate { get; }
        public string TagNameForDeactivatingMechanics { get; }

        public SetMechanicsStatusBasedOnDistanceToPlayerState(string tagNameForDeactivatingMechanics, int distanceToActivate = 512)
        {
            TagNameForDeactivatingMechanics = tagNameForDeactivatingMechanics;
            DistanceToActivate = distanceToActivate;
        }
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            Task.Factory.StartNew(() =>
            {
                //add tag to enemies
                foreach (var enemySpaceship in machine
                    .SharedContext
                    .DataLayer
                    .Enemies
                    .Where(e => machine.SharedContext.DataLayer.Players.Any(p =>
                        Vector2.Distance(e.CurrentImage.Position, p.CurrentImage.Position) > DistanceToActivate &&
                        !e.Tags.Contains(TagNameForDeactivatingMechanics))))
                    enemySpaceship.Tags.Add(TagNameForDeactivatingMechanics);


                //remove tag from enemies 
                foreach (var enemySpaceship in machine
                    .SharedContext
                    .DataLayer
                    .Enemies
                    .Where(e => machine.SharedContext.DataLayer.Players.Any(p =>
                        Vector2.Distance(e.CurrentImage.Position, p.CurrentImage.Position) < DistanceToActivate &&
                        e.Tags.Contains(TagNameForDeactivatingMechanics))))
                    enemySpaceship.Tags.Remove(TagNameForDeactivatingMechanics);
            });


        }
    }
}