namespace Apocalypse.Any.Core.Drawing.UI
{
    public interface IWindow : IImage, IGameObjectDictionary, IUIElement
    {
        void Close();

        void Show();

        void ShowDialog();
    }
}