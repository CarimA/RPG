using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Core;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Engine.Graphics;
using PhotoVs.Engine.Graphics.Filters;
using PhotoVs.Engine.Graphics.Particles;
using PhotoVs.Logic.Mechanics.Camera.Systems;
using PhotoVs.Logic.Mechanics.Movement.Components;
using PhotoVs.Logic.Particles;
using PhotoVs.Utils.Collections;
using PhotoVs.Utils.Extensions;
using SpriteFontPlus;

namespace PhotoVs.Logic.Mechanics.World.Systems
{
    public class SRenderOverworld : IDrawableSystem
    {
        private readonly SCamera _camera;
        private readonly ICanvasSize _canvasSize;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;
        private readonly IGameState _gameState;

        private Texture2D _checkerboardTexture;
        private readonly ColorAverager _colorAverager;
        private readonly ColorGradingFilter _colorGrade;
        private RenderTarget2D _combinedWater;

        private readonly LinearTweener<Texture2D> _dayNight;
        private readonly Effect _displaceEffect;
        private readonly Texture2D _displacementTexture;
        private readonly Texture2D _displacementTexture2;
        private RenderTarget2D _final;

        private readonly GameDate _gameDate;

        private GameObjectList _gameObjects;
        private readonly Texture2D _indexMaterialTexture;
        private readonly Texture2D _indexTexture;
        private RenderTarget2D _material;
        private readonly Texture2D _noiseA;
        private readonly Texture2D _noiseB;
        private CPosition _playerPosition;
        private readonly IRenderer _renderer;
        private RenderTarget2D _target;
        private readonly Effect _tilemapEffect;
        private readonly Texture2D _tilemapTexture;
        private RenderTarget2D _water;
        private RenderTarget2D _waterDisplace;
        private readonly Effect _waterEffect;
        private RenderTarget2D _waterReflection;
        private readonly Effect _waterReflectionEffect;
        private readonly DynamicSpriteFont bold;

        private float throttleRender;
        private float time;

        private Vector2 waterA;
        private Vector2 waterB;

        private RenderTarget2D _windDisplace;
        private Effect _windEffect;
        private Wind _wind;
        private Vector2 _windDir;

        private Emitter<Leaf> _particleTest;

        public SRenderOverworld(IAssetLoader assetLoader, IRenderer renderer, IOverworld overworld,
            SpriteBatch spriteBatch, IGameState gameState,
            ISignal signal, GraphicsDevice graphicsDevice, ICanvasSize canvasSize)
        {
            _renderer = renderer;
            _spriteBatch = spriteBatch;
            _gameState = gameState;
            _graphicsDevice = graphicsDevice;
            _canvasSize = canvasSize;
            _camera = gameState.Camera;

            _playerPosition = gameState.Player.PlayerData.Position;

            _gameDate = new GameDate(signal);
            bold = assetLoader.Get<DynamicSpriteFont>("ui/fonts/ubuntu.ttf");

            _colorAverager = new ColorAverager(renderer,
                assetLoader.Get<Effect>("shaders/average.fx"));
            _colorGrade = new ColorGradingFilter(renderer, spriteBatch,
                assetLoader.Get<Effect>("shaders/color.fx"));
            _dayNight = new LinearTweener<Texture2D>();
            _waterReflectionEffect = assetLoader.Get<Effect>("shaders/water_reflection.fx");
            _waterEffect = assetLoader.Get<Effect>("shaders/water.fx");
            _displaceEffect = assetLoader.Get<Effect>("shaders/displace.fx");

            _tilemapTexture = assetLoader.Get<Texture2D>("debug/outmap.png");
            _indexTexture = assetLoader.Get<Texture2D>("debug/outts.png");
            _indexMaterialTexture = assetLoader.Get<Texture2D>("debug/outts_mat.png");
            _tilemapEffect = assetLoader.Get<Effect>("shaders/tilemap.fx");

            _noiseA = assetLoader.Get<Texture2D>("ui/noise.png");
            _noiseB = assetLoader.Get<Texture2D>("ui/noise2.png");
            _displacementTexture = assetLoader.Get<Texture2D>("ui/displacement.png");
            _displacementTexture2 = assetLoader.Get<Texture2D>("ui/displacement2.png");
            _checkerboardTexture = assetLoader.Get<Texture2D>("ui/checkerboard.png");

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

            _particleTest = new Emitter<Leaf>(30, assetLoader.Get<Texture2D>("particles/leaf.png"), new Rectangle(8445, 5928, 50, 50));

            _wind = new Wind();
            _windEffect = assetLoader.Get<Effect>("shaders/wind.fx");


            OnResize();
            _canvasSize.OnResize += OnResize;
        }

        public int Priority { get; set; } = 0;
        public bool Active { get; set; } = true;

        public Type[] Requires { get; } =
        {
        };

        public void BeforeDraw(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime, GameObjectList gameObjects)
        {
            _wind.Update(gameTime);

            var cameraRect = _camera.VisibleArea();
            var tPos = Vector2.Zero;

            _gameDate.Update(gameTime);

            var (phase, texA, texB) = _dayNight.GetPhase(_gameDate.TimeScale);
            _colorAverager.SetTextures(texA, texB);
            _colorAverager.SetPhase(phase);

            _colorGrade.LookupTable = _colorAverager.Average(_spriteBatch);

            _gameObjects = gameObjects;


            throttleRender -= gameTime.GetElapsedSeconds();
            if (throttleRender <= 0f)
            {
                var frametime = 1f / 14f;
                throttleRender = frametime;
                _windDir = _wind.Direction * _wind.Progress() * frametime;

                waterA -= new Vector2(.53f, 2.40f) * frametime;
                waterB -= new Vector2(.12f, -4.2f) * frametime;
                time += frametime;
            }


            _renderer.RequestSubRenderer(_target);


            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
                transformMatrix: _camera.Transform);


            var tileSize = 16;
            //_tilemapEffect.Parameters["MatrixTransform"].SetValue(_camera.Transform);
            _tilemapEffect.Parameters["Texture"].SetValue(_tilemapTexture);
            _tilemapEffect.Parameters["texIndex"].SetValue(_indexTexture);
            //_tilemapEffect.Parameters["viewSize"].SetValue(new Vector2(_renderer.GameWidth, _renderer.GameHeight));
            //_tilemapEffect.Parameters["viewOffset"].SetValue(new Vector2(cameraRect.Left, cameraRect.Top));
            _tilemapEffect.Parameters["tileSize"].SetValue(new Vector2(tileSize, tileSize));
            _tilemapEffect.Parameters["inverseIndexTexSize"]
                .SetValue(new Vector2(1f / _indexTexture.Width, 1f / _indexTexture.Height));
            _tilemapEffect.Parameters["mapSize"]
                .SetValue(new Vector2(_tilemapTexture.Width * tileSize, _tilemapTexture.Height * tileSize));

            _tilemapEffect.CurrentTechnique.Passes[0].Apply();

            _spriteBatch.Draw(_tilemapTexture,
                new Rectangle(0, 0, _tilemapTexture.Width / 2 * tileSize, _tilemapTexture.Height * tileSize),
                new Rectangle(0, 0, _tilemapTexture.Width / 2, _tilemapTexture.Height),
                Color.White);

            /*_overworld.GetMap()
                .Draw(_spriteBatch,
                    gameTime,
                    _camera,
                    EntityDraw);*/

            _spriteBatch.End();



            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
                transformMatrix: _camera.Transform);
            _particleTest.Update(gameTime);
            _particleTest.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();



            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
                transformMatrix: _camera.Transform);


            //_tilemapEffect.Parameters["MatrixTransform"].SetValue(_camera.Transform);
            _tilemapEffect.Parameters["Texture"].SetValue(_tilemapTexture);
            _tilemapEffect.Parameters["texIndex"].SetValue(_indexTexture);
            //_tilemapEffect.Parameters["viewSize"].SetValue(new Vector2(_renderer.GameWidth, _renderer.GameHeight));
            //_tilemapEffect.Parameters["viewOffset"].SetValue(new Vector2(cameraRect.Left, cameraRect.Top));
            _tilemapEffect.Parameters["tileSize"].SetValue(new Vector2(tileSize, tileSize));
            _tilemapEffect.Parameters["inverseIndexTexSize"]
                .SetValue(new Vector2(1f / _indexTexture.Width, 1f / _indexTexture.Height));
            _tilemapEffect.Parameters["mapSize"]
                .SetValue(new Vector2(_tilemapTexture.Width * tileSize, _tilemapTexture.Height * tileSize));

            _tilemapEffect.CurrentTechnique.Passes[0].Apply();

            _spriteBatch.Draw(_tilemapTexture,
                new Rectangle(0, 0, _tilemapTexture.Width / 2 * tileSize, _tilemapTexture.Height * tileSize),
                new Rectangle(_tilemapTexture.Width / 2, 0, _tilemapTexture.Width / 2, _tilemapTexture.Height),
                Color.White);

            /*_overworld.GetMap()
                .Draw(_spriteBatch,
                    gameTime,
                    _camera,
                    EntityDraw);*/

            _spriteBatch.End();


            _renderer.RelinquishSubRenderer();


            _renderer.RequestSubRenderer(_material);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
                transformMatrix: _camera.Transform);


            //_tilemapEffect.Parameters["MatrixTransform"].SetValue(_camera.Transform);
            _tilemapEffect.Parameters["Texture"].SetValue(_tilemapTexture);
            _tilemapEffect.Parameters["texIndex"].SetValue(_indexMaterialTexture);
            //_tilemapEffect.Parameters["viewSize"].SetValue(new Vector2(_renderer.GameWidth, _renderer.GameHeight));
            //_tilemapEffect.Parameters["viewOffset"].SetValue(new Vector2(cameraRect.Left, cameraRect.Top));
            _tilemapEffect.Parameters["tileSize"].SetValue(new Vector2(tileSize, tileSize));
            _tilemapEffect.Parameters["inverseIndexTexSize"]
                .SetValue(new Vector2(1f / _indexTexture.Width, 1f / _indexTexture.Height));
            _tilemapEffect.Parameters["mapSize"]
                .SetValue(new Vector2(_tilemapTexture.Width * tileSize, _tilemapTexture.Height * tileSize));

            _tilemapEffect.CurrentTechnique.Passes[0].Apply();

            _spriteBatch.Draw(_tilemapTexture,
                new Rectangle(0, 0, _tilemapTexture.Width / 2 * tileSize, _tilemapTexture.Height * tileSize),
                new Rectangle(0, 0, _tilemapTexture.Width / 2, _tilemapTexture.Height),
                Color.White);


            _spriteBatch.Draw(_tilemapTexture,
                new Rectangle(0, 0, _tilemapTexture.Width / 2 * tileSize, _tilemapTexture.Height * tileSize),
                new Rectangle(_tilemapTexture.Width / 2, 0, _tilemapTexture.Width / 2, _tilemapTexture.Height),
                Color.White);

            /*_overworld.GetMap()
                .Draw(_spriteBatch,
                    gameTime,
                    _camera,
                    EntityDraw);*/

            _spriteBatch.End();

            _renderer.RelinquishSubRenderer();


            _renderer.RequestSubRenderer(_waterReflection);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);

            //_waterEffect.Parameters["texMap"].SetValue(_material);
            _waterReflectionEffect.Parameters["Texture"].SetValue(_material);
            _waterReflectionEffect.Parameters["texInput"].SetValue(_target);
            _waterReflectionEffect.Parameters["pixHeight"].SetValue(1f / _waterReflection.Height);

            _waterReflectionEffect.Parameters["water"]
                .SetValue(new Vector4(0.03529411764f, 0.3725490196f, 0.47843137254f, 1.0f));

            _waterReflectionEffect.Parameters["water"]
                .SetValue(new Vector4(0.03529411764f, 0.3725490196f, 0.47843137254f, 1.0f));

            _waterReflectionEffect.CurrentTechnique.Passes[0].Apply();
            _spriteBatch.Draw(_material, Vector2.Zero, Color.White);

            _spriteBatch.End();

            _renderer.RelinquishSubRenderer();


            _renderer.RequestSubRenderer(_water);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap,
                transformMatrix: _camera.Transform);

            var zoomFactor = 1f / _camera.CurrentZoom * (_gameState.ZoomScale * 2);

            _waterEffect.Parameters["Texture"].SetValue(_material);
            _waterEffect.Parameters["texNoiseA"].SetValue(_noiseA);
            _waterEffect.Parameters["texNoiseB"].SetValue(_noiseB);
            _waterEffect.Parameters["contrast"].SetValue(1.1f);
            _waterEffect.Parameters["step"].SetValue(6);
            _waterEffect.Parameters["scale"].SetValue(zoomFactor);
            _waterEffect.Parameters["pixWidth"].SetValue(1f / _water.Width);
            _waterEffect.Parameters["pixHeight"].SetValue(1f / _water.Height);

            _waterEffect.Parameters["water"].SetValue(new Vector4(0.03529411764f, 0.3725490196f, 0.47843137254f, 1.0f));
            _waterEffect.Parameters["highlightWater"]
                .SetValue(new Vector4(0.37647058823f, 0.70588235294f, 0.84705882352f, 1.0f));

            _waterEffect.Parameters["offsetXA"].SetValue(waterA.X * zoomFactor * (1f / _water.Width));
            _waterEffect.Parameters["offsetYA"]
                .SetValue((waterA.Y) * zoomFactor * (1f / _water.Height));
            _waterEffect.Parameters["offsetXB"].SetValue((waterB.X) * zoomFactor * (1f / _water.Width));
            _waterEffect.Parameters["offsetYB"]
                .SetValue((waterB.Y) * zoomFactor * (1f / _water.Height));
            _waterEffect.Parameters["time"].SetValue(time / 6000000f);

            _waterEffect.CurrentTechnique.Passes[0].Apply();

            _spriteBatch.Draw(_material, cameraRect, Color.White);

            _spriteBatch.End();

            _renderer.RelinquishSubRenderer();


            _renderer.RequestSubRenderer(_combinedWater);
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap);
            _spriteBatch.Draw(_water, Vector2.Zero, Color.White * 0.76f);
            _spriteBatch.Draw(_waterReflection, Vector2.Zero, Color.White * 0.45f);
            _spriteBatch.End();
            _renderer.RelinquishSubRenderer();


            _renderer.RequestSubRenderer(_waterDisplace);
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap,
                transformMatrix: _camera.Transform);
            _displaceEffect.Parameters["Texture"].SetValue(_combinedWater);
            _displaceEffect.Parameters["texMask"].SetValue(_material);
            _displaceEffect.Parameters["texDisplace"].SetValue(_displacementTexture);
            _displaceEffect.Parameters["texDisplace2"].SetValue(_displacementTexture2);
            _displaceEffect.Parameters["offsetX"]
                .SetValue((waterA.X * 15 + tPos.X) % _material.Width / _material.Width);
            _displaceEffect.Parameters["offsetY"]
                .SetValue((waterA.Y * 8 + tPos.Y) % _material.Height / _material.Height);
            //_displaceEffect.Parameters["time"].SetValue(time * 1);
            _displaceEffect.Parameters["pixWidth"].SetValue(1f / _waterDisplace.Width);
            _displaceEffect.Parameters["pixHeight"].SetValue(1f / _waterDisplace.Height);
            _displaceEffect.Parameters["maxDisplace"].SetValue(4f);
            _displaceEffect.Parameters["water"].SetValue(new Vector4(0.01568628F, 0.172549F, 0.2235294F, 1.0f));
            _displaceEffect.CurrentTechnique.Passes[0].Apply();
            _spriteBatch.Draw(_combinedWater, cameraRect, Color.White);
            _spriteBatch.End();
            _renderer.RelinquishSubRenderer();





            /*_renderer.RequestSubRenderer(_windDisplace);
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap);
            _windEffect.Parameters["Texture"].SetValue(_target);
// _windEffect.Parameters["texNoise"].SetValue(_noiseB);
            _windEffect.Parameters["texMask"].SetValue(_material);
            _windEffect.Parameters["time"].SetValue(time);
            _windEffect.Parameters["waveSpeed"].SetValue(0.008f);
            _windEffect.Parameters["waveFreq"].SetValue(_wind.Force);
            _windEffect.Parameters["windDir"].SetValue(_windDir);
            _windEffect.Parameters["waveAmp"].SetValue((1f / _windDisplace.Height) * _wind.Amplitude * _wind.Progress());
            _windEffect.Parameters["texelSize"]
                .SetValue(new Vector2(1f / _windDisplace.Width, 1f / _windDisplace.Height));
            _windEffect.Parameters["offset"]
                .SetValue(new Vector2(((tPos.X) % _material.Width) / _material.Width,
                    ((tPos.Y) % _material.Height) / _material.Height));
            _windEffect.Parameters["texelSize"]
                .SetValue(new Vector2(1f / _windDisplace.Width, 1f / _windDisplace.Height));
            _windEffect.CurrentTechnique.Passes[0].Apply();
            _spriteBatch.Draw(_target, Vector2.Zero, Color.White);
            _spriteBatch.End();
            _renderer.RelinquishSubRenderer();*/





            _renderer.RequestSubRenderer(_final);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            _spriteBatch.Draw(_target, Vector2.Zero, Color.White);
            //_spriteBatch.Draw(_windDisplace, Vector2.Zero, Color.White);
            //_spriteBatch.Draw(_water, Vector2.Zero, Color.White);
            _spriteBatch.Draw(_waterDisplace, Vector2.Zero, Color.White);
            //_spriteBatch.Draw(_combinedWater, Vector2.Zero, Color.White);
            //_spriteBatch.Draw(_waterReflection, Vector2.Zero, Color.White * 0.5f);
            _spriteBatch.End();
            _renderer.RelinquishSubRenderer();

            var (hour, minute) = _gameDate.Time;
            var phaseName = Enum.GetName(typeof(TimePhase), _gameDate.TimePhase);
            var t = $"Time: {phaseName} - {hour}:{minute}\n" +
                    $"Wind Amplitude: {_wind.Amplitude}\n" +
                    $"Wind Direction: {_wind.Direction}\n" +
                    $"Wind Force: {_wind.Force}";

            var output = _colorGrade.Filter(_spriteBatch, _final);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            //_spriteBatch.Draw(_material, Vector2.Zero, Color.White);
            _spriteBatch.Draw(_final, Vector2.Zero, Color.White);
            _spriteBatch.DrawString(bold, 
                t, 
                Vector2.Zero,
                Color.Yellow);

            _spriteBatch.End();

            /*using var stream = File.Create("test.png");
            {
                var tex = output;
                tex.SaveAsPng(stream, tex.Width, tex.Height);
            }*/
        }

        public void AfterDraw(GameTime gameTime)
        {
        }

        public void DrawUI(GameTime gameTime, GameObjectList gameObjectCollection, Matrix uiOrigin)
        {
        }

        private void OnResize()
        {
            _target = _renderer.CreateRenderTarget(_canvasSize.DisplayWidth, _canvasSize.DisplayHeight);
            _material = _renderer.CreateRenderTarget(_canvasSize.DisplayWidth, _canvasSize.DisplayHeight);
            _waterReflection = _renderer.CreateRenderTarget(_canvasSize.DisplayWidth, _canvasSize.DisplayHeight);
            _water = _renderer.CreateRenderTarget(_canvasSize.DisplayWidth, _canvasSize.DisplayHeight);
            _final = _renderer.CreateRenderTarget(_canvasSize.DisplayWidth, _canvasSize.DisplayHeight);
            _combinedWater = _renderer.CreateRenderTarget(_canvasSize.DisplayWidth, _canvasSize.DisplayHeight);
            _waterDisplace = _renderer.CreateRenderTarget(_canvasSize.DisplayWidth, _canvasSize.DisplayHeight);
            _windDisplace = _renderer.CreateRenderTarget(_canvasSize.DisplayWidth, _canvasSize.DisplayHeight);
        }

        public void EntityDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var gameObject in _gameObjects)
            {
            }
        }
    }
}