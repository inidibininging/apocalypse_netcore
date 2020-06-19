using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Text;

namespace Apocalypse.Any.Core.Utilities
{
    public static class VisualTextExtensions
    {
        public static VisualText MakeVisualText(this string obj, MovementBehaviour movement, string fontName = null)
        {
            return new VisualText(fontName) { Text = obj };
        }
    }
}