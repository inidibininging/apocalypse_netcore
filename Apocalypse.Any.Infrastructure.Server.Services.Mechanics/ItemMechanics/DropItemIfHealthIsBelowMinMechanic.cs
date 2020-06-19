using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Transformations;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces;
using System;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.EnemyMechanics
{
    public class DropItemIfHealthIsBelowMinMechanic : IDropMechanic
    {
        private IGenericTypeFactory<Item> ItemFactory { get; set; }
        private RectangleToSectorBoundaryTransformationService RectangleToSectorBoundary { get; set; }
        private BoundingBoxTransformationService BoundingBoxTransformator { get; set; }

        public DropItemIfHealthIsBelowMinMechanic(
            IGenericTypeFactory<Item> itemFactory,
            RectangleToSectorBoundaryTransformationService rectangleToSectorBoundary,
            BoundingBoxTransformationService boundingBoxTransformator)
        {
            if (itemFactory == null)
                throw new ArgumentNullException(nameof(itemFactory));
            ItemFactory = itemFactory;

            if (rectangleToSectorBoundary == null)
                throw new ArgumentNullException(nameof(rectangleToSectorBoundary));
            RectangleToSectorBoundary = rectangleToSectorBoundary;

            if (boundingBoxTransformator == null)
                throw new ArgumentNullException(nameof(boundingBoxTransformator));
            BoundingBoxTransformator = boundingBoxTransformator;
        }

        public Item Update(CharacterEntity character, int offsetX, int offsetY)
        {
            if (character.Stats.Health > character.Stats.GetMinAttributeValue())
                return null;
            
            //the drop rate should be included
            ////calculate drop rate
            ////this is just a mock
            //if (!Randomness.Instance.TrueOrFalse())
            //    return null;

            //calculate what type of items you get
            //a rolling system

            //the character is the center of a rectangle where the items can be spawned

            var boundBox = BoundingBoxTransformator.Transform(
                            character.CurrentImage,
                            (int)offsetX,
                            (int)offsetY);
            
            var sect = RectangleToSectorBoundary.ToSectorBoundary(boundBox);
            Console.WriteLine("MIN X :"+sect.MinSectorX);
            Console.WriteLine("MAX X :"+sect.MaxSectorX);
            Console.WriteLine("MIN Y :"+sect.MinSectorY);
            Console.WriteLine("MAX Y :"+sect.MaxSectorY);
            return ItemFactory.Create(sect);
        }
    }
}