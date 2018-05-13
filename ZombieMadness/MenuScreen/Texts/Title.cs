using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MenuScreen.MenuScreen.Texts
{
    public class Title : SimpleText
    {
        private const int MINIMUM_Y_WHEN_LEAVING = -40;
        private const int POSITION_INCREMENT = 3;
        private const int COLOR_INCREMENT = 10;

        public Title(Game game, string text) : base(game, text, false)
        {
            Color = new Color(Color.Green, 0);
        }

        public override void UpdatePosition()
        {
            if (IsEntering)
            {
                Vector2 position = Position;
                Color color = Color;
                position.Y += POSITION_INCREMENT;
                int temp = Color.A;
                color.A += COLOR_INCREMENT;
                if (temp > color.A)
                {
                    IsEntering = false;
                    color.A = 255;
                }
                Position = position;
                Color = color;
            }
            if (IsLeaving)
            {
                Vector2 position = Position;
                Color color = Color;
                position.Y -= POSITION_INCREMENT;
                color.A -= COLOR_INCREMENT;
                if (position.Y < MINIMUM_Y_WHEN_LEAVING)
                {
                    IsLeaving = false;
                }
                Position = position;
                Color = color;
            }
        }
    }
}