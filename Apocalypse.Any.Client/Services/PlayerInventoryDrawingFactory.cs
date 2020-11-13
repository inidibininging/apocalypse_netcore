using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.Client.Services.Creation
{
    /// <summary>
    /// Draws the players inventory as a grid
    /// TODO: abstract this. this can be used with all image types and is not actually bound to an inventory
    /// </summary>
    public class PlayerInventoryDrawingFactory
    {
        private static List<ImageClient> _emptyList = new List<ImageClient>();
        public string IdPrefix { get; set; } = "inventory";

        public static IEnumerable<ImageClient> CreateInventoryGrid
        (
            IImage currentPositionHolder,
            IEnumerable<ImageClient> inventory
        )
        {
            inventory = inventory.ToList();
            var startingPosition = currentPositionHolder.Position;

            var posHolderPosX = startingPosition.X + 0;
            var posHolderPosY = startingPosition.Y + 0;

            var columnCount = (currentPositionHolder.Scale.Y / 32) - 1 ;
            var rowCount = (int)MathF.Round(inventory.Count() / columnCount);

            if ((inventory.Count() % columnCount > 0))
                rowCount += 1;

            //so much magic numbers here!
            var maxWidth = inventory
                            .Select(i => i.Width)
                            .Max();
            var maxHeight = inventory
                            .Select(i => i.Height)
                            .Max();

            var offsetX = 16;
            var offsetY = 16;
            var startingGridPosX = posHolderPosX + offsetX;
            var startingGridPosY = posHolderPosY + offsetY;

            //this only works with same size inventory
            using (var itemEnumerator = inventory.GetEnumerator())
            {
                for (var indexX = 0; indexX < rowCount; indexX++)
                {
                    for (var indexY = 0; indexY < columnCount; indexY++)
                    {
                        var isFinal = !itemEnumerator.MoveNext();
                        if (isFinal)
                            continue;

                        var item = itemEnumerator.Current;

                        var itemPosX = (indexX * maxWidth) + startingGridPosX;
                        var itemPosY = (indexY * maxHeight) + startingGridPosY;

                        //copy image data with proper grid position
                        //add to draw list
                        if (item == null) continue;
                        item.Position = new MovementBehaviour()
                        {
                            X = itemPosX,
                            Y = itemPosY,
                        };
                        item.LayerDepth = DrawingPlainOrder.UI + (DrawingPlainOrder.PlainStep * 2);
                        yield return item;
                    }
                }
            }
        }
    }
}