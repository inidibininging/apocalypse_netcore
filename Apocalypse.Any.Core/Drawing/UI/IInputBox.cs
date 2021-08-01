using Apocalypse.Any.Core.Text;

namespace Apocalypse.Any.Core.Drawing.UI
{
    public interface IInputBox : IUIElement
    {
        VisualText Text { get; set; }

        void OnTextChanged(IGameObject sender, string changedText);
    }
}