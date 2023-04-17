using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.MediaFoundation;

namespace Hedgehog
{
    public class Game1 : Game
    {
        static public string LevelName = @"Content\\level1.txt";
        static public string BackupName = @"Content\\backup.txt";

        //DISPLAY
        const int SCREENWIDTH = 1920, SCREENHEIGHT = 1080;
        const bool FULLSCREEN = true;
        private GraphicsDeviceManager graphics;
        PresentationParameters pp;
        private SpriteBatch spriteBatch;
        static public int screenW, screenH;
        static public Vector2 screenCenter;

        //INPUT
        Input inp;

        //TEXTURES
        Texture2D farBackground, midBackground;
        Texture2D tilesImages;

        //SOURCE RECTANGLES
        Rectangle screenRect, desktopRect;

        //POSITIONS
        static public Vector2 backGroundPos;

        //RENDERTARGETS
        RenderTarget2D MainTarget;

        //MAP DATA
        const int MAXSHEETPARTS = 300;
        Sheet[] sheet;
        SheetManager sheetMgr;

        //CAMERA
        static public Vector2 CamPos;

        public Game1()
        {
            int initialScreenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 10; // -10 убрать по завершению (для отладки)
            int initialScreenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 10;


            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = initialScreenWidth,
                PreferredBackBufferHeight = initialScreenHeight,
                IsFullScreen = FULLSCREEN, PreferredDepthStencilFormat = DepthFormat.Depth16
            };

            Window.IsBorderless = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            MainTarget = new RenderTarget2D(GraphicsDevice, SCREENWIDTH, SCREENHEIGHT);
            screenW = MainTarget.Width;
            screenH = MainTarget.Height;

            pp = GraphicsDevice.PresentationParameters;
            SurfaceFormat format = pp.BackBufferFormat;

            desktopRect = new Rectangle(0, 0, pp.BackBufferWidth, pp.BackBufferHeight);
            screenRect = new Rectangle(0, 0, screenW, screenH);
            screenCenter = new Vector2(screenW / 2.0f, screenH / 2.0f) - new Vector2(32f, 32f);

            inp = new Input();

            base.Initialize();

            //ИНИЦИАЛИЗАЦИЯ КАРТЫ ::::TODO
            sheet = new Sheet[MAXSHEETPARTS];
            sheetMgr = new SheetManager();
        }

        protected override void LoadContent()
        {
            //ЗАГРУЗКА ГРАФИКИ
            farBackground = Content.Load<Texture2D>("BackgroundV2");
            midBackground = Content.Load<Texture2D>("midBackground");
            tilesImages = Content.Load<Texture2D>("SPRITEPNG");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed 
                || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            inp.Update();

            if (inp.KeyPress(Keys.Escape)) Exit();

            //ВРЕМЕННО
            if (inp.KeyDown(Keys.Left)) backGroundPos.X++;
            if (inp.KeyDown(Keys.Right)) backGroundPos.X--;
            if (inp.KeyDown(Keys.Down)) backGroundPos.Y--;
            if (inp.KeyDown(Keys.Up)) backGroundPos.Y++;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(MainTarget);

            //ЗАДНИЙ ФОН
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.LinearWrap);
            spriteBatch.Draw(farBackground, screenRect,
                new Rectangle((int)(-backGroundPos.X * 0.5f), 0, farBackground.Width, farBackground.Height), Color.White);
            spriteBatch.End();

            //СРЕДНИЙ ФОН
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearWrap);
            spriteBatch.Draw(midBackground, screenRect,
                new Rectangle((int)(-backGroundPos.X), (int)(-backGroundPos.Y), farBackground.Width, farBackground.Height), Color.White);
            spriteBatch.End();

            //ПЛИТЫ
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque,
                SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone);
            spriteBatch.Draw(MainTarget, desktopRect, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}