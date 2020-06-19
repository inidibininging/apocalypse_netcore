namespace Apocalypse.Any.Domain.Common.DrawingOrder
{
    /// <summary>
    /// Holds all the information for drawing sprites from front to back
    /// </summary>
    public static class DrawingPlainOrder
    {
        public const float PlainStep = 0.1f;
        public const float MicroPlainStep = 0.01f;
        public const float Background = 0f;
        public const float Foreground = 0.9f;
        public const float BackgroundFx = Background + PlainStep;
        public const float Entities = BackgroundFx + PlainStep;
        public const float EntitiesFX = Entities + PlainStep;
        public const float UI = EntitiesFX + PlainStep;
        public const float UIFX = UI + PlainStep;
        public const float ForegroundFX = Foreground + PlainStep;
    }
}