using System;
using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data
{
    /// <summary>
    /// In memory IO Data Layer for accessing game states of client. 
    /// This should be seen as the "rendering" of game state data
    /// </summary>
    public class InMemoryGameStateDataLayer :
        IWorldGameStateDataIOLayer
    {
        protected static List<GameStateData> DataCache { get; set; } = new List<GameStateData>();

        private IEnumerable<GameStateData> GetGameStateDataById(string id) => from gameState in DataCache
                                                                              where gameState.Id == id
                                                                              select gameState;

        private IEnumerable<GameStateData> GetGameStateDataByLoginToken(string loginToken) => from gameState in DataCache
                                                                                              where gameState.LoginToken == loginToken
                                                                                              select gameState;

        protected IGenericTypeFactory<GameStateData> GameStateFactory { get; set; }
        protected IGenericTypeFactory<PlayerSpaceship> PlayerSpaceshipFactory { get; set; }

        public InMemoryGameStateDataLayer(
            IGenericTypeFactory<GameStateData> gameStateFactory,
            IGenericTypeFactory<PlayerSpaceship> playerSpaceshipFactory)
        {
            GameStateFactory = gameStateFactory ?? throw new ArgumentNullException(nameof(gameStateFactory));
            PlayerSpaceshipFactory = playerSpaceshipFactory ?? throw new ArgumentNullException(nameof(playerSpaceshipFactory));
        }

        public GameStateData GetGameStateByLoginToken(string loginToken)
        {
            var foundGameStates = GetGameStateDataByLoginToken(loginToken);
            if (!foundGameStates.Any())
                throw new GameStateNotFoundException(loginToken);

            return foundGameStates.FirstOrDefault();
        }

        public GameStateData RegisterGameStateData(string loginToken)
        {
            Console.WriteLine($"{nameof(RegisterGameStateData)} in {nameof(InMemoryGameStateDataLayer)}");
            if (string.IsNullOrWhiteSpace(loginToken))
                throw new NoLoginTokenException();

            try
            {
                //TODO: Get game state from game
                return GetGameStateByLoginToken(loginToken);
            }
            catch (GameStateNotFoundException gameStateNotFoundEx)
            {
                Console.WriteLine(gameStateNotFoundEx.Message);
                var player = PlayerSpaceshipFactory.Create(loginToken);
                var gameState = GameStateFactory.Create(player);
                DataCache.Add(gameState);
                Console.WriteLine($"{gameState.Id} added player. {player.DisplayName}");
                return gameState;
            }
        }

        public bool ForwardClientDataToGame(GameStateUpdateData updateData)
        {
            //Console.WriteLine("Forwarding client info to server...");
            return DataCache.Any(cache =>
            {
                if (cache.LoginToken != updateData.LoginToken)
                    return false;
                cache.Commands = updateData.Commands ?? Array.Empty<string>().ToList();
                if(updateData.Screen != null)
                    cache.Screen = updateData.Screen;
                return true;
            });
        }

        public bool ForwardServerDataToGame(GameStateData gameStateData)
        {
            return DataCache.Any(cache =>
            {
                if (cache.LoginToken != gameStateData.LoginToken)
                    return false;


                cache.Camera = gameStateData.Camera;
                cache.Images = gameStateData.Images;
                cache.Metadata = gameStateData.Metadata;                
                cache.Screen = gameStateData.Screen;
                // cache.Sounds = gameStateData.Sounds;
                return true;
            });
        }

    }
}