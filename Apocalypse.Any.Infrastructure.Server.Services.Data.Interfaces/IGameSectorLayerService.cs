using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Server.PubSub.Interfaces;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Echse.Domain;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces
{
    public interface IGameSectorLayerService : IGameSectorData, IEchseContext
    {
        GameTime CurrentGameTime { get; set; }

        /// <summary>
        /// This is the clients perception of the world.
        /// This can be viewed as a list of all data a client needs at the current moment for this sector
        /// Proto view port ... sort of
        /// </summary>
        IWorldGameStateDataIOLayer IODataLayer { get; set; }

        /// <summary>
        /// Represents an interface to the player dialogs
        /// </summary>
        IPlayerDialogService PlayerDialogService { get; set; }

        /// <summary>
        /// Dispatches events within the server. 
        /// Events can be anywhere. In this implementation, the event queues are stored as data layers
        /// Means: Every event has it's own event queue. Also consumers / notifiable instances that consume events are also implemented within the data layers
        /// For more information check the class within the GameServer project "BuildDataLayerState"
        /// </summary>
        IEventDispatcher EventDispatcher { get; set; }
        

        //Dictionary<string,IGameSectorFactory<GameStateData>> GameStateDataLayer { get; set; }

        /// <summary>
        /// General purpouse message cache. Should be replaced in the future with a ILoggerBlablabla
        /// </summary>
        IList<string> Messages { get; set; }



        IExpandedGameSectorDataLayer<PlayerSpaceship,
                             EnemySpaceship,
                             Item,
                             Projectile,
                             CharacterEntity,
                             CharacterEntity,
                             ImageData> DataLayer
        { get; set; }



        IGameSectorSingularMechanicsLayer<string,
                                          IExpandedGameSectorDataLayer<
                                              PlayerSpaceship,
                                              EnemySpaceship,
                                              Item,
                                              Projectile,
                                              CharacterEntity,
                                              CharacterEntity,
                                              ImageData>,
                                          PlayerSpaceship,
                                          EnemySpaceship,
                                          Item,
                                          Projectile,
                                          CharacterEntity,
                                          CharacterEntity,
                                          ImageData> SingularMechanics
        { get; set; }

        IGameSectorPluralMechanicsLayer<string,
                                        IExpandedGameSectorDataLayer<PlayerSpaceship,
                                                             EnemySpaceship,
                                                             Item,
                                                             Projectile,
                                                             CharacterEntity,
                                                             CharacterEntity,
                                                             ImageData>,
                                        PlayerSpaceship,
                                        EnemySpaceship,
                                        Item,
                                        Projectile,
                                        CharacterEntity,
                                        CharacterEntity,
                                        ImageData> PluralMechanics
        { get; set; }


        IGameSectorFactoryLayer<PlayerSpaceship,
                                EnemySpaceship,
                                Item,
                                Projectile,
                                CharacterEntity,
                                CharacterEntity,
                                ImageData,
                                string,
                                string,
                                string,
                                string,
                                string,
                                string> Factories
        { get; set; }
    }
}