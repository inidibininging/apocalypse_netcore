using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing.UI;
using Apocalypse.Any.Core.Text;
using Apocalypse.Any.Domain.Common.Drawing.UI;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Model;
using Microsoft.Xna.Framework;
using System;

namespace Apocalypse.Any.Infrastructure.Client.Services.UI.Factories
{
    //TODO: Add inheritance in client to generic type factory.
    //The generic type factory is located in a server model namespace. 
    //this needs to be moved to the domain common factories interfaces namespace
    //=> Apocalypse.Any.Domain.Common.Factories.Interfaces
    public class DialogFactory 
    {
        public string Create(DialogNode dialogNode, IWindow parent)
        {            
            var dialogWindow = new ApocalypseWindow
            {
                Position = new MovementBehaviour() { X = parent.Position.X, Y = parent.Position.Y },
                Rotation = new RotationBehaviour() { Rotation = parent.Rotation },
                Scale = new Vector2(parent.Scale.X, parent.Scale.Y),
                Color = Color.DarkGray,
                IsVisible = true
            };
            dialogWindow.Alpha.Alpha = parent.Alpha.Alpha;
            dialogWindow.LayerDepth = DrawingPlainOrder.UI;

            //build an info text here
            dialogWindow.Add(new VisualText());
            dialogWindow.As<VisualText>(typeof(VisualText).Name).Text = dialogNode.Content;
            dialogWindow.As<VisualText>(typeof(VisualText).Name).Color = Color.Pink;
            dialogWindow.As<VisualText>(typeof(VisualText).Name).Position.X = dialogWindow.Position.X + dialogWindow.Scale.X;
            dialogWindow.As<VisualText>(typeof(VisualText).Name).Position.Y = dialogWindow.Position.Y + dialogWindow.Scale.Y;
            
            parent.Add(dialogNode.Id, dialogWindow);
            //dialogNode?..ForEach(subNode => Create(subNode, dialogWindow));
            return dialogNode.Id;
        }
        
    }
}
