using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Infrastructure.Common.Services.Network;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using States.Core.Infrastructure.Services;
using System;
using System.Linq;

namespace Apocalypse.Any.Client.States
{
    /// <summary>
    /// Updates the metadata state
    /// </summary>
    public class UpdateMetadataState : IState<string, INetworkGameScreen>
    {
        private NetworkCommandDataConverterService DataConverter { get; set; }

        public UpdateMetadataState(NetworkCommandDataConverterService dataConverter)
        {
            DataConverter = dataConverter ?? throw new ArgumentNullException(nameof(dataConverter));
        }

        int LastItemCount { get; set; }
        /// <summary>
        /// Convert thes metadata branch into the proper type and passes it to the client for further processing
        /// </summary>
        /// <param name="machine"></param>
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            machine.SharedContext.Messages.Add(nameof(UpdateMetadataState));
            var viewPortRect = ScreenService.Instance.GraphicsDevice.Viewport.Bounds;
            var cam = ScreenService.Instance.DefaultScreenCamera.Position;

            machine.SharedContext.Messages.Add($"CX:{cam.X}");
            machine.SharedContext.Messages.Add($"CY:{cam.Y}");
            //machine.SharedContext.Messages.Add($"X:{viewPortRect.X}");
            //machine.SharedContext.Messages.Add($"Y:{viewPortRect.Y}");
            machine.SharedContext.Messages.Add($"W:{viewPortRect.Width}");
            machine.SharedContext.Messages.Add($"H:{viewPortRect.Height}");

            if (machine.SharedContext.CurrentGameStateData == null)
                return;
            if (machine.SharedContext.CurrentGameStateData.Metadata == null)
                return;

            machine.SharedContext.PlayerImageId = machine.SharedContext.CurrentGameStateData.Metadata.Id;
            if (string.IsNullOrWhiteSpace(machine.SharedContext.CurrentGameStateData.Metadata.Data))
                return;

            //convert to object
            var metadataAsObject = DataConverter.ConvertToObject(machine.SharedContext.CurrentGameStateData.Metadata);
            machine.SharedContext.Messages.Add($"{nameof(metadataAsObject)} {metadataAsObject.GetType().FullName}");
            //TODO: how to abstract this so that one can use the JObject (dynamic casting the jobject to a real object)
            var metadataBagAsJObject = (metadataAsObject as PlayerMetadataBag);

            if ((metadataBagAsJObject) == null)
            {
                machine.SharedContext.Messages.Add("metadata cannot be converted to JObject");
                return;
            }
            machine.SharedContext.Messages.Add("metadata bag as object  was converted");

            //var metadataBagConverted = metadataBagAsJObject.ToObject<PlayerMetadataBag>();
            var metadataBagConverted = metadataBagAsJObject;
            if (metadataBagConverted == null)
            {
                machine.SharedContext.Messages.Add("metadata cannot be converted to player metadata bag");
                return;
            }

            if(metadataBagConverted.Items != null)
            {
                machine.SharedContext.Messages.Add($"items:{metadataBagConverted.Items?.Count}");
                if (LastItemCount < metadataBagConverted.Items.Count)
                    ScreenService.Instance.Sounds.Play("ItemGather");
                LastItemCount = metadataBagConverted.Items.Count;
            }
            
            machine.SharedContext.Messages.Add(metadataBagConverted.GameSectorTag.ToString());
            machine.SharedContext.LastMetadataBag = metadataBagConverted;

            // //assign the first character sheet
            if (machine.SharedContext.FirstSheetSnapshot == null)
                machine.SharedContext.FirstSheetSnapshot = metadataBagConverted.Stats;
            machine.SharedContext.CurrentSheetSnapshot = metadataBagConverted.Stats;



            var playerImage = machine.SharedContext.Images.FirstOrDefault(img => img.ServerData.Id == machine.SharedContext.PlayerImageId);
            if (playerImage == null || machine.SharedContext.LastMetadataBag == null)
                return;
            //WHY IS THIS NOT WORKING????
//            var cursorImageAsVector = machine.SharedContext.CursorImage.Position.ToVector2();
//            if (Vector2.Distance(cursorImageAsVector, playerImage.Position.ToVector2()) < 16)
//            {
                
//                machine.SharedContext.MultiplayerText.Text = $@"
//ATK: {machine.SharedContext.LastMetadataBag.Stats.Attack}
//DEF: {machine.SharedContext.LastMetadataBag.Stats.Defense}
//STR: {machine.SharedContext.LastMetadataBag.Stats.Strength}
//SPD: {machine.SharedContext.LastMetadataBag.Stats.Speed}
//TEC: {machine.SharedContext.LastMetadataBag.Stats.Technology}
//AUR: {machine.SharedContext.LastMetadataBag.Stats.Aura}
//CHR: {machine.SharedContext.LastMetadataBag.Stats.Charisma}";
//                var spacing = machine.SharedContext.MultiplayerText.Text.Split(Environment.NewLine).Length;
//                machine.SharedContext.MultiplayerText.Alpha.Alpha = 1;
//                machine.SharedContext.MultiplayerText.LayerDepth = DrawingPlainOrder.UIFX;
//                machine.SharedContext.MultiplayerText.Color = Color.Purple;
//                machine.SharedContext.MultiplayerText.Scale = new Vector2(1.5f);
//                machine.SharedContext.MultiplayerText.Position.X = playerImage.Position.X + 16;
//                machine.SharedContext.MultiplayerText.Position.Y = playerImage.Position.Y + spacing;
//                machine.SharedContext.Messages.Add(machine.SharedContext.MultiplayerText.Text);
//            }
        }
    }
}