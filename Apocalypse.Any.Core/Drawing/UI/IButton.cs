using Apocalypse.Any.Core.Text;

namespace Apocalypse.Any.Core.Drawing.UI
{
    public interface IButton : UIElement
    {
        VisualText Text { get; set; }
    }
}