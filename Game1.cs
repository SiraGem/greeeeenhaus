using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Greeeeenhaus
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        //STATES
        private GameState _currentState;
        //BASICS
        private Texture2D _buildingBG;
        private Texture2D _playerTexture;
        private SeaAnimation _seaAnimation;
        private Texture2D _mainMenuBG;
        //PLAYER
        private Player _player;
        //UI
        private Texture2D _whiteBorder;
        private Texture2D _watercolorOverlay;
        private Texture2D _playerFace;
        private Texture2D _dialogueBox;
        private Texture2D _inventoryMenu;
        private Texture2D _returnButton;
        private Rectangle _returnButtonArea;
        private Texture2D _playButton;
        private Rectangle _playButtonArea;
        private Texture2D _settingsButton;
        private Rectangle _settingsButtonArea;
        private Texture2D _miniSettingsButton;
        private Rectangle _miniSettingsButtonArea;
        private Texture2D _exitGameButton;
        private Rectangle _exitGameButtonArea;
        private Texture2D _creditsButton;
        private Rectangle _creditsButtonArea;
        private Texture2D _discardButton;
        private Rectangle _discardButtonArea;
        private List<Vector2> _inventorySlots;
        private const int INVENTORY_SLOT_SIZE = 71;
        //FLOATINGOBJECTS
        private const int MAX_ITEMS_SEA = 3;
        List<FloatingObject> _currentFloatingObjects = new List<FloatingObject>();
        Dictionary<string, Texture2D> _floatingObjectsTexMat = new Dictionary<string, Texture2D>();
        Texture2D _foamTexture;
        private int _totalPickedUpObjects;
        //AREAS
        private Rectangle _gameArea;
        private Rectangle _playableSeaArea;
        private Rectangle _storageArea;
        private Rectangle _goToBuildingArea;
        private Rectangle _cabinArea;
        //STORED OBJECTS
        private const int MAX_ITEMS_STORAGE = 5;
        List<FloatingObject> _storedItems = new List<FloatingObject>();
        private int _storedItemCount;
        private FloatingObject _selectedDragObject;
        //BUILDING PARTS
        private List<BuildingPart> _buildingParts;
        private int _currentBuildIndex;

        Random random = new Random();

        public Game1() //untouched
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.ApplyChanges(); //applied resolution changes


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _currentState = GameState.MainMenu;
            //Base things load
            _mainMenuBG = Content.Load<Texture2D>("UI/MainMenu/MainMenuBackground");
            _buildingBG = Content.Load<Texture2D>("Environment/Cabin/BuildingBG");
            _playerTexture = Content.Load<Texture2D>("Player/placeholderPlayer");
            _gameArea = new Rectangle(0, 0, GraphicsDevice.PresentationParameters.Bounds.Width, GraphicsDevice.PresentationParameters.Bounds.Height);
            _playableSeaArea = new Rectangle(90, 3, GraphicsDevice.PresentationParameters.Bounds.Width - 20, GraphicsDevice.PresentationParameters.Bounds.Height - 20);
            _seaAnimation = new SeaAnimation();
            _seaAnimation.Load();
            GenerateSeaParts();

            _player = new Player();
            _player.Load(_playerTexture);
            //UI Load
            _whiteBorder = Content.Load<Texture2D>("UI/WhiteEdge_overlay");
            _watercolorOverlay = Content.Load<Texture2D>("UI/Watercolor_overlay");
            _playerFace = Content.Load<Texture2D>("Player/placeholderHappy");
            _dialogueBox = Content.Load<Texture2D>("UI/DialogueBox");
            _inventoryMenu = Content.Load<Texture2D>("UI/InventoryMenu");
            _returnButton = Content.Load<Texture2D>("UI/placeholderReturn");
            _returnButtonArea = new Rectangle(590, 45, _returnButton.Width, _returnButton.Height);
            _discardButton = Content.Load<Texture2D>("UI/placeholderDiscard");
            _discardButtonArea = new Rectangle(690, 45, _discardButton.Width, _discardButton.Height);
            _playButton = Content.Load<Texture2D>("UI/MainMenu/PlayButton");

            _settingsButton = Content.Load<Texture2D>("UI/MainMenu/SettingsButton");

            _creditsButton = Content.Load<Texture2D>("UI/MainMenu/CreditsButton");

            //define each inventory square
            _inventorySlots = new List<Vector2>();
            GenerateInventorySlots();
            //Define floatingObject textures
            _floatingObjectsTexMat["wood"] = Content.Load<Texture2D>("Environment/Objects/Wood");
            _floatingObjectsTexMat["fabric"] = Content.Load<Texture2D>("Environment/Objects/Shirt");
            _floatingObjectsTexMat["branch"] = Content.Load<Texture2D>("Environment/Objects/Branch");
            _floatingObjectsTexMat["metal"] = Content.Load<Texture2D>("Environment/Objects/Metal");
            _floatingObjectsTexMat["plastic"] = Content.Load<Texture2D>("Environment/Objects/Plastic");
            _floatingObjectsTexMat["shells"] = Content.Load<Texture2D>("Environment/Objects/Shells");
            _foamTexture = Content.Load<Texture2D>("Environment/Objects/ObjectFoam");

            GenerateFloatingObjects();

            _selectedDragObject = null;

            _storedItemCount = 0;
            _totalPickedUpObjects = 0;
            //main menu areas
            _playButtonArea = new Rectangle(489, 327, _playButton.Width, _playButton.Height);
            _settingsButtonArea = new Rectangle(516, 402, _settingsButton.Width, _settingsButton.Height);
            _creditsButtonArea = new Rectangle(490, 481, _creditsButton.Width, _creditsButton.Height);
            //Sea screen areas
            _storageArea = new Rectangle(40, 320, 80, 80);
            _goToBuildingArea = new Rectangle(40, 140, 65, 65);
            //Building screen areas
            _cabinArea = new Rectangle(130, 180, 260, 240);
            _buildingParts = new List<BuildingPart>();
            InitializeBuildingParts();
            _currentBuildIndex = 0;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            if (_currentState == GameState.MainMenu)
            {
                MouseState mouse = Mouse.GetState();

                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    if (_playButtonArea.Contains(mouse.Position))
                    {
                        _currentState = GameState.Sea;
                    }
                    if (_settingsButtonArea.Contains(mouse.Position))
                    {
                        _currentState = GameState.Settings;
                    }
                    if (_creditsButtonArea.Contains(mouse.Position))
                    {
                        _currentState = GameState.Credits;
                    }
                }
            }
            if (_currentState == GameState.Sea)
            {
                _player.Update(gameTime, _playableSeaArea);
                _seaAnimation.Update(gameTime, _totalPickedUpObjects);
                foreach (var obj in _currentFloatingObjects)
                {
                    obj.Update(gameTime);
                }
                if (!_player.HasItem)
                {
                    foreach (var obj in _currentFloatingObjects)
                    {
                        if (!obj.IsCollected && _player.GetArea().Intersects(obj.GetArea()))
                        {
                            obj.IsCollected = true;
                            _player.HasItem = true;
                            _player.CurrentObject = obj;
                            break;
                        }
                    }
                }
                if (_currentFloatingObjects.All(o => o.IsCollected) && _player.HasItem == false) //if all objects are collected (although invisible)
                {
                    GenerateFloatingObjects();
                }
                if (_player.HasItem && _storageArea.Intersects(_player.GetArea()))
                {
                    _storedItems.Add(_player.CurrentObject);
                    _storedItemCount++;
                    _totalPickedUpObjects++;
                    _player.HasItem = false;
                    _player.CurrentObject = null;

                    // BONUS: sonido
                    // soundEffect.Play();

                    if (_storedItemCount >= MAX_ITEMS_STORAGE)
                    {
                        _currentState = GameState.Building;
                    }
                }
                if (!_player.HasItem && _goToBuildingArea.Intersects(_player.GetArea()))
                {
                    _currentState = GameState.Building;
                }
                _storedItemCount = _storedItems.Count;
            }
            if (_currentState == GameState.Building)
            {
                MouseState mouse = Mouse.GetState();
                //before anything, check if cabin is finished
                if (_currentBuildIndex > _buildingParts.size) _currentState = GameState.EndingScene;
                else
                {
                    //Mouse is clicking and check if its one of the slots
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        for (int i = 0; i < _storedItems.Count && _selectedDragObject == null; i++)
                        {
                            Rectangle slot = new Rectangle((int)_inventorySlots[i].X,
                                                           (int)_inventorySlots[i].Y,
                                                           INVENTORY_SLOT_SIZE,
                                                           INVENTORY_SLOT_SIZE);
                            if (slot.Contains(mouse.Position))
                            {
                                _selectedDragObject = _storedItems[i];
                            }
                        }
                    }
                    //is dragging an objet and releases, what to do whether on cabin area or outside..
                    if (_selectedDragObject != null && mouse.LeftButton == ButtonState.Released)
                    {
                        if (!_cabinArea.Contains(mouse.Position) && !_discardButtonArea.Contains(mouse.Position))
                        {
                            _selectedDragObject = null;
                        }
                        if (_discardButtonArea.Contains(mouse.Position)) //dragged to discard area
                        {
                            _storedItems.Remove(_selectedDragObject);
                            _selectedDragObject = null;
                        }
                        if (_cabinArea.Contains(mouse.Position) && _currentBuildIndex < _buildingParts.Count)//mouse released, with object and ON cabin area...
                        {
                            BuildingPart currentPart = _buildingParts[_currentBuildIndex];
                            if (!currentPart.IsPlaced)
                            {
                                if (currentPart.TryBuild(_selectedDragObject))
                                {
                                    //build success
                                    _storedItems.Remove(_selectedDragObject);
                                    _currentBuildIndex++;
                                }
                            }
                            _selectedDragObject = null;
                        }
                    }
                    //CHECK IF CLICK ON RETURN BUTTON (and is not dragging object :S  )
                    if (mouse.LeftButton == ButtonState.Pressed && _returnButtonArea.Contains(mouse.Position) && _selectedDragObject == null)
                    {
                        _player.Position.X += 40;
                        _currentState = GameState.Sea;
                    }
                }

            }
            if (_currentState == GameState.EndingScene){

            }
            
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();
            if (_currentState == GameState.MainMenu){
                _spriteBatch.Draw(_mainMenuBG, Vector2.Zero, Color.White);
                _spriteBatch.Draw(_playButton, _playButtonArea, Color.White);
                _spriteBatch.Draw(_settingsButton, _settingsButtonArea, Color.White);
                _spriteBatch.Draw(_creditsButton, _creditsButtonArea, Color.White);
            }
            if (_currentState == GameState.Sea)
            {
                _seaAnimation.Draw(_spriteBatch);
                _player.Draw(_spriteBatch);
                foreach (var obj in _currentFloatingObjects)
                {

                    obj.Draw(_spriteBatch, _foamTexture);

                }
                if (_player.HasItem && _player.CurrentObject != null)
                {
                    Vector2 offset = new Vector2(20, -20); // to the left of the siren
                    _spriteBatch.Draw(_player.CurrentObject.Texture, _player.Position + offset, Color.White);
                }
                /*//TESTING ZONAS
                // Crear textura de 1x1
                Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.White });

                // Dibujo de zonas
                _spriteBatch.Draw(pixel, _storageArea, Color.FromNonPremultiplied(255, 100, 100, 90)); // caja de entrega
                _spriteBatch.Draw(pixel, _goToBuildingArea, Color.FromNonPremultiplied(200, 255, 100, 90)); // acceso playa
               */
            }
            if (_currentState == GameState.Building)
            {
                _spriteBatch.Draw(_buildingBG, Vector2.Zero, Color.White);
                _spriteBatch.Draw(_inventoryMenu, new Vector2(550, 178), Color.White);
                _spriteBatch.Draw(_returnButton, new Vector2(590, 45), Color.White);
                _spriteBatch.Draw(_discardButton, new Vector2(690, 45), Color.White);
                //TESTING AREAS
                /*Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.White });
                // Dibujo de zona cabaña
                _spriteBatch.Draw(pixel, _cabinArea, Color.FromNonPremultiplied(255, 100, 100, 90));

               
                //dibujar inventory slots
                // Dibujar los slots del inventario como cuadros grises semitransparentes
                for (int i = 0; i < _inventorySlots.Count; i++)
                {
                    Rectangle slotBox = new Rectangle(
                        (int)_inventorySlots[i].X,
                        (int)_inventorySlots[i].Y,
                        INVENTORY_SLOT_SIZE,
                        INVENTORY_SLOT_SIZE
                    );

                    _spriteBatch.Draw(pixel, slotBox, Color.FromNonPremultiplied(200, 200, 200, 100)); // gris claro semitransparente

                    // Dibujar contorno (marco) negro más opaco para ver el borde
                    _spriteBatch.Draw(pixel, new Rectangle(slotBox.X, slotBox.Y, slotBox.Width, 2), Color.Black); // arriba
                    _spriteBatch.Draw(pixel, new Rectangle(slotBox.X, slotBox.Bottom - 2, slotBox.Width, 2), Color.Black); // abajo
                    _spriteBatch.Draw(pixel, new Rectangle(slotBox.X, slotBox.Y, 2, slotBox.Height), Color.Black); // izquierda
                    _spriteBatch.Draw(pixel, new Rectangle(slotBox.Right - 2, slotBox.Y, 2, slotBox.Height), Color.Black); // derecha
                }*/
                DrawCabinParts();
                DrawInventoryItems();
                //draw dragging item if exists
                if (_selectedDragObject != null)
                {
                    int size = INVENTORY_SLOT_SIZE;
                    Vector2 dragPos = new Vector2(Mouse.GetState().X - size / 2, Mouse.GetState().Y - size / 2);
                    _spriteBatch.Draw(_selectedDragObject.Texture, new Rectangle((int)dragPos.X, (int)dragPos.Y, size, size), Color.White);
                }
            }
            if (_currentState == GameState.EndingScene){
                 _spriteBatch.Draw(_buildingBG, Vector2.Zero, Color.White);
                 DrawCabinParts();
            }
            
            _spriteBatch.Draw(_watercolorOverlay, Vector2.Zero, Color.White * 0.11f);
            _spriteBatch.Draw(_whiteBorder, Vector2.Zero, Color.White);
            if (_currentState == GameState.Sea || _currentState == GameState.Building)
            {
                _spriteBatch.Draw(_dialogueBox, new Vector2(0, _gameArea.Height - _dialogueBox.Height - 12), Color.White);
                _spriteBatch.Draw(_playerFace, new Vector2(28, _gameArea.Height - _dialogueBox.Height + 32), Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        //this method is called at Load game and everytime all objects have been removed from the sea
        public void GenerateFloatingObjects()
        {
            _currentFloatingObjects.Clear();
            var materialKeys = _floatingObjectsTexMat.Keys.ToList();

            for (int i = 0; i < MAX_ITEMS_SEA; i++)
            {
                string randomMaterial = materialKeys[random.Next(materialKeys.Count)];
                Texture2D tex = _floatingObjectsTexMat[randomMaterial];

                Vector2 pos = new Vector2( //geneerate random position taking into account window borders
                    random.Next(_playableSeaArea.X, _playableSeaArea.Width - tex.Width),
                    random.Next(_playableSeaArea.Y, _playableSeaArea.Height - tex.Height)
                );

                FloatingObject obj = new FloatingObject();
                obj.Load(tex, pos, randomMaterial);
                _currentFloatingObjects.Add(obj);

            }
        }

        public void GenerateInventorySlots()
        {
            _inventorySlots.Clear();

            Vector2 startPosition = new Vector2(573, 199);
            int spacingX = 80;
            int spacingY = 74;

            for (int i = 0; i < MAX_ITEMS_STORAGE; i++)
            {
                int column = i % 2;
                int row = i / 2;

                float x;
                float y = startPosition.Y + row * spacingY;

                if (i == MAX_ITEMS_STORAGE - 1)
                {
                    float totalRowWidth = spacingX;
                    x = startPosition.X + (totalRowWidth / 2);
                }
                else
                {
                    x = startPosition.X + column * spacingX;
                }

                _inventorySlots.Add(new Vector2(x, y));
            }
        }

        public void InitializeBuildingParts()
        {
            //1. floor
            BuildingPart floor = new BuildingPart("floor", _cabinArea, new Vector2(91, 299), 3);
            floor.SetAcceptedObjects(new string[] { "wood", "metal" });
            floor.AddMaterialDictionaryEntry("wood", Content.Load<Texture2D>("Environment/Cabin/1_floor/floor_wood"));
            floor.AddMaterialDictionaryEntry("metal", Content.Load<Texture2D>("Environment/Cabin/1_floor/floor_metal"));
            _buildingParts.Add(floor);
            //2. backWall
            BuildingPart backWall = new BuildingPart("backWall", _cabinArea, new Vector2(90, 150), 1);
            backWall.SetAcceptedObjects(new string[] { "wood", "metal", "branch" });
            backWall.AddMaterialDictionaryEntry("wood", Content.Load<Texture2D>("Environment/Cabin/2_backWall/backWall_wood"));
            backWall.AddMaterialDictionaryEntry("metal", Content.Load<Texture2D>("Environment/Cabin/2_backWall/backWall_metal"));
            backWall.AddMaterialDictionaryEntry("branch", Content.Load<Texture2D>("Environment/Cabin/2_backWall/backWall_branch"));
            backWall.AddMaterialOffsetDictionary("branch", new Vector2(-26, -40));
            _buildingParts.Add(backWall);
            //3.rightWall
            BuildingPart rightWall = new BuildingPart("rightWall", _cabinArea, new Vector2(285, 149), 2);
            rightWall.SetAcceptedObjects(new string[] { "wood", "metal", "branch" });
            rightWall.AddMaterialDictionaryEntry("wood", Content.Load<Texture2D>("Environment/Cabin/3_rightWall/rightWall_wood"));
            rightWall.AddMaterialDictionaryEntry("metal", Content.Load<Texture2D>("Environment/Cabin/3_rightWall/rightWall_metal"));
            rightWall.AddMaterialDictionaryEntry("branch", Content.Load<Texture2D>("Environment/Cabin/3_rightWall/rightWall_branch"));
            rightWall.AddMaterialOffsetDictionary("branch", new Vector2(0, -24));
            _buildingParts.Add(rightWall);
            //4. leftWall
            BuildingPart leftWall = new BuildingPart("leftWall", _cabinArea, new Vector2(89, 180), 4);
            leftWall.SetAcceptedObjects(new string[] { "wood", "metal", "branch" });
            leftWall.AddMaterialDictionaryEntry("wood", Content.Load<Texture2D>("Environment/Cabin/4_leftWall/leftWall_wood"));
            leftWall.AddMaterialDictionaryEntry("metal", Content.Load<Texture2D>("Environment/Cabin/4_leftWall/leftWall_metal"));
            leftWall.AddMaterialDictionaryEntry("branch", Content.Load<Texture2D>("Environment/Cabin/4_leftWall/leftWall_branch"));
            leftWall.AddMaterialOffsetDictionary("branch", new Vector2(0, -27));
            _buildingParts.Add(leftWall);
            //5. frontWall
            BuildingPart frontWall = new BuildingPart("frontWall", _cabinArea, new Vector2(218, 218), 7);
            frontWall.SetAcceptedObjects(new string[] { "wood", "metal", "branch" });
            frontWall.AddMaterialDictionaryEntry("wood", Content.Load<Texture2D>("Environment/Cabin/5_frontWall/frontWall_wood"));
            frontWall.AddMaterialDictionaryEntry("metal", Content.Load<Texture2D>("Environment/Cabin/5_frontWall/frontWall_metal"));
            frontWall.AddMaterialDictionaryEntry("branch", Content.Load<Texture2D>("Environment/Cabin/5_frontWall/frontWall_branch"));
            frontWall.AddMaterialOffsetDictionary("branch", new Vector2(-26, -47));
            _buildingParts.Add(frontWall);
            //6. rightRoof
            BuildingPart rightRoof = new BuildingPart("rightRoof", _cabinArea, new Vector2(192, 125), 5);
            rightRoof.SetAcceptedObjects(new string[] { "wood", "metal", "branch", "plastic" });
            rightRoof.AddMaterialDictionaryEntry("wood", Content.Load<Texture2D>("Environment/Cabin/6_rightRoof/rightRoof_wood"));
            rightRoof.AddMaterialDictionaryEntry("metal", Content.Load<Texture2D>("Environment/Cabin/6_rightRoof/rightRoof_metal"));
            rightRoof.AddMaterialDictionaryEntry("branch", Content.Load<Texture2D>("Environment/Cabin/6_rightRoof/rightRoof_branch"));
            rightRoof.AddMaterialDictionaryEntry("plastic", Content.Load<Texture2D>("Environment/Cabin/6_rightRoof/rightRoof_plastic"));
            _buildingParts.Add(rightRoof);
            //7. leftRoof
            BuildingPart leftRoof = new BuildingPart("leftRoof", _cabinArea, new Vector2(59, 125), 6);
            leftRoof.SetAcceptedObjects(new string[] { "wood", "metal", "branch", "plastic" });
            leftRoof.AddMaterialDictionaryEntry("wood", Content.Load<Texture2D>("Environment/Cabin/7_leftRoof/leftRoof_wood"));
            leftRoof.AddMaterialDictionaryEntry("metal", Content.Load<Texture2D>("Environment/Cabin/7_leftRoof/leftRoof_metal"));
            leftRoof.AddMaterialDictionaryEntry("branch", Content.Load<Texture2D>("Environment/Cabin/7_leftRoof/leftRoof_branch"));
            leftRoof.AddMaterialDictionaryEntry("plastic", Content.Load<Texture2D>("Environment/Cabin/7_leftRoof/leftRoof_plastic"));
            leftRoof.AddMaterialOffsetDictionary("plastic", new Vector2(10, 0));
            leftRoof.AddMaterialOffsetDictionary("branch", new Vector2(-22, 0));
            _buildingParts.Add(leftRoof);
            //8. leftWindow
            BuildingPart leftWindow = new BuildingPart("leftWindow", _cabinArea, new Vector2(116, 247), 8);
            leftWindow.SetAcceptedObjects(new string[] { "plastic", "shirt" });
            leftWindow.AddMaterialDictionaryEntry("plastic", Content.Load<Texture2D>("Environment/Cabin/8_door_windows/leftwindow_plastic"));
            leftWindow.AddMaterialDictionaryEntry("shirt", Content.Load<Texture2D>("Environment/Cabin/8_door_windows/leftwindow_shirt"));
            //leftWindow.AddMaterialOffsetDictionary("plastic", new Vector2(10, 0));
            _buildingParts.Add(leftWindow);
            //9. leftWindow
            BuildingPart rightWindow = new BuildingPart("rightWindow", _cabinArea, new Vector2(345, 257), 9);
            rightWindow.SetAcceptedObjects(new string[] { "plastic", "shirt" });
            rightWindow.AddMaterialDictionaryEntry("plastic", Content.Load<Texture2D>("Environment/Cabin/8_door_windows/rightwindow_plastic"));
            rightWindow.AddMaterialDictionaryEntry("shirt", Content.Load<Texture2D>("Environment/Cabin/8_door_windows/leftwindow_shirt"));
            //rightWindow.AddMaterialOffsetDictionary("plastic", new Vector2(10, 0));
            _buildingParts.Add(rightWindow);
            //10. door
            BuildingPart door = new BuildingPart("door", _cabinArea, new Vector2(250, 272), 10);
            door.SetAcceptedObjects(new string[] { "plastic", "shirt" });
            door.AddMaterialDictionaryEntry("plastic", Content.Load<Texture2D>("Environment/Cabin/8_door_windows/door_plastic"));
            door.AddMaterialDictionaryEntry("shirt", Content.Load<Texture2D>("Environment/Cabin/8_door_windows/leftwindow_shirt"));
            //door.AddMaterialOffsetDictionary("plastic", new Vector2(10, 0));
            _buildingParts.Add(door);

        }

        public void DrawInventoryItems()
        {
            for (int i = 0; i < _storedItems.Count && i < _inventorySlots.Count; i++)
            {
                FloatingObject obj = _storedItems[i];
                Texture2D objTex = obj.Texture;
                if (_selectedDragObject == obj) objTex = null;
                Vector2 slotPos = _inventorySlots[i];
                int resize = 60;
                Vector2 slotCenter = new Vector2(slotPos.X + (INVENTORY_SLOT_SIZE - resize) / 2f,
                                                 slotPos.Y + (INVENTORY_SLOT_SIZE - resize) / 2f);
                Rectangle destinationRect = new Rectangle((int)slotCenter.X, (int)slotCenter.Y, resize, resize);
                if (objTex != null) _spriteBatch.Draw(objTex, destinationRect, Color.White);
            }
        }

        public void DrawCabinParts()
        {
            foreach (BuildingPart part in _buildingParts.OrderBy(p => p.DrawOrder)) //draw from the list but ordered, by the attribute
            {
                part.Draw(_spriteBatch);
            }
        }

        public void GenerateSeaParts()
        {
            _seaAnimation.AddPart(Content.Load<Texture2D>("Environment/Sea/Water"), Vector2.Zero);
            _seaAnimation.AddPart(Content.Load<Texture2D>("Environment/Sea/Sand"), Vector2.Zero);
            _seaAnimation.AddPart(Content.Load<Texture2D>("Environment/Sea/Waves_1"), new Vector2(5, 0));
            _seaAnimation.AddPart(Content.Load<Texture2D>("Environment/Sea/Waves_2"), new Vector2(15, 0));
            _seaAnimation.AddPart(Content.Load<Texture2D>("Environment/Sea/Foam"), new Vector2(15, 0));
            _seaAnimation.AddPart(Content.Load<Texture2D>("Environment/Sea/Sparkles_1"), new Vector2(25, 0));
            _seaAnimation.AddPart(Content.Load<Texture2D>("Environment/Sea/Sparkles_2"), new Vector2(0, 0));
            _seaAnimation.AddPart(Content.Load<Texture2D>("Environment/Sea/ClearOverlay_1"), Vector2.Zero);
            _seaAnimation.AddPart(Content.Load<Texture2D>("Environment/Sea/ClearOverlay_2"), Vector2.Zero);
            _seaAnimation.AddPart(Content.Load<Texture2D>("Environment/Sea/Beach_Sign"), Vector2.Zero);
            _seaAnimation.AddPart(Content.Load<Texture2D>("Environment/Sea/Inventory_Box"), Vector2.Zero);
        }

    }
}
