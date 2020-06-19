namespace Apocalypse.Any.Core.Drawing.UI
{
    public interface IWindow : IImage, IGameObjectDictionary, UIElement
    {
        void Close();

        void Show();

        void ShowDialog();
    }
}