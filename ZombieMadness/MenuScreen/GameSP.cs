using MenuScreen.MenuScreen.Texts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MenuScreen.MenuScreen
{
    public class GameSP : Father.MenuScreen
    {
        public GameSP(Game game, Father.MenuScreen previousMenuScreen)
            : base(game)
        {
            PreviousMenuScreen = previousMenuScreen;
            MenuEntries.Add(new SimpleText(Game, "Back", true));
            Title = new Title(Game, "");
            SetPositions();


            Text = new SimpleText(Game, "", true)
            {
                Position = new Vector2(MenuEntries[0].Position.X, 300)
            };
            Text.SpriteFont = game.Content.Load<SpriteFont>("SpriteFont2");
            Text.EnterScreen();

            Game.Components.Add(Text);
        }
   
    }
}