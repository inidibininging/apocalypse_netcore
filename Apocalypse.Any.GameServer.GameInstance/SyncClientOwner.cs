using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Model.PubSub;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Domain.Server.Model.Network;
using Apocalypse.Any.Domain.Server.Sector.Model;
using Apocalypse.Any.GameServer.Services;
using Apocalypse.Any.GameServer.States.Sector;
using Apocalypse.Any.GameServer.States.Sector.Storage;
using Apocalypse.Any.Infrastructure.Common.Services.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.YamlAdapter;
using Apocalypse.Any.Infrastructure.Server.Adapters.Redis;
using Apocalypse.Any.Infrastructure.Server.Language;
using Apocalypse.Any.Infrastructure.Server.PubSub;
using Apocalypse.Any.Infrastructure.Server.PubSub.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.CLI;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.RoutingMechanics;
using Apocalypse.Any.Infrastructure.Server.States;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Apocalypse.Any.Domain.Client.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Factories;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.SectorMechanics;
using Apocalypse.Any.Infrastructure.Server.Worker;
using Apocalypse.Any.Core.Input.Translator;
using Apocalypse.Any.Infrastructure.Server.Services.Data;
using Apocalypse.Any.Infrastructure.Server.Services.Network;
using Apocalypse.Any.Core.Input;

namespace Apocalypse.Any.GameServer.GameInstance
{
    public interface ISyncClientOwner
    {
        bool LoggedInToPressRelease { get; }
        string LoginToken { get; }

        void Connect();
        void DelegateOtherPlayerCommandsToLocalServer(ILogger<byte> logger);
        void DelegatePlayerCommandsToSyncServer(int lastSectorOfClient, IEnumerable<string> commands);
        void DelegatePlayerPositionToSyncServer(PlayerPositionUpdateData playerPositionUpdateData);
        void DelegateSyncServerDataToLocalServer(IGameSectorLayerService playerSector, ILogger<byte> logger);
        void TryLoginToSyncServer(ILogger<byte> logger);
        void UpdateSectorOfPlayerInsideSyncClient(int playerSectorKey);
    }

    public class NullSyncClientOwner : ISyncClientOwner
    {
        public bool LoggedInToPressRelease => false;

        public string LoginToken => string.Empty;

        public void Connect()
        {
            
        }

        public void DelegateOtherPlayerCommandsToLocalServer(ILogger<byte> logger)
        {
            
        }

        public void DelegatePlayerCommandsToSyncServer(int lastSectorOfClient, IEnumerable<string> commands)
        {
            
        }

        public void DelegatePlayerPositionToSyncServer(PlayerPositionUpdateData playerPositionUpdateData)
        {
        }

        public void DelegateSyncServerDataToLocalServer(IGameSectorLayerService playerSector, ILogger<byte> logger)
        {
            
        }

        public void TryLoginToSyncServer(ILogger<byte> logger)
        {
            
        }

        public void UpdateSectorOfPlayerInsideSyncClient(int playerSectorKey)
        {
            
        }
    }

    public class SyncClientOwner : ISyncClientOwner
    {
        public SyncClientOwner(SyncClient<PlayerSpaceship, EnemySpaceship, Item, Projectile, CharacterEntity, CharacterEntity, ImageData> syncClient,
                               IInputTranslator<IEnumerable<string>, IEnumerable<string>>  pressReleaseTranslator)
        {
            SyncClient = syncClient;
            PressReleaseTranslator = pressReleaseTranslator;
        }
        private SyncClient<PlayerSpaceship, EnemySpaceship, Item, Projectile, CharacterEntity, CharacterEntity, ImageData> SyncClient
        {
            get;
            set;
        }

        /// <summary>
        /// Tells if this instance is logged in to a sync server
        /// </summary>
        /// <value></value>
        public bool LoggedInToPressRelease { get; private set; }

        public void Connect()
        {
            SyncClient.Connect();
        }

        public void TryLoginToSyncServer(ILogger<byte> logger)
        {
            if (LoggedInToPressRelease) return;
            var loginAttempt = (SyncClient?.TryLogin()).GetValueOrDefault();
            logger.LogInformation(Enum.GetName(typeof(NetSendResult),loginAttempt));
            LoggedInToPressRelease = loginAttempt == NetSendResult.Sent;
        }

        /// <summary>
        /// Compares a user with the user that plays locally.
        /// If it matches, the commands of the player (player owned by user) will be delegated to the sync server
        /// </summary>
        /// <param name="loginToken">Login token from a player in the sync server</param>
        /// <param name="commands"></param>
        public void DelegatePlayerCommandsToSyncServer(int lastSectorOfClient, IEnumerable<string> commands)
        {

            if (commands?.Any() != true)
                return;

            if (!LoggedInToPressRelease || SyncClient.ClientConfiguration == null)
                return;

            SyncClient.LastSectorKey = lastSectorOfClient;


            var commandsAsPressRelease = PressReleaseTranslator.Translate(commands);
            
            SyncClient.ProcessIncomingMessages(commandsAsPressRelease.Select(cmd => SyncToPressReleaseCommandTranslator.Translate(cmd)));
        }

        /// <summary>
        /// Passes all the data layer data from the SyncServer to the sector of the player
        /// </summary>
        public void DelegateSyncServerDataToLocalServer(IGameSectorLayerService playerSector, ILogger<byte> logger)
        {
            if (!SyncClient.NewDataLayer)
                return;

            logger.LogInformation(nameof(DelegateSyncServerDataToLocalServer));

            // var playerSector = GameSectorLayerServices.Values.FirstOrDefault(s => s.SharedContext.DataLayer.Players.Any(p => p.LoginToken == SyncClient.LoginToken));
            if (playerSector == null)
            {
                logger.LogError("Something is wrong. Player not found. Maybe the player is now in another sector");
                throw new NotImplementedException();
            }

            var serverPlayersInLocalServer = SyncClient.DataLayer.Players.Where(p => playerSector.DataLayer.Players.Any(localPlayer => localPlayer.LoginToken == p.LoginToken));
            var serverPlayersNotInLocalServer = SyncClient.DataLayer.Players.Except(serverPlayersInLocalServer);

            foreach (var serverPlayer in serverPlayersInLocalServer)
            {
                var localPlayer = playerSector.DataLayer.Players.FirstOrDefault(p => p.LoginToken == serverPlayer.LoginToken);

                logger.LogInformation("Passing server player data to local player");

                //only apply the position and rotation value for testing purpouses
                localPlayer.CurrentImage.Position.X = serverPlayer.CurrentImage.Position.X;
                localPlayer.CurrentImage.Position.Y = serverPlayer.CurrentImage.Position.Y;
                localPlayer.CurrentImage.Rotation.Rotation = serverPlayer.CurrentImage.Rotation.Rotation;
                localPlayer.CurrentImage.Path = serverPlayer.CurrentImage.Path;
                localPlayer.CurrentImage.Color = Color.Yellow; // for debugging purpouses only
            }

            foreach (var serverPlayerNotInLocal in serverPlayersNotInLocalServer)
                playerSector.DataLayer.Players.Add(serverPlayerNotInLocal);

            
            SyncClient.NewDataLayer = false;
        }

        public void DelegateOtherPlayerCommandsToLocalServer(ILogger<byte> logger)
        {
            //TODO: 
            if (SyncClient.CommandsToLocalServer.Count == 0) return;
            while (SyncClient.CommandsToLocalServer.TryDequeue(out (string loginToken, string command) nextCommand))
            {
                //TODO: get player, add commands and see what happens
                logger.LogInformation($"TODO: get player {nextCommand.loginToken}, add {nextCommand.command} and see what happens");
            }
        }

        /// <summary>
        /// Converts any input to an int command
        /// </summary>
        /// <returns></returns>
        private IInputTranslator<string,int> SyncToPressReleaseCommandTranslator { get; } = new IntToStringCommandTranslator();

        private IInputTranslator<IEnumerable<string>, IEnumerable<string>> PressReleaseTranslator { get; }

        public string LoginToken
        {
            get => SyncClient.LoginToken;
        }

        //this doesnt belong here. Need refactoring
        public void UpdateSectorOfPlayerInsideSyncClient(int playerSectorKey)
        {
            // var sectorKV = GameSectorLayerServices.FirstOrDefault(kv => kv.Value.SharedContext.DataLayer.Players.Any(p => p.LoginToken == SyncClient.LoginToken));
            // if (sectorKV.Value == null) return;
            SyncClient.LastSectorKey = playerSectorKey;
        }

    }
}
