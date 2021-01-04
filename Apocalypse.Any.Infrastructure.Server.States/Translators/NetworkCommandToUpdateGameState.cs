using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Apocalypse.Any.Infrastructure.Server.States.Translators
{
    public class NetworkCommandToUpdateGameState : INetworkCommandConnectionToGameStateTranslator
    {
        private IWorldGameStateDataInputLayer<GameStateData> GameStatesDataLayer { get; set; }
        public ISerializationAdapter SerializationAdapter { get; }

        public NetworkCommandToUpdateGameState(IWorldGameStateDataInputLayer<GameStateData> gameStateDataLayer, ISerializationAdapter serializationAdapter)
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
            if (input.CommandName != NetworkCommandConstants.UpdateCommand)
                return null;

            //data to general model + id assignment
            var types = input.CommandArgument.LoadType(true, false);

            var currentState = SerializationAdapter.DeserializeObject(input.Data, types.FirstOrDefault()) as GameStateData;

            if (currentState == null)
                throw new ArgumentNullException(nameof(GameStateData));

            if (string.IsNullOrWhiteSpace(currentState.Id) || Guid.Empty.ToString() == currentState.Id)
                throw new InvalidOperationException("No registered game state");

            var foundGameStateData = GameStatesDataLayer.GetGameStateByLoginToken(currentState.LoginToken);
            Console.WriteLine($"{nameof(NetworkCommandToUpdateGameState)}:found in GetGameStateByLoginToken:{foundGameStateData}");

            return foundGameStateData;
        }
    }
}