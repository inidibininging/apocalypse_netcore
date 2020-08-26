using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Input.Translator;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Infrastructure.Common.Services;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Transformations;
using Apocalypse.Any.Infrastructure.Server.Services.Data;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics
{
    /// <summary>
    /// Creates or updates circular locations for building blocks in cities
    /// </summary>
    public class CreateOrUpdateIdentifiableCircularLocationState : IState<string, IGameSectorLayerService>
    {
        public CreateOrUpdateIdentifiableCircularLocationState(
            IRectangularFrameGeneratorService frameGeneratorService,
            ImageToRectangleTransformationService imageToRectangleTransformationService,
            string dialogLocationRelationLayerName)
        {
            FrameGeneratorService = frameGeneratorService ?? throw new ArgumentNullException(nameof(frameGeneratorService));
            ImageToRectangleTransformationService = imageToRectangleTransformationService ?? throw new ArgumentNullException(nameof(imageToRectangleTransformationService));
            DialogLocationRelationLayerName = dialogLocationRelationLayerName;
        }

        public IRectangularFrameGeneratorService FrameGeneratorService { get; }
        public ImageToRectangleTransformationService ImageToRectangleTransformationService { get; }
        public string DialogLocationRelationLayerName { get; }

        private const string MiniCityImagePath = "miniCity";
        private Dictionary<string, Rectangle> FramesWithCircularLocation() => 
                FrameGeneratorService.GenerateGameSheetAtlas(MiniCityImagePath, 32, 32, 3, 6, 2, 2)
                                    .Union(FrameGeneratorService.GenerateGameSheetAtlas(MiniCityImagePath, 32, 32, 0, 6, 3, 4).Select(kv => kv))
                                    .Union(FrameGeneratorService.GenerateGameSheetAtlas(MiniCityImagePath, 32, 32, 0, 5, 4, 0).Select(kv => kv))
                                    .Union(FrameGeneratorService.GenerateGameSheetAtlas(MiniCityImagePath, 32, 32, 0, 0, 5, 5).Select(kv => kv))
                                    .ToDictionary(kv => kv.Key, kv => kv.Value);
        
        private IEnumerable<DialogNode> GetDialogNodesWithBuildingLocations(IStateMachine<string, IGameSectorLayerService> machine)
        {
            /* 1. Get all extra layers with type circular location.
             * 2. Get the circular locations as rectangles
             *    that intersect with the images of "FramesWithCircularLocation" 
             * 3. 
            */
            var circularLocationsOfBuildings = machine.SharedContext
                                .DataLayer
                                .Layers
                                .SelectMany(circularLocationList => circularLocationList.DataAsEnumerable<IdentifiableCircularLocation>())
                                .Where(circularLocation => machine.SharedContext
                                                    .DataLayer
                                                    .ImageData
                                                    .Any(buildingImage => FramesWithCircularLocation().Any(
                                                                                frameWithCircularLocationKeyValuePair => frameWithCircularLocationKeyValuePair.Key == buildingImage.SelectedFrame &&
                                                                                                                         frameWithCircularLocationKeyValuePair.Value.Intersects(ImageToRectangleTransformationService.TransformInRespectToCenter(buildingImage))
                                                                            )));

            var dialogsWithSameIdAsCircularLocations = machine.SharedContext
                                                        .DataLayer
                                                        .Layers
                                                        .Where(l => l as IDialogService != null)
                                                        .Cast<IDialogService>()
                                                        .SelectMany(dialogService => circularLocationsOfBuildings.Select(circularLocation => dialogService.GetDialogNode(circularLocation.Id)));
            return dialogsWithSameIdAsCircularLocations;
        }

        private volatile bool HandleOnce = true;

        /// <summary>
        /// Create circular locations out of the building images
        /// These circular locations will be consumed later on by ProcessPlayerDialogsRequestsState
        /// </summary>
        /// <param name="machine"></param>
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if(HandleOnce)
            {
                HandleOnce = false;
                Task.Factory.StartNew(() =>
                {
                    Task.Delay(1.Seconds());

                    var buildingImages = machine.SharedContext.DataLayer.ImageData.Where(imageData => FramesWithCircularLocation().Keys.Contains(imageData.SelectedFrame));

                    //only take first extra layer
                    var firstAppearingCircularLocationListLayer = machine.SharedContext
                                                            .DataLayer
                                                            .Layers
                                                            .FirstOrDefault(circularLocationList => circularLocationList.GetValidTypes().Any(t => t == typeof(IdentifiableCircularLocation)));

                    //get the first relation layer of that type
                    var firstRelationLayerRelatedtoLocations = machine.SharedContext
                                                            .DataLayer
                                                            .Layers
                                                            .FirstOrDefault(layer => layer.DisplayName == DialogLocationRelationLayerName && layer.GetValidTypes().Any(t => t == typeof(DynamicRelation)));

                    if (firstAppearingCircularLocationListLayer == null ||
                        firstRelationLayerRelatedtoLocations == null)
                        return;

                    var locations = firstAppearingCircularLocationListLayer.DataAsEnumerable<IdentifiableCircularLocation>().ToList();
                    var imagesAlreadyBoundToLocations = buildingImages.Where(imageData => locations.Any(location => location.Id == imageData.Id));


                    buildingImages
                    .Except(imagesAlreadyBoundToLocations)
                    .Select(image => new IdentifiableCircularLocation()
                    {
                        Id = image.Id,
                        DisplayName = $"BUILDING BLOCK {Guid.NewGuid()}", //TODO: INSERT_PLACE_NAME_GENERATOR
                        Position = new MovementBehaviour()
                        {
                            X = image.Position.X,
                            Y = image.Position.Y
                        },
                        Radius = image.Width > image.Height ?
                                    image.Width : image.Height
                    })
                    .ToList()
                    .ForEach(newLocation =>
                    {
                        firstAppearingCircularLocationListLayer.Add(newLocation);
                        //create a relationship to a generic people starting dialog
                        //TODO: change this in the future, by adding dialogs generated by yaml files for example
                        firstRelationLayerRelatedtoLocations.Add(new DynamicRelation()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Entity1 = typeof(IdentifiableCircularLocation),
                            Entity1Id = newLocation.Id,
                            Entity2 = typeof(DialogNode),
                            Entity2Id = ExampleDialogService.GenericPeopleStartDialog
                        });
                    });

                    foreach (var circularLocation in locations)
                    {
                        var imageDataToCircularLocation = buildingImages.FirstOrDefault(buildingImage => buildingImage.Id == circularLocation.Id);
                        if (imageDataToCircularLocation == null)
                        {
                            //concurrency strikes back or something changed the id
                            continue;
                        }
                        circularLocation.Position.X = imageDataToCircularLocation.Position.X;
                        circularLocation.Position.Y = imageDataToCircularLocation.Position.Y;

                        //since an image is a rectangle and not a circle,
                        //one needs to take the max of either X or Y multiplied by its scale for acquiring the size
                        float newRadius = imageDataToCircularLocation.Width > imageDataToCircularLocation.Height ?
                                            imageDataToCircularLocation.Width : imageDataToCircularLocation.Height;
                        circularLocation.Radius = newRadius;
                    }
                    HandleOnce = true;
                });
            }
            

        }
    }
}
