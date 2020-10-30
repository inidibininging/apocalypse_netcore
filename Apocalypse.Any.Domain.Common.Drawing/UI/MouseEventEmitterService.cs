using System;
using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Drawing.UI;
using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Core.Input.Translator;
using Apocalypse.Any.Core.Screen;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Domain.Common.Drawing.UI
{
    public class MouseEventEmitterService<TContext> : IState<string,TContext>
    where TContext : IGameScreen
    {
        public void Handle(IStateMachine<string, TContext> machine)
        {
            //TODO: inject input service and make this class generic ( IInputService ) 
            var mouse = machine.SharedContext.CursorImage; //TODO: get mouse events
            var mouseClicked = Mouse.GetState().LeftButton == ButtonState.Pressed;
            
            foreach (var window in machine
                .SharedContext
                .Values
                .OfType<ApocalypseWindow>()
                .Where(window => mouse.Position.X > window.Position.X &&
                                 mouse.Position.X < window.Position.X + (window.Scale.X * window.Width) &&
                                 mouse.Position.Y > window.Position.Y &&
                                 mouse.Position.Y < window.Position.Y + (window.Scale.Y * window.Height)))
            {
                window.OnMouseEnter(this, EventArgs.Empty);
                DispatchEvents(window, mouse, mouseClicked, machine.SharedContext.UpdateGameTime);
            }
        }


        public List<IdentifiableShortCounterThreshold> MouseEnter { get; set; } = new List<IdentifiableShortCounterThreshold>();
        private void DispatchEvents(IGameObjectDictionary window, IImage cursor, bool clicked, GameTime gameTime)
        {
            foreach (var childKV in window
                .Where(childUIElement =>
                    {
                        var isGameObject = childUIElement.Value is IGameObject;
                        var isVisualGameObject = childUIElement.Value is IVisualGameObject;
                        var hasEvents = childUIElement.Value is IUIEvents;
                        var isMovableGameObject = childUIElement.Value is IMovableGameObject;
                        var isScaleHolder = childUIElement.Value is IScaleHolder;
                        var isSizeHolder = childUIElement.Value is ISizeHolder;

                        return isGameObject &&
                               isVisualGameObject &&
                               hasEvents &&
                               isMovableGameObject &&
                               isScaleHolder &&
                               isSizeHolder
                               
                               &&
                               
                               cursor.Position.X > (childUIElement.Value as IMovableGameObject).Position.X &&
                               cursor.Position.X < (childUIElement.Value as IMovableGameObject).Position.X +
                               ((childUIElement.Value as IScaleHolder).Scale.X *
                                (childUIElement.Value as ISizeHolder).Width) &&
                               cursor.Position.Y > (childUIElement.Value as IMovableGameObject).Position.Y &&
                               cursor.Position.Y < (childUIElement.Value as IMovableGameObject).Position.Y +
                               (((childUIElement.Value as IScaleHolder).Scale.Y) *
                                (childUIElement.Value as ISizeHolder).Height);
                    }
                ))
            {
                //Event integration comes here
                (childKV.Value as IUIEvents)?.OnMouseEnter(this, EventArgs.Empty);

                HandleClick(clicked, gameTime, childKV);
            }
        }

        private void HandleClick(bool mouseClicked, GameTime gameTime, KeyValuePair<string, IGameObject> childKV)
        {
            IdentifiableShortCounterThreshold clickThreshold;

            // Create  
            if ((clickThreshold = MouseEnter.FirstOrDefault(threshold => threshold.Id == childKV.Key)) == null)
            {
                clickThreshold = new IdentifiableShortCounterThreshold()
                {
                    Id = childKV.Key,
                    CooldownDeadline = TimeSpan.FromSeconds(1),
                    Counter = 0,
                    Max = 1
                };
                MouseEnter.Add(clickThreshold);
            }

            if (mouseClicked)
            {
                /* Count once if clicked. If so, a cooldown timer will be started,
                        preventing the click event to be fired more than once.
                        If the cool down reaches zero, the click threshold will be reset.*/
                if (!clickThreshold.MaxReached)
                {
                    (childKV.Value as IUIEvents)?.OnClick(this, EventArgs.Empty);
                    clickThreshold.Counter = 1;
                }
            }
            else
            {
                clickThreshold.CooldownDeadline -=
                    TimeSpan.FromMilliseconds(gameTime.ElapsedGameTime.TotalMilliseconds);

                if (clickThreshold.CooldownDeadline <= TimeSpan.Zero)
                {
                    clickThreshold.Counter = 0;
                    clickThreshold.CooldownDeadline = TimeSpan.FromSeconds(2);
                }
            }
        }
    }
}
