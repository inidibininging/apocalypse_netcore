using Apocalypse.Any.Core.Text;

namespace Apocalypse.Any.Core.Drawing.UI
{
    public interface IInputBox : UIElement
    {
        VisualText Text { get; set; }

        void OnTextChanged(IGameObject sender, string changedText);
    }
}