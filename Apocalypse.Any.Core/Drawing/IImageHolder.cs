namespace Apocalypse.Any.Core.Drawing
{
    /// <summary>
    /// Describes a game object that holds an image as property. The image is accessed through the CurrentImage property
    /// </summary>
	public interface IImageHolder : IImageHolder<IImage> { }

    /// <summary>
    /// /Describes a game object that holds an image
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGameObjectImageHolder<T> : IUpdateableLite, IImageHolder { }

    /// <summary>
    /// Describes an object that holds an image as property. The image is accessed through the CurrentImage property
    /// </summary>
    public interface IImageHolder<T>
        where T : IImage
    {
        T CurrentImage
        {
            get;
        }
    }
}