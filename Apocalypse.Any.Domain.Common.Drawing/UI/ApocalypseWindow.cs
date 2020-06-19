using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Collision;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Drawing.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Apocalypse.Any.Domain.Common.Drawing.UI
{
    public class ApocalypseWindow : Image, IWindow
    {
        // public MovementBehaviour Position { get; set; }
        // public RotationBehaviour Rotation { get; set; }

        public bool Colliding { get; private set; }
        public bool IsVisible { get; set; }

        public ApocalypseWindow()
        {
            Path = "Image/blank";
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

        public virtual void OnClick(IGameObject sender, EventArgs args)
        {
        }

        public virtual void OnHover(IGameObject sender, EventArgs args)
        {
        }

        #endregion Window Interface

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        #region Collision Inteface

        public virtual void OnCollision(ICollidable collidable)
        {
        }

        public virtual Rectangle GetCurrentRectangle()
        {
            return SourceRect;
        }

        #endregion Collision Inteface
    }
}