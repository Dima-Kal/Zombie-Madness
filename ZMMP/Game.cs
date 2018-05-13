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
using Lidgren.Library.Network;
using Lidgren.Library.Network.Xna;


namespace ZombieMadnessMP
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
        private Player NetworkPlayer;
        private bool shoot = false, Reloading = false;
        private bool paused = true;
        public static ContentManager content;

        private SoundEffect GunShot;
        private SoundEffect GunReload;
        private SoundEffect ZombieDeath;

        private List<Zombie> Zombies = new List<Zombie>();
        private List<Zombie> ZombieBodies = new List<Zombie>();

        int SpawnCount = 0, BodiesCount = 0;

        private NetClient _client;
        private NetServer _server;
        private NetLog _log;
        private NetAppConfiguration _nac;
        private Vector2 OldPos;
        private Vector2 PlayerPos;
        private Vector2 NetworkPlayerPos;
        private string IP;
        private bool ClientConnected = false;

        private System.Windows.Forms.Form ConnectForm = new System.Windows.Forms.Form();
        private System.Windows.Forms.TextBox Textbox = new System.Windows.Forms.TextBox();
        private System.Windows.Forms.Button ConnectButton = new System.Windows.Forms.Button();
        private System.Windows.Forms.Label Label = new System.Windows.Forms.Label();

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferWidth = 1024;
            this.graphics.PreferredBackBufferHeight = 768;
            ScreenSize.X = this.graphics.PreferredBackBufferWidth;
            ScreenSize.Y = this.graphics.PreferredBackBufferHeight;
            graphics.SynchronizeWithVerticalRetrace = false;
            //this.graphics.IsFullScreen = true;

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
            NetworkPlayer = new Player(Content);
            NetworkPlayer.X = this.Window.ClientBounds.Width / 2;
            NetworkPlayer.Y = this.Window.ClientBounds.Height / 2;
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

        public void SetUpConnectForm()
        {
            Label.Text = "Enter IP to connect";
            Label.Location = new System.Drawing.Point(40, 10);
            Label.Size = new System.Drawing.Size(100, 15);
            ConnectForm.Controls.Add(Label);

            Textbox.Location = new System.Drawing.Point(45, 30);
            Textbox.Size = new System.Drawing.Size(90, 10);
            ConnectForm.Controls.Add(Textbox);

            ConnectButton.Text = "Connect";
            ConnectButton.Size = new System.Drawing.Size(80,20);
            ConnectButton.Location = new System.Drawing.Point(47, 60);
            ConnectButton.Click += new System.EventHandler(Connect);
            ConnectForm.Controls.Add(ConnectButton);


            ConnectForm.Show();
            ConnectForm.Width = 200;
            ConnectForm.Height = 140;
            ConnectForm.Location = new System.Drawing.Point((int)ScreenSize.X / 2, (int)ScreenSize.Y / 2);


        }

        public void Connect(object sender, System.EventArgs e)
        {
            IP = Textbox.Text;
            ClientClicked();
            ConnectForm.Visible = false;
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
              // SpawnZombie();
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
                            //GunShot.Play();
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


            if (_client != null || _server != null)
            {
                UpdateNetwork();
            }
            else
            {
                //We get the state of the keyboard at this moment which will hold keys pressed ect.
                //if the H key is pressed we call the HostClicked Routine.
                if (keyboard.IsKeyDown(Keys.H) || keyboard.IsKeyDown(Keys.S))
                {
                    HostClicked();
                }
                //If the C key is pressed we call the ClientClicked Routine
                if (keyboard.IsKeyDown(Keys.C))
                {
                    SetUpConnectForm();
                }
            }
        }

        public void HostClicked()
        {
            //This sets up the configuration, when your setting the config up for server we pick the application name and the port that the server 
            //will bound to when we create it
            _nac = new NetAppConfiguration("ZombieMadness", 26530);
            //set the maximum connects, im not sure the biggest number aloud at this moment in time
            _nac.MaximumConnections = 32;
            //This is used for somthing later on in the tutorials, where the client can discover hosts.
            _nac.ServerName = "ZombieMadness!";
            //Initiate the log
            _log = new NetLog();
            //Create the server using the Log and the Configuration, this will bound the port on the computer.
            _server = new NetServer(_nac, _log);
            // Add an Event Handler for when a client connects or disconnects.
            _server.StatusChanged += new EventHandler<NetStatusEventArgs>(server_StatusChanged);
        }
        public void ClientClicked()
        {
            _nac = new NetAppConfiguration("ZombieMadness");
            _log = new NetLog();
            //Set it up so that the Log Igores Nothing
            _log.IgnoreTypes = NetLogEntryTypes.None;
            //We want to Output to file, this shows how to do so.
            _log.IsOutputToFileEnabled = true;
            //If we output to a file we need to big a name, this was simple
            _log.OutputFileName = "Client.html";
            _client = new NetClient(_nac, _log);
            _client.StatusChanged += new EventHandler<NetStatusEventArgs>(client_StatusChanged);
            _client.Configuration.ApplicationIdentifier = "ZombieMadness";
            _client.ServerDiscovered += new EventHandler<NetServerDiscoveredEventArgs>(_client_ServerDiscovered);
            _client.DiscoverKnownServer(IP, 26530);
        }

        void _client_ServerDiscovered(object sender, NetServerDiscoveredEventArgs e)
        {
            if (e.ServerInformation.ServerName == "ZombieMadness!")
                _client.Connect(e.ServerInformation.RemoteEndpoint.Address, e.ServerInformation.RemoteEndpoint.Port);
        }

        void client_StatusChanged(object sender, NetStatusEventArgs e)
        {
            //If the client has connected to the server
            if (e.Connection.Status == NetConnectionStatus.Connected)
            {
                ClientConnected = true;
            }
            //If the Client was disconnected
            if (e.Connection.Status == NetConnectionStatus.Disconnected)
            {
                ClientConnected = false;
            }
        }
        void server_StatusChanged(object sender, NetStatusEventArgs e)
        {
            if (e.Connection.Status == NetConnectionStatus.Connecting)
            {
                ClientConnected = true;
            }
            if (e.Connection.Status == NetConnectionStatus.Disconnected)
            {
                ClientConnected = false;
            }
        }

        //Called everytime the main games UpdateRoutine is called
        public void UpdateNetwork()
        {
            PlayerPos.X = Player.X;
            PlayerPos.Y = Player.Y;
            //int angle = (int)((Player.Angle * 180) / Math.PI);
            Int64 angle = (Int64)(Player.Angle * 1000000000000000000);

            //If the client is chosen this will be called
            if (_client != null)
            {
                //Pump the client session with any new information sent or received
                _client.Heartbeat();
                //Initialize a new message
                NetMessage msg;
                //Message will continue you iterate though any messages received from the Server.
                while ((msg = _client.ReadMessage()) != null)
                {
                    //We Send the message of to be read and interpreted
                    HandleNetworkingMessage(msg);
                }

               // if (PlayerPos != OldPos)
                {
                    msg = new NetMessage();
                    msg.Write(angle);
                    XnaSerialization.Write(msg, PlayerPos);
                    _client.SendMessage(msg, NetChannel.ReliableUnordered);

                    OldPos = PlayerPos;
                }
            }
            //If the Server is chosen this will be called
            if (_server != null)
            {
                _server.Heartbeat();
                NetMessage msg;

                while ((msg = _server.ReadMessage()) != null)
                {
                    HandleNetworkingMessage(msg);
                }

                msg = new NetMessage();
                msg.Write(angle);
                XnaSerialization.Write(msg, PlayerPos);

                foreach (NetConnection conn in _server.Connections)
                {
                    if (conn != null && conn.Status == NetConnectionStatus.Connected)
                        _server.SendMessage(msg, conn, NetChannel.ReliableUnordered);
                }

            }
        }
        public void HandleNetworkingMessage(NetMessage msg)
        {
            //We read the Messages
            try
            {
                Int64 angle = msg.ReadInt64();
                //angle = (double)((angle * 180) / Math.PI);
                NetworkPlayer.Angle = (double)angle / 1000000000000000000;
                //NetworkPlayer.Angle = (double)((angle * 180) / Math.PI);
                NetworkPlayerPos = XnaSerialization.ReadVector2(msg);
                NetworkPlayer.X = NetworkPlayerPos.X;
                NetworkPlayer.Y = NetworkPlayerPos.Y;
            }
            catch (Exception)
            {
                throw;
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
            if ((_client != null) || (_server != null))
            {
                DrawZombieBodies();
                DrawBullets();
                DrawPlayer();
                DrawZombies();
                particleEngine.Draw(spriteBatch);
            }
            DrawPlayerInfo();


            if (paused)
                if ((_client != null) || (_server != null))
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
            if ((_client != null) || (_server != null))
            {
                Player.Draw(spriteBatch);
                if (ClientConnected)
                    NetworkPlayer.Draw(spriteBatch);
                spriteBatch.Draw(Cursor, new Rectangle(Mouse.GetState().X - 10 - ((int)(Player.DigressionAngle / 1.2)), Mouse.GetState().Y - 10 - ((int)(Player.DigressionAngle / 1.2)), (int)Player.DigressionAngle + 50, (int)Player.DigressionAngle + 50), Color.White);
            }
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

            if ((_client == null) && (_server == null))
            {
                spriteBatch.DrawString(font, "Press C to be a client", new Vector2(ScreenSize.X / 2 - 150, ScreenSize.Y / 2), Color.Salmon);
                spriteBatch.DrawString(font, "Press H to be a host", new Vector2(ScreenSize.X / 2 - 150, ScreenSize.Y / 2-40), Color.Salmon);

            }

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

