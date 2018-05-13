using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MenuScreen.MenuScreen.Texts
{
    public class ColoredText : SimpleText
    {

        public ColoredText(Game game, string text, bool entering) : base(game, text, entering)
        {
            Color = entering ? new Color(Color.Yellow, 0) : Color.Yellow;
        }

        public ColoredText(Game game, string text, bool entering, string name)
            : base(game, text, entering, name)
        {
            Color = entering ? new Color(Color.Yellow, 0) : Color.Yellow;

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, Text, Position, Color, 0.0f, Vector2.Zero, 1, SpriteEffects.None,
                                   0.0f);
            spriteBatch.End();
        }
    }
}