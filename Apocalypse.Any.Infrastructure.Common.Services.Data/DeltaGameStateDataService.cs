using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Data.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Apocalypse.Any.Constants;

namespace Apocalypse.Any.Infrastructure.Common.Services.Data
{
    public class DeltaGameStateDataService : IDeltaGameStateDataService
    {
        const float FloatTolerance = 0.2f;
        public DeltaGameStateData GetDelta(GameStateData gameStateDataBefore, GameStateData gameStateDataAfter)
        {
            if (gameStateDataBefore.LoginToken != gameStateDataAfter.LoginToken)
                return new DeltaGameStateData();

            var deltaGameState = new DeltaGameStateData
            {
                Id = Guid.NewGuid().ToString(),
                LoginToken = gameStateDataAfter.LoginToken,
                CameraX = null,
                CameraY = null,
                CameraRotation = null,
                Images = GetDeltaImages(gameStateDataBefore, gameStateDataAfter),
                Sounds = new List<int>(),
                ScreenHeight = null,
                ScreenWidth = null,
                Commands = new List<string>()
            };

            //write delta of cam settings
            if (Math.Abs(gameStateDataBefore.Camera.Position.X - gameStateDataAfter.Camera.Position.X) > 0.1f)
                deltaGameState.CameraX = gameStateDataAfter.Camera.Position.X;
            if (Math.Abs(gameStateDataBefore.Camera.Position.Y - gameStateDataAfter.Camera.Position.Y) > 0.01f)
                deltaGameState.CameraY = gameStateDataAfter.Camera.Position.Y;
            if (gameStateDataBefore.Camera.Rotation != gameStateDataAfter.Camera.Rotation)
                deltaGameState.CameraRotation = gameStateDataAfter.Camera.Rotation.Rotation;
            if (gameStateDataBefore.Screen.ScreenHeight != gameStateDataAfter.Screen.ScreenHeight)
                deltaGameState.ScreenHeight = gameStateDataAfter.Screen.ScreenHeight;
            if (gameStateDataBefore.Screen.ScreenWidth != gameStateDataAfter.Screen.ScreenWidth)
                deltaGameState.ScreenWidth = gameStateDataAfter.Screen.ScreenWidth;

            return deltaGameState;
        }

        private static List<DeltaImageData> GetDeltaImages(GameStateData gameStateDataBefore, GameStateData gameStateDataAfter)
        {
            //new images
            var newImages = gameStateDataAfter
                                .Images
                                .Select(img => img.Id)
                                .Except(gameStateDataBefore
                                            .Images
                                            .Select(imgBefore => imgBefore.Id));

            var imagesFoundInBeforeAndAfter = gameStateDataAfter
                                                .Images
                                                .Select(img => img.Id)
                                                .Intersect
                                                (
                                                    gameStateDataBefore.Images.Select(img => img.Id)
                                                );    
            var images = new List<DeltaImageData>();

            //add delta of changed images
            foreach (var imgAfter in gameStateDataAfter
                                .Images
                                .Where(img => imagesFoundInBeforeAndAfter.Contains(img.Id)))
            {
                var imgBefore = gameStateDataBefore.Images.FirstOrDefault(imgB => imgB.Id == imgAfter.Id);
                if(imgBefore == null)
                {
                    images.Add((DeltaImageData)imgAfter);
                }
                else
                {
                    images.Add(new DeltaImageData()
                    {
                        Id = imgAfter.Id,
                        Alpha = Math.Abs(imgAfter.Alpha.Alpha - imgBefore.Alpha.Alpha) > FloatTolerance ? (float?)imgAfter.Alpha.Alpha : null,
                        Width = Math.Abs(imgAfter.Width - imgBefore.Width) > FloatTolerance ? (float?)imgAfter.Width : null,
                        Height = Math.Abs(imgAfter.Height - imgBefore.Height) > FloatTolerance ? (float?)imgAfter.Height : null,
                        LayerDepth = Math.Abs(imgAfter.LayerDepth - imgBefore.LayerDepth) > FloatTolerance ? (float?)imgAfter.LayerDepth : null,
                        
                        X = Math.Abs(imgAfter.Position.X - imgBefore.Position.X) > FloatTolerance ? (float?)imgAfter.Position.X : null,
                        Y = Math.Abs(imgAfter.Position.Y - imgBefore.Position.Y) > FloatTolerance ? (float?)imgAfter.Position.Y : null,
                        Rotation = Math.Abs(imgAfter.Rotation.Rotation - imgBefore.Rotation.Rotation) > FloatTolerance ? (float?)imgAfter.Rotation.Rotation : null,
                        
                        ScaleX = Math.Abs(imgAfter.Scale.X - imgBefore.Scale.X) > FloatTolerance ? (float?)imgAfter.Scale.X : null,
                        ScaleY = Math.Abs(imgAfter.Scale.Y - imgBefore.Scale.Y) > FloatTolerance ? (float?)imgAfter.Scale.Y : null,
                        SelectedFrame = imgAfter.SelectedFrame != imgBefore.SelectedFrame ? imgAfter.SelectedFrame : (ImagePaths.UndefinedFrame, 0, 0),
                        Path = imgAfter.Path != imgBefore.Path ? imgAfter.Path : ImagePaths.empty,
                        R = imgAfter.Color.R != imgBefore.Color.R ? (byte?)imgAfter.Color.R : null,
                        G = imgAfter.Color.G != imgBefore.Color.G ? (byte?)imgAfter.Color.G : null,
                        B = imgAfter.Color.B != imgBefore.Color.B ? (byte?)imgAfter.Color.B : null
                    });
                }
                
            }

            //add new images
            if (newImages.Any())
                images.AddRange(gameStateDataAfter
                                    .Images
                                    .Where(i => newImages.Contains(i.Id))
                                    .Select(i => (DeltaImageData)i));

            return images;
        }

        private static GameStateData GetChangedImagesFromDelta(GameStateData gameStateDataBefore, DeltaGameStateData gameStateDataAfter)
        {
            //update shared images
            var images = from img in gameStateDataBefore.Images
                    join imgDelta in gameStateDataAfter.Images
                    on img.Id equals imgDelta.Id
                    select (id: img.Id,
                            imgBefore: img,
                            delta: imgDelta);

            var newImages = from imgAfter in gameStateDataAfter.Images
                            where
                            !(from img
                              in gameStateDataBefore.Images
                              select img.Id).Contains(imgAfter.Id)
                            select (ImageData)imgAfter;

            //first remove unnecessary images from the "before" package
            var imagesToRemove = gameStateDataBefore
                                    .Images
                                    .Where(afterImageAsDelta => !images
                                                                .Select(g => g.id)
                                                                .Concat(newImages.Select(i => i.Id))
                                                                .Contains(afterImageAsDelta.Id));
            gameStateDataBefore.Images = gameStateDataBefore
                                        .Images
                                        .Except(imagesToRemove)
                                        .ToList();
                                        


            
            foreach (var imgPack in images)
            {
                if (imgPack.delta.Id == null)
                    continue;

                imgPack.imgBefore.Id = imgPack.delta.Id;
                imgPack.imgBefore.Alpha.Alpha = imgPack.delta.Alpha.HasValue && Math.Abs(imgPack.delta.Alpha.Value - imgPack.imgBefore.Alpha.Alpha) > FloatTolerance ? imgPack.delta.Alpha.Value : imgPack.imgBefore.Alpha.Alpha;
                imgPack.imgBefore.Width = imgPack.delta.Width.HasValue && Math.Abs(imgPack.delta.Width.Value - imgPack.imgBefore.Width) > FloatTolerance ? imgPack.delta.Width.Value : imgPack.imgBefore.Width;
                imgPack.imgBefore.Height = imgPack.delta.Height.HasValue && Math.Abs(imgPack.delta.Height.Value - imgPack.imgBefore.Height) > FloatTolerance ? imgPack.delta.Height.Value : imgPack.imgBefore.Height;
                imgPack.imgBefore.LayerDepth = imgPack.delta.LayerDepth.HasValue && Math.Abs(imgPack.delta.LayerDepth.Value - imgPack.imgBefore.LayerDepth) > FloatTolerance ? imgPack.delta.LayerDepth.Value : imgPack.imgBefore.LayerDepth;
                
                imgPack.imgBefore.Rotation.Rotation = imgPack.delta.Rotation.HasValue && Math.Abs(imgPack.delta.Rotation.Value - imgPack.imgBefore.Rotation.Rotation) > FloatTolerance ? imgPack.delta.Rotation.Value : imgPack.imgBefore.Rotation.Rotation;
                imgPack.imgBefore.Position.X = imgPack.delta.X.HasValue && Math.Abs(imgPack.delta.X.Value - imgPack.imgBefore.Position.X) > FloatTolerance ? imgPack.delta.X.Value : imgPack.imgBefore.Position.X;
                imgPack.imgBefore.Position.Y = imgPack.delta.Y.HasValue && Math.Abs(imgPack.delta.Y.Value - imgPack.imgBefore.Position.Y) > FloatTolerance ? imgPack.delta.Y.Value : imgPack.imgBefore.Position.Y;
                
                imgPack.imgBefore.Path = imgPack.delta.Path != ImagePaths.empty && imgPack.delta.Path != imgPack.imgBefore.Path ? imgPack.delta.Path : imgPack.imgBefore.Path;
                imgPack.imgBefore.SelectedFrame = imgPack.delta.SelectedFrame.frame != ImagePaths.UndefinedFrame && imgPack.delta.SelectedFrame != imgPack.imgBefore.SelectedFrame ? imgPack.delta.SelectedFrame : imgPack.imgBefore.SelectedFrame;
                var scaleXChanged = imgPack.delta.ScaleX.HasValue && Math.Abs(imgPack.delta.ScaleX.Value - imgPack.imgBefore.Scale.X) > FloatTolerance;
                var scaleYChanged = imgPack.delta.ScaleY.HasValue && Math.Abs(imgPack.delta.ScaleY.Value - imgPack.imgBefore.Scale.Y) > FloatTolerance;
                if (scaleXChanged || scaleYChanged)
                {
                    imgPack.imgBefore.Scale = new Microsoft.Xna.Framework.Vector2(
                        scaleXChanged ? imgPack.delta.ScaleX.Value : imgPack.imgBefore.Scale.X,
                        scaleYChanged ? imgPack.delta.ScaleY.Value : imgPack.imgBefore.Scale.Y
                        );
                }
                var colorRChanged = imgPack.delta.R.HasValue && imgPack.delta.R.Value != imgPack.imgBefore.Color.R;
                var colorGChanged = imgPack.delta.G.HasValue && imgPack.delta.G.Value != imgPack.imgBefore.Color.G;
                var colorBChanged = imgPack.delta.B.HasValue && imgPack.delta.B.Value != imgPack.imgBefore.Color.B;

                if (colorRChanged || colorGChanged || colorBChanged)
                {
                    imgPack.imgBefore.Color = new Microsoft.Xna.Framework.Color(
                        colorRChanged ? imgPack.delta.R.Value : imgPack.imgBefore.Color.R,
                        colorGChanged ? imgPack.delta.G.Value : imgPack.imgBefore.Color.G,
                        colorBChanged ? imgPack.delta.B.Value : imgPack.imgBefore.Color.B
                        );
                }
            }

            

            if ((newImages = newImages.Where(i => i.Path != null)).Any())
            {
                gameStateDataBefore.Images.AddRange(newImages);
            }






            return gameStateDataBefore;
        }

        public GameStateData UpdateGameStateData(GameStateData gameStateData, DeltaGameStateData deltaGameStateData)
        {

            if (deltaGameStateData.LoginToken != gameStateData.LoginToken)
                return gameStateData;
            if (deltaGameStateData.CameraX != null && gameStateData.Camera.Position.X != deltaGameStateData.CameraX)
                gameStateData.Camera.Position.X = deltaGameStateData.CameraX.Value;
            if (deltaGameStateData.CameraY != null && gameStateData.Camera.Position.Y != deltaGameStateData.CameraY)
                gameStateData.Camera.Position.Y = deltaGameStateData.CameraY.Value;
            if (deltaGameStateData.CameraRotation != null && gameStateData.Camera.Rotation.Rotation != deltaGameStateData.CameraRotation)
                gameStateData.Camera.Rotation.Rotation = deltaGameStateData.CameraRotation.Value;

            if (deltaGameStateData.ScreenWidth != null && gameStateData.Screen.ScreenWidth != deltaGameStateData.ScreenWidth)
                gameStateData.Screen.ScreenWidth = deltaGameStateData.ScreenWidth.Value;
            if (deltaGameStateData.ScreenHeight != null && gameStateData.Screen.ScreenHeight != deltaGameStateData.ScreenHeight)
                gameStateData.Screen.ScreenHeight = deltaGameStateData.ScreenHeight.Value;

            gameStateData = GetChangedImagesFromDelta(gameStateData, deltaGameStateData);
            
            return gameStateData;
        }
    }
}
