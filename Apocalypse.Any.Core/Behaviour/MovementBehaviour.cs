using Apocalypse.Any.Core.Services;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Core.Behaviour
{
    /// <summary>
    /// Describes the actual representation of a movement / position of an object, bound to 2 coordinates.
    /// This behaviour can be used for velocity or anything that is meant to change the position / location of a game object
    ///
    /// </summary>
    public class MovementBehaviour //: Behaviour
    {
        //public MovementBehaviour(IGameObject target) : base(target)
        //{
        //}

        /// <summary>
        /// Sets the coordinates to the center of the screen
        /// </summary>
        public void Center()
        {
            X = ScreenService.Instance.Resolution.X / 2; //TODO: decouple screen service resolution from MovementBehaviour
            Y = ScreenService.Instance.Resolution.Y / 2; //TODO: decouple screen service resolution from MovementBehaviour
        }

        public static implicit operator Vector2(MovementBehaviour p)
        {
            return p.ToVector2();
        }

        public Vector2 ToVector2()
        {
            return new Vector2(
                (X + XDirection) * Delta,
                (Y + YDirection) * Delta
                ) * ScreenService.Instance.Ratio;
        }

        #region properties

        public float XDirection { get; set; } = 0;
        public float YDirection { get; set; } = 0;

        private float _x;

        public float X
        {
            get
            {
                return _x;
            }
            set
            {
                // TODO: Inject x in relation to the camera's position
                _x = value;
            }
        }

        private float _y;

        public float Y
        {
            get
            {
                return _y;
            }
            set
            {
                // TODO: Inject y in relation to the camera's position
                _y = value;
            }
        }

        public float Delta { get; set; } = 1;

        #endregion properties
    }
}