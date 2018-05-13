using MenuScreen.MenuScreen.Texts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MenuScreen.MenuScreen
{
    public class About : Father.MenuScreen
    {
        public About(Game game, Father.MenuScreen previousMenuScreen) : base(game)
        {
            PreviousMenuScreen = previousMenuScreen;
            MenuEntries.Add(new SimpleText(Game, "Back", true));
            Title = new Title(Game, "");
            SetPositions();

            Text = new SimpleText(Game, "This game was created by Dima Kalinichenko\nFrom Mekif Gimel, Ashdod 12th grade\nIn 2011", true)
            {
                Position = new Vector2(MenuEntries[0].Position.X, 300)
            };
            Text.SpriteFont = game.Content.Load<SpriteFont>("SpriteFont2");
            Text.EnterScreen();

            Game.Components.Add(Text);
        }
    }
}