using System;
using System.Collections.Generic;
using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Drawing.UI;
using Apocalypse.Any.Core.Text;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Apocalypse.Any.Domain.Common.Drawing.UI
{
    public class ApocalypseButton<TContext> 
        : ApocalypseRectangle<TContext>, IUIEvents 
    {
        private VisualText UnderlyingText { get; set; } = new VisualText();
        public ApocalypseButton(Dictionary<(int frame, int x, int y), Rectangle> frames) : base(frames)
        {
        }

        public string Text { get; set; }

        #region Internal
        public override void Initialize()
        {
            
            base.Initialize();
        }

        public override void LoadContent(ContentManager manager)
        {
            UnderlyingText.LoadContent(manager);
            base.LoadContent(manager);
        }

        public override void UnloadContent()
        {
            UnderlyingText.UnloadContent();
            base.UnloadContent();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            UnderlyingText.Draw(spriteBatch);
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime time)
        {
            UpdateUnderlyingText(time);

            base.Update(time);
        }

        private void UpdateUnderlyingText(GameTime time)
        {
            //Now the text position depends on where the position of the cell is
            var textLength = UnderlyingText.TextLength();
            const float margin = 48f;

            
            var adjustedScale = (X: (Width * Scale.X) / (textLength.X + margin),
                Y: (Height * Scale.Y) / (textLength.Y + margin + 16f));

            //set the text position to the underlying image center
            if (ParentPosition == null)
            {
                UnderlyingText.Position.X = Position.X + ((textLength.X * adjustedScale.X) / 2);
                UnderlyingText.Position.Y = Position.Y + ((textLength.Y * adjustedScale.Y) / 2);
            }
            else
            {
                UnderlyingText.Position.X = Position.X + ((textLength.X * adjustedScale.X) / 2) + ParentPosition.X;
                UnderlyingText.Position.Y = Position.Y + ((textLength.Y * adjustedScale.Y) / 2) + ParentPosition.Y;
            }

            UnderlyingText.Text = Text;

            //adjust underlying text scale to the box size
            UnderlyingText.Scale = new Vector2(adjustedScale.X, adjustedScale.Y);
            UnderlyingText.LayerDepth = LayerDepth + DrawingPlainOrder.PlainStep;
            UnderlyingText.Update(time);
        }

        #endregion

        public virtual void OnClick(object sender, EventArgs args)
        {
            Console.WriteLine("button pressed");
            
        }

        public virtual void OnMouseEnter(object sender, EventArgs args)
        {
        }

        public void OnMouseLeave(object sender, EventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}