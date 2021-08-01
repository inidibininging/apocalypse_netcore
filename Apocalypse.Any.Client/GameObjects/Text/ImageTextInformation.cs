using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Text;

namespace Apocalypse.Any.Client.GameObjects.Text
{
    public class ImageTextInformation : GenericInfoText<AnimatedImage>
    {
        public ImageTextInformation(AnimatedImage target) : base(target)
        {
        }

        public override string GetText()
        {
            return $@"
Alpha:{Target.Alpha.Alpha}
Position:{Target.Position.X},{Target.Position.Y}
Rotation:{Target.Rotation.Rotation}";
        }
    }
}