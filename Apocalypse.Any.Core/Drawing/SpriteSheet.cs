using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Apocalypse.Any.Core.Drawing
{
    /// <summary>
    /// A spritesheet representation of an image (Texture2d actually)
    /// </summary>
    public class SpriteSheet : Image
    {
        public Dictionary<string, Rectangle> SpriteSheetRectangle { get; private set; }
        //this is a good idea for good constants :D
        //private const string DefaultSelectedFrameName = nameof(DefaultSelectedFrameName);

        private string selectedFrame;

        public new string SelectedFrame
        {
            get
            {
                return selectedFrame;
            }
            set
            {
                //Console.WriteLine(value);
                if (!string.IsNullOrEmpty(value) && SpriteSheetRectangle.ContainsKey(value))
                    selectedFrame = value;
            }
        }

        public SpriteSheet(Dictionary<string, Rectangle> spriteSheetRectangle) : base()
        {
            SpriteSheetRectangle = spriteSheetRectangle ?? new Dictionary<string, Rectangle>();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!string.IsNullOrEmpty(SelectedFrame))
                SourceRect = SpriteSheetRectangle[SelectedFrame];
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
        }
    }
}