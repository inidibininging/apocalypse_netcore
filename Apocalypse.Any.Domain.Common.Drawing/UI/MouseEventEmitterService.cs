using System;
using System.Linq;
using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Drawing.UI;
using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Core.Input.Translator;
using Apocalypse.Any.Core.Screen;
using Apocalypse.Any.Core.Services;
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
                DispatchEvents(window, mouse, mouseClicked);
            }
        }

        private void DispatchEvents(IGameObjectDictionary window, IImage mouse, bool mouseClicked)
        {
            foreach (var child in window
                .Values
                .Where(child => child is IGameObject &&
                                child is IVisualGameObject &&
                                child is IChildUIElement &&
                                child is IUIEvents &&
                                child is IMovableGameObject &&
                                child is IScaleHolder &&
                                child is ISizeHolder &&
                                mouse.Position.X > (child as IMovableGameObject).Position.X &&
                                mouse.Position.X < (child as IMovableGameObject).Position.X +
                                ((child as IScaleHolder).Scale.X * (child as ISizeHolder).Width) &&
                                mouse.Position.Y > (child as IMovableGameObject).Position.Y &&
                                mouse.Position.Y < (child as IMovableGameObject).Position.Y +
                                ((child as IScaleHolder).Scale.Y) * (child as ISizeHolder).Height))
            {
                //Event integration comes here
                (child as IUIEvents)?.OnMouseEnter(this, EventArgs.Empty);

                if (mouseClicked) 
		        {
		            Console.WriteLine("Clicked");	
                            (child as IUIEvents)?.OnClick(this, EventArgs.Empty);
		        }
            }
        }
    }
}
