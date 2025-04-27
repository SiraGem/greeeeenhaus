using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Greeeeenhaus
{
    public class BuildingPart
    {
        public string Name;
        public Rectangle TargetArea;
        public Vector2 PlacementPos;
        public bool IsPlaced;

        public List<string> AcceptedObjects { get; private set; } = new List<string>();
        public Dictionary<string, Texture2D> PossibleTextures;
        public Dictionary<string, Vector2> Offsets;
        public Texture2D ChosenTexture = null;
        public string ChosenMaterial;
        public int DrawOrder;

        public BuildingPart(string name, Rectangle targetArea, Vector2 placementPos, int drawOrder)
        {
            Name = name;
            TargetArea = targetArea;
            PlacementPos = placementPos;
            IsPlaced = false;
            PossibleTextures = new Dictionary<string, Texture2D>();
            Offsets = new Dictionary<string, Vector2>();
            DrawOrder = drawOrder;
            ChosenMaterial = "";
        }

        public void SetAcceptedObjects(string[] acceptedObjects)
        {
            AcceptedObjects = acceptedObjects.ToList();
        }

        public void AddMaterialDictionaryEntry(string materialName, Texture2D materialTexture)
        {
            PossibleTextures.Add(materialName, materialTexture);
        }

        public void AddMaterialOffsetDictionary(string materialName, Vector2 offset)
        {
            Offsets.Add(materialName, offset);
        }

        public bool TryBuild(FloatingObject obj) //check if dropped obj is valid and set texture
        {
            if (IsPlaced) return false;
            if (AcceptedObjects.Contains(obj.Material))
            {
                ChosenTexture = PossibleTextures[obj.Material];
                ChosenMaterial = obj.Material;
                IsPlaced = true;
                return true;
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = PlacementPos;
            if (IsPlaced && ChosenTexture != null)
            {
                if (Offsets.ContainsKey(ChosenMaterial)){
                    pos += Offsets[ChosenMaterial];
                }
                spriteBatch.Draw(ChosenTexture, pos, Color.White);
            }
        }

    }

}
