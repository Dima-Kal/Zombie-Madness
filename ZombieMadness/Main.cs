using System;
using System.Collections.Generic;
using System.Linq;
using MenuScreen.MenuScreen;
using MenuScreen.MenuScreen.Texts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace MenuScreen
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Main : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private MainMenu menu;
        private Texture2D Background,about,howToPlay;
        private Song Music;
        private bool PlayOnce = false;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferWidth = 1024;
            this.graphics.PreferredBackBufferHeight = 768;
            Content.RootDirectory = "Content";
            
        }

        protected override void Initialize()
        {
            menu = new MainMenu(this);
            Components.Add(menu);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Background = Content.Load<Texture2D>("background");
            about = Content.Load<Texture2D>("about");
            howToPlay = Content.Load<Texture2D>("howtoplay");
            Music = Content.Load<Song>("MenuMusic");
            MediaPlayer.Play(Music);
            MediaPlayer.IsRepeating = true;

        }

        protected override void Update(GameTime gameTime)
        {
            if (!this.IsActive)
            {
                if (PlayOnce)
                    MediaPlayer.Pause();
                PlayOnce = false;
            }
            else
            {
                if (!PlayOnce)
                {
                    MediaPlayer.Resume();
                    PlayOnce = true;
                }
            }
                
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(Background, new Vector2(0, 0), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
