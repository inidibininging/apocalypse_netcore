using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Core.FXBehaviour;
using Apocalypse.Any.Core.Input.Translator;
using Apocalypse.Any.Domain.Common.Network;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System;
using System.Linq;

namespace Apocalypse.Any.Client.States.UI.Character
{
    public class UpdateCharacterWindowState : IState<string, INetworkGameScreen>
    {
        public FadeToBehaviour FadeTo { get; set; }

        public UpdateCharacterWindowState(FadeToBehaviour fadeToBehaviour)
        {
            FadeTo = fadeToBehaviour ?? throw new ArgumentNullException(nameof(fadeToBehaviour));
        }

        private ImageClient GetPlayerImage(IStateMachine<string, INetworkGameScreen> machine)
        {
            return machine.SharedContext.Images.Where(oldImg => oldImg.ServerData.Id == machine.SharedContext.PlayerImageId).FirstOrDefault();
        }

        private float StatInPercentage(int currentPercentage, int maxPercentage)
        {
            return (float)currentPercentage / (float)maxPercentage;
        }

        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            machine.SharedContext.Messages.Add(nameof(UpdateCharacterWindowState));

            var player = GetPlayerImage(machine);
            if (player == null)
                return;
            if (player.Position == null)
                return;

            if (machine.SharedContext.CharacterWindow == null)
                return;

            if (machine.SharedContext.InputService.InputNow.ToList().Contains(DefaultKeys.CloseCharacter))
            {
                machine.SharedContext.CharacterWindow.IsVisible = false;
            }

            if (machine.SharedContext.InputService.InputNow.ToList().Contains(DefaultKeys.OpenCharacter))
            {
                machine.SharedContext.CharacterWindow.IsVisible = true;
            }

            if (machine.SharedContext.CharacterWindow.IsVisible)
            {
                FadeTo.Update(machine.SharedContext.HealthImage.Alpha, 1, 0.1f);
                FadeTo.Update(machine.SharedContext.SpeedImage.Alpha, 1, 0.1f);
                FadeTo.Update(machine.SharedContext.StrenghImage.Alpha, 1, 0.1f);
                FadeTo.Update(machine.SharedContext.DialogImage.Alpha, 1, 0.1f);
                machine.SharedContext.MoneyCount.Update(machine.SharedContext.UpdateGameTime);
            }
            else
            {
                FadeTo.Update(machine.SharedContext.HealthImage.Alpha, 0, 0.1f);
                FadeTo.Update(machine.SharedContext.SpeedImage.Alpha, 0, 0.1f);
                FadeTo.Update(machine.SharedContext.StrenghImage.Alpha, 0, 0.1f);
                FadeTo.Update(machine.SharedContext.DialogImage.Alpha, 0, 0.1f);
            }

            machine.SharedContext.CharacterWindow.Position.X = MathHelper.Lerp(machine.SharedContext.CharacterWindow.Position.X, machine.SharedContext.CursorImage.Position.X - 128, 0.1f);
            machine.SharedContext.CharacterWindow.Position.Y = MathHelper.Lerp(machine.SharedContext.CharacterWindow.Position.Y, machine.SharedContext.CursorImage.Position.Y - 128, 0.1f);

            var money = machine.SharedContext.LastMetadataBag?.MoneyCount.ToString();
            machine.SharedContext.MoneyCount = money;

            machine.SharedContext.MoneyCount.Position.X = machine.SharedContext.CharacterWindow.Position.X + 64;
            machine.SharedContext.MoneyCount.Position.Y = machine.SharedContext.CharacterWindow.Position.Y - 64;
            machine.SharedContext.HealthImage.Position.X = machine.SharedContext.CharacterWindow.Position.X + 32;
            machine.SharedContext.HealthImage.Position.Y = machine.SharedContext.CharacterWindow.Position.Y + 32;
            machine.SharedContext.SpeedImage.Position.X = machine.SharedContext.CharacterWindow.Position.X + (32 + 64);
            machine.SharedContext.SpeedImage.Position.Y = machine.SharedContext.CharacterWindow.Position.Y + 32;
            machine.SharedContext.StrenghImage.Position.X = machine.SharedContext.CharacterWindow.Position.X + 64;
            machine.SharedContext.StrenghImage.Position.Y = machine.SharedContext.CharacterWindow.Position.Y + 32;
            machine.SharedContext.DialogImage.Position.X = machine.SharedContext.CharacterWindow.Position.X + 64 + 64;
            machine.SharedContext.DialogImage.Position.Y = machine.SharedContext.CharacterWindow.Position.Y + 32;

            if (machine.SharedContext.CurrentSheetSnapshot == null ||
                machine.SharedContext.FirstSheetSnapshot == null)
                return;

            var healthPercentage = StatInPercentage
            (
                machine.SharedContext.CurrentSheetSnapshot.Health,
                machine.SharedContext.FirstSheetSnapshot.Health
            );

            var speedPercentage = StatInPercentage
            (
                machine.SharedContext.CurrentSheetSnapshot.Speed,
                1000
            );

            var strengthPercentage = StatInPercentage
            (
                machine.SharedContext.CurrentSheetSnapshot.Strength,
                100
            );

            machine.SharedContext.HealthImage.Alpha.Alpha = 0.5f;
            machine.SharedContext.StrenghImage.Alpha.Alpha = 0.5f;
            machine.SharedContext.SpeedImage.Alpha.Alpha = 0.5f;
            machine.SharedContext.DialogImage.Alpha.Alpha = 1.0f;

            if (!string.IsNullOrWhiteSpace(machine.SharedContext.LastMetadataBag?.ServerEventName))
            {
                if (machine.SharedContext.LastMetadataBag?.ServerEventName == "Dialog")
                {
                    machine.SharedContext.DialogImage.SelectedFrame = "hud_misc_edit_8_8";
                }
                if (machine.SharedContext.LastMetadataBag?.ServerEventName == "Enemies")
                {
                    machine.SharedContext.DialogImage.SelectedFrame = "hud_misc_edit_7_8";
                }
            }                
            else
            {
                machine.SharedContext.DialogImage.SelectedFrame = "hud_misc_edit_0_1";
            }

            //health
            if (healthPercentage >= 0.9f)
                machine.SharedContext.HealthImage.SelectedFrame = "hud_misc_edit_7_0";

            if (healthPercentage <= 0.7f)
                machine.SharedContext.HealthImage.SelectedFrame = "hud_misc_edit_4_5";

            if (healthPercentage <= 0.6f)
                machine.SharedContext.HealthImage.SelectedFrame = "hud_misc_edit_3_5";

            if (healthPercentage <= 0.5f)
                machine.SharedContext.HealthImage.SelectedFrame = "hud_misc_edit_2_5";

            if (healthPercentage <= 0.4f)
                machine.SharedContext.HealthImage.SelectedFrame = "hud_misc_edit_1_5";

            if (healthPercentage <= 0.3f)
                machine.SharedContext.HealthImage.SelectedFrame = "hud_misc_edit_0_5";

            if (healthPercentage <= 0.2f)
                machine.SharedContext.HealthImage.SelectedFrame = "hud_misc_edit_8_4";

            if (healthPercentage <= 0.1f)
                machine.SharedContext.HealthImage.SelectedFrame = "hud_misc_edit_7_4";

            //speed
            if (speedPercentage >= 0.9f)
                machine.SharedContext.SpeedImage.SelectedFrame = "hud_misc_edit_1_1";

            if (speedPercentage <= 0.7f)
                machine.SharedContext.SpeedImage.SelectedFrame = "hud_misc_edit_4_5";

            if (speedPercentage <= 0.6f)
                machine.SharedContext.SpeedImage.SelectedFrame = "hud_misc_edit_3_5";

            if (speedPercentage <= 0.5f)
                machine.SharedContext.SpeedImage.SelectedFrame = "hud_misc_edit_2_5";

            if (speedPercentage <= 0.4f)
                machine.SharedContext.SpeedImage.SelectedFrame = "hud_misc_edit_1_5";

            if (speedPercentage <= 0.3f)
                machine.SharedContext.SpeedImage.SelectedFrame = "hud_misc_edit_0_5";

            if (speedPercentage <= 0.2f)
                machine.SharedContext.SpeedImage.SelectedFrame = "hud_misc_edit_8_4";

            if (speedPercentage <= 0.1f)
                machine.SharedContext.SpeedImage.SelectedFrame = "hud_misc_edit_7_4";

            //strength
            if (strengthPercentage >= 0.9f)
                machine.SharedContext.StrenghImage.SelectedFrame = "hud_misc_edit_4_0";

            if (strengthPercentage <= 0.7f)
                machine.SharedContext.StrenghImage.SelectedFrame = "hud_misc_edit_4_5";

            if (strengthPercentage <= 0.6f)
                machine.SharedContext.StrenghImage.SelectedFrame = "hud_misc_edit_3_5";

            if (strengthPercentage <= 0.5f)
                machine.SharedContext.StrenghImage.SelectedFrame = "hud_misc_edit_2_5";

            if (strengthPercentage <= 0.4f)
                machine.SharedContext.StrenghImage.SelectedFrame = "hud_misc_edit_1_5";

            if (strengthPercentage <= 0.3f)
                machine.SharedContext.StrenghImage.SelectedFrame = "hud_misc_edit_0_5";

            if (strengthPercentage <= 0.2f)
                machine.SharedContext.StrenghImage.SelectedFrame = "hud_misc_edit_8_4";

            if (strengthPercentage <= 0.1f)
                machine.SharedContext.StrenghImage.SelectedFrame = "hud_misc_edit_7_4";

            
        }
    }
}