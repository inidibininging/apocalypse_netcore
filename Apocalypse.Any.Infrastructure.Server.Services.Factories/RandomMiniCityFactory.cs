using System;
using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories {
    public class RandomMiniCityFactory : CheckWithReflectionFactoryBase<IEnumerable<ImageData>> {
        private static float CityLayer;
        private const int Up = 0;
        private const int Down = 1;
        private const int Left = 2;
        private const int Right = 3;
        private const int Horizontal = 0;
        private const int Vertical = 1;
        public RandomMiniCityFactory (
            IGenericTypeFactory<ImageData> streetCenterMaker, 
            IGenericTypeFactory<ImageData> streetHorizontalMaker,
            IGenericTypeFactory<ImageData> streetVerticalMaker, 
            IGenericTypeFactory<ImageData> buildingMaker,
            IGenericTypeFactory<ImageData> buildingTopMaker,
            int chancePercentageToRecall, 
            int chanceOfHorizontalOrVertical, 
            int chancePercentageOfLeftRight, 
            int chancePercentageOfUpDown) {
            this.StreetCenterMaker = streetCenterMaker;
            this.StreetHorizontalMaker = streetHorizontalMaker;
            this.StreetVerticalMaker = streetVerticalMaker;
            this.BuildingMaker = buildingMaker;
            this.BuildingTopMaker = buildingTopMaker;
            this.ChancePercentageToRecall = chancePercentageToRecall;
            this.ChanceOfHorizontalOrVertical = chanceOfHorizontalOrVertical;
            this.ChancePercentageOfLeftRight = chancePercentageOfLeftRight;
            this.ChancePercentageOfUpDown = chancePercentageOfUpDown;

        }
        private IGenericTypeFactory<ImageData> StreetCenterMaker { get; }
        private IGenericTypeFactory<ImageData> StreetHorizontalMaker { get; }
        private IGenericTypeFactory<ImageData> StreetVerticalMaker { get; }
        private IGenericTypeFactory<ImageData> BuildingMaker { get; set; }
        private IGenericTypeFactory<ImageData> BuildingTopMaker { get; set; }

        public int ChancePercentageToRecall { get; set; } = 50;
        public int ChanceOfHorizontalOrVertical { get; set; } = 50;
        public int ChancePercentageOfLeftRight { get; set; } = 50;
        public int ChancePercentageOfUpDown { get; set; } = 50;

        private static List<Tuple<int,Vector2>> Nodes { get; set; } = new List<Tuple<int, Vector2>>();
        private static List<Vector2> CenterNodes { get; set; } = new List<Vector2>();

        private static List<Vector2> Buildings { get; set; } = new List<Vector2>();

        public override bool CanUse<TParam> (TParam instance) => CanUseByTType<TParam, IGameSectorBoundaries> ();



        protected override IEnumerable<ImageData> UseConverter<TParam> (TParam parameter) 
        {
            var sectorBoundaries = parameter as IGameSectorBoundaries;
            var divisorX = (int)MathF.Round(sectorBoundaries.MaxSectorX / 1024);
            var divisorY = (int)MathF.Round(sectorBoundaries.MaxSectorX / 1024);
            var x = Randomness.Instance.From(0, divisorX) * 1024; //Randomness.Instance.From(sectorBoundaries.MinSectorX + 200, sectorBoundaries.MaxSectorX - 200);
            var y = Randomness.Instance.From(0, divisorY) * 1024;//Randomness.Instance.From(sectorBoundaries.MinSectorY + 200, sectorBoundaries.MaxSectorY - 200);
            
            var centers = Randomness.Instance.From(2, 12);
            var tileSize = 0;

            while (centers >= 0)
            {
                ImageData streetCenter;
                if(CenterNodes.Count == 0)
                {
                    streetCenter = StreetCenterMaker.Create(new MovementBehaviour()
                    {
                        X = x,
                        Y = y
                    });
                    streetCenter.LayerDepth -= CityLayer;
                    CenterNodes.Add(streetCenter.Position);
                    tileSize = (int)MathF.Round(streetCenter.Width);
                    centers--;
                    yield return streetCenter;
                }
                else
                {
                    var lastNode = Nodes.Last();                    
                    var nextPosition = new MovementBehaviour()
                    {
                        X = lastNode.Item2.X,
                        Y = lastNode.Item2.Y
                    };
                    switch (lastNode.Item1)
                    {
                        case Up:
                            nextPosition.Y -= tileSize;
                            break;
                        case Down:
                            nextPosition.Y += tileSize;
                            break;
                        case Left:
                            nextPosition.X -= tileSize;
                            break;
                        case Right:
                            nextPosition.X += tileSize;
                            break;
                    }
                    if (CenterNodes.Any(t => Vector2.Distance(t, nextPosition) <= tileSize))
                    {
                        centers--;
                        continue;
                    }
                    streetCenter = StreetCenterMaker.Create(nextPosition);
                    streetCenter.LayerDepth -= CityLayer;
                    CenterNodes.Add(streetCenter.Position);
                    centers--;
                    yield return streetCenter;
                }

                var streetSize = Randomness.Instance.From(0, 12);
                while (streetSize-- >= 0)
                {
                    var nextDirection = Randomness.Instance.From(0, 4);
                    switch (nextDirection)
                    {
                        case Up:
                            var streetUp = new MovementBehaviour()
                            {
                                X = streetCenter.Position.X,
                                Y = streetCenter.Position.Y - streetCenter.Height
                            };
                            if (Nodes.Any(t => Vector2.Distance(t.Item2, streetUp) <= tileSize))
                                break;
                            var streetUpImageData = StreetVerticalMaker.Create(streetUp);
                            streetUpImageData.LayerDepth -= CityLayer;
                            Nodes.Add(new Tuple<int, Vector2>(Up, streetUp));
                            yield return streetUpImageData;
                            break;
                        case Down:
                            var streetDown = new MovementBehaviour()
                            {
                                X = streetCenter.Position.X,
                                Y = streetCenter.Position.Y + streetCenter.Height
                            };
                            if (Nodes.Any(t => Vector2.Distance(t.Item2, streetDown) <= tileSize))
                                break;
                            var streetDownImageData = StreetVerticalMaker.Create(streetDown);
                            streetDownImageData.LayerDepth -= CityLayer;
                            Nodes.Add(new Tuple<int, Vector2>(Down, streetDown));
                            yield return streetDownImageData;
                            break;
                        case Left:
                            var streetLeft = new MovementBehaviour()
                            {
                                X = streetCenter.Position.X - streetCenter.Width,
                                Y = streetCenter.Position.Y
                            };
                            if (Nodes.Any(t => Vector2.Distance(t.Item2, streetLeft) <= tileSize))
                                break;
                            var streeLeftImageData = StreetHorizontalMaker.Create(streetLeft);
                            streeLeftImageData.LayerDepth -= CityLayer;
                            Nodes.Add(new Tuple<int, Vector2>(Left, streetLeft));
                            yield return streeLeftImageData;
                            break;
                        case Right:
                        default:
                            var streetRight = new MovementBehaviour()
                            {
                                X = streetCenter.Position.X + streetCenter.Width,
                                Y = streetCenter.Position.Y
                            };
                            if (Nodes.Any(t => Vector2.Distance(t.Item2, streetRight) <= tileSize))
                                break;
                            var streetRightImageData = StreetHorizontalMaker.Create(streetRight);
                            streetRightImageData.LayerDepth -= CityLayer;
                            Nodes.Add(new Tuple<int, Vector2>(Right, streetRight));
                            yield return streetRightImageData;
                            break;
                    }
                }
            }

            //build cities in between
            //get maxed out 
            var buildings = 1;
            while(buildings-- >= 0)
            {
                var atmostUp = Nodes.FirstOrDefault(m => m.Item2.Y == Nodes.Min(n => n.Item2.Y));
                var atmostDown = Nodes.FirstOrDefault(m => m.Item2.Y == Nodes.Max(n => n.Item2.Y));
                var atmostLeft = Nodes.FirstOrDefault(m => m.Item2.X == Nodes.Min(n => n.Item2.X));
                var atmostRight = Nodes.FirstOrDefault(m => m.Item2.X == Nodes.Max(n => n.Item2.X));

                var buildingSize = Randomness.Instance.From(2,5);

                var nextDirection = Randomness.Instance.From(0, 4);                
                var buildingTopCreated = false;

                while (buildingSize-- >= 0)
                {
                    
                    ImageData nextBuildingChunk = null;
                    MovementBehaviour nextPosition = null;
                    switch (nextDirection)
                    {
                        case Up:
                            nextPosition = new MovementBehaviour() 
                            { 
                                X = atmostUp.Item2.X,
                                Y = atmostUp.Item2.Y - (tileSize * buildingSize)
                            };
                            break;
                        case Down:
                            nextPosition = new MovementBehaviour()
                            {
                                X = atmostDown.Item2.X,
                                Y = atmostDown.Item2.Y + (tileSize * buildingSize)
                            };
                            break;
                        case Left:
                            nextPosition = new MovementBehaviour()
                            {
                                X = atmostLeft.Item2.X - (tileSize * buildingSize),
                                Y = atmostLeft.Item2.Y 
                            };
                            break;
                        case Right:
                            nextPosition = new MovementBehaviour()
                            {
                                X = atmostLeft.Item2.X + (tileSize * buildingSize),
                                Y = atmostLeft.Item2.Y
                            };
                            break;
                    }
                    if (Nodes.Any(t => Vector2.Distance(t.Item2, nextPosition) <= tileSize) || CenterNodes.Any(t => Vector2.Distance(t, nextPosition) <= tileSize))
                        continue;
                    if (buildingTopCreated)
                    {                        
                        nextBuildingChunk = BuildingMaker.Create(nextPosition);
                    }
                    else
                    {
                        nextBuildingChunk = BuildingTopMaker.Create(nextPosition);
                        buildingTopCreated = true;
                    }
                    Buildings.Add(nextPosition);
                    yield return nextBuildingChunk;
                }
            }
            CityLayer -= DrawingPlainOrder.MicroPlainStep;
        }
    }
}