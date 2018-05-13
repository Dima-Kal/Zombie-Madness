using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// This is the zombie class
    /// it inherits from the ItemOnScreen base class
    /// </summary>
    class Zombie:ItemOnScreen
    {
        /// <summary>
        /// The zombie's total health
        /// </summary>
        private int health;
        /// <summary>
        /// The zombie's walking speed
        /// </summary>
        private int speed;
        /// <summary>
        /// boolean to check if the zombie is alive
        /// </summary>
        private bool alive;
        /// <summary>
        /// Player to attack
        /// </summary>
        private Player player;
        private ContentManager content;
        private Vector2 ScreenSize;

        public Zombie(Player player,Vector2 ScreenSize,bool alive,ContentManager content)
        {
            this.ScreenSize = ScreenSize;
            this.origin = new Vector2(37, 40);
            this.height = 70;
            this.width = 70;
            this.content = content;
            this.health = 100;
            this.speed = 3;
            this.player = player;
            this.Angle = Math.Atan2(player.Y - Y, player.X - X);
            this.alive = alive;
            if (alive)
                this.image = content.Load<Texture2D>("Character/Zombie");
            if (!alive)
                this.image = content.Load<Texture2D>("Character/ZombieBody");
            StartPoint();
        }

        public void WalkToPlayer()
        {
            Angle = Math.Atan2(player.Y - Y, player.X - X);
            
            X += (float)((double)speed * Math.Cos(Angle));
            Y += (float)((double)speed * Math.Sin(Angle));
        }

        public bool Alive
        {
            get { return alive; }
        }

        public void DecreaseHealth()
        {
            this.health -= new Random().Next(5, 30);
            if (health < 0)
                this.alive = false;
        }

        public void StartPoint()
        {
            int Section = new Random().Next(1, 5);

            if (Section == 1)
            {
                y = 0;
                x = new Random().Next(0, (int)ScreenSize.X);
            }

            if (Section == 2)
            {
                y = ScreenSize.Y;
                x = new Random().Next(0, (int)ScreenSize.X);
            }

            if (Section == 3)
            {
                x = 0;
                y = new Random().Next(0, (int)ScreenSize.Y);
            }

            if (Section == 1)
            {
                x = ScreenSize.X;
                y = new Random().Next(0, (int)ScreenSize.X);
            }


        }
    }
}
