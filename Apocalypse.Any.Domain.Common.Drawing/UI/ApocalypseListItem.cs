using System.Collections.Generic;
using Apocalypse.Any.Core.Drawing.UI;
using Apocalypse.Any.Core.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Apocalypse.Any.Domain.Common.Drawing.UI
{
    public class ApocalypseListItem<TContext>
        : ApocalypseRectangle<TContext>
    {
        private VisualText UnderlyingText { get; set; } = new VisualText();
        
        public string Text { get; set; }

        /// <summary>
        /// Sets the left margin of the checkbox. For now, this is the only thing needed
        /// </summary>
        public int TextMarginLeft { get; set; } = 0;

        public int TextMarginRight { get; set; } = 16;
        
        public ApocalypseListItem(Dictionary<(int frame, int x, int y), Rectangle> frames)
            : base(frames)
        {
        }

        public ApocalypseListItem(Dictionary<(int frame, int x, int y), Rectangle> frames, TContext context)
            : base(frames, context)
        {
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
            // if (UnderlyingText.Position.X + UnderlyingText.TextLength().X > (ParentPosition?.X ?? 0) + ParentScale.X)
                UnderlyingText.Draw(spriteBatch);    
            
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime time)
        {
            UnderlyingText.Text = Text;
            UnderlyingText.Alpha.Alpha = IsVisible ? 1 : 0; //TODO: Potential bug. If setting the visibility to something else than 0 or 1 will not work!!!
            var textLength = UnderlyingText.TextLength();
            
            //TODO: Reduce this embarrassing code
            UnderlyingText.Position.X = ParentPosition.X + Position.X;
            UnderlyingText.Position.Y = ParentPosition.Y + Position.Y;
	        UnderlyingText.LayerDepth = LayerDepth + DrawingOrder.DrawingPlainOrder.UI;

            var parentSizeX = ParentScale.X;
            var widthBetweenWindowAndCheckbox = UnderlyingText.Position.X - ParentPosition.X;
	     
	        // Scales the text to a specific size if the text overflows the parents size   
	        // TODO: Let the user decide if the text should be scaled. Extract this to a function
	        if (UnderlyingText.Position.X + textLength.X > ParentPosition.X + parentSizeX) {
	            // ParentScale.X is the real Width because of the apocalypse parent pixel size == 1	
            	UnderlyingText.Scale = new Vector2((parentSizeX - (widthBetweenWindowAndCheckbox + TextMarginLeft)) / (UnderlyingText.TextLength().X + TextMarginRight), Scale.Y);
            } 
	        else
	        {
		        UnderlyingText.Scale = new Vector2(0.75f);		   
	        }
            
            UnderlyingText.Position.X = ParentPosition.X + Position.X + SourceRect.Width + (textLength.X * UnderlyingText.Scale.X) + TextMarginLeft;
            UnderlyingText.Position.Y = ParentPosition.Y + Position.Y + (textLength.Y * UnderlyingText.Scale.Y);
            UnderlyingText.Update(time);
            base.Update(time);
        }
    }
}
