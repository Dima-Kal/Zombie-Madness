using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace ZombieMadnessSP
{

    /// <summary>
    /// This is the main game class
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        private Int64 ShootDelay = 0, ReloadDelay = 0, dAngleDelay = 0, SpawnDelay = 0, DamageDelay = 0, PauseDelay = 0;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D Blank, Cursor, Bar, Bullet,particle;        
        private Map map;
        private SpriteFont font;
        private Vector2 ScreenSize;
        private ParticleEngine particleEngine;        
        private Player Player;
        private bool shoot = false, Reloading = false;
        private bool paused = false;
        public static ContentManager content;

        private SoundEffect GunShot;
        private SoundEffect GunReload;
        private SoundEffect ZombieDeath;

        private List<Zombie> Zombies = new List<Zombie>();
        private List<Zombie> ZombieBodies = new List<Zombie>();

        int SpawnCount = 0, BodiesCount = 0;


       

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferWidth = 1024;
            this.graphics.PreferredBackBufferHeight = 768;
            ScreenSize.X = this.graphics.PreferredBackBufferWidth;
            ScreenSize.Y = this.graphics.PreferredBackBufferHeight;
            graphics.SynchronizeWithVerticalRetrace = false;
            this.graphics.IsFullScreen = true;

            Content.RootDirectory = "Content";


        }
 
        protected override void Initialize()
        {        
            base.Initialize();
            CollisionDetection2D.AdditionalRenderTargetForCollision = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 1, graphics.GraphicsDevice.DisplayMode.Format);
            content = Content;
        }
  
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);         
            Player = new Player(Content);
            Player.X = this.Window.ClientBounds.Width / 2;
            Player.Y = this.Window.ClientBounds.Height / 2;
            map = new Map(Player,Content,ScreenSize);


            Cursor = Content.Load<Texture2D>("Crosshair");
            Bullet = Content.Load<Texture2D>("Bullet");
            Blank = Content.Load<Texture2D>("Blank");
            Bar = Content.Load<Texture2D>("Bar");
            particle = Content.Load<Texture2D>("Circle");
            font = Content.Load<SpriteFont>("MyFont");

            GunShot = Content.Load<SoundEffect>("Audio/GunShot");
            GunReload = Content.Load<SoundEffect>("Audio/GunReload");
            ZombieDeath = Content.Load<SoundEffect>("Audio/Zombie");

            particleEngine = new ParticleEngine(particle, new Vector2(400, 240),ScreenSize);
         
        }


        protected override void UnloadContent()
        {

        }

      
        protected override void Update(GameTime gameTime)
        {
            SpawnDelay++;
            ShootDelay++;
            ReloadDelay++;
            dAngleDelay++;
            DamageDelay++;
            PauseDelay++;

            Recoil();

            ProcessKeyboard();
            if (Reloading)
                Reload();

            particleEngine.UpdateParticeMovement();

            CheckBulletCollision();
            CheckZombieCollision();

            if (SpawnCount <50)
            {
               SpawnZombie();
            }

            base.Update(gameTime);
        }

        private void ProcessKeyboard()
        {
            KeyboardState keyboard = Keyboard.GetState();

            if (Player.Alive)
            {
                if (PauseDelay > 10)
                {
                    if (keyboard.IsKeyDown(Keys.Escape))
                    {
                        if (!paused)
                            paused = true;
                        else paused = false;
                        PauseDelay = 0;
                    }
                }
            }

            if (paused)
            {
                if (keyboard.IsKeyDown(Keys.Q))
                {
                    this.Exit();
                }
            }

            if (paused)
            {
                if (keyboard.IsKeyDown(Keys.R))
                {
                    Player = new Player(content);
                    Player.X = ScreenSize.X/ 2;
                    Player.Y = ScreenSize.Y / 2;
                    BodiesCount = 0;
                    SpawnCount = 0;
                    Zombies.Clear();
                    ZombieBodies.Clear();
                    paused = false;
                    
                }
            }

            if (!paused)
            {
                if (keyboard.IsKeyDown(Keys.W))
                {
                    Player.X += (float)((double)Player.Speed * Math.Cos(Player.Angle));
                    Player.Y += (float)((double)Player.Speed * Math.Sin(Player.Angle));
                }
                if (keyboard.IsKeyDown(Keys.S))
                {
                    if ((Player.X < ScreenSize.X) && (Player.X > 0))
                        Player.X -= (float)((double)Player.Speed * Math.Cos(Player.Angle));
                    else
                        Player.X += (float)((double)10 * Math.Cos(Player.Angle));

                    if ((Player.Y < ScreenSize.Y) && (Player.Y > 0))
                        Player.Y -= (float)((double)Player.Speed * Math.Sin(Player.Angle));
                    else
                        Player.Y += (float)((double)10 * Math.Cos(Player.Angle));
                }

                if (keyboard.IsKeyDown(Keys.R))
                {
                    GunReload.Play();
                    ReloadWeapon();
                }

                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (!Reloading)
                    {
                        if (ShootDelay > 5)
                        {
                            Player.Shoot(Player);
                            ShootDelay = 0;
                            GunShot.Play();
                        }
                        if (Player.CurrentBullet == 30)
                        {
                            GunReload.Play();
                            ReloadDelay = 0;
                            Reloading = true;
                        }

                        shoot = true;
                    }

                }
                Player.Angle = Math.Atan2(Mouse.GetState().Y + 10 - Player.Y, Mouse.GetState().X + 10 - Player.X);

                foreach (Zombie zombie in Zombies)
                {
                    zombie.WalkToPlayer();
                }
            }
        }

        

        public void SpawnZombie()
        {
            if (!paused)
            {
                if (SpawnDelay > 60)
                {
                    Zombies.Add(new Zombie(Player, ScreenSize, true, Content));
                    SpawnCount++;
                    SpawnDelay = 0;
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            DrawScenery();
            DrawZombieBodies();
            DrawBullets();
            DrawPlayer();
            DrawZombies();
            particleEngine.Draw(spriteBatch);
            DrawPlayerInfo();

            if (paused)
                DrawPauseMenu();
      
            

            spriteBatch.End();

            base.Draw(gameTime);
        }


        #region Draw Game Content
        private void DrawScenery()
        {
            map.Draw(spriteBatch);            
        }

        private void DrawPlayer()
        {
            Player.Draw(spriteBatch);          
            spriteBatch.Draw(Cursor, new Rectangle(Mouse.GetState().X - 10 - ((int)(Player.DigressionAngle / 1.2)), Mouse.GetState().Y - 10 - ((int)(Player.DigressionAngle / 1.2)), (int)Player.DigressionAngle + 50, (int)Player.DigressionAngle + 50), Color.White);     
        }

        private void DrawZombies()
        {
            foreach (Zombie zombie in Zombies)
            {
                zombie.Draw(spriteBatch);
            }
        }

        private void DrawZombieBodies()
        {
            foreach (Zombie deadZombie in ZombieBodies)
            {
                deadZombie.Draw(spriteBatch);
            }
        }

        private void DrawBullets()
        {
            if (shoot)
            {
                for (int i = 0; i < Player.CurrentBullet; i++)
                {
                    if (Player.Bullet[i] != null)
                    {
                        Player.Bullet[i].Draw(spriteBatch);
                        Player.Bullet[i].X += (float)((double)Player.Bullet[i].Speed * Math.Cos(Player.Bullet[i].Angle));
                        Player.Bullet[i].Y += (float)((double)Player.Bullet[i].Speed * Math.Sin(Player.Bullet[i].Angle));
                    }
                }
                
            }

        }

        private void DrawPlayerInfo()
        {
            int mag = 30;
            spriteBatch.DrawString(font, (mag - Player.CurrentBullet).ToString(), new Vector2(ScreenSize.X-100,0), Color.White);
            //Draw the bullet amount
            spriteBatch.Draw(Bar, new Rectangle((int)ScreenSize.X - 110, 35, 110, 15), new Rectangle(0, 45, Bar.Width, 44), Color.Gray);
            //Draw the box around the  reloading bar
            spriteBatch.Draw(Bar, new Rectangle((int)ScreenSize.X - 110, 35, 110, 15), new Rectangle(0, 0, Bar.Width, 44), Color.White);
            //Draw the box inside the reloading bar

            if (!paused)
            {
                if (Reloading)
                    spriteBatch.Draw(Bar, new Rectangle((int)ScreenSize.X - 110, 36, (int)(110 * ((double)ReloadDelay / 50)), 13), new Rectangle(0, 45, Bar.Width, 44), Color.Blue);
                //draw the reloading process
            }

            spriteBatch.Draw(Bar, new Rectangle(10, 35, 110, 15), new Rectangle(0, 45, Bar.Width, 44), Color.Gray);
            //Draw the box around the health bar
            spriteBatch.Draw(Bar, new Rectangle(10, 35, 110, 15), new Rectangle(0, 0, Bar.Width, 44), Color.White);
            //Draw the current health level based on the current Health
            spriteBatch.Draw(Bar, new Rectangle((int)10, 36, (int)(Player.Health + 10), 13), new Rectangle(0, 45, Bar.Width, 44), Color.Red);

            spriteBatch.DrawString(font, "Kills:" + BodiesCount.ToString(), new Vector2(ScreenSize.X - 150, ScreenSize.Y - 50), Color.Red);

        }

        private void DrawPauseMenu()
        {
            if (Player.Alive)
            {
                spriteBatch.DrawString(font, "Press ESC to resume", new Vector2(ScreenSize.X / 2 - 150, ScreenSize.Y / 2 - 50), Color.Salmon);
            }
            if (!Player.Alive)
            {
                spriteBatch.DrawString(font, "The zombies got you!, you have managed to kill " + BodiesCount.ToString() + " Zombies!", new Vector2(ScreenSize.X / 2 - 500, ScreenSize.Y / 2 - 100), Color.Red);
            }

            spriteBatch.DrawString(font, "Press Q to quit to menu", new Vector2(ScreenSize.X / 2 - 150, ScreenSize.Y / 2), Color.Salmon);
            spriteBatch.DrawString(font, "Press R to play again", new Vector2(ScreenSize.X / 2 - 150, ScreenSize.Y / 2+50), Color.Salmon);

        }
        #endregion

          
        private void Recoil()
        {
            if (Reloading)
            {
                if (dAngleDelay > 1)
                {
                    if (!(Player.DigressionAngle <= 20))
                    {
                        Player.DigressionAngle -= 4;
                        dAngleDelay = 0;
                    }
                }
            }
            else
            {
                if (dAngleDelay > 1)
                {
                    if (!(Player.DigressionAngle <= 20))
                    {
                        Player.DigressionAngle--;
                        dAngleDelay = 0;
                    }
                }
            }
        }

        #region Weapon Reload
        private void Reload()
        {
            if (ReloadDelay > 50)
            {
                Player.DecreaseAmmo();
                Reloading = false;
                Player.CurrentBullet = 0;
                ReloadDelay = 0;
            }
        }

        private void ReloadWeapon()
        {
            ReloadDelay = 0;
            Reloading = true;
        }
        #endregion

       
        public void CheckBulletCollision()
        {
            if (!paused)
            {
                for (int i = 0; i < Player.CurrentBullet; i++)
                {
                    for (int j = 0; j < SpawnCount; j++)
                    {
                        if (Player.Bullet[i] != null)
                        {
                            if (Zombies[j] != null)
                            {
                                if (CollisionDetection2D.BoundingRectangleNR((int)Zombies[j].X - (int)Player.Origin.X, (int)Zombies[j].Y - (int)Player.Origin.Y, Zombies[j].Height, Zombies[j].Width, (int)Player.Bullet[i].X, (int)Player.Bullet[i].Y, Player.Bullet[i].Height, Player.Bullet[i].Width))
                                {
                                    if (CollisionDetection2D.PerPixelWR(Zombies[j].Image, Bullet, new Vector2(Zombies[j].X, Zombies[j].Y), new Vector2(Player.Bullet[i].X, Player.Bullet[i].Y), Zombies[j].Origin, Player.Bullet[i].Origin, Zombies[j].RectanglePoints, Player.Bullet[i].RectanglePoints, (float)Zombies[j].Angle, (float)Player.Bullet[i].Angle, spriteBatch))
                                    {
                                        Zombies[j].DecreaseHealth();
                                        if (!Zombies[j].Alive)
                                        {
                                            ZombieDeath.Play();
                                            ZombieBodies.Add(new Zombie(Player, ScreenSize, false, content));
                                            ZombieBodies[BodiesCount].X = Zombies[j].X;
                                            ZombieBodies[BodiesCount].Y = Zombies[j].Y;
                                            ZombieBodies[BodiesCount].Angle = Zombies[j].Angle;
                                            BodiesCount++;
                                            Zombies.Remove(Zombies[j]);
                                            SpawnCount--;
                                        }
                                        particleEngine.UpdateBullet(Player.Bullet[i]);
                                        particleEngine.TotalParticles = 20;
                                        particleEngine.EmitterLocation = new Vector2(Player.Bullet[i].X, Player.Bullet[i].Y);
                                        Player.Bullet[i] = null;

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void CheckZombieCollision()
        {
            if (!paused)
            {
                if (DamageDelay > 10)
                {

                    for (int j = 0; j < SpawnCount; j++)
                    {
                        {
                            if (Zombies[j] != null)
                            {
                                if (CollisionDetection2D.BoundingRectangleNR((int)Player.X - (int)Zombies[j].Origin.X, (int)Player.Y - (int)Zombies[j].Origin.Y, Player.Height, Player.Width, (int)Zombies[j].X, (int)Zombies[j].Y, Zombies[j].Height, Zombies[j].Width))
                                {
                                    if (CollisionDetection2D.PerPixelWR(Player.Image, Zombies[j].Image, new Vector2(Player.X, Player.Y), new Vector2(Zombies[j].X, Zombies[j].Y), Player.Origin, Zombies[j].Origin, Player.RectanglePoints, Zombies[j].RectanglePoints, (float)Player.Angle, (float)Zombies[j].Angle, spriteBatch))
                                    {                                       
                                        Player.DecreaseHealth();
                                        if (!Player.Alive)
                                            paused = true;
                                        particleEngine.TotalParticles = 10;
                                        particleEngine.EmitterLocation = new Vector2(Player.X, Player.Y);
                                        DamageDelay = 0;

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

