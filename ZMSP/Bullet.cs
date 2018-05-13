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
    /// This class creates a single bullet object that used by the player
    /// </summary>
    public class Bullet : ItemOnScreen
    {
        /// <summary>
        /// Bullet movement speed
        /// </summary>
        private const int speed = 30;
        /// <summary>
        /// Random angle for the recoil effect
        /// </summary>
        private double randAngle;

        public int Speed
        {
            get { return speed; }
        }

        public Bullet(double angle, double digressionAngle, float x, float y,ContentManager content)
        {
            if (content != null)
                this.image = content.Load<Texture2D>("Bullet");
            this.height = 4;
            this.width = 16;
            this.origin = new Vector2(1, 1);
            randAngle = new Random().Next(((int)digressionAngle * (-1)), (int)digressionAngle);
            randAngle /= 1000;
            if (randAngle > 0)
                this.angle = angle + randAngle;
            else
                this.angle = angle - Math.Abs(randAngle);

            this.x = x + ((float)Math.Cos(angle)) * 38;
            this.y = y + ((float)Math.Sin(angle)) * 38;
        }


    }
}
