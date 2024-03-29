using System.Collections.Generic;
using Apocalypse.Any.Constants;
using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Drawing.UI;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Apocalypse.Any.Domain.Common.Drawing.UI
{
    public class ApocalypseRectangle<TContext> 
        : SpriteSheet, IChildUIElement
    {
        private TContext Context { get; }

        protected ApocalypseRectangle(Dictionary<(int frame, int x, int y), Rectangle> frames) : base(frames)
        {
            PrepareUnderlyingImage(frames);
        }
        protected ApocalypseRectangle(Dictionary<(int frame, int x, int y), Rectangle> frames, TContext context) : base(frames)
        {
            Context = context;
            PrepareUnderlyingImage(frames);
        }

        private void PrepareUnderlyingImage(Dictionary<(int frame, int x, int y), Rectangle> frames)
        {
            Path = ImagePaths.hud_misc_edit;
            SelectedFrame = (ImagePaths.HUDFrame, 0, 0);
            LayerDepth = DrawingPlainOrder.UI;
            ForceDraw = true;
            Position = new MovementBehaviour();
            Color = Color.DarkViolet;
        }

        #region Internal


        public new MovementBehaviour Position { get; set; }

        public override void Update(GameTime time)
        {
            base.Update(time);
            
            //set positions to underlying image
            //TODO: Look for unnecessary parenthesis in the project.
            if (ParentPosition == null) return;
            base.Position.X = ParentPosition.X + Position.X;
            base.Position.Y = ParentPosition.Y + Position.Y;
             
        }
        #endregion

        public MovementBehaviour ParentPosition { get; set; }
        public Vector2 ParentScale { get; set; }
        public float ParentWidth { get; set; }
        public float ParentHeight { get; set; }
        public bool IsVisible
        {
            get => Alpha.Alpha > 0;
            set => Alpha.Alpha = value ? 1 : 0;
        }
    }
}