using Apocalypse.Any.Core.Text;

namespace Apocalypse.Any.Core.Drawing.UI
{
    public interface IButton : IUIElement
    {
        VisualText Text { get; set; }
    }
}