using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Engine.Graphics;
using PhotoVs.Engine.Graphics.Filters;
using PhotoVs.Logic.Mechanics.Camera.Systems;
using PhotoVs.Logic.Mechanics.Movement.Components;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Utils.Collections;
using PhotoVs.Utils.Extensions;
using System;

namespace PhotoVs.Logic.Mechanics.World.Systems
{
    public class SRenderOverworld : IDrawableSystem
    {
        private readonly Overworld _overworld;
        private readonly SpriteBatch _spriteBatch;
        private readonly SCamera _camera;

        private GameObjectList _gameObjects;

        public int Priority { get; set; } = 0;
        public bool Active { get; set; } = true;

        public Type[] Requires { get; } =
        {
        };

        private float day = 0f;
        private LinearTweener<Texture2D> _dayNight;
        private ColorGradingFilter _colorGrade;
        private ColorAverager _colorAverager;
        private RenderTarget2D _target;
        private RenderTarget2D _material;
        private RenderTarget2D _waterReflection;
        private RenderTarget2D _water;
        private RenderTarget2D _combinedWater;
        private RenderTarget2D _waterDisplace;
        private RenderTarget2D _final;
        private Effect _waterReflectionEffect;
        private Effect _waterEffect;
        private Effect _displaceEffect;
        private Renderer _renderer;
        private Wind _wind;
        private Vector2 _windDir;
        private Texture2D _noiseA;
        private Texture2D _noiseB;
        private Texture2D _displacementTexture;
        private Texture2D _displacementTexture2;
        private CPosition _playerPosition;

        public SRenderOverworld(Overworld overworld, SpriteBatch spriteBatch, SCamera camera, Services services)
        {
            var assetLoader = services.Get<IAssetLoader>();
            _renderer = services.Get<Renderer>();
            _overworld = overworld;
            _spriteBatch = spriteBatch;
            _camera = camera;

            _playerPosition = services.Get<Player>().PlayerData.Position;

            _wind = new Wind();

            _colorAverager = new ColorAverager(services.Get<Renderer>(),
                assetLoader.Get<Effect>("shaders/average.fx"));
            _colorGrade = new ColorGradingFilter(services.Get<Renderer>(),
                assetLoader.Get<Effect>("shaders/color.fx"));
            _dayNight = new LinearTweener<Texture2D>();
            _waterReflectionEffect = assetLoader.Get<Effect>("shaders/water_reflection.fx");
            _waterEffect = assetLoader.Get<Effect>("shaders/water.fx");
            _displaceEffect = assetLoader.Get<Effect>("shaders/displace.fx");

            _noiseA = assetLoader.Get<Texture2D>("ui/noise.png");
            _noiseB = assetLoader.Get<Texture2D>("ui/noise2.png");
            _displacementTexture = assetLoader.Get<Texture2D>("ui/displacement.png");
            _displacementTexture2 = assetLoader.Get<Texture2D>("ui/displacement2.png");

            _dayNight.AddPoint(0, assetLoader.Get<Texture2D>("luts/daycycle1.png"));
            _dayNight.AddPoint(0.1875f, assetLoader.Get<Texture2D>("luts/daycycle1.png"));
            _dayNight.AddPoint(0.208334f, assetLoader.Get<Texture2D>("luts/daycycle2.png"));
            _dayNight.AddPoint(0.30208333f, assetLoader.Get<Texture2D>("luts/daycycle3.png"));
            _dayNight.AddPoint(0.41667f, assetLoader.Get<Texture2D>("luts/daycycle4.png"));
            _dayNight.AddPoint(0.46f, assetLoader.Get<Texture2D>("luts/daycycle5.png"));
            _dayNight.AddPoint(0.54f, assetLoader.Get<Texture2D>("luts/daycycle5.png"));
            _dayNight.AddPoint(0.58333f, assetLoader.Get<Texture2D>("luts/daycycle6.png"));
            _dayNight.AddPoint(0.6875f, assetLoader.Get<Texture2D>("luts/daycycle7.png"));
            _dayNight.AddPoint(0.7708333f, assetLoader.Get<Texture2D>("luts/daycycle8.png"));
            _dayNight.AddPoint(0.82291667f, assetLoader.Get<Texture2D>("luts/daycycle9.png"));
            _dayNight.AddPoint(0.90625f, assetLoader.Get<Texture2D>("luts/daycycle10.png"));

            var ts = TimeSpan.FromSeconds(20);
            timeScale = (float)ts.TotalSeconds;
        }

        public void BeforeDraw(GameTime gameTime)
        {

        }

        private Texture2D lut;
        private float timeScale;

        private Vector2 waterA;
        private Vector2 waterB;
        private float time;

        private float throttleRender;

        public void Draw(GameTime gameTime, GameObjectList gameObjects)
        {


            var cameraRect = _camera.VisibleArea();
            var tPos = (new Vector2(cameraRect.Left, cameraRect.Top));

            _wind.Update(gameTime);

            if (_target == null || _target.Width != _renderer.GameWidth ||
                _target.Height != _renderer.GameHeight)
            {
                _target = new RenderTarget2D(_renderer.GraphicsDevice, _renderer.GameWidth, _renderer.GameHeight);
                _material = new RenderTarget2D(_renderer.GraphicsDevice, _renderer.GameWidth, _renderer.GameHeight);
                _waterReflection =
                    new RenderTarget2D(_renderer.GraphicsDevice, _renderer.GameWidth, _renderer.GameHeight);
                _water = new RenderTarget2D(_renderer.GraphicsDevice, _renderer.GameWidth, _renderer.GameHeight);
                _final = new RenderTarget2D(_renderer.GraphicsDevice, _renderer.GameWidth, _renderer.GameHeight);
                _combinedWater = new RenderTarget2D(_renderer.GraphicsDevice, _renderer.GameWidth, _renderer.GameHeight);
                _waterDisplace = new RenderTarget2D(_renderer.GraphicsDevice, _renderer.GameWidth, _renderer.GameHeight);
            }

            day += (gameTime.GetElapsedSeconds() / timeScale);
            if (day >= 1f)
                day %= 1f;

            var (phase, texA, texB) = _dayNight.GetPhase(day);
            _colorAverager.Set(phase, texA, texB);
            lut = _colorAverager.Average(_spriteBatch);

            _colorGrade.LookupTable = lut;

            _gameObjects = gameObjects;


            throttleRender -= gameTime.GetElapsedSeconds();
            if (throttleRender <= 0f)
            {
                throttleRender = 0.06666666666f;

                _windDir += _wind.Direction * _wind.Force * throttleRender;

                waterA -= new Vector2(.53f, 2.40f) * throttleRender;
                waterB -= new Vector2(.12f, -4.2f) * throttleRender;
                time += throttleRender;
            }

            _renderer.RequestSubRenderer(_material);

                _spriteBatch.Begin(SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp,
                    transformMatrix: _camera.Transform);

                _overworld.GetMap()
                    .DrawMaterial(_spriteBatch,
                        gameTime,
                        _camera,
                        EntityDraw);

                _spriteBatch.End();

                _renderer.RelinquishSubRenderer();




                _renderer.RequestSubRenderer(_target);

                _spriteBatch.Begin(SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp,
                    transformMatrix: _camera.Transform);

                _overworld.GetMap()
                    .Draw(_spriteBatch,
                        gameTime,
                        _camera,
                        EntityDraw);

                _spriteBatch.End();

                _renderer.RelinquishSubRenderer();







                _renderer.RequestSubRenderer(_waterReflection);

                _renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);

                //_waterEffect.Parameters["texMap"].SetValue(_material);
                _waterReflectionEffect.Parameters["Texture"].SetValue(_material);
                _waterReflectionEffect.Parameters["texInput"].SetValue(_target);
                _waterReflectionEffect.Parameters["pixHeight"].SetValue(1f / _waterReflection.Height);

                _waterReflectionEffect.Parameters["water"].SetValue(new Vector4(0.03529411764f, 0.3725490196f, 0.47843137254f, 1.0f));

                _waterReflectionEffect.Parameters["water"].SetValue(new Vector4(0.03529411764f, 0.3725490196f, 0.47843137254f, 1.0f));

            _waterReflectionEffect.CurrentTechnique.Passes[0].Apply();
                _renderer.SpriteBatch.Draw(_material, Vector2.Zero, Color.White);

                _renderer.SpriteBatch.End();

                _renderer.RelinquishSubRenderer();




                _renderer.RequestSubRenderer(_water);

                _renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap,
                    transformMatrix: _camera.Transform);

                _waterEffect.Parameters["Texture"].SetValue(_material);
                _waterEffect.Parameters["texNoiseA"].SetValue(_noiseA);
                _waterEffect.Parameters["texNoiseB"].SetValue(_noiseB);
                _waterEffect.Parameters["contrast"].SetValue(1.1f);
                _waterEffect.Parameters["step"].SetValue(6);
            _waterEffect.Parameters["pixWidth"].SetValue(1f / _water.Width);
                _waterEffect.Parameters["pixHeight"].SetValue(1f / _water.Height);

                _waterEffect.Parameters["water"].SetValue(new Vector4(0.03529411764f, 0.3725490196f, 0.47843137254f, 1.0f));
                _waterEffect.Parameters["highlightWater"].SetValue(new Vector4(0.37647058823f, 0.70588235294f, 0.84705882352f, 1.0f));

                _waterEffect.Parameters["offsetXA"].SetValue(((waterA.X + tPos.X) % _material.Width) / _material.Width);
                _waterEffect.Parameters["offsetYA"]
                    .SetValue(((waterA.Y + tPos.Y) % _material.Height) / _material.Height);
                _waterEffect.Parameters["offsetXB"].SetValue(((waterB.X + tPos.X) % _material.Width) / _material.Width);
                _waterEffect.Parameters["offsetYB"]
                    .SetValue(((waterB.Y + tPos.Y) % _material.Height) / _material.Height);
                _waterEffect.Parameters["time"].SetValue(time / 60f);

                _waterEffect.CurrentTechnique.Passes[0].Apply();

                _renderer.SpriteBatch.Draw(_material, cameraRect, Color.White);

                _renderer.SpriteBatch.End();

                _renderer.RelinquishSubRenderer();






                _renderer.RequestSubRenderer(_combinedWater);
                _renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap);
                _renderer.SpriteBatch.Draw(_water, Vector2.Zero, Color.White * 0.76f);
                _renderer.SpriteBatch.Draw(_waterReflection, Vector2.Zero, Color.White * 0.45f);
                _renderer.SpriteBatch.End();
                _renderer.RelinquishSubRenderer();






                _renderer.RequestSubRenderer(_waterDisplace);
                _renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap,
                    transformMatrix: _camera.Transform);
                _displaceEffect.Parameters["Texture"].SetValue(_combinedWater);
                _displaceEffect.Parameters["texMask"].SetValue(_material);
                _displaceEffect.Parameters["texDisplace"].SetValue(_displacementTexture);
                _displaceEffect.Parameters["texDisplace2"].SetValue(_displacementTexture2);
                _displaceEffect.Parameters["offsetX"]
                    .SetValue((((waterA.X * 15) + tPos.X) % _material.Width) / _material.Width);
                _displaceEffect.Parameters["offsetY"]
                    .SetValue((((waterA.Y * 8) + tPos.Y) % _material.Height) / _material.Height);
            //_displaceEffect.Parameters["time"].SetValue(time * 1);
            _displaceEffect.Parameters["pixWidth"].SetValue(1f / _waterDisplace.Width);
                _displaceEffect.Parameters["pixHeight"].SetValue(1f / _waterDisplace.Height);
                _displaceEffect.Parameters["maxDisplace"].SetValue(4f);
                _displaceEffect.Parameters["water"].SetValue(new Vector4(0.01568628F, 0.172549F, 0.2235294F, 1.0f));
            _displaceEffect.CurrentTechnique.Passes[0].Apply();
                _renderer.SpriteBatch.Draw(_combinedWater, cameraRect, Color.White);
                _renderer.SpriteBatch.End();
                _renderer.RelinquishSubRenderer();




            _renderer.RequestSubRenderer(_final);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            _spriteBatch.Draw(_target, Vector2.Zero, Color.White);
            //_spriteBatch.Draw(_water, Vector2.Zero, Color.White);
            //_spriteBatch.Draw(_water, Vector2.Zero, Color.White * 0.5f);
            _spriteBatch.Draw(_waterDisplace, Vector2.Zero, Color.White);
            //_spriteBatch.Draw(_combinedWater, Vector2.Zero, Color.White);
            //_spriteBatch.Draw(_waterReflection, Vector2.Zero, Color.White * 0.5f);
            _spriteBatch.End();
            _renderer.RelinquishSubRenderer();





            var output = _colorGrade.Filter(_spriteBatch, _final);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            //_spriteBatch.Draw(_material, Vector2.Zero, Color.White);
            _spriteBatch.Draw(_final, Vector2.Zero, Color.White);
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

        public void DrawUI(GameTime gameTime, GameObjectList gameObjectCollection, Matrix uiOrigin)
        {


        }
    }
}
