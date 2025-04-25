using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Greeeeenhaus
{
    public class SeaAnimation
    {
        public List<Texture2D> SeaParts;
        public List<Vector2> PartsPositions;
        public int ObjectsToClearWater;
        public float OverlayProgress;
        //0. water, 1. sand, 2.waves1, 3.waves2, 4.foam, 5.sparkles1, 6.sparkles2, 7.overlay1, 8.overlay2, 9.beachsign, 10.inventorybox
        public float MaxMovementPerUpdate;
        public float MinMovementPerUpdate;
        public float TotalMovement;
        List<bool> GoingLefts;
        List<float> CurrentMovements;
        List<float> Speeds;

        Random random = new Random();
        public void Load()
        {
            SeaParts = new List<Texture2D>();
            PartsPositions = new List<Vector2>();
            ObjectsToClearWater = 18;
            OverlayProgress = 0;

            MaxMovementPerUpdate = 0.155f;
            MinMovementPerUpdate = 0.09f;
            TotalMovement = 30;
            GoingLefts = new List<bool>();
            CurrentMovements = new List<float>();
            Speeds = new List<float>();

        }

        public void AddPart(Texture2D tex, Vector2 pos)
        {
            float rand;
            SeaParts.Add(tex);
            PartsPositions.Add(pos);
            GoingLefts.Add(true);  
            CurrentMovements.Add(0f); 

            rand = (float)(random.NextDouble() * (MaxMovementPerUpdate - MinMovementPerUpdate) + MinMovementPerUpdate);
            Speeds.Add(rand);
        }
        public void Update(GameTime gameTime, int totalPickedUp)
        {
            
            //Change overlay power based on totalobejcts
            if (totalPickedUp >= ObjectsToClearWater) OverlayProgress = 100;
            else
            {
                OverlayProgress = totalPickedUp * 100 / ObjectsToClearWater;
            }
            //change movement (of movable parts only, 2-6)
            for (int i = 0; i < PartsPositions.Count; i++)
            {
                if (i > 1 && i < 7)
                {
                    Vector2 modifyingPosition = PartsPositions[i];
                    if (GoingLefts[i])
                    {
                        modifyingPosition.X -= Speeds[i];
                        CurrentMovements[i] += Speeds[i];
                    }
                    else
                    {
                        modifyingPosition.X += Speeds[i];
                        CurrentMovements[i] += Speeds[i];
                    }
                    PartsPositions[i] = modifyingPosition;
                    if (CurrentMovements[i] >= TotalMovement)
                    {
                        CurrentMovements[i] = 0;
                        GoingLefts[i] = !GoingLefts[i];
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(SeaParts[0], PartsPositions[0], Color.White);
            spriteBatch.Draw(SeaParts[1], PartsPositions[1], Color.White);

            spriteBatch.Draw(SeaParts[2], PartsPositions[2], Color.White);
            spriteBatch.Draw(SeaParts[3], PartsPositions[3], Color.White);
            spriteBatch.Draw(SeaParts[4], PartsPositions[4], Color.White);
            spriteBatch.Draw(SeaParts[5], PartsPositions[5], Color.White);
            spriteBatch.Draw(SeaParts[6], PartsPositions[6], Color.White);

            spriteBatch.Draw(SeaParts[7], PartsPositions[7], Color.White * .23f * (OverlayProgress / 100));
            spriteBatch.Draw(SeaParts[8], PartsPositions[8], Color.White * .32f * (OverlayProgress / 100));
            spriteBatch.Draw(SeaParts[9], PartsPositions[9], Color.White);
            spriteBatch.Draw(SeaParts[10], PartsPositions[10], Color.White);

        }
    }
}
