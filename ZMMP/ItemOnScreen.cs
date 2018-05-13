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
    /// This is a basic class for an active item that appear on screen
    /// </summary>
    public abstract class ItemOnScreen
    {
        /// <summary>
        /// Item's X
        /// </summary>
        protected float x;
        /// <summary>
        /// Item's Y
        /// </summary>
        protected float y;
        /// <summary>
        /// Item's angle
        /// </summary>
        protected double angle;
        /// <summary>
        /// Item's width
        /// </summary>
        protected int width;
        /// <summary>
        /// Item's height
        /// </summary>
        protected int height;
        /// <summary>
        /// Item's image
        /// </summary>
        protected Texture2D image;
        /// <summary>
        /// Item's rotation origin point
        /// </summary>
        protected Vector2 origin;

        public Vector2 RectUpperLeftCorner
        {
            get { return new Vector2(x - width / 2, y - height / 2); }
        }

        public List<Vector2> RectanglePoints
        {
            get
            {
                return new List<Vector2>()
                {
                     RotatePoint(new Vector2(x,y)-origin,new Vector2(x,y), (float)angle),
                     RotatePoint(new Vector2() { X = (new Vector2(x,y)-origin).X + width, Y = (new Vector2(x,y)-origin).Y }, new Vector2(x,y), (float)angle),
                     RotatePoint(new Vector2() { X = (new Vector2(x,y)-origin).X + width, Y = (new Vector2(x,y)-origin).Y + height }, new Vector2(x,y), (float)angle),
                     RotatePoint(new Vector2() { X = (new Vector2(x,y)-origin).X, Y = (new Vector2(x,y)-origin).Y + height }, new Vector2(x,y), (float)angle)
                };
            }

        }

        public static Vector2 RotatePoint(Vector2 PointToRotate, Vector2 OriginOfRotation, float ThetaInRads)
        {
            Vector2 RotationVector = PointToRotate - OriginOfRotation;
            Vector2 RotatedVector = new Vector2()
            {
                X = (float)(RotationVector.X * Math.Cos(ThetaInRads) - RotationVector.Y * Math.Sin(ThetaInRads)),
                Y = (float)(RotationVector.X * Math.Sin(ThetaInRads) + RotationVector.Y * Math.Cos(ThetaInRads))
            };

            return OriginOfRotation + RotatedVector;
        }

        public void Draw(SpriteBatch Sbatch)
        {
            Sbatch.Draw(this.image, new Vector2(this.x, this.y), null, Color.White, (float)this.angle, this.origin, 1, SpriteEffects.None, 1);
        }

        public Vector2 Origin 
        {
            get { return origin; }
        }

        public Texture2D Image
        {
            get { return image; }
            set { image = value; }
        }

        

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public double Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }


        public int Height
        {
            get { return height; }
            set { height = value; }
        }
    }
}
