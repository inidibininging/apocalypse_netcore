namespace Apocalypse.Any.Core.Drawing
{
    public interface ILayerDepthHolder
    {
        /// <summary>
        /// Represents the layer depth where the image will be rendered aka. Z-Order
        /// </summary>
        float LayerDepth { get; set; }
    }
}