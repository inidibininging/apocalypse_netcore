using System;
using Apocalypse.Any.Client.GameObjects.Scene;
using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Constants;

namespace Apocalypse.Any.Client.States
{
    /// <summary>
    /// Updates all images data with data from server
    /// </summary>
    public class UpdateImagesState : IState<string, INetworkGameScreen>
    {
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            machine.SharedContext.Messages.Add(nameof(UpdateImagesState));
            if (machine.SharedContext.CurrentGameStateData?.Images == null)
                return;
            if (machine.SharedContext.Images == null)
                return;

            //udpate old images
            var newImgs = new List<ImageData>();
            foreach (var newImg in machine.SharedContext.CurrentGameStateData.Images.ToList())
            {
                var foundOldImg = machine.SharedContext.Images.Find(oldImg => oldImg.ServerData.Id == newImg.Id);
                if (foundOldImg == default(ImageClient))
                {
                    //no registered img in old
                    if (newImg.SelectedFrame.frame == ImagePaths.ProjectileFrame)
                    {
                        
                    }
                    var newImageClient = new ImageClient(newImg, machine.SharedContext.GameSheet.Frames);
                    newImageClient.LoadContent(ScreenService.Instance.Content);
                    machine.SharedContext.Images.Add(newImageClient);
                    if (newImageClient.SelectedFrame.frame != ImagePaths.UndefinedFrame && newImageClient.SelectedFrame.frame == ImagePaths.ProjectileFrame)
                        ScreenService.Instance.Sounds.Play($"SynthLaser0{Randomness.Instance.From(0, 3)}");
                }
                else
                {
                    foundOldImg.ApplyImageData(newImg);
                }
            }
            var newImgBundle = machine.SharedContext.Images.Where(img => machine.SharedContext.CurrentGameStateData.Images.Any(gImg => gImg.Id == img.ServerData.Id)).ToList();
            var imagesToDispose = machine.SharedContext.Images.Except(newImgBundle);

            foreach (var img in imagesToDispose)
            {
                if (img.SelectedFrame.frame == ImagePaths.EnemyFrame)
                {
                    img.SelectedFrame = (ImagePaths.ExplosionFrame, 0, 8);
                    img.Scale = new Vector2(img.Scale.X + 1, img.Scale.Y + 1);
                    img.Color = Color.Yellow;

                    for (var i = 0; i < 10; i++)
                    {
                        machine.SharedContext
                                .As<RandomDebrisField>(nameof(RandomDebrisField))
                                .Add(img.Position.X,
                                    img.Position.Y,
                                    new Color(
                                        new Vector3(108,
                                                    108,
                                                    255)),
                                    ((float)Randomness.Instance.From(20, 150) / 200f));
                    }

                    ScreenService.Instance.Sounds.Play($"SynthRainbow0{Randomness.Instance.From(0, 3)}");
                }
                
                if (img.SelectedFrame.frame == ImagePaths.ProjectileFrame)
                {
                    img.SelectedFrame = (ImagePaths.ExplosionFrame, 0, 8);
                    img.Scale = new Vector2(img.Scale.X + 1, img.Scale.Y + 1);
                    
                    for (var i = 0; i < 2; i++)
                    {
                        machine.SharedContext
                            .As<RandomSparkField>(nameof(RandomSparkField))
                            .Add(img.Position.X,
                                img.Position.Y,
                                img.Color,
                                ((float)Randomness.Instance.From(20, 150) / 200f), 
                                img.LayerDepth);
                    }

                    ScreenService.Instance.Sounds.Play($"SynthRainbow0{Randomness.Instance.From(0, 3)}");
                }

                if (img.SelectedFrame == (ImagePaths.ThrustFrame, 6, 8))
                {
                    img.LayerDepth = DrawingPlainOrder.EntitiesFX;
                    img.Alpha.Alpha -= 0.05f;
                    img.SelectedFrame = (ImagePaths.ThrustFrame, 7, 8);
                    continue;
                }
                if (img.SelectedFrame == (ImagePaths.ThrustFrame, 7, 8))
                {
                    img.LayerDepth = DrawingPlainOrder.EntitiesFX;
                    img.Alpha.Alpha -= 0.05f;
                    img.SelectedFrame = (ImagePaths.ThrustFrame, 8, 8);
                    continue;
                }
                if (img.SelectedFrame == (ImagePaths.ThrustFrame, 8, 8))
                {
                    img.LayerDepth = DrawingPlainOrder.EntitiesFX;
                    img.Alpha.Alpha -= 0.05f;
                    img.SelectedFrame = (ImagePaths.ThrustFrame, 0, 9);
                    continue;
                }
                if (img.SelectedFrame == (ImagePaths.ThrustFrame, 0, 9))
                {
                    img.LayerDepth = DrawingPlainOrder.EntitiesFX;
                    img.Alpha.Alpha -= 0.05f;
                    img.SelectedFrame = (ImagePaths.ThrustFrame, 1, 9);
                    continue;
                }
                if (img.SelectedFrame == (ImagePaths.ThrustFrame, 1, 9))
                {
                    img.LayerDepth = DrawingPlainOrder.EntitiesFX;
                    img.Alpha.Alpha -= 0.05f;
                    img.SelectedFrame = (ImagePaths.ThrustFrame, 2, 9);
                    // ScreenService.Instance.Sounds.Play($"SynthHitElectro0{Randomness.Instance.From(0, 2)}");
                    continue;
                }
                if (img.SelectedFrame == (ImagePaths.ThrustFrame, 2, 9))
                {
                    img.LayerDepth = DrawingPlainOrder.EntitiesFX;
                    img.Alpha.Alpha -= 0.05f;
                    img.SelectedFrame = (ImagePaths.ThrustFrame, 3, 9);
                    continue;
                }
                if (img.SelectedFrame == (ImagePaths.ThrustFrame, 3, 9))
                {
                    img.LayerDepth = DrawingPlainOrder.EntitiesFX;
                    img.Alpha.Alpha -= 0.05f;
                    img.SelectedFrame = (ImagePaths.ThrustFrame, 4, 9);
                    continue;
                }
                if (img.SelectedFrame == (ImagePaths.ThrustFrame, 4, 9))
                {
                    img.LayerDepth = DrawingPlainOrder.EntitiesFX;
                    img.Alpha.Alpha -= 0.05f;
                    img.SelectedFrame = (ImagePaths.ThrustFrame, 5, 9);
                    continue;
                }
                if (img.SelectedFrame == (ImagePaths.ThrustFrame, 5, 9))
                {
                    img.LayerDepth = DrawingPlainOrder.EntitiesFX;
                    img.Alpha.Alpha -= 0.05f;
                    img.SelectedFrame = (ImagePaths.ThrustFrame, 6, 9);
                    continue;
                }
                if (img.SelectedFrame == (ImagePaths.ThrustFrame, 6, 9))
                {
                    img.LayerDepth = DrawingPlainOrder.EntitiesFX;
                    img.Alpha.Alpha -= 0.05f;
                    img.SelectedFrame = (ImagePaths.ThrustFrame, 7, 9);
                    continue;
                }
                if (img.SelectedFrame == (ImagePaths.ProjectileFrame, 4, 7))
                {
                    img.SelectedFrame = (ImagePaths.ProjectileFrame, 0, 8);
                    ScreenService.Instance.Sounds.Play($"SynthBoomElector0{Randomness.Instance.From(0, 2)}");//{Randomness.Instance.From(0, 1)}");
                    continue;
                }

                if (img.SelectedFrame == (ImagePaths.ProjectileFrame, 0, 8))
                {
                    img.SelectedFrame = (ImagePaths.ProjectileFrame, 1, 8);
                    continue;
                }

                if (img.SelectedFrame == (ImagePaths.ProjectileFrame, 2, 8))
                {
                    img.SelectedFrame = (ImagePaths.ProjectileFrame, 3, 8);
                    continue;
                }

                img.UnloadContent();
            }
                        
            machine.SharedContext.Images = newImgBundle;
            machine.SharedContext.Images.AddRange(imagesToDispose.Where(img => !img.Disposed));
        }
    }
}