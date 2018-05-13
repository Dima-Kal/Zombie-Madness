using System.Collections.Generic;
using MenuScreen.MenuScreen.Texts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Threading;

namespace MenuScreen.MenuScreen.Father
{
    public class MenuScreen : DrawableGameComponent
    {
        private Title title;
        private List<SimpleText> menuEntries;
        private MenuScreen previousMenuScreen;

        protected SimpleText Text;

        private KeyboardState previousKeyboardState;
        private KeyboardState keyboardState;

        Thread threadSP = new Thread(GameSP);
        Thread threadMP = new Thread(GameMP);

        private const int OFFSET = 80;

        protected MenuScreen PreviousMenuScreen
        {
            get
            {
                return previousMenuScreen;
            }
            set
            {
                previousMenuScreen = value;
            }
        }

        protected Title Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
            }
        }

        protected List<SimpleText> MenuEntries
        {
            get
            {
                return menuEntries;
            }
            set
            {
                menuEntries = value;
            }
        }

        public MenuScreen(Game game)
            : base(game)
        {
            menuEntries = new List<SimpleText>();
            previousKeyboardState = Keyboard.GetState();
        }

        public MenuScreen(Game game, MenuScreen screen)
            : this(game)
        {
            foreach (SimpleText text in screen.menuEntries)
            {
                menuEntries.Add(new SimpleText(Game, text.Text, true, text.Name));
            }
            title = new Title(Game, screen.title.Text);
            SetPositions();
        }

        public override void Initialize()
        {
            foreach (SimpleText text in menuEntries)
            {
                text.EnterScreen();
            }
            title.EnterScreen();
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            if (!menuEntries[0].IsEntering && !menuEntries[0].IsLeaving)
            {
                if (keyboardState.IsKeyDown(Keys.Down) && previousKeyboardState.IsKeyUp(Keys.Down))
                {
                    Next();
                }
                if (keyboardState.IsKeyDown(Keys.Up) && previousKeyboardState.IsKeyUp(Keys.Up))
                {
                    Previous();
                }
                if (keyboardState.IsKeyDown(Keys.Enter) && previousKeyboardState.IsKeyUp(Keys.Enter))
                {
                    Enter();
                }
            }

            foreach (SimpleText text in menuEntries)
            {
                text.Update(gameTime);
            }
            previousKeyboardState = keyboardState;
            title.Update(gameTime);
            base.Update(gameTime);

            if (!title.IsEntering && !title.IsLeaving && title.Position.Y < -30)
                Game.Components.Remove(this);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (SimpleText text in menuEntries)
            {
                text.Draw(gameTime);
            }
            title.Draw(gameTime);
            base.Draw(gameTime);
        }

        private void Next()
        {
            for (int i = 0; i < menuEntries.Count; i++)
            {
                if (menuEntries[i] is ColoredText)
                {
                    if (i == menuEntries.Count - 1)
                    {
                        menuEntries[i] = new SimpleText(Game, menuEntries[i].Text, false, menuEntries[i].Name);
                        menuEntries[0] = new ColoredText(Game, menuEntries[0].Text, false, menuEntries[0].Name);
                    }
                    else
                    {
                        menuEntries[i] = new SimpleText(Game, menuEntries[i].Text, false, menuEntries[i].Name);
                        menuEntries[i + 1] = new ColoredText(Game, menuEntries[i + 1].Text, false, menuEntries[i + 1].Name);
                    }
                    SetPositionsAfterEntarence();
                    return;
                }
            }
        }

        private void Previous()
        {
            for (int i = 0; i < menuEntries.Count; i++)
            {
                if (menuEntries[i] is ColoredText)
                {
                    if (i == 0)
                    {
                        menuEntries[0] = new SimpleText(Game, menuEntries[0].Text, false, menuEntries[0].Name);
                        menuEntries[menuEntries.Count - 1] = new ColoredText(Game,
                                                                            menuEntries[menuEntries.Count - 1].Text,
                                                                            false,
                                                                            menuEntries[menuEntries.Count - 1].Name);
                    }
                    else
                    {
                        menuEntries[i] = new SimpleText(Game, menuEntries[i].Text, false, menuEntries[i].Name);
                        menuEntries[i - 1] = new ColoredText(Game, menuEntries[i - 1].Text, false,
                                                            menuEntries[i - 1].Name);
                    }
                    SetPositionsAfterEntarence();
                    return;
                }
            }
        }

        public void Enter()
        {
            foreach (SimpleText text in menuEntries)
            {
                text.LeaveScreen();
            }
            title.LeaveScreen();
            foreach (SimpleText text in menuEntries)
            {
                if (text is ColoredText)
                {
                    switch (text.Name)
                    {
                        case "GameSP":
                            GameSP GameSP = new GameSP(Game, this);
                            Game.Components.Add(GameSP);
                            threadSP.Start();
                            break;
                        case "GameMP":
                            GameMP GameMP = new GameMP(Game, this);
                            Game.Components.Add(GameMP);
                            threadMP.Start();
                            break;
                        case "HowToPlay":
                            HowToPlay howToPlay = new HowToPlay(Game, this);
                            Game.Components.Add(howToPlay);
                            break;
                        case "About":
                            About credits = new About(Game, this);
                            Game.Components.Add(credits);
                            break;
                        case "Back":
                            MenuScreen back = new MenuScreen(Game, previousMenuScreen);
                            Game.Components.Add(back);
                            Text.LeaveScreen();
                            threadMP.Abort();
                            threadSP.Abort();
                            break;
                        case "Exit":
                            Game.Exit();
                            break;
                    }
                }
            }
        }

        public void SetPositions()
        {
            title.Position = new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - 200, -40);
            Vector2 pos = new Vector2(Game.GraphicsDevice.Viewport.Width - 306, 250);
            int count = 0;
            foreach (SimpleText text in menuEntries)
            {
                if (text is ColoredText)
                    count++;
                text.Position = pos;
                pos.Y += OFFSET;
            }
            if (count == 0)
            {
                Vector2 tempPos = menuEntries[0].Position;
                menuEntries[0] = new ColoredText(Game, menuEntries[0].Text, true, menuEntries[0].Name)
                {
                    Position = tempPos
                };
            }
        }

        public void SetPositionsAfterEntarence()
        {
            Vector2 pos = new Vector2(30, 250);
            foreach (SimpleText text in menuEntries)
            {
                text.Position = pos;
                pos.Y += OFFSET;
            }
        }

        private static void GameSP()
        {
            using (ZombieMadnessSP.Game game = new ZombieMadnessSP.Game())
            {
                game.Run();
            }
        }

        private static void GameMP()
        {
            using (ZombieMadnessMP.Game game = new ZombieMadnessMP.Game())
            {
                game.Run();
            }
        }
    }
}