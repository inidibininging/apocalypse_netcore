using Apocalypse.Any.Domain.Common.Model.Network;
using System.Collections.Generic;

namespace Apocalypse.Any.Infrastructure.Common.Services.Data.Interfaces
{
    public interface IDeltaGameStateDataService
    {
        DeltaGameStateData GetDelta(GameStateData gameStateDataBefore, GameStateData gameStateDataAfter);
        GameStateData UpdateGameStateData(GameStateData gameStateData, DeltaGameStateData deltaGameStateData);
        IEnumerable<ImageData> GetImagesToRemove(IEnumerable<ImageData> images, IEnumerable<DeltaImageData> deltaImages);
        IEnumerable<ImageData> GetNewImagesFromDelta(IEnumerable<ImageData> images, IEnumerable<DeltaImageData> deltaImages);
        IEnumerable<(string imageId, ImageData imageData, DeltaImageData deltaImage)> GetSharedImagesWithDelta(IEnumerable<ImageData> images, IEnumerable<DeltaImageData> deltaImages);
    }
}