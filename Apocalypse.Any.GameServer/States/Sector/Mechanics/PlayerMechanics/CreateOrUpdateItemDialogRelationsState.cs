using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.PubSub;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Server.PubSub;
using Apocalypse.Any.Infrastructure.Server.PubSub.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Common;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.PlayerMechanics
{
    /// <summary>
    /// This state creates dialog nodes with links to items
    /// </summary>
    public class CreateOrUpdateItemDialogRelationsState : IState<string, IGameSectorLayerService>     
    {

        public string DialogEventService { get; set; }

        public CreateOrUpdateItemDialogRelationsState(string dialogEventService)
        {
            DialogEventService = dialogEventService ?? throw new ArgumentNullException(nameof(dialogEventService));
        }

        /// <summary>
        /// This state creates dialog nodes
        /// with links to items
        /// </summary>
        /// <param name="machine"></param>
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            /*TODO: For now I decided to set a unique name for every item. If the items name is not unique, 
                    the algorithm for choosing items will be broken. 
                    To fix this, every item MUST have a unique name OR id */
            var dataLayer = machine.SharedContext.DataLayer;
            var players = dataLayer.Players;

            var itemsGroupedByPlayer = dataLayer
                                        .Items
                                        .Where(item => !string.IsNullOrWhiteSpace(item.OwnerName))
                                        .GroupBy(item => item.OwnerName)
                                        .Where(g => players.Any(player => player.LoginToken == g.Key));

            var relationLayer = dataLayer
                                .GetLayersByType<DynamicRelation>()
                                .FirstOrDefault(l => l.Name == DialogEventService);

            var dialogLayer = dataLayer
                                .GetLayersByType<DialogNode>()
                                .FirstOrDefault();

            var dialogs = dialogLayer.DataAsEnumerable<DialogNode>();
            
            //remove player dialogs
            var dialogsToRemove = dialogs
                                    .Where(d => players.Select(p => p.LoginToken)
                                    .Any(id => d.Id.Contains(id)))
                                    .ToList();

            //remove player dialog nodes
            dialogLayer.Remove<DialogNode>(d => dialogsToRemove.Any(dtr => dtr.Id == d.Id));

            //remove any relation containing something with dialogs
            if(relationLayer == null)
            {
                return;                
            }
            relationLayer.Remove<DynamicRelation>(dr =>
                                            dialogsToRemove.Any(d =>
                                            (d.Id == dr.Entity2Id && dr.Entity2 == typeof(DialogNode)) ||
                                            (d.Id == dr.Entity1Id && dr.Entity1 == typeof(DialogNode))));



            var dropItemQueue = dataLayer                                
                                .GetLayersByName(DialogEventService)
                                .FirstOrDefault(l => (l as IEventQueue) != null && 
                                                      l.Name == DialogEventService);
                
            foreach (var itemGroup in itemsGroupedByPlayer)
            {
                var dialogRootNodeKey = $"{nameof(DialogNode)}{itemGroup.Key}";
                var dialogRoutes = new List<Tuple<string, string>>();

                foreach (var item in itemGroup)
                {
                    var itemId = item.Id;
                    var itemDialogKey = $"{dialogRootNodeKey}_{item.Id}";
                    dialogLayer.Add(new DialogNode()
                    {
                        Id = itemDialogKey,
                        Content = item.DisplayName,
                        DialogIdContent = new List<Tuple<string, string>>()
                        {
                            new Tuple<string, string>(ExampleDialogService.GenericPeopleStartDialog, "Back")
                        }
                    });
                    dialogRoutes.Add(new Tuple<string,string>(itemDialogKey, item.DisplayName));
                    
                    if(dropItemQueue != null)
                    {                        
                        var relation = new DynamicRelation()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Entity1 = typeof(Item),
                            Entity1Id = item.Id,
                            Entity2 = typeof(DialogNode),
                            Entity2Id = itemDialogKey
                        };

                        relationLayer.Add(relation);

                        ////send event to queue
                        //dropItemQueue.Add(new EventQueueArgument()
                        // {
                        //     Id = Guid.NewGuid().ToString(),
                        //     EventName = DialogEventService,
                        //     DynamicRelationId = relation.Id
                        // });
                    }

                }
                dialogLayer.Add(new DialogNode()
                {
                    Id = dialogRootNodeKey,
                    Content = "Players items",
                    DialogIdContent = dialogRoutes
                });

                var startDialog = dialogs.Where(d => d.Id == ExampleDialogService.QuestionDropPeople).FirstOrDefault();                
                if(!startDialog.DialogIdContent.Any(p => p.Item1 == dialogRootNodeKey))
                {
                    startDialog.DialogIdContent.Add(new Tuple<string, string>(
                        dialogRootNodeKey, 
                        $"My Items"));
                }
            }


        }

    }
}
