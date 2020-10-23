using System;
using Apocalypse.Any.Core.Behaviour;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Core.Drawing
{
    public interface IImageData : 
        IFullPositionHolder, 
        ISizeHolder, 
        IColorChannelHolder, 
        IScaleHolder,
        ILayerDepthHolder
    {
        (int frame, int x, int y)  SelectedFrame { get; set; }
        int Path { get; set; }
    }
}