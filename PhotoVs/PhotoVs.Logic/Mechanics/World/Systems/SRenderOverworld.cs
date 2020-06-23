using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Logic.Mechanics.Camera.Systems;
using System;
using PhotoVs.Engine;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Graphics;
using PhotoVs.Utils.Collections;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Mechanics.World.Systems
{
    public class SRenderOverworld : IDrawableSystem
    {
        private readonly Overworld _overworld;
        private readonly SpriteBatch _spriteBatch;
        private readonly SCamera _camera;

        private IGameObjectCollection _gameObjects;

        public int Priority { get; set; } = 0;
        public bool Active { get; set; } = true;

        public Type[] Requires { get; } =
        {
        };

        private float day = 0f;
        private LinearTweener<Texture2D> _dayNight;
        private ColorGrading _colorGrade;
        private ColorAverager _colorAverager;
        private RenderTarget2D _target;

        public SRenderOverworld(Overworld overworld, SpriteBatch spriteBatch, SCamera camera, Services services)
        {
            _overworld = overworld;
            _spriteBatch = spriteBatch;
            _camera = camera;

            _colorAverager= new ColorAverager(services.Get<GraphicsDevice>(), 
                services.Get<IAssetLoader>().Get<Effect>(services.Get<IPlatform>().AverageShader));
            _colorGrade = new ColorGrading(services.Get<GraphicsDevice>(), new CanvasSize(640, 320), 
                services.Get<IAssetLoader>().Get<Effect>(services.Get<IPlatform>().PaletteShader), null);
            _dayNight = new LinearTweener<Texture2D>();
            _target = new RenderTarget2D(spriteBatch.GraphicsDevice, 640, 320);

            var assetLoader = services.Get<IAssetLoader>();
            _dayNight.AddPoint(0.041667f, assetLoader.Get<Texture2D>("luts/daycycle1.png"));
            _dayNight.AddPoint(0.1875f, assetLoader.Get<Texture2D>("luts/daycycle2.png"));
            _dayNight.AddPoint(0.30208333f, assetLoader.Get<Texture2D>("luts/daycycle3.png"));
            _dayNight.AddPoint(0.41667f, assetLoader.Get<Texture2D>("luts/daycycle4.png"));
            _dayNight.AddPoint(0.5f, assetLoader.Get<Texture2D>("luts/daycycle5.png"));
            _dayNight.AddPoint(0.58333f, assetLoader.Get<Texture2D>("luts/daycycle6.png"));
            _dayNight.AddPoint(0.6875f, assetLoader.Get<Texture2D>("luts/daycycle7.png"));
            _dayNight.AddPoint(0.7708333f, assetLoader.Get<Texture2D>("luts/daycycle8.png"));
            _dayNight.AddPoint(0.82291667f, assetLoader.Get<Texture2D>("luts/daycycle9.png"));
            _dayNight.AddPoint(0.90625f, assetLoader.Get<Texture2D>("luts/daycycle10.png"));

            var ts = TimeSpan.FromMinutes(45);
            timeScale = (float)ts.TotalSeconds;
        }

        public void BeforeDraw(GameTime gameTime)
        {

        }

        private Texture2D lut;
        private float timeScale;

        public void Draw(GameTime gameTime, IGameObjectCollection gameObjects)
        {
            day += (gameTime.GetElapsedSeconds() / timeScale);
            if (day >= 1f)
                day %= 1f;

            var (phase, texA, texB) = _dayNight.GetPhase(day);
            _colorAverager.Set(phase, texA, texB);
            _colorAverager.Average(_spriteBatch);
            lut = _colorAverager.GetOutputTexture();

            _colorGrade.SetTexture(lut);

            _gameObjects = gameObjects;

            _spriteBatch.GraphicsDevice.SetRenderTarget(_target);

            _spriteBatch.Begin(SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp,
                transformMatrix: _camera.Transform);

            _overworld.GetMap()
                .Draw(_spriteBatch,
                    gameTime,
                    _camera,
                    EntityDraw);

            _spriteBatch.End();

            _spriteBatch.GraphicsDevice.SetRenderTarget(null);

            var output = _colorGrade.Filter(_spriteBatch, _target);


            _spriteBatch.Begin(SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(output, Vector2.Zero, Color.White);
            _spriteBatch.Draw(lut, Vector2.Zero, Color.White);
            //_spriteBatch.Draw(texA, Vector2.Zero + new Vector2(0, texA.Height), Color.White);
            //_spriteBatch.Draw(texB, Vector2.Zero + new Vector2(0, texA.Height) + new Vector2(0, texB.Height), Color.White);

            _spriteBatch.End();

            /*using var stream = File.Create("test.png");
            {
                var tex = output;
                tex.SaveAsPng(stream, tex.Width, tex.Height);
            }*/
        }

        public void EntityDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var gameObject in _gameObjects)
            {

            }
        }

        public void AfterDraw(GameTime gameTime)
        {

        }

        public void DrawUI(GameTime gameTime, IGameObjectCollection gameObjectCollection, Matrix uiOrigin)
        {


        }
    }
}
