using System.Collections.Concurrent;
using System.Linq;
using System.Reflection.Metadata;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Infrastructure.Common.Services.Network;
using Apocalypse.Any.Infrastructure.Server.Services.Data;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using Lidgren.Network;
using Microsoft.Extensions.Logging;

namespace Apocalypse.Any.Infrastructure.Server.States
{
    public class ReceiveGameStateDataLayerPartGameSate<TWorld>
        : INetworkLayerState<TWorld>
        where TWorld : IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {
        private NetworkCommandDataConverterService NetworkCommandDataConverterService { get; }

        public ReceiveGameStateDataLayerPartGameSate(NetworkCommandDataConverterService networkCommandDataConverterService)
        {
            NetworkCommandDataConverterService = networkCommandDataConverterService;
        }


        private bool GoBackIfNotSyncSector(INetworkStateContext<TWorld> gameStateContext, NetworkCommandConnection networkCommandConnectionToHandle){
            if (networkCommandConnectionToHandle.CommandName == NetworkCommandConstants.SyncSectorCommand)
                return false;
            gameStateContext.Logger.LogError($"{nameof(ReceiveGameStateDataLayerPartGameSate<TWorld>)} Command name is not ReceiveWorkCommand. Given: {networkCommandConnectionToHandle.CommandName}");
            var back = gameStateContext.GameStateRegistrar.GetNetworkLayerState((byte)ServerInternalGameStates.SendPressedRelease);
            gameStateContext.ChangeHandlerEasier(back, networkCommandConnectionToHandle);      
            return true;      
        }

        private bool GoBackIfRequestInvalid(INetworkStateContext<TWorld> gameStateContext, NetworkCommandConnection networkCommandConnectionToHandle, ReceiveGameStateDataLayerPartRequest request){
            if (string.IsNullOrWhiteSpace(request.LoginToken))
            {
                gameStateContext.Logger.LogError($"{nameof(ReceiveGameStateDataLayerPartGameSate<TWorld>)} Request has no login token {nameof(ReceiveGameStateDataLayerPartRequest)}");
                return true;
            }
            if (request.SectorKey == -1)
            {
                gameStateContext.Logger.LogError($"{nameof(ReceiveGameStateDataLayerPartGameSate<TWorld>)} Sector key given is invalid");
                return true;
            }
            return false;
        }

        private bool ProcessPlayerPosIfNeeded(INetworkStateContext<TWorld> gameStateContext, NetworkCommandConnection networkCommandConnectionToHandle, ReceiveGameStateDataLayerPartRequest request){
            //get players position based on its login token and sector. 
            //if the players sector differs between the local server and sync server, the sync server will send an out of sync command to the client
            if (request.GetProperty != "PlayerPos")
            {
                return false;
            }

            gameStateContext.Logger.LogWarning($"{nameof(ReceiveGameStateDataLayerPartGameSate<TWorld>)} PlayerPos. Sending Player pos...");
            var sector = gameStateContext.GameStateRegistrar.WorldGameStateDataLayer.GetSector(request.SectorKey);
            var player = sector.DataLayer.Players.FirstOrDefault(p => p.LoginToken == request.LoginToken);

            if (player == null) {
                gameStateContext
                .CurrentNetOutgoingMessageBusService
                .SendToClient(NetworkCommandConstants.SendPressReleaseCommand,
                    NetworkCommandConstants.OutOfSyncCommand, //Tells the sync client that is is out of sync. Next logical step should be ReceiveGameStateDataLayerPartGameSate
                    NetDeliveryMethod.ReliableOrdered,
                    0,
                    networkCommandConnectionToHandle.Connection
                );                
            }
            else {
                gameStateContext
                    .CurrentNetOutgoingMessageBusService
                    .SendToClient(NetworkCommandConstants.SyncSectorCommand,
                        player.CurrentImage.Position,
                        NetDeliveryMethod.ReliableOrdered,
                        0,
                        networkCommandConnectionToHandle.Connection);               
            }
            return true;
        }

        private bool ProcessDataLayerIfNeeded(INetworkStateContext<TWorld> gameStateContext, NetworkCommandConnection networkCommandConnectionToHandle, ReceiveGameStateDataLayerPartRequest request){
            if (request.GetProperty != "DataLayer")            
                return false;
            gameStateContext.Logger.LogWarning($"{nameof(ReceiveGameStateDataLayerPartGameSate<TWorld>)} GetProperty value is empty. Sending the whole game state data layer");

            var sector = gameStateContext.GameStateRegistrar.WorldGameStateDataLayer.GetSector(request.SectorKey);

            GameStateDataLayer dataLayer = new GameStateDataLayer() {
                Players = new ConcurrentBag<PlayerSpaceship>(sector.DataLayer.Players),
                Enemies = new ConcurrentBag<EnemySpaceship>(sector.DataLayer.Enemies),
                Projectiles = new ConcurrentBag<Projectile>(sector.DataLayer.Projectiles),
                Items = new ConcurrentBag<Item>(sector.DataLayer.Items),
                PlayerItems = new ConcurrentBag<Item>(sector.DataLayer.PlayerItems),
                ImageData = new ConcurrentBag<ImageData>(sector.DataLayer.ImageData),
                GeneralCharacter = new ConcurrentBag<CharacterEntity>(sector.DataLayer.GeneralCharacter)
            };

            //TODO: conversion of IExpandedDataLayerService will fail, because it's an interface. Make a conversion format that can be sent between client (for now, a local server) and (sync) server 
            gameStateContext
                .CurrentNetOutgoingMessageBusService
                .SendToClient(NetworkCommandConstants.SyncSectorCommand,
                    dataLayer,
                    NetDeliveryMethod.ReliableOrdered,
                    0,
                    networkCommandConnectionToHandle.Connection);
            return true;            
        }

        private object TryGetValue(INetworkStateContext<TWorld> gameStateContext, NetworkCommandConnection networkCommandConnectionToHandle, ReceiveGameStateDataLayerPartRequest request){
            var sectorLayerService = gameStateContext.GameStateRegistrar.WorldGameStateDataLayer.GetSector(request.SectorKey);
            if (sectorLayerService == null)
            {
                gameStateContext.Logger.LogError($"{nameof(ReceiveGameStateDataLayerPartGameSate<TWorld>)} WorldGameStateDataLayer returns nothing after requesting sector key: {request.SectorKey}");
                return null;
            }

            var propertyToCast = sectorLayerService.GetType().GetProperty(request.GetProperty);
            if (propertyToCast == null)
            {
                gameStateContext.Logger.LogWarning($"{nameof(ReceiveGameStateDataLayerPartGameSate<TWorld>)} GetProperty: {request.GetProperty} not found in {nameof(GameStateDataLayer)}");                
                return null;
            }
            
            var content = propertyToCast?.GetValue(sectorLayerService);
            if (content == null)
            {
                gameStateContext.Logger.LogError($"{nameof(ReceiveGameStateDataLayerPartGameSate<TWorld>)} GetProperty: {request.GetProperty} value is null");
                return null;
            }
            return content;
        }
        public void Handle(INetworkStateContext<TWorld> gameStateContext, NetworkCommandConnection networkCommandConnectionToHandle)
        {
            GoBackIfNotSyncSector(gameStateContext, networkCommandConnectionToHandle);
            
            var request = NetworkCommandDataConverterService.ConvertToObject(networkCommandConnectionToHandle) as ReceiveGameStateDataLayerPartRequest;
            if (request == null)
            {
                gameStateContext.Logger.LogError($"{nameof(ReceiveGameStateDataLayerPartGameSate<TWorld>)} Network command cannot be converted to {nameof(ReceiveGameStateDataLayerPartRequest)}");
                return;
            }

            if(GoBackIfRequestInvalid(gameStateContext, networkCommandConnectionToHandle, request))
                return;

            //get players position based on its login token and sector. 
            //if the players sector differs between the local server and sync server, the sync server will send an out of sync command to the client
            if(ProcessPlayerPosIfNeeded(gameStateContext, networkCommandConnectionToHandle, request))
                return;

            if(ProcessDataLayerIfNeeded(gameStateContext, networkCommandConnectionToHandle, request))
                return;
            
            var valueResponse = TryGetValue(gameStateContext, networkCommandConnectionToHandle, request);
                       
            gameStateContext
                .CurrentNetOutgoingMessageBusService
                .SendToClient(NetworkCommandConstants.SyncSectorCommand,
                    valueResponse,
                    NetDeliveryMethod.ReliableOrdered,
                    0,
                    networkCommandConnectionToHandle.Connection);
            
        }
    }
}