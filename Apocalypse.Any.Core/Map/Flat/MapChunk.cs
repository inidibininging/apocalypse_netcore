using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.Core.Map.Flat
{
    public class MapChunk :
        GameObject,
        IMovableGameObject,
        IVisualGameObject
    {
        #region properties

        private int _sizeX;

        public int SizeX
        {
            get { return _sizeX; }
            private set { _sizeX = value; }
        }

        private int _sizeY;

        public int SizeY
        {
            get { return _sizeY; }
            private set { _sizeY = value; }
        }

        private List<Layer> _layers = new List<Layer>();

        public List<Layer> Layers
        {
            get { return _layers; }
            set { _layers = value; }
        }

        private MovementBehaviour _movement;

        public MovementBehaviour Position
        {
            get { return _movement; }
            set { _movement = value; }
        }

        #endregion properties

        internal MapChunk(int sizeX, int sizeY)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            Position = new MovementBehaviour();
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            Layers.ForEach(currentLayer =>
            {
                currentLayer.Tiles.ForEach((tile) =>
                {
                    int width = (int)tile.CurrentImage.Texture.Width;
                    int height = (int)tile.CurrentImage.Texture.Height;
                    spriteBatch.Draw(
                    tile.CurrentImage.Texture,
                    tile.CurrentImage.Position * ScreenService.Instance.Ratio,
                    new Rectangle(0, 0, width, height),
                    tile.CurrentImage.Color * tile.CurrentImage.Alpha,
                    tile.CurrentImage.Rotation,
                    new Vector2(width / 2, height / 2),
                    tile.CurrentImage.Scale * ScreenService.Instance.Ratio,

                    SpriteEffects.None,
                    0.0f);

                    //drawing is expensive!!! Whoosh!
                    tile.CurrentImage.Position.X = tile.LocationUnit.X + Position.X;  // this part has no abstraction layer now. Need to implement it later.
                    tile.CurrentImage.Position.Y = tile.LocationUnit.Y + Position.Y;
                    tile.CurrentImage.Draw(spriteBatch); //TODO: If I uncomment this part -> will it fix the flimmering image bug?
                });
            });
        }

        public override void Update(GameTime time)
        {
            Layers.ToList().ForEach(layer =>
            {
                layer.Tiles.ToList().ForEach(tile =>
                {
                    //tile.CurrentImage.Rotation.Rotation += 1;
                    tile.CurrentImage.Update(time);
                });
            });
            return;
        }
    }
}