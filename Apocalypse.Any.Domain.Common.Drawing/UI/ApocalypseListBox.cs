using System;
using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Constants;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing.UI;
using Apocalypse.Any.Core.Utilities;
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
        public int SpaceBetweenListItem { get; set; } = 32;
        public int SpaceFromLeft { get; set; } = 16;
        
        public ApocalypseListBox(Dictionary<(int frame, int x, int y), Rectangle> frames) : base(frames)
        {
        }

        public ApocalypseListBox(Dictionary<(int frame, int x, int y), Rectangle> frames, TContext context) : base(frames, context)
        {
        }

        public void Add(string text)
        {
            var items = AllOfType<ApocalypseListItem<TContext>>().OrderByDescending(item => item.Position.Y);
        
            var itemToAdd = items.FirstOrDefault();
            
            //TODO: extract this to an interface for adding stuff to a UIElement
            Add(Guid.NewGuid().ToString(),new ApocalypseListItem<TContext>(SpriteSheetRectangle, default(TContext))
            {
                Position = new MovementBehaviour()
                {
                    X = SpaceFromLeft,
                    Y = SpaceBetweenListItem * (items.Count() + 1) //+ (itemToAdd?.Position.Y ?? SpaceBetweenListItem) * items.Count()
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
            });
            // TODO: Erase the TContext
        }

        public void Remove(string text)
        {
            foreach (var listItem in AllOfType<ApocalypseListItem<TContext>>().Where((item) => item.Text == text))
                Remove(listItem);
        }

        public override void Update(GameTime time)
        {
            foreach (var child in AllOfType<IChildUIElement>())
            {
                child.ParentPosition.X = ParentPosition.X;
                child.ParentPosition.Y = ParentPosition.Y;    
                child.ParentWidth = ParentWidth;
                child.ParentHeight = ParentHeight;
                child.ParentScale = ParentScale;
            }
            //TODO: pass the space between items in some form to the child item        
            base.Update(time);
        }
    }
}

