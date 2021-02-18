using System.Collections.Concurrent;
using System.Reflection.Metadata;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Infrastructure.Common.Services.Network;
using Apocalypse.Any.Infrastructure.Server.Services.Data;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
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

        public void Handle(INetworkStateContext<TWorld> gameStateContext, NetworkCommandConnection networkCommandConnectionToHandle)
        {
            if (networkCommandConnectionToHandle.CommandName != NetworkCommandConstants.SyncSectorCommand)
            {
                gameStateContext.Logger.LogError($"{nameof(ReceiveGameStateDataLayerPartGameSate<TWorld>)} Command name is not ReceiveWorkCommand. Given: {networkCommandConnectionToHandle.CommandName}");
                var back = gameStateContext.GameStateRegistrar.GetNetworkLayerState((byte)ServerInternalGameStates.SendPressedRelease);
                gameStateContext.ChangeHandlerEasier(back, networkCommandConnectionToHandle);
                return;
            }

            var request = NetworkCommandDataConverterService.ConvertToObject(networkCommandConnectionToHandle) as ReceiveGameStateDataLayerPartRequest;
            if (request == null)
            {
                gameStateContext.Logger.LogError($"{nameof(ReceiveGameStateDataLayerPartGameSate<TWorld>)} Network command cannot be converted to {nameof(ReceiveGameStateDataLayerPartRequest)}");
                return;
            }
            if (string.IsNullOrWhiteSpace(request.LoginToken))
            {
                gameStateContext.Logger.LogError($"{nameof(ReceiveGameStateDataLayerPartGameSate<TWorld>)} Request has no login token {nameof(ReceiveGameStateDataLayerPartRequest)}");
                return;
            }
            if (request.SectorKey == -1)
            {
                gameStateContext.Logger.LogError($"{nameof(ReceiveGameStateDataLayerPartGameSate<TWorld>)} Sector key given is invalid");
                return;
            }
            if (request.GetProperty == "DataLayer")
            {
                // gameStateContext.Logger.log($"{nameof(ReceiveGameStateDataLayerPartGameSate<TWorld>)} GetProperty value is empty. Sending the whole game state data layer");

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
                        Lidgren.Network.NetDeliveryMethod.ReliableOrdered,
                        0,
                        networkCommandConnectionToHandle.Connection);
                return;
            }
            
            var sectorLayerService = gameStateContext.GameStateRegistrar.WorldGameStateDataLayer.GetSector(request.SectorKey);
            if (sectorLayerService == null)
            {
                gameStateContext.Logger.LogError($"{nameof(ReceiveGameStateDataLayerPartGameSate<TWorld>)} WorldGameStateDataLayer returns nothing after requesting sector key: {request.SectorKey}");
                return;
            }

            var propertyToCast = sectorLayerService.GetType().GetProperty(request.GetProperty);
            if (propertyToCast == null)
            {
                gameStateContext.Logger.LogWarning($"{nameof(ReceiveGameStateDataLayerPartGameSate<TWorld>)} GetProperty: {request.GetProperty} not found in {nameof(GameStateDataLayer)}");
                
                return;
            }
            
            var content = propertyToCast?.GetValue(sectorLayerService);
            if (content == null)
            {
                gameStateContext.Logger.LogError($"{nameof(ReceiveGameStateDataLayerPartGameSate<TWorld>)} GetProperty: {request.GetProperty} value is null");
                return;
            }
            
            gameStateContext
                .CurrentNetOutgoingMessageBusService
                .SendToClient(NetworkCommandConstants.SyncSectorCommand,
                    content,
                    Lidgren.Network.NetDeliveryMethod.ReliableOrdered,
                    0,
                    networkCommandConnectionToHandle.Connection);
            
        }
    }
}