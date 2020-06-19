//#define USE_STREAM
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Core.Drawing
{
    /// <summary>
    /// This class is the representation of an image displayed on the game screen.
    /// This can be seen as a wrapper for Texture2d
    /// </summary>
    public class Image : GameObject, IImage, IDisposable, IImageData
    {
        #region static

        private static Image _emptyImage;
        
        public static Image EmptyImage // the image should be "empty"
        {
            get
            {
                return _emptyImage ?? (_emptyImage = new Image()
                {
                    Path = "Image/dummytile.png"
                });
            }
        }

        #endregion static

        private Vector2 _origin;
        private RenderTarget2D _renderTarget;
        private string _path;
        
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                if (Path != value)
                {
                    _path = value;
                    //
                }
                if (!string.IsNullOrWhiteSpace(Path))
                    LoadContent(ScreenService.Instance.Content); // PORTS HERE
            }
        }

        private int TimesContentLoaded { get; set; }

        public float LayerDepth { get; set; }

        public Vector2 Scale { get; set; }
        public Rectangle SourceRect { get; set; }
        public Texture2D Texture { get; set; }

        public bool Disposed { get; private set; }

        public MovementBehaviour Position
        {
            get;
            set;
        }

        public RotationBehaviour Rotation
        {
            get;
            set;
        }

        public AlphaBehaviour Alpha
        {
            get;
            set;
        }

        public Color Color { get; set; }

        public float Height
        {
            get
            {
                return SourceRect.Height;
            }
        }

        public float Width
        {
            get
            {
                return SourceRect.Width;
            }
        }

        public string SelectedFrame { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool ForceDraw {get;set;}
        public Image()
        {
            Position = new MovementBehaviour();
            Rotation = new RotationBehaviour();
            Alpha = new AlphaBehaviour();

            _path = string.Empty;
            Scale = Vector2.One;
            SourceRect = Rectangle.Empty;
            Color = Color.White;
        }

        public void Dispose()
        {
            _renderTarget?.Dispose();
            //Texture.Dispose();
            _renderTarget = null;
            Texture = null;
            Disposed = true;
        }

        public override void LoadContent(ContentManager manager)
        {

            //http://community.monogame.net/t/loading-png-jpg-etc-directly/7403/3
#if USE_STREAM
            using(var imgStream = System.IO.File.Open(Path,System.IO.FileMode.Open))
            {
                Texture = Texture2D.FromStream(ScreenService.Instance.GraphicsDevice,imgStream);
            }
#else            
            Texture = manager.Load<Texture2D>(Path);
#endif

            var dimensions = Vector2.Zero;

            if (Texture != null)
                dimensions.X += Texture.Width;

            if (Texture != null)
                dimensions.Y = Texture.Height;

            if (SourceRect == Rectangle.Empty)
                SourceRect = new Rectangle(0, 0, (int)dimensions.X, (int)dimensions.Y);

            //Set Screen as own RenderTarget
            _renderTarget = new RenderTarget2D(ScreenService.Instance.GraphicsDevice,
                (int)dimensions.X, (int)dimensions.Y);
            lock (_renderTarget)
            {
                try
                {
                    ScreenService.Instance.GraphicsDevice.SetRenderTarget(_renderTarget);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            ScreenService.Instance.GraphicsDevice.Clear(Color.Transparent);
            ScreenService.Instance.SpriteBatch.Begin();

            if (Texture != null)
                ScreenService.Instance.SpriteBatch.Draw(Texture, Vector2.Zero, Color.White);

            ScreenService.Instance.SpriteBatch.End();

            Texture = _renderTarget;

            ScreenService.Instance.GraphicsDevice.SetRenderTarget(null);
            base.LoadContent(manager);
        }

        public override void UnloadContent()
        {
            Dispose();
            base.UnloadContent();
        }

        public bool CannotDraw() => (ScreenService.Instance.DefaultScreenCamera.Position.X - ScreenService.Instance.Resolution.X/2 > Position.X ||
               ScreenService.Instance.DefaultScreenCamera.Position.X + ScreenService.Instance.Resolution.X/2 < Position.X ||
               ScreenService.Instance.DefaultScreenCamera.Position.Y - ScreenService.Instance.Resolution.Y/2 > Position.Y ||
               ScreenService.Instance.DefaultScreenCamera.Position.Y + ScreenService.Instance.Resolution.Y/2 < Position.Y) && Alpha.Alpha <= 0 && !ForceDraw;
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Disposed)
                return;

            if (string.IsNullOrEmpty(Path))
                return;

            if(_origin == Vector2.Zero)
                _origin = new Vector2(SourceRect.Width / 2, SourceRect.Height / 2);

            //only draw texture if it lies within the camera bounds
            if(CannotDraw())
            {
                return;            
            }
            
            spriteBatch.Draw(
                Texture,
                Position,
                SourceRect,
                Color * Alpha,
                Rotation,
                _origin,
                Scale * ScreenService.Instance.Ratio,
                SpriteEffects.None,
                (LayerDepth > 1 && LayerDepth > 0 ? LayerDepth / 100f : LayerDepth));

            foreach (var visuals in AllOfType<IVisualGameObject>())
            {
                visuals.Draw(spriteBatch);
            }
            
        }

        public override void Update(GameTime time) => ForEach(obj => obj.Update(time));
    }
}