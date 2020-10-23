using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Apocalypse.Any.Constants;

namespace Apocalypse.Any.Core.Drawing
{
    /// <summary>
    /// A spritesheet representation of an image (Texture2d actually)
    /// </summary>
    public class SpriteSheet : Image
    {
        public Dictionary<(int frame,int x, int y), Rectangle> SpriteSheetRectangle { get; private set; }
        //this is a good idea for good constants :D
        //private const string DefaultSelectedFrameName = nameof(DefaultSelectedFrameName);

        private (int frame,int x, int y) selectedFrame;

        public new (int frame,int x, int y) SelectedFrame
        {
            get
            {
                return selectedFrame;
            }
            set
            {
                //Console.WriteLine(value);
                if (value.frame != ImagePaths.UndefinedFrame && SpriteSheetRectangle.ContainsKey(value))
                {
                    
                    selectedFrame = value;
                    Origin = Vector2.Zero;
                }
                    
            }
        }

        public SpriteSheet(Dictionary<(int frame,int x, int y), Rectangle> spriteSheetRectangle) : base()
        {
            SpriteSheetRectangle = spriteSheetRectangle ?? new Dictionary<(int frame,int x, int y), Rectangle>();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (SelectedFrame.frame != ImagePaths.UndefinedFrame)
                SourceRect = SpriteSheetRectangle[SelectedFrame];
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
        }
    }
}