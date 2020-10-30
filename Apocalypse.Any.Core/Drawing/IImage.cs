using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apocalypse.Any.Core.Drawing
{
    /// <summary>
    /// Describes an object that has the properties of an image/texture
    /// </summary>
	public interface IImage
        : IVisualGameObject,
            IUpdateableLite,
        IFullPositionHolder,
        IColorChannelHolder,
        IScaleHolder,
        ILayerDepthHolder
    {
        /// <summary>
        /// The current texture that is used. For now the setter is public.
        /// This might change in the future, due to the fact that the game object should only modify itself or by a game object that inherits it
        /// </summary>
        Texture2D Texture
        {
            get;
            set;
        }

        /// <summary>
        /// The actual image size as a rectangle. See Texture for future modifications
        /// </summary>
        Rectangle SourceRect
        {
            get;
            set;
        }

        /// <summary>
        /// Represents the image path. Warning! The image should be in the pipe line. Otherwise the whole game will stop working
        /// </summary>
        int Path
        {
            get;
            set;
        }
        
    }
}