using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Lidgren.Network;
using Newtonsoft.Json;
using States.Core.Infrastructure.Services;
using System;
using System.Linq;

namespace Apocalypse.Any.Client.States
{
    /// <summary>
    /// Fetches the data from the netserver
    /// </summary>
    public class FetchDataState : IState<string, INetworkGameScreen>
    {
        public INetIncomingMessageBusService IncomingMessageBusService { get; private set; }
        public ISerializationAdapter SerializationAdapter { get; }

        public FetchDataState(INetIncomingMessageBusService incomingMessageBusService, ISerializationAdapter serializationAdapter)
        {
            IncomingMessageBusService = incomingMessageBusService ?? throw new ArgumentNullException(nameof(incomingMessageBusService));
            SerializationAdapter = serializationAdapter ?? throw new ArgumentNullException(nameof(serializationAdapter));
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
                var readMsg = currentMessage.ReadString();

                bool failsToUpcast = false;
                try 
                {
                    var lol = SerializationAdapter.DeserializeObject<IdentifiableNetworkCommand>(readMsg);
                    machine.SharedContext.CurrentNetworkCommand = lol;
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
                        var lol = SerializationAdapter.DeserializeObject<NetworkCommand>(readMsg);
                        machine.SharedContext.CurrentNetworkCommand = new IdentifiableNetworkCommand()
                        {
                            Data = lol.Data,
                            CommandArgument = lol.CommandArgument,
                            CommandName = lol.CommandName
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
                    
                    
                    

                    machine.SharedContext.CurrentGameStateData = SerializationAdapter.DeserializeObject<GameStateData>(machine.SharedContext.CurrentNetworkCommand.Data);
                    machine.SharedContext.Messages.Add("Added CurrentNetworkCommand");

                    machine.SharedContext.LoginToken = machine.SharedContext.CurrentGameStateData.LoginToken;
                    machine.SharedContext.Messages.Add("Added LoginToken");
                }
                catch (Exception ex)
                {
                    machine.SharedContext.Messages.Add(ex.Message);
                }
            }
        }
    }
}