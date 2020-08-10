using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Core.Input.Translator;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Lidgren.Network;
using Newtonsoft.Json;
using States.Core.Infrastructure.Services;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.Client.States
{
    /// <summary>
    /// Sends all relevant information to the server in order to receive an update of the data
    /// </summary>
    public class SendGameStateUpdateDataState : IState<string, INetworkGameScreen>
    {
        public ISerializationAdapter SerializationAdapter { get; }

        public SendGameStateUpdateDataState(ISerializationAdapter serializationAdapter)
        {
            SerializationAdapter = serializationAdapter ?? throw new ArgumentNullException(nameof(serializationAdapter));
        }
        private NetOutgoingMessage CreateMessage<T>(IStateMachine<string, INetworkGameScreen> machine, string commandName, T instanceToSend)
        {
            return machine.SharedContext.Client.CreateMessage(

                    SerializationAdapter.SerializeObject
                    (
                        new NetworkCommand()
                        {
                            CommandName = commandName,
                            CommandArgument = typeof(T).FullName,
                            Data = SerializationAdapter.SerializeObject(instanceToSend)
                        }
                    )
                );
        }

        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            //machine.SharedContext.InputService.Update(machine.SharedContext.UpdateGameTime);

            if (string.IsNullOrWhiteSpace(machine.SharedContext.LoginToken))
                return;

            // var input = machine.SharedContext.InputService.InputNow;
            // if(input == null || !input.Any())
            //     input = new List<string>();
            // else
            //     input = input.ToList();


            var cmds = machine.SharedContext.InputService.InputNow.ToList();
            cmds.ForEach(cmd =>
            {
                machine.SharedContext.Messages.Add(cmd);
            });

            if (machine.SharedContext.LastMetadataBag != null && machine.SharedContext.LastMetadataBag.Stats != null &&
                machine.SharedContext.LastMetadataBag.Stats.Health <= machine.SharedContext.LastMetadataBag.Stats.GetMinAttributeValue())
            {
                if (cmds.Contains(DefaultKeys.Continue))
                    cmds = new List<string>() { DefaultKeys.Continue };
                else
                    cmds.Clear();
            }

            //this only works if last meta data bag is not overwritten
            if(machine.SharedContext.LastMetadataBag != null &&
                machine.SharedContext.LastMetadataBag.ClientEventName != null &&
                machine.SharedContext.LastMetadataBag.ClientEventName.Contains("Exit") &&
                cmds.Contains(DefaultKeys.Use))
            {
                cmds.Add(DefaultKeys.CloseDialog);
                machine.SharedContext.LastMetadataBag.ClientEventName = null;
            }
            else
            {
                if (machine.SharedContext.LastMetadataBag != null &&
                machine.SharedContext.LastMetadataBag.ClientEventName != null &&
                cmds.Contains(DefaultKeys.Use))
                {
                    var selectedDialog = machine.SharedContext.LastMetadataBag.CurrentDialog?.DialogIdContent.FirstOrDefault(d => d.Item2 == machine.SharedContext.LastMetadataBag.ClientEventName);
                    if(selectedDialog != null)
                    {
                        cmds.Add($"{DefaultKeys.OpenDialog} {selectedDialog.Item1}");
                    }
                    machine.SharedContext.LastMetadataBag.ClientEventName = null;
                }
            }
            
            if(machine.SharedContext.CurrentGameStateData == null || 
                    cmds.Contains("Exit") || 
                    cmds.Contains(DefaultKeys.Use) ||
                    cmds.Contains(DefaultKeys.CloseDialog))
            {
                var sendResult = machine.SharedContext.Client.SendMessage(
                                        CreateMessage(
                                            machine,
                                            NetworkCommandConstants.UpdateCommand,
                                            new GameStateUpdateData()

                                            {
                                                LoginToken = machine.SharedContext.LoginToken,
                                                Commands = cmds,
                                                Screen = new ScreenData()
                                                {
                                                    ScreenWidth = (int)MathF.Round(ScreenService.Instance.Resolution.X),
                                                    ScreenHeight = (int)MathF.Round(ScreenService.Instance.Resolution.Y)
                                                }
                                            }),
                                        NetDeliveryMethod.UnreliableSequenced);
                if (sendResult == NetSendResult.FailedNotConnected)
                    machine.GetService.Get(ClientGameScreenBook.Connect).Handle(machine);
            }
            else
            {
                var sendResult = machine.SharedContext.Client.SendMessage(
                                        CreateMessage(
                                            machine,
                                            NetworkCommandConstants.UpdateCommandDelta,
                                            new GameStateUpdateData()
                                            {
                                                LoginToken = machine.SharedContext.LoginToken,
                                                Commands = cmds,
                                                Screen = new ScreenData()
                                                {
                                                    ScreenWidth = (int)MathF.Round(ScreenService.Instance.Resolution.X),
                                                    ScreenHeight = (int)MathF.Round(ScreenService.Instance.Resolution.Y)
                                                }
                                            }),
                                        NetDeliveryMethod.UnreliableSequenced);
                if (sendResult == NetSendResult.FailedNotConnected)
                    machine.GetService.Get(ClientGameScreenBook.Connect).Handle(machine);
            }


            
        }
    }
}