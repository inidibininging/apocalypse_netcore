using System;
using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Constants;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing.UI;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apocalypse.Any.Domain.Common.Drawing.UI
{
    public class ApocalypseListBox<TContext>
        : ApocalypseRectangle<TContext>
    {
         
        /// <summary>
        /// Vertical space between items.
        /// </summary>
        public int SpaceBetweenListItem { get; set; } = 8;
        public int SpaceFromLeft { get; set; } = 16;
        
        private int ItemSizeY { get; set; } = 32;

        public IEnumerable<ApocalypseListItem<TContext>> Items
        {
            get => AllOfType<ApocalypseListItem<TContext>>().OrderByDescending(item => item.Position.Y);
        }
        
        public ApocalypseListBox(Dictionary<(int frame, int x, int y), Rectangle> frames) : base(frames)
        {
        }

        public ApocalypseListBox(Dictionary<(int frame, int x, int y), Rectangle> frames, TContext context) : base(frames, context)
        {
        }

        public string Add(string text, (int frameName, int x, int y) selectedFrame, bool useTextAsKey = false)
        {
            var itemToAdd = Items.FirstOrDefault();
            
            //TODO: extract this to an interface for adding stuff to a UIElement
            var listItem = new ApocalypseListItem<TContext>(SpriteSheetRectangle, default(TContext))
            {
                Position = new MovementBehaviour()
                {
                    X = SpaceFromLeft,
                    Y = Items.Any() ? (SpaceBetweenListItem + ItemSizeY) * (Items.Count()) : 0
                },
                ParentPosition = new MovementBehaviour()
                {
                    X = Position.X,
                    Y = Position.Y
                },
                ParentScale = new Vector2(1),
                ParentWidth = Width,
                ParentHeight = Height,
                Text = text
            };
            listItem.SelectedFrame = SelectedFrame = selectedFrame;
            var newKey = useTextAsKey ? text : Guid.NewGuid().ToString();
            Add(newKey, listItem);
            // TODO: Erase the TContext
            return newKey;
        }

        public void Remove(string text)
        {
            foreach (var listItem in AllOfType<ApocalypseListItem<TContext>>().Where((item) => item.Text == text))
                Remove(listItem);
        }

        public override void Update(GameTime time)
        {
            ItemSizeY = SourceRect.Height;
            
            foreach (var child in AllOfType<IChildUIElement>())
            {
                child.ParentPosition.X = ParentPosition.X;
                child.ParentPosition.Y = ParentPosition.Y;    
                child.ParentWidth = ParentWidth;
                child.ParentHeight = ParentHeight;
                child.ParentScale = ParentScale;
                child.IsVisible = IsVisible;
                child.LayerDepth = LayerDepth + DrawingPlainOrder.MicroPlainStep;
            }
            //TODO: pass the space between items in some form to the child item        
            base.Update(time);
        }
    }
}

