using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Greeeeenhaus
{
    public class DialogueManager
    {
        private string _currentText;
        private SpriteFont _font;
        private Vector2 _position;
        private Color _color;
        public Dictionary<string, string> _dialoguesDictionary;
        public bool IsActive => !string.IsNullOrEmpty(_currentText);

        public void Load(SpriteFont font, Color color)
        {
            _font = font;
            _color = color;
            _position = new Vector2(145, 500);
            _dialoguesDictionary = new Dictionary<string, string>();
            InitializeDialogueDictionary();
        }

        public void ShowDialogue(string text)
        {
            _currentText = _dialoguesDictionary[text];
        }

        public void ClearDialogue()
        {
            _currentText = string.Empty;
        }
        public void InitializeDialogueDictionary()
        {
            _dialoguesDictionary["notValid"] = "I don't think I can use that here...";
            _dialoguesDictionary["seaInstructions"] = "Let's pick up all this stuff and put it \nin that box!";
            _dialoguesDictionary["buildingInstructions"] = "We have some materials now, so drag \nand drop anything on this empty area.";
            _dialoguesDictionary["successBuild1"] = "Nice, that worked!";
            _dialoguesDictionary["successBuild2"] = "This is looking great.";
            _dialoguesDictionary["successBuild3"] = "Yeah, that can go there.";
            _dialoguesDictionary["successBuild4"] = "Let's keep going, it's almost done.";
            _dialoguesDictionary["boxFull"] = "Oh, the box is full. \nLet's use these materials.";

            _dialoguesDictionary["cabinFinished"] = "Wow, I did it...?? Now I truly am ready\nto start this new chapter in my life...";
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!string.IsNullOrEmpty(_currentText))
            {
                spriteBatch.DrawString(_font, _currentText, _position, _color);
            }
        }

    }
}
