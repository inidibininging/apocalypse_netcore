using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Apocalypse.Any.Core.Text
{
    /// <summary>
    /// Sprite fonts olleeeee. Similar representation of something to the display, like image.
    /// This is only used for representing text to the screen.
    /// The text default font is Courier New
    /// </summary>
    public class VisualText : GameObject,
        IVisualGameObject,
        IFullPositionHolder
    {
        private SpriteFont Font { get; set; }
        private const string DefaultFontName = "Font/Arial";
        public string RegisteredFontName { get; private set; }

        private string text;

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                LoadContent(ScreenService.Instance.Content); // Fancy dirty trick that might break the whole code.. I think
            }
        }

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

        public Vector2 Scale { get; set; }

        public Color Color { get; set; }

        public float LayerDepth { get; set; }

        public VisualText(string registeredFontName = null)
        {
            SetRegisteredFontName(registeredFontName);
            Position = new MovementBehaviour();
            Alpha = new AlphaBehaviour();
            Rotation = new RotationBehaviour();
            Color = Color.White;
            Scale = Vector2.One;
        }

        private void SetRegisteredFontName(string registeredFontName)
        {
            if (string.IsNullOrWhiteSpace(registeredFontName))
            {
                RegisteredFontName = DefaultFontName;
                return;
            }
            RegisteredFontName = registeredFontName;
        }

        public override void LoadContent(ContentManager manager)
        {
            Font = manager.Load<SpriteFont>(RegisteredFontName);
            base.LoadContent(manager);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (string.IsNullOrWhiteSpace(Text) || Font == null)
                return;

            Font.MeasureString(Text);
            spriteBatch.DrawString(
                Font,
                Text,
                Position,
                Color * Alpha,
                Rotation,
                Font.MeasureString(Text),
                Scale,
                SpriteEffects.None,
                LayerDepth
                );
        }

        public override void Update(GameTime time)
        {
            foreach (var obj in this)
                obj.Value.Update(time);
        }

        public static implicit operator VisualText(string val)
        {
            return new VisualText() { Text = val };
        }

        public static implicit operator string(VisualText val)
        {
            return val.Text;
        }
    }
}