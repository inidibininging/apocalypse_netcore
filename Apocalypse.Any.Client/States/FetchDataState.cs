using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Lidgren.Network;
using Newtonsoft.Json;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apocalypse.Any.Client.States
{
    /// <summary>
    /// Fetches the data from the netserver
    /// </summary>
    public class FetchDataState : IState<string, INetworkGameScreen>
    {
        public INetIncomingMessageBusService IncomingMessageBusService { get; private set; }
        public IByteArraySerializationAdapter SerializationAdapter { get; }
        public IDeltaGameStateDataService DeltaGameStateDataService { get; }
        
        public FetchDataState(INetIncomingMessageBusService incomingMessageBusService, IByteArraySerializationAdapter serializationAdapter, IDeltaGameStateDataService deltaGameStateDataService)
        {
            IncomingMessageBusService = incomingMessageBusService ?? throw new ArgumentNullException(nameof(incomingMessageBusService));
            SerializationAdapter = serializationAdapter ?? throw new ArgumentNullException(nameof(serializationAdapter));
            DeltaGameStateDataService = deltaGameStateDataService ?? throw new ArgumentNullException(nameof(deltaGameStateDataService));
        }


        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            machine.SharedContext.Messages.Add(nameof(FetchDataState));

            // read valid messages
            var messages = IncomingMessageBusService.FetchMessageChunk();
            if (!messages.Any())
                return;



            foreach (var currentMessage in messages)
            {
                if (currentMessage == null)
                    continue;

                //Console.WriteLine(currentMessage.MessageType);
                if (currentMessage.MessageType != NetIncomingMessageType.Data)
                    continue;

                machine.SharedContext.Messages.Add("Data incoming...");
                var readMsgLength = currentMessage.LengthBytes;
                var readMsg = currentMessage.ReadBytes(readMsgLength);
                bool failsToUpcast = false;
                try
                {
                    var serverMessage = SerializationAdapter.DeserializeObject<IdentifiableNetworkCommand>(readMsg);
                    machine.SharedContext.CurrentNetworkCommand = serverMessage;
                    machine.SharedContext.Messages.Add("Added CurrentNetworkCommand as IdentifiableNetworkCommand");
                }
                catch (Exception ex)
                {
                    machine.SharedContext.Messages.Add(ex.Message);
                    failsToUpcast = true;
                }

                if (failsToUpcast)
                {
                    try
                    {
                        var serverMessage = SerializationAdapter.DeserializeObject<NetworkCommand>(readMsg);
                        machine.SharedContext.CurrentNetworkCommand = new IdentifiableNetworkCommand()
                        {
                            Data = serverMessage.Data,
                            CommandArgument = serverMessage.CommandArgument,
                            CommandName = serverMessage.CommandName
                        };
                        machine.SharedContext.Messages.Add("Added CurrentNetworkCommand as NetworkCommand");
                    }
                    catch (Exception ex)
                    {
                        machine.SharedContext.Messages.Add(ex.Message);
                        failsToUpcast = true;
                    }
                }

                try
                {
                    if (machine.SharedContext.CurrentNetworkCommand.CommandName == NetworkCommandConstants.UpdateCommand)
                    {
                        var a = DateTime.Now;
                        machine.SharedContext.CurrentGameStateData = SerializationAdapter.DeserializeObject<GameStateData>(machine.SharedContext.CurrentNetworkCommand.Data);
                        var diff = DateTime.Now - a;
                        machine.SharedContext.Messages.Add("Added CurrentNetworkCommand from UpdateCommand");
                    }

                    if (machine.SharedContext.CurrentNetworkCommand.CommandName == NetworkCommandConstants.UpdateCommandDelta)
                    {
                        var a = DateTime.Now;
                        var delta = SerializationAdapter.DeserializeObject<DeltaGameStateData>(machine.SharedContext.CurrentNetworkCommand.Data);
                        var gameStateData = DeltaGameStateDataService.UpdateGameStateData(machine.SharedContext.CurrentGameStateData, delta);
                        machine.SharedContext.CurrentGameStateData = gameStateData;
                        var diff = DateTime.Now - a;
                        machine.SharedContext.Messages.Add("Added CurrentNetworkCommand from UpdateCommandDelta");
                    }

                    if (machine.SharedContext.CurrentGameStateData != null)
                    {
                        machine.SharedContext.LoginToken = machine.SharedContext.CurrentGameStateData.LoginToken;
                        machine.SharedContext.Messages.Add("Added LoginToken");
                    }

                }
                catch (Exception ex)
                {
                    machine.SharedContext.CurrentGameStateData = null;
                    machine.SharedContext.Messages.Add(ex.Message);
                }
            }
           
        }
    }
}