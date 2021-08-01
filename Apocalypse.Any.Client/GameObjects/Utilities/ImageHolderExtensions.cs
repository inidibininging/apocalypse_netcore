using System;
using Apocalypse.Any.Client.GameObjects.Text;
using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Drawing;

namespace Apocalypse.Any.Client.GameObjects.Utilities
{
    public static class ImageHolderExtensions
    {
        public static void AddDebugInfo<T>(this T self)
            where T : IImageHolder<AnimatedImage>, IGameObjectDictionary
        {
            self.Add($"{Guid.NewGuid().ToString()}_debug_info", new ImageTextInformation(self.CurrentImage));
        }
    }
}