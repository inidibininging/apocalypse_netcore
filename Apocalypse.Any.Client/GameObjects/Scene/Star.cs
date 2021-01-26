using Apocalypse.Any.Client.Mechanics;
using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Collision;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.FXBehaviour;
using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Core.Input.Translator;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Constants;

namespace Apocalypse.Any.Client.GameObjects.Scene
{
    public class Star : Image, ICollidable
    {

        public FadeToBehaviour FadeToZero { get; set; } = new FadeToBehaviour();
        public FadeToBehaviour FadeToOne { get; set; } = new FadeToBehaviour();

        //new List<string>(InputMapper.DefaultMap.Select(col => col.Key));

        //private static int sechs = 5;
        public Star(float x = 0, float y = 0)
        {
            Path = ImagePaths.stars;
            Scale = new Vector2((float)Randomness.Instance.From(1, 2) / 10);
            LayerDepth = DrawingPlainOrder.Background;
            var position = Position;
            var cam = ScreenService.Instance.DefaultScreenCamera;

            if (cam != null)
            {
                if(x == 0)
                    position.X = Randomness.Instance.From(0, (int)ScreenService.Instance.Resolution.X * 48);
                if(y == 0)
                    position.Y = Randomness.Instance.From(0, (int)ScreenService.Instance.Resolution.Y * 48);
                // position.X = Randomness.Instance.From(0, (int)(Math.Abs(cam.Position.X)));
                // position.Y = Randomness.Instance.From(0, (int)(Math.Abs(cam.Position.Y)));
            }
            Rotation.Rotation = Randomness.Instance.From(0, 360);
            Alpha.Alpha = Randomness.Instance.TrueOrFalse() ? 1f : 0f;
            Add(nameof(AsteroidScreenBehaviour), new AsteroidScreenBehaviour(this));
        }

        private bool colliding = false;

        public bool Colliding
        {
            get
            {
                return colliding;
            }
            private set
            {
                colliding = value;
            }
        }

        public Rectangle GetCurrentRectangle()
        {
            var croppedRect = this.SourceRect;
            croppedRect.Width = (int)(Convert.ToSingle(croppedRect.Width) * this.Scale.X);
            croppedRect.Height = (int)(Convert.ToSingle(croppedRect.Height) * this.Scale.Y);
            croppedRect.Location = ((Vector2)this.Position).ToPoint();
            return croppedRect;
        }

        public void MoveRandomly()
        {
            var randomMap = new List<string>() {
                DefaultKeys.Left,
                DefaultKeys.Right
            };
            var randomValue = Randomness.Instance.From(0, randomMap.Count - 1);
            InputMapper
                .DefaultMovementMap
                .Select(cmdTrans => cmdTrans.Translate(randomMap[randomValue]))
                .Where(cmdTrans => cmdTrans != null)
                .ToList()
                .ForEach(cmdList => (cmdList as IList<ICommand<MovementBehaviour>>).ToList().ForEach(cmd => cmd.Execute(Position)));
        }

        public void OnCollision(ICollidable collidable)
        {
            Colliding = true;
        }

        public bool ToZero { get; set; }

        public override void Update(GameTime gameTime)
        {
            if (Alpha.Alpha == 1f)
                ToZero = true;

            if (Alpha.Alpha == 0)
                ToZero = false;

            if (ToZero)
                FadeToZero.Update(Alpha, 0 , (float)Randomness.Instance.From(10, 100) / (float)Randomness.Instance.From(1000, 10000));
            else
                FadeToOne.Update(Alpha, 1, (float)Randomness.Instance.From(10, 100) / (float)Randomness.Instance.From(1000, 10000));
            
            base.Update(gameTime);
        }
    }
}