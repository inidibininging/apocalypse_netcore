using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Collision;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Drawing.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Apocalypse.Any.Constants;
using Apocalypse.Any.Core.Behaviour;

namespace Apocalypse.Any.Domain.Common.Drawing.UI
{
    public class ApocalypseWindow : Image, IWindow, IUIEvents
    {

        public bool Colliding { get; private set; }
        public bool IsVisible { get; set; }

        public ApocalypseWindow()
        {
            Path = ImagePaths.blank; //TODO: CHECK IT
            
            Scale = new Vector2(320, 256); //this is the default size of a window if none given
            Color = Color.Purple;
        }

        #region Window Interface

        public virtual void Close()
        {
            IsVisible = false;
        }

        public virtual void Show()
        {
            IsVisible = true;
        }

        public virtual void ShowDialog()
        {
        }

        #endregion Window Interface

        public override void Update(GameTime time)
        {
            AllOfType<IChildUIElement>()
                .ToList()
                .ForEach(child =>
                {
                    child.ParentPosition = child.ParentPosition ?? new MovementBehaviour();
                    child.ParentPosition.X = this.Position.X;
                    child.ParentPosition.Y = this.Position.Y;
                    child.ParentScale = new Vector2(Scale.X,Scale.Y);
                    child.ParentWidth = Width;
                    child.ParentHeight = Height;
                });
            base.Update(time);
        }

        #region Collision Interface

        public virtual void OnCollision(ICollidable collidable)
        {
        }

        public virtual Rectangle GetCurrentRectangle()
        {
            return SourceRect;
        }

        #endregion Collision Interface
        
        public virtual void OnClick(object sender, EventArgs args)
        {
            
        }

        public virtual void OnMouseEnter(object sender, EventArgs args)
        {
            
        }

        public void OnMouseLeave(object sender, EventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
