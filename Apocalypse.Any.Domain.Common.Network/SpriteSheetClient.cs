﻿using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Domain.Common.Model.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Apocalypse.Any.Domain.Common.Network
{
    public class SpriteSheetClient :
        SpriteSheet
    {
        public ImageData ServerData { get; private set; }

        public SpriteSheetClient(
            ImageData data,
            Dictionary<(int frame,int x, int y), Rectangle> registeredFrames) : base(registeredFrames)
        {
            ServerData = data;
            ApplyImageData(data);
        }

        public void ApplyImageData(ImageData data)
        {
            ServerData = data;
            Alpha = data.Alpha;
            Color = data.Color;

            //lets ignore this for now
            //Width = data.Width;
            //Height = data.Height;

            LayerDepth = data.LayerDepth;
            if (data.Path != Path)
                Path = data.Path;
            Position = data.Position;
            Rotation = data.Rotation;
            Scale = data.Scale;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Position == null)
                return;
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime time)
        {
            //Data validation
            var currentImageData = (this as IImageData);
            var data = new ImageData();

            //code smells here
            if (ServerData?.Id == null)
            {
                //first registration
                data.Alpha = currentImageData.Alpha;
                data.Color = currentImageData.Color;
                data.Height = currentImageData.Height;
                data.Width = currentImageData.Width;
                data.LayerDepth = currentImageData.LayerDepth;
                data.Path = currentImageData.Path;
                data.Position = currentImageData.Position;
                data.Rotation = currentImageData.Rotation;
                data.Scale = currentImageData.Scale;
                data.SelectedFrame = this.SelectedFrame;
            }
            else
            {
                Alpha = ServerData.Alpha;
                Color = ServerData.Color;
                LayerDepth = ServerData.LayerDepth;
                Path = ServerData.Path;
                Position = ServerData.Position;
                Rotation = ServerData.Rotation;
                Scale = ServerData.Scale;
                SelectedFrame = ServerData.SelectedFrame;
            }

            base.Update(time);
        }
    }
}