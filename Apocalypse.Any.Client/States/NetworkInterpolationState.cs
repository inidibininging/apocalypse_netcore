using System;
using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Common.Services.Interpolation;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Client.States
{
    /// <summary>
    ///     Records data of fetched game states and makes an interpolation if there is no network connection
    /// </summary>
    public class NetworkInterpolationState : IState<string, INetworkGameScreen>
    {
        public NetworkInterpolationState(IDeltaGameStateDataService deltaGameStateDataService)
        {
            DeltaGameStateDataService = deltaGameStateDataService ??
                                        throw new ArgumentNullException(nameof(deltaGameStateDataService));
        }

        private List<DeltaImageDataInterpolated> ImagesInterpolation { get; } = new();
        private List<DeltaImageDataInterpolatedDelta> ImageInterpolationalChange { get; set; } = new();

        public IDeltaGameStateDataService DeltaGameStateDataService { get; }
        public DeltaGameStateData LastDeltaGameStateData { get; set; }
        public GameStateData LastGameStateData { get; set; }

        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            if (machine.SharedContext.CurrentGameStateData == null)
                return;

            LastGameStateData = machine.SharedContext.CurrentGameStateData;
            if (string.IsNullOrWhiteSpace(LastGameStateData?.Id)) return;

            LastDeltaGameStateData =
                DeltaGameStateDataService.GetDelta(LastGameStateData, machine.SharedContext.CurrentGameStateData);

            //should only happen once
            if (ImagesInterpolation.Count == 0)
                ImagesInterpolation.AddRange(
                    LastDeltaGameStateData.Images.Select(i => new DeltaImageDataInterpolated().Update(i)));

            //remove unneeded images
            if (LastGameStateData.Images.Count != ImagesInterpolation.Count)
                foreach (var img in LastGameStateData.Images)
                {
                    var interpolatedImage = ImagesInterpolation.FirstOrDefault(intImg => intImg.Id == img.Id);
                    if (interpolatedImage == null)
                        ImagesInterpolation.Add(new DeltaImageDataInterpolated().Update(img));
                    else
                        interpolatedImage.Update(img);
                }
        }
    }
}