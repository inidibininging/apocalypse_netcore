using Apocalypse.Any.Core.Drawing;
using Microsoft.Xna.Framework;
using System.IO;
using System.Linq;

namespace Apocalypse.Any.Core.Map.Flat
{
    public class MapImage :
    IImageHolder, IMapLocatable
    {
        #region properties

        public const string BasePath = "Image/Map/";

        private IImage _currentImage;

        public IImage CurrentImage
        {
            get { return _currentImage; }
            set { _currentImage = value; }
        }

        public Vector2 LocationUnit
        {
            get;
            set;
        }

        #endregion properties

        public MapImage(int x, int y, params int[] image) : this(x, y, image[0])
        {
            image.ToList().ForEach(imgPath =>
            {
                ((AnimatedImage)CurrentImage).Add(imgPath);
            });
            ((AnimatedImage)CurrentImage).SecondsBetweenImage = 2;
        }

        internal MapImage(int x, int y, int image)
        {
            LocationUnit = new Vector2(x, y);
            CurrentImage = new AnimatedImage() { Path = image };
        }
    }
}