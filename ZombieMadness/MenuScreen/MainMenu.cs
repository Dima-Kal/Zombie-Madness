using MenuScreen.MenuScreen.Texts;
using Microsoft.Xna.Framework;

namespace MenuScreen.MenuScreen
{
    public class MainMenu : Father.MenuScreen
    {
        public MainMenu(Game game) : base(game)
        {
            MenuEntries.Add(new SimpleText(Game, "Single Player", true, "GameSP"));
            MenuEntries.Add(new SimpleText(Game, "Multiplayer", true, "GameMP"));
            MenuEntries.Add(new SimpleText(Game, "How To Play", true, "HowToPlay"));
            MenuEntries.Add(new SimpleText(Game, "About", true, "About"));
            MenuEntries.Add(new SimpleText(Game, "Exit", true));
            Title = new Title(Game, "");
            SetPositions();
        }
    }
}