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

namespace ZombieMadnessMP
{
    /// <summary>
    /// This class manages the particles in the game
    /// </summary>
    public class ParticleEngine
    {
        /// <summary>
        /// Random number generator
        /// </summary>
        private Random random;
        /// <summary>
        /// Starting position of the particle emitter
        /// </summary>
        public Vector2 EmitterLocation { get; set; }
        /// <summary>
        /// list of particles
        /// </summary>
        private List<Particle> particles;
        /// <summary>
        /// Particle texture
        /// </summary>
        private Texture2D textures;
        /// <summary>
        /// Bullet object for the emitter location position
        /// </summary>
        private Bullet bullet;
        /// <summary>
        /// Game screen size
        /// </summary>
        private Vector2 screenSize;
        /// <summary>
        /// Amount of particles
        /// </summary>
        public int TotalParticles { get; set; }

        public ParticleEngine(Texture2D textures, Vector2 location,Vector2 ScreenSize)
        {
            this.bullet = new Bullet(0, 0, 0, 0,null);
            EmitterLocation = location;
            this.textures = textures;
            this.particles = new List<Particle>();
            this.TotalParticles = 0;
            screenSize = ScreenSize;
            random = new Random();
        }


        private Particle GenerateNewParticle( )
        {
            int x1, x2, y1, y2;
            Texture2D texture = this.textures;
            Vector2 position = EmitterLocation;
            float angle = (float)bullet.Angle;
            int range = 30; //Particle Spread Range

            Vector2 velocity = new Vector2();

            x1 = (int)((((double)angle * Math.Cos(angle)) * 100) + range);
            y1 = (int)((((double)angle * Math.Sin(angle)) * 100) + range);

            x2 = (int)((((double)angle * Math.Cos(angle)) * 100) - range);
            y2 = (int)((((double)angle * Math.Sin(angle)) * 100) - range);

            if (x1 < x2)
                velocity.X = random.Next(x1, x2) / 100f;
            else
                velocity.X = random.Next(x2, x1) / 100f;

            if (y1 < y2)
                velocity.Y = random.Next(y1, y2) / 100f;
            else
                velocity.Y = random.Next(y2, y1) / 100f;

            if (bullet.Y > screenSize.Y / 2)
                velocity = -velocity;


            float angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);

            Color color = new Color(
                    (float)(random.Next(40, 80) / 100.0),
                    0,
                    0);
            float size = (float)(random.Next(4,30)/100.0);
            int ttl = random.Next(40);

            return new Particle(texture, position, velocity, angle, angularVelocity, color, size, ttl);
        }


        public void UpdateBullet(Bullet bullet)
        {
            this.bullet = bullet;
        }

        public void UpdateParticeMovement()
        {

            for (int i = 0; i < TotalParticles; i++)
            {
                particles.Add(GenerateNewParticle());
            }

            TotalParticles = 0;

            for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if (particles[particle].TTL <= 0)
                {
                    particles.RemoveAt(particle);
                    particle--;
                }
            }
        }

        public int GetParticlesAmount()
        {
            return particles.Count;
        }

        public void Draw(SpriteBatch spriteBatch)
        {      
            for (int index = 0; index < particles.Count; index++)
            {
                particles[index].Draw(spriteBatch);
            }
        }
    }

 

}
