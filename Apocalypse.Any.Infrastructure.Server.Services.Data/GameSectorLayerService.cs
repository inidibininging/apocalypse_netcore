using System;
using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Server.PubSub.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Echse.Domain;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data
{
    /// <summary>
    /// Representation of a game sector
    /// </summary>
    public class GameSectorLayerService
        : IGameSectorLayerService
    {
        public IList<string> Messages { get; set; }
        public int MaxEnemies { get; set; }
        public int MaxPlayers { get; set; }
        public IGameSectorBoundaries SectorBoundaries { get; set; }
        public Dictionary<string, IGenericTypeFactory<GameStateData>> GameStateDataLayer { get; set; }
        public int Tag { get; set; }
        public GameSectorStatus CurrentStatus { get; set; }
        public GameTime CurrentGameTime { get; set; }
        

        public IGameSectorFactoryLayer<PlayerSpaceship,
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

        public IExpandedGameSectorDataLayer<PlayerSpaceship,
                                    EnemySpaceship,
                                    Item,
                                    Projectile,
                                    CharacterEntity,
                                    CharacterEntity,
                                    ImageData> DataLayer
        { get; set; }


        public IGameSectorSingularMechanicsLayer<string,
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
                                                 ImageData> SingularMechanics
        { get; set; }

        public IGameSectorPluralMechanicsLayer<string,
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


        public IUserLoginService LoginService { get; set; }
        public IWorldGameStateDataIOLayer IODataLayer { get; set; }
        public IPlayerDialogService PlayerDialogService { get; set; }
        public IEventDispatcher EventDispatcher { get; set; }
        public TimeSpan LanguageTick => CurrentGameTime.ElapsedGameTime;

        public IEnumerable<TagVariable> Variables => DataLayer.GetLayersByType<TagVariable>().FirstOrDefault()?.DataAsEnumerable<TagVariable>();

        public void AddVariable(TagVariable variable)
        {
            DataLayer.GetLayersByType<TagVariable>().FirstOrDefault()?.Add(variable);
        }

        public void RemoveTagByName(string tagName)
        => DataLayer.GetLayersByType<TagVariable>().FirstOrDefault()
                ?.Remove(new Func<TagVariable, bool>((v) => v.Name == tagName));
        

        public void RemoveTagByNameAndScope(string tagName, string scope)
        => DataLayer.GetLayersByType<TagVariable>().FirstOrDefault()
            ?.Remove(new Func<TagVariable, bool>((v) => v.Name == tagName && v.Scope == scope));

    }
}