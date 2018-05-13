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
    /// This is the main player class
    /// </summary>
    
    public class Player : ItemOnScreen
    {
        /// <summary>
        /// Player's health
        /// </summary>
        private int health;
        /// <summary>
        /// Player's current ammo amount
        /// </summary>
        private int ammo;
        /// <summary>
        /// Bullet counter
        /// </summary>
        private int currentBullet = 0;
        /// <summary>
        /// Player's walking speed
        /// </summary>
        private int speed = 5;
        /// <summary>
        /// Is player alive
        /// </summary>
        private bool alive;
        /// <summary>
        /// Bullet's diggresion angle for the recoil
        /// </summary>
        private double digressionAngle = 10;
        /// <summary>
        /// Bullets array
        /// </summary>
        private Bullet[] Magazine = new Bullet[30];
        private ContentManager content;

        public Bullet[] Bullet
        {
            get { return Magazine; }
        }

        public bool Alive
        {
            get { return alive; }
        }

        public int CurrentBullet
        {
            get { return currentBullet; }
            set { currentBullet = value; }
        }

        public int Health
        {
            get { return health; }
            set { health = value; }
        }
      
        public int Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public int Ammo
        {
            get { return ammo; }
            set { ammo = value; }
        }       

        public Double DigressionAngle
        {
            get { return digressionAngle; }
            set { digressionAngle = value; }
        }
	

        public Player(ContentManager content)
        {
            this.content = content;
            height = 70;
            width = 70;
            ammo = 180;
            health = 100;
            alive = true;

            origin = new Vector2(37, 40);
            image = content.Load<Texture2D>("Character/SoldierDefault");
            for (int i = 0; i < 30; i++)
            {
                Magazine[i] = new Bullet(0, 0, 0, 0,content);   
            }
        }

        public void Shoot(Player player)
        {
            digressionAngle += 5;
            if (currentBullet < 30)
            {
                Magazine[currentBullet] = new Bullet(player.Angle, digressionAngle, player.X, player.Y,content);
            }
            else
            {
                currentBullet = 0;
            }
            currentBullet++;  
        }

        public void DecreaseAmmo()
        {
            this.ammo -= 30;
        }


        public void DecreaseHealth()
        {
            health -= new Random().Next(5, 15);
            if (health < 0)
                alive = false;
        }
    }
}
