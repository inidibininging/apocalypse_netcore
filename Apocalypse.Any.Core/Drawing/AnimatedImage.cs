using Apocalypse.Any.Collections;
using Apocalypse.Any.Core.Utilities;
using Microsoft.Xna.Framework;
using System;

namespace Apocalypse.Any.Core.Drawing
{
    /// <summary>
    /// This class is actually a list of images that are circulated till infinity and displayed through the draw method.
    /// </summary>
    public class AnimatedImage : Image
    {
        private double _seconds;

        public double SecondsBetweenImage
        {
            get
            {
                return _seconds;
            }
            set
            {
                if (value == 0)
                {
                    throw new ArgumentNullException(nameof(SecondsBetweenImage)); // Specify Exception
                }
                _seconds = value;
            }
        }

        public AnimatedImage() : base()
        {
            SecondsBetweenImage = 5;
        }

        public void Add(int imagePath)
        {
            ImagePaths.Add(imagePath);
        }

        public void RemoveAt(int index)
        {
            ImagePaths.RemoveAt(index);
        }

        protected CircularQueue<int> ImagePaths { get; set; } = new CircularQueue<int>();

        private int NextIndex { get; set; } = 0;

        private TimeSpan NextImageTime { get; set; } = TimeSpan.Zero;

        public override void Update(GameTime time)
        {
            if (NextImageTime == TimeSpan.Zero)
                NextImageTime = time.TotalGameTime + SecondsBetweenImage.Seconds();
            if (NextImageTime <= time.TotalGameTime)
            {
                NextImageTime = TimeSpan.Zero;
                var oldPath = Path;
                Path = ImagePaths.MoveNext();
            }

            base.Update(time);
        }
    }
}