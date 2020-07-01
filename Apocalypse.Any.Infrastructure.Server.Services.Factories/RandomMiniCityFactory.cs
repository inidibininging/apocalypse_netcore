using System;
using System.Collections.Generic;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories {
    public class RandomMiniCityFactory : CheckWithReflectionFactoryBase<IEnumerable<ImageData>> {
        public RandomMiniCityFactory (RandomTilesetPartFactory streetCenterMaker, RandomTilesetPartFactory streetHorizontalMaker, RandomTilesetPartFactory streetVerticalMaker, int chancePercentageToRecall, int chanceOfHorizontalOrVertical, int chancePercentageOfLeftRight, int chancePercentageOfUpDown) {
            this.StreetCenterMaker = streetCenterMaker;
            this.StreetHorizontalMaker = streetHorizontalMaker;
            this.StreetVerticalMaker = streetVerticalMaker;
            this.ChancePercentageToRecall = chancePercentageToRecall;
            this.ChanceOfHorizontalOrVertical = chanceOfHorizontalOrVertical;
            this.ChancePercentageOfLeftRight = chancePercentageOfLeftRight;
            this.ChancePercentageOfUpDown = chancePercentageOfUpDown;

        }
        private RandomTilesetPartFactory StreetCenterMaker { get; }
        private RandomTilesetPartFactory StreetHorizontalMaker { get; }
        private RandomTilesetPartFactory StreetVerticalMaker { get; }
        public int ChancePercentageToRecall { get; set; } = 50;
        public int ChanceOfHorizontalOrVertical { get; set; } = 50;
        public int ChancePercentageOfLeftRight { get; set; } = 50;
        public int ChancePercentageOfUpDown { get; set; } = 50;
        public override bool CanUse<TParam> (TParam instance) => CanUseByTType<TParam, IGameSectorBoundaries> ();

        protected override IEnumerable<ImageData> UseConverter<TParam> (TParam parameter) {
            var sectorBoundaries = parameter as IGameSectorBoundaries;
            var x = Randomness.Instance.From (sectorBoundaries.MinSectorX, sectorBoundaries.MaxSectorX);
            var y = Randomness.Instance.From (sectorBoundaries.MinSectorY, sectorBoundaries.MaxSectorY);
            foreach (var currentImage in GenerateFromCenter(new MovementBehaviour() {
                X = x,
                Y = y
            }))
            {
                yield return currentImage;
            }

        }
        private IEnumerable<ImageData> GenerateFromCenter (MovementBehaviour position) {
            var streetCenter = StreetCenterMaker.Create (position);
            yield return streetCenter;
            int sizeOfWidth = Randomness.Instance.From (1, 10);
            int sizeOfHeight = Randomness.Instance.From (1, 10);

            var lastLeftPosition = streetCenter.Position.X + streetCenter.Width;
            var lastRightPosition = streetCenter.Position.X - streetCenter.Width;
            var lastUpPosition = streetCenter.Position.Y + streetCenter.Height;
            var lastDownPosition = streetCenter.Position.Y - streetCenter.Height;

            while (Randomness.Instance.TrueOrFalse ()) {
                for (var currentSizeOfWidth = 0; currentSizeOfWidth < sizeOfWidth; currentSizeOfWidth++) {
                    if (Randomness.Instance.TrueOrFalse ()) {
                        lastLeftPosition -= streetCenter.Width;
                        yield return StreetHorizontalMaker.Create (
                            new MovementBehaviour () {
                                X = lastLeftPosition,
                                    Y = streetCenter.Position.Y
                            });
                    } else {
                        lastRightPosition += streetCenter.Width;
                        yield return StreetHorizontalMaker.Create (
                            new MovementBehaviour () {
                                X = lastRightPosition,
                                    Y = streetCenter.Position.Y
                            });
                    }
                }

                for (var currentSizeOfHeight = 0; currentSizeOfHeight < sizeOfHeight; currentSizeOfHeight++) {
                    if (Randomness.Instance.TrueOrFalse ()) {
                        lastUpPosition += streetCenter.Height;
                        yield return StreetVerticalMaker.Create (
                            new MovementBehaviour () {
                                X = streetCenter.Position.X,
                                    Y = lastUpPosition
                            });
                    } else {
                        lastDownPosition -= streetCenter.Height;
                        yield return StreetVerticalMaker.Create (
                            new MovementBehaviour () {
                                X = streetCenter.Position.X,
                                    Y = lastDownPosition
                            });
                    }
                }
            }
        }
    }
}