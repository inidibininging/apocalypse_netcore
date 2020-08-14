using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Data;
using Apocalypse.Any.Infrastructure.Common.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Common.Services.Interpolation;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Apocalypse.Any.Client.States
{
    /// <summary>
    /// Records data of fetched game states and makes an interpolation if there is no network connection
    /// </summary>
    public class NetworkInterpolationState : IState<string, INetworkGameScreen>
    {        
        private List<DeltaImageDataInterpolated> ImagesInterpolation { get; set; } = new List<DeltaImageDataInterpolated>();
        private List<DeltaImageDataInterpolatedDelta> ImageInterpolationalChange { get; set; } = new List<DeltaImageDataInterpolatedDelta>();

        public IDeltaGameStateDataService DeltaGameStateDataService { get; }
        public DeltaGameStateData LastDeltaGameStateData { get; set; }
        public GameStateData LastGameStateData { get; set; }
        public NetworkInterpolationState(IDeltaGameStateDataService deltaGameStateDataService)
        {
            DeltaGameStateDataService = deltaGameStateDataService ?? throw new ArgumentNullException(nameof(deltaGameStateDataService));
        }
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            if (machine.SharedContext.CurrentGameStateData == null)
                return;

            if (string.IsNullOrWhiteSpace(LastGameStateData.Id))
            {
                LastGameStateData = machine.SharedContext.CurrentGameStateData;
                return;
            }
            LastDeltaGameStateData = DeltaGameStateDataService.GetDelta(LastGameStateData, machine.SharedContext.CurrentGameStateData);
            
            //should only happen once
            if (ImagesInterpolation.Count == 0)
                ImagesInterpolation.AddRange(LastDeltaGameStateData.Images.Select(i => new DeltaImageDataInterpolated().Update(i)));

            //remove unneeded images
            var idOfImagesToRemove = DeltaGameStateDataService.GetImagesToRemove(LastGameStateData.Images, LastDeltaGameStateData.Images).Select(i => i.Id);
            ImagesInterpolation.RemoveAll(img => idOfImagesToRemove.Contains(img.Id));
            
            var sharedImages = LastDeltaGameStateData.Images
                               

            //add new images
            var newImages = DeltaGameStateDataService.GetNewImagesFromDelta(LastGameStateData.Images, LastDeltaGameStateData.Images);
            if(newImages.Any())
                ImagesInterpolation.AddRange(newImages.Select(i => new DeltaImageDataInterpolated().Update(i)));


        }   
    }
}
