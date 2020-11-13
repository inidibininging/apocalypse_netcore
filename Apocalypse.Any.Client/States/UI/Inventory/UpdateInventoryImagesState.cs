using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Client.Services.Creation;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.Client.States.UI.Inventory
{
    /// <summary>
    /// Updates all images data with data from server
    /// </summary>
    public class UpdateInventoryImagesState : IState<string, INetworkGameScreen>
    {
        public PlayerInventoryDrawingFactory InventoryDrawingFactory { get; set; }

        public UpdateInventoryImagesState(PlayerInventoryDrawingFactory playerInventoryDrawingFactory)
        {
            InventoryDrawingFactory = playerInventoryDrawingFactory ?? throw new ArgumentNullException(nameof(playerInventoryDrawingFactory));
        }

        private ImageClient GetPlayerImage(IStateMachine<string, INetworkGameScreen> machine)
        {
            return machine.SharedContext.Images.Where(oldImg => oldImg.ServerData.Id == machine.SharedContext.PlayerImageId).FirstOrDefault();
        }

        private void TransformToGrid(IStateMachine<string, INetworkGameScreen> machine)
        {
            //changed the order of update. this part was in the bottom
            var playerImage = GetPlayerImage(machine);
            if (playerImage == null)
            {
                machine.SharedContext.Messages.Add("player image not found.");
                return;
            }
            if (machine.SharedContext.InventoryImages.Count != 0)
                machine.SharedContext.InventoryImages = PlayerInventoryDrawingFactory.CreateInventoryGrid(machine.SharedContext.InventoryWindow, machine.SharedContext.InventoryImages).ToList();
        }

        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            machine.SharedContext.Messages.Add(nameof(UpdateInventoryImagesState));
            

            if (machine.SharedContext.LastMetadataBag == null)
            {
                machine.SharedContext.Messages.Add("last meta data bag is not available");
                return;
            }

            if (machine.SharedContext.LastMetadataBag.Items == null)
            {
                machine.SharedContext.Messages.Add("last meta data bag items is not available");
                return;
            }

            if (machine.SharedContext.LastMetadataBag.Items.Count > 0)
            {
                //udpate old images
                var newImgs = new List<ImageData>();
                var itemImages = machine.SharedContext.LastMetadataBag.Items.OrderByDescending(itm => itm.Order).Select(itm => itm.CurrentImage);
                if (machine.SharedContext.LastMetadataBag.Items.Count == 0)
                {
                    foreach (var imageToDispose in machine.SharedContext.InventoryImages)
                    {
                        machine.SharedContext.Messages.Add(imageToDispose.ServerData.Id);
                        imageToDispose.UnloadContent();
                    }
                    machine.SharedContext.InventoryImages.Clear();
                    return;
                }

                foreach (var newImg in itemImages)
                {
                    var foundOldImg = machine.SharedContext.InventoryImages.FirstOrDefault(oldImg => oldImg.ServerData.Id == newImg.Id);
                    if (foundOldImg == default(ImageClient))
                    {
                        //no registered img in old
                        var newImageClient = new ImageClient(newImg, machine.SharedContext.GameSheet.Frames);
                        newImageClient.LoadContent(ScreenService.Instance.Content);
                        machine.SharedContext.InventoryImages.Add(newImageClient);
                    }
                    else
                    {
                        foundOldImg.ApplyImageData(newImg);
                    }
                }
                var newImgBundle = machine.SharedContext.InventoryImages.Where(img => itemImages.Any(gImg => gImg.Id == img.ServerData.Id)).ToList();

                // //clean up old inventory images
                var imagesToDispose = machine.SharedContext.InventoryImages.Except(newImgBundle);
                foreach (var imageToDispose in imagesToDispose)
                {
                    machine.SharedContext.Messages.Add(imageToDispose.ServerData.Id);
                    imageToDispose.UnloadContent();
                }

                machine.SharedContext.InventoryImages.AddRange(imagesToDispose.Where(img => !img.Disposed));
                TransformToGrid(machine);
            }
        }
    }
}