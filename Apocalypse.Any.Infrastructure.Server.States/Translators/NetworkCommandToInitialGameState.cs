using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Echse.Net.Domain;
using Newtonsoft.Json;
using System;
using System.Linq;
using Echse.Net.Serialization;

namespace Apocalypse.Any.Infrastructure.Server.States.Translators
{
    public class NetworkCommandToInitialGameState : INetworkCommandConnectionToGameStateTranslator
    {
        private IWorldGameStateDataInputLayer<GameStateData> GameStatesDataLayer { get; set; }
        public IByteArraySerializationAdapter SerializationAdapter { get; }

        public NetworkCommandToInitialGameState(IWorldGameStateDataInputLayer<GameStateData> gameStateDataLayer, IByteArraySerializationAdapter serializationAdapter)
        {
            if (gameStateDataLayer == null)
                throw new ArgumentNullException(nameof(gameStateDataLayer));
            GameStatesDataLayer = gameStateDataLayer;
            SerializationAdapter = serializationAdapter ?? throw new ArgumentNullException(nameof(serializationAdapter));
        }

        public GameStateData Translate(NetworkCommandConnection input)
        {
            if (input == null)
                return null;
            if (input.CommandName != NetworkCommandConstants.InitializeCommand)
                return null;

            //data to general model + id assignment
            var types = input.CommandArgument.LoadType(true, false);
            var initialState = SerializationAdapter.DeserializeObject(input.Data, types.FirstOrDefault()) as GameStateData;

            types = null;
            if (initialState == null)
                throw new ArgumentNullException(nameof(GameStateData));

            /*
             * If the login token for the game state has not been found =>
             * This means the game state should be registered
             */
            try
            {
                return (string.IsNullOrWhiteSpace(initialState.Id) || Guid.Empty.ToString() == initialState.Id) ?
                GameStatesDataLayer.RegisterGameStateData(initialState.LoginToken) :
                GameStatesDataLayer.GetGameStateByLoginToken(initialState.LoginToken);
            }
            catch (NoLoginTokenException)
            {
                throw;
            }
        }
    }
}