

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Greeeeenhaus
{
    public class FloatingObject
    {
        public Vector2 Position;
        public Vector2 FoamPosition;
        public Texture2D Texture;
        public string Material;
        public bool IsCollected = false;

        public float MovementPerUpdate;
        public float TotalMovement;
        public float CurrentMovement;
        public bool GoingUp;

        public void Load(Texture2D texture, Vector2 position, string material)
        {
            Texture = texture;
            Position = position;
            FoamPosition = position;
            FoamPosition.Y += 14;
            FoamPosition.X -= 6;
            Material = material;

            MovementPerUpdate = 0.1f;
            TotalMovement = 7;
            CurrentMovement = 0;
            GoingUp = true;
        }

        public void Update (GameTime gameTime)
        {
            if (GoingUp)
            {
                Position.Y -= MovementPerUpdate;
                CurrentMovement += MovementPerUpdate;
            }
            else if (!GoingUp)
            {
                Position.Y += MovementPerUpdate;
                CurrentMovement += MovementPerUpdate;
            }
            if (CurrentMovement >= TotalMovement)
            {
                CurrentMovement = 0;
                GoingUp = !GoingUp;
            }
        }
        public void Draw(SpriteBatch spriteBatch, Texture2D foamTex)
        {
       
           if (!IsCollected)
            {
                spriteBatch.Draw(Texture, Position, Color.White);
                spriteBatch.Draw(foamTex, FoamPosition, Color.White);

            }
        }

        public Rectangle GetArea() //position within game area is checked back in Game1 helped by calling this method
        {
            return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
        }

    }
}
