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
    /// Class that creates a singe particle object
    /// </summary>
    public class Particle
    {
        /// <summary>
        /// The texture that will be drawn to represent the particle
        /// </summary>
        public Texture2D Texture { get; set; }
        /// <summary>
        /// The current position of the particle
        /// </summary>
        public Vector2 Position { get; set; }
        /// <summary>
        /// The speed of the particle at the current instance
        /// </summary>
        public Vector2 Velocity { get; set; }
        /// <summary>
        /// The current angle of rotation of the particle
        /// </summary>
        public float Angle { get; set; }
        /// <summary>
        /// The speed that the angle is changing
        /// </summary>
        public float AngularVelocity { get; set; }
        /// <summary>
        /// The color of the particle
        /// </summary>
        public Color Color { get; set; }
        /// <summary>
        /// The size of the particle
        /// </summary>
        public float Size { get; set; }
        /// <summary>
        /// The 'time to live' of the particle
        /// </summary>
        public int TTL { get; set; }

        public Particle(Texture2D texture, Vector2 position, Vector2 velocity,
            float angle, float angularVelocity, Color color, float size, int ttl)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Angle = angle;
            AngularVelocity = angularVelocity;
            Color = color;
            Size = size;
            TTL = ttl;
        }

        public void Update()
        {
            TTL--;
            Position += Velocity;
            Angle += AngularVelocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            
            spriteBatch.Draw(Texture, Position, sourceRectangle, Color,
                Angle, origin, Size, SpriteEffects.None, 0f);
        }




    }
}
