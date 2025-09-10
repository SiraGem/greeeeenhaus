using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Greeeeenhaus
{
    public class Player
    {
        public Vector2 Position;
        public Texture2D Left;
        public Texture2D Right;
        public float Speed = 350f;
        public bool HasItem;
        public FloatingObject CurrentObject;
        public State CurrentState;
        public State LookingAt;
        public Texture2D CurrentTexture;

        public enum State
        {
            IdleLeft,
            IdleRight,
            IdleDown,
            IdleUp,
            MovingLeft,
            MovingRight,
            MovingDown,
            MovingUp
        }

        public void Load(Texture2D left, Texture2D right)
        {
            this.Left = left;
            this.Right = right;
            CurrentTexture = Right;
            this.Position = new Vector2(200, 200); //starting position
            CurrentState = State.IdleDown;
        }

        public void Update(GameTime gameTime, Rectangle playableSeaArea)
        {
            Vector2 movement = Vector2.Zero;
            KeyboardState kb = Keyboard.GetState();
            Vector2 newPosition;

            if (kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.Up)) {
                movement.Y -= 1; 
                CurrentState = State.MovingUp;
                LookingAt = State.IdleUp;
            }

            if (kb.IsKeyDown(Keys.S) || kb.IsKeyDown(Keys.Down))
            {
                movement.Y += 1;
                CurrentState = State.MovingDown;
                LookingAt = State.IdleDown;
            }
            if (kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left))
            {
                movement.X -= 1;
                CurrentState = State.MovingLeft;
                LookingAt = State.IdleLeft;
                CurrentTexture = Left;
            }
            if (kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right))
            {
                movement.X += 1;
                CurrentState = State.MovingRight;
                LookingAt = State.IdleRight;
                CurrentTexture = Right;
            }

            if (movement != Vector2.Zero) movement.Normalize();

            newPosition = this.Position + (movement * this.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds);


            if (InsideWindowBounds(newPosition, playableSeaArea)) this.Position = newPosition;


        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(CurrentTexture, this.Position, Color.White);
        }

        public bool InsideWindowBounds(Vector2 newPosition, Rectangle gameWindowRectangle)
        {
            Rectangle playerRectangle = new Rectangle((int)newPosition.X, (int)newPosition.Y, this.Left.Width, this.Left.Height);

            if (gameWindowRectangle.Contains(playerRectangle)) return true;
            else return false;

        }
        public Rectangle GetArea() //position within game area is checked back in Game1 helped by calling this method
        {
            return new Rectangle((int)Position.X, (int)Position.Y, Left.Width, Left.Height);
        }
    }
}
