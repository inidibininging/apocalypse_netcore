using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Domain.Server.DataLayer;
using Apocalypse.Any.GameServer.States.Sector;
using Apocalypse.Any.GameServer.States.Sector.Factories;
using Apocalypse.Any.Infrastructure.Common.Services.Network;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.States
{
    /// <summary>
    /// Used for gathering data from the game
    /// </summary>
    public class ReceiveWorkNetworkGameState<TWorld>
        : INetworkLayerState<TWorld>
        where TWorld : IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {
        private readonly NetworkCommandDataConverterService NetworkCommandDataConverterService;

        public ReceiveWorkNetworkGameState(NetworkCommandDataConverterService networkCommandDataConverterService)
        {
            NetworkCommandDataConverterService = networkCommandDataConverterService ?? throw new ArgumentNullException(nameof(networkCommandDataConverterService));
        }
        public void Handle(INetworkStateContext<TWorld> gameStateContext, NetworkCommandConnection networkCommandConnectionToHandle)
        {
            //If login command is here. it means that this state is fired first
            if (networkCommandConnectionToHandle.CommandName == NetworkCommandConstants.LoginCommand)
            {
                //send an "ACK" for the worker
                gameStateContext
                    .CurrentNetOutgoingMessageBusService
                    .SendToClient(NetworkCommandConstants.ReceiveDataLayerCommand,
                                  true,
                                  networkCommandConnectionToHandle.Connection);
                return;
            }

            var clientsResponse = string.Empty;
            if (networkCommandConnectionToHandle.CommandName == NetworkCommandConstants.ReceiveDataLayerCommand ||
                networkCommandConnectionToHandle.CommandName == NetworkCommandConstants.ReceiveMessagesCommand ||
                networkCommandConnectionToHandle.CommandName == NetworkCommandConstants.SuggestCommand)
            {
                try
                {
                    clientsResponse = NetworkCommandDataConverterService.ConvertToObject(networkCommandConnectionToHandle) as string;
                    //gameStateContext.Logger.LogDebug(clientsResponse);
                }
                catch (Exception ex)
                {
                    clientsResponse = string.Empty;
                    gameStateContext.Logger.LogWarning(ex.Message);
                }
            }
            //If receive command
            if (networkCommandConnectionToHandle.CommandName == NetworkCommandConstants.ReceiveDataLayerCommand)
            {
                
                if (string.IsNullOrWhiteSpace(clientsResponse))
                    return;
                var sectorLayerService = gameStateContext.GameStateRegistrar.WorldGameStateDataLayer.GetSector(clientsResponse);
                gameStateContext
                    .CurrentNetOutgoingMessageBusService
                    .SendToClient(NetworkCommandConstants.ReceiveDataLayerCommand,
                                  sectorLayerService.DataLayer as GameStateDataLayer,
                                  networkCommandConnectionToHandle.Connection);
                return;
            }

            //if 
            if (networkCommandConnectionToHandle.CommandName == NetworkCommandConstants.ReceiveMessagesCommand)
            {
                
                if (string.IsNullOrWhiteSpace(clientsResponse))
                    return;
                var sectorLayerService = gameStateContext.GameStateRegistrar.WorldGameStateDataLayer.GetSector(clientsResponse);
                gameStateContext
                    .CurrentNetOutgoingMessageBusService
                    .SendToClient(NetworkCommandConstants.ReceiveMessagesCommand,
                                  sectorLayerService.Messages,
                                  networkCommandConnectionToHandle.Connection);
                return;
            }

            if (networkCommandConnectionToHandle.CommandName == NetworkCommandConstants.SuggestCommand)
            {
                
                if (string.IsNullOrWhiteSpace(clientsResponse))
                    return;

                var sectorLayerService = gameStateContext.GameStateRegistrar.WorldGameStateDataLayer.GetSector("hub");                
                
                //var currentObj = sectorLayerService.GetType().GetProperties().Where(p => p.Name.ToLower().Contains(suggestion)).Select(p => p.Name);
                
                gameStateContext
                    .CurrentNetOutgoingMessageBusService
                    .SendToClient(NetworkCommandConstants.ReceiveMessagesCommand,
                                  clientsResponse.Contains(".") ? 
                                  Inspect(sectorLayerService, clientsResponse.Split('.')) :
                                  sectorLayerService.GetType().GetProperties().Where(p => p.Name.ToLower().Contains(clientsResponse)).Select(p => p.Name),
                                  networkCommandConnectionToHandle.Connection);
                return;
            }
        }

        private List<string> Inspect(object subject, IEnumerable<string> depthSuggestion)
        {
            if (subject == null)
                return new List<string>();
            //must be exact match
            //strip the string into depth levels
            var subjectType = subject.GetType();
            if (depthSuggestion.Count() == 1)
            {
                if(subjectType.IsGenericType && 
                    (
                        subjectType.GetGenericTypeDefinition() == typeof(Dictionary<,>) ||
                        subjectType.GetGenericTypeDefinition() == typeof(ConcurrentDictionary<,>)
                    )){
                    var kEnumerator = (subject as IDictionary)?.Keys.GetEnumerator();
                    if(kEnumerator != null)
                    {
                        var result = new List<string>();
                        while (kEnumerator.MoveNext())
                            result.Add(kEnumerator.Current.ToString());
                    }
                    
                }
                if (string.IsNullOrWhiteSpace(depthSuggestion.First()))
                    return subject.GetType().GetProperties().Select(p => p.Name).ToList();
                return subject.GetType().GetProperties().Where(p => p.Name.ToLower().Contains(depthSuggestion.First().ToLower())).Select(p => p.Name)
                        .Concat(subject.GetType().GetMethods().Select(m => m.Name)).ToList();
            }
            var propertyFound = subjectType.GetProperties().Where(p => p.Name.ToLower() == depthSuggestion.First().ToLower()).FirstOrDefault();
            if (propertyFound == null)
                return subjectType.GetProperties().Where(p => p.Name.ToLower().Contains(depthSuggestion.First().ToLower())).Select(p => p.Name).ToList();
            var newSubject = propertyFound.GetValue(subject);
            if(newSubject == null)
            {
                return subjectType.GetProperties().Where(p => p.Name.ToLower().Contains(depthSuggestion.First().ToLower())).Select(p => p.Name)
                        .Concat(subject.GetType().GetMethods().Select(m => m.Name)).ToList();
            }
            return Inspect(newSubject, depthSuggestion.Skip(1));
        }
    }
}
