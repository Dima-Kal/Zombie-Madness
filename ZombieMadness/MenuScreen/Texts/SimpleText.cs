using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MenuScreen.MenuScreen.Texts
{
    public class SimpleText : DrawableGameComponent
    {
        private Vector2 position;
        private Color color;
        private string text;
        private string name;
        private bool isEntering;
        private bool isLeaving;

        private const int POSITION_DECREMENT = 16;
        private const int LEAVING_DECREMENT = 30;
        protected const int COLOR_ENTARENCE_INCREMENT = 6;
        private const int MINIMUM_X_WHEN_LEAVING = -100;

        protected SpriteBatch spriteBatch;
        protected SpriteFont spriteFont;

        public SimpleText(Game game, string text, bool entering) : base(game)
        {
            this.text = text;
            if (entering)
            {
                color = new Color(Color.Red, 0);
                position = Vector2.Zero;
            }
            else
            {
                color = Color.Red;
            }
            name = text;
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            spriteFont = Game.Content.Load<SpriteFont>("SpriteFont1");
        }

        public SimpleText(Game game, string text, bool entering, string name) : this(game, text, entering)
        {
            this.name = name;
        }

        public SpriteFont SpriteFont
        {
            set
            {
                spriteFont = value;
            }
        }

        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
            }
        }



        public bool IsEntering
        {
            get
            {
                return isEntering;
            }
            set
            {
                isEntering = value;
            }
        }

        public bool IsLeaving
        {
            get
            {
                return isLeaving;
            }
            set
            {
                isLeaving = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
    
        public void EnterScreen()
        {
            isEntering = true;
        }

        public void LeaveScreen()
        {
            isLeaving = true;
        }

        public override void Update(GameTime gameTime)
        {
            UpdatePosition();
            base.Update(gameTime);
        }

        public virtual void UpdatePosition()
        {
            if (isEntering)
            {
                position.X -= POSITION_DECREMENT;
                int temp = color.A;
                color.A += COLOR_ENTARENCE_INCREMENT;
                if (temp > color.A)
                {
                    isEntering = false;
                    color.A = byte.MaxValue;
                }
            }
            if (isLeaving)
            {
                position.X -= POSITION_DECREMENT;
                int temp = color.A;
                color.A -= LEAVING_DECREMENT;
                if (color.A > temp)
                {
                    color.A = byte.MinValue;
                }
                if (position.X < MINIMUM_X_WHEN_LEAVING)
                {
                    isLeaving = false;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, text, position, color);
            spriteBatch.End();
            base.Draw(gameTime);
        }

    }
}