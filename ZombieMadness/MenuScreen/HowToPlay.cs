using MenuScreen.MenuScreen.Texts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MenuScreen.MenuScreen
{
    class HowToPlay : Father.MenuScreen
    {
        public HowToPlay(Game game, Father.MenuScreen previousMenuScreen) : base(game)
        {
            PreviousMenuScreen = previousMenuScreen;
            MenuEntries.Add(new SimpleText(Game, "Back", true));
            Title = new Title(Game, "");
            SetPositions();

            Text = new SimpleText(Game, "The objective of the game\nis to kill as many zombies as possible\nbefore they will kill you.\n\n\nControls:\nW-Move forward towrads the cursor\nS-Move backwards away from the cursor\nLeft mouse click to shoot\nP-Pause the game", true)
            {
                Position = new Vector2(MenuEntries[0].Position.X, 300)
            };
            Text.SpriteFont = game.Content.Load<SpriteFont>("SpriteFont2");
            Text.EnterScreen();

            Game.Components.Add(Text);
        }
    }
}