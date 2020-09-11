using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.Graphics;
using PhotoVs.Engine.Graphics.Filters;
using PhotoVs.Engine.Graphics.Particles;
using PhotoVs.Logic.Filters;
using PhotoVs.Logic.Mechanics.Components;
using PhotoVs.Logic.Mechanics.World;
using PhotoVs.Utils.Collections;

namespace PhotoVs.Logic.Mechanics
{
    public class DrawMap
    {
        private readonly IOverworld _overworld;
        private readonly SpriteBatch _spriteBatch;
        private readonly IRenderer _renderer;
        private readonly IAssetLoader _assetLoader;
        private readonly Camera _camera;
        private readonly Primitive _primitive;
        private readonly CanvasSize _canvasSize;

        private readonly TilemapFilter _tileMapFilter;
        private readonly WaterReflectionFilter _waterReflectionFilter;
        private readonly WaterFilter _waterFilter;
        private readonly WaterDisplacementFilter _waterDisplacementFilter;
        private readonly DayNightFilter _dayNightFilter;

        private readonly Texture2D _superTileset;
        private readonly Texture2D _tempTileMap;

        private RenderTarget2D _tilemapRenderTarget;
        private RenderTarget2D _waterTarget;
        private RenderTarget2D _reflectionTarget;
        private RenderTarget2D _displaceTarget;
        private RenderTarget2D _maskTarget;
        private RenderTarget2D _fringeTarget;
        private RenderTarget2D _finalTarget;
        private RenderTarget2D _colorGradeTarget;

        public DrawMap(IOverworld overworld, SpriteBatch spriteBatch, IRenderer renderer, IAssetLoader assetLoader, Camera camera, Primitive primitive, CanvasSize canvasSize, GameDate gameDate)
        {
            _overworld = overworld;
            _spriteBatch = spriteBatch;
            _renderer = renderer;
            _assetLoader = assetLoader;
            _camera = camera;
            _primitive = primitive;
            _canvasSize = canvasSize;

            _tempTileMap = _assetLoader.Get<Texture2D>("debug/outmap.png");
            _superTileset = _assetLoader.Get<Texture2D>("debug/supertileset.png");

            _tileMapFilter = new TilemapFilter(_renderer, _spriteBatch,
                _assetLoader.Get<Effect>("shaders/tilemap.fx"), 16);
            _tileMapFilter.SetSuperTileset(_superTileset);
            _tileMapFilter.SetTileMapTexture(_tempTileMap);

            _waterReflectionFilter = new WaterReflectionFilter(_renderer, _spriteBatch,
                _assetLoader.Get<Effect>("shaders/water_reflection.fx"));

            _waterFilter = new WaterFilter(_renderer, _spriteBatch, _camera, canvasSize,
                _assetLoader.Get<Effect>("shaders/water.fx"), 
                _assetLoader.Get<Texture2D>("ui/noise.png"),
                _assetLoader.Get<Texture2D>("ui/noise2.png"));

            _waterDisplacementFilter = new WaterDisplacementFilter(_renderer, _spriteBatch, _canvasSize, 
                _assetLoader.Get<Effect>("shaders/displace.fx"),
                _assetLoader.Get<Texture2D>("ui/displacement.png"),
                _assetLoader.Get<Texture2D>("ui/displacement2.png"));

            _dayNightFilter = new DayNightFilter(_renderer, _spriteBatch, gameDate,
                _assetLoader.Get<Effect>("shaders/average.fx"),
                _assetLoader.Get<Effect>("shaders/color.fx"),
                new List<(float, Texture2D)>()
                {
                    (0, assetLoader.Get<Texture2D>("luts/daycycle1.png")),
                    (0.1875f, assetLoader.Get<Texture2D>("luts/daycycle1.png")),
                    (0.208334f, assetLoader.Get<Texture2D>("luts/daycycle2.png")),
                    (0.30208333f, assetLoader.Get<Texture2D>("luts/daycycle3.png")),
                    (0.41667f, assetLoader.Get<Texture2D>("luts/daycycle4.png")),
                    (0.46f, assetLoader.Get<Texture2D>("luts/daycycle5.png")),
                    (0.54f, assetLoader.Get<Texture2D>("luts/daycycle5.png")),
                    (0.58333f, assetLoader.Get<Texture2D>("luts/daycycle6.png")),
                    (0.6875f, assetLoader.Get<Texture2D>("luts/daycycle7.png")),
                    (0.7708333f, assetLoader.Get<Texture2D>("luts/daycycle8.png")),
                    (0.82291667f, assetLoader.Get<Texture2D>("luts/daycycle9.png")),
                    (0.90625f, assetLoader.Get<Texture2D>("luts/daycycle10.png"))
                });

            OnResize();
            _canvasSize.OnResize += OnResize;
        }

        private void OnResize()
        {
            _tilemapRenderTarget?.Dispose();
            _tilemapRenderTarget = null;

            _reflectionTarget?.Dispose();
            _reflectionTarget = null;

            _waterTarget?.Dispose();
            _waterTarget = null;

            _displaceTarget?.Dispose();
            _displaceTarget = null;

            _maskTarget?.Dispose();
            _maskTarget = null;

            _fringeTarget?.Dispose();
            _fringeTarget = null;

            _finalTarget?.Dispose();
            _finalTarget = null;

            _colorGradeTarget?.Dispose();
            _colorGradeTarget = null;

            _tilemapRenderTarget = _renderer.CreateRenderTarget(_canvasSize.TrueCurrentWidth, _canvasSize.TrueCurrentHeight);
            _waterTarget = _renderer.CreateRenderTarget(_canvasSize.TrueCurrentWidth, _canvasSize.TrueCurrentHeight);
            _reflectionTarget = _renderer.CreateRenderTarget(_canvasSize.TrueCurrentWidth, _canvasSize.TrueCurrentHeight);
            _displaceTarget = _renderer.CreateRenderTarget(_canvasSize.TrueCurrentWidth, _canvasSize.TrueCurrentHeight);
            _maskTarget = _renderer.CreateRenderTarget(_canvasSize.TrueCurrentWidth, _canvasSize.TrueCurrentHeight);
            _fringeTarget = _renderer.CreateRenderTarget(_canvasSize.TrueCurrentWidth, _canvasSize.TrueCurrentHeight);
            _finalTarget = _renderer.CreateRenderTarget(_canvasSize.TrueCurrentWidth, _canvasSize.TrueCurrentHeight);
            _colorGradeTarget = _renderer.CreateRenderTarget(_canvasSize.TrueCurrentWidth, _canvasSize.TrueCurrentHeight);
        }

        [System(RunOn.Draw)]
        public void Draw(GameTime gameTime, GameObjectList gameObjects)
        {
            DrawMask(ref _maskTarget, gameTime);
            DrawFringe(ref _fringeTarget, gameTime);

            _renderer.RequestSubRenderer(_finalTarget);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_maskTarget, Vector2.Zero, Color.White);
            _spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
                transformMatrix: _camera.Transform);
            DrawMaskParticles(gameTime);
            DrawEntities(gameObjects);
            _spriteBatch.End();

            _spriteBatch.Begin();
            _spriteBatch.Draw(_fringeTarget, Vector2.Zero, Color.White);
            _spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
                transformMatrix: _camera.Transform);
            DrawFringeParticles(gameTime);
            _spriteBatch.End();

            _renderer.RelinquishSubRenderer();

            _dayNightFilter.Filter(ref _colorGradeTarget, _spriteBatch, _finalTarget);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_colorGradeTarget, Vector2.Zero, Color.White);
            _spriteBatch.End();
        }

        private void DrawEntities(GameObjectList gameObjects)
        {
            var mapObjects = gameObjects.All(typeof(CSprite), typeof(CAnimation), typeof(CPosition));
            foreach (var mapObject in mapObjects)
            {
                var sprite = mapObject.Components.Get<CSprite>();
                var animation = mapObject.Components.Get<CAnimation>();
                var position = mapObject.Components.Get<CPosition>();

                var positionVec = position.Position;
                if (mapObject.Components.TryGet(out CSize size))
                    positionVec += (size.Size / 2);

                _spriteBatch.Draw(sprite.Texture, 
                    new Vector2(
                        (float)Math.Round(positionVec.X, MidpointRounding.AwayFromZero), 
                        (float)Math.Round(positionVec.Y, MidpointRounding.AwayFromZero)), 
                    animation.GetFrame(), Color.White, 0, sprite.Origin, Vector2.One, SpriteEffects.None, 0f);
            }
        }

        public void DrawMask(ref RenderTarget2D target, GameTime gameTime)
        {
            var mask = DrawTilemap(true);

            _waterFilter.Update(gameTime);
            _waterFilter.Filter(ref _waterTarget, _spriteBatch, mask);
            _waterReflectionFilter.Filter(ref _reflectionTarget, _spriteBatch, mask);

            _waterDisplacementFilter.SetTextures(_reflectionTarget);
            _waterDisplacementFilter.Update(gameTime);
            _waterDisplacementFilter.Filter(ref _displaceTarget, _spriteBatch, _displaceTarget);

            _renderer.RequestSubRenderer(target);
            _spriteBatch.Begin();
            _spriteBatch.Draw(_waterTarget, Vector2.Zero, Color.White);
            _spriteBatch.Draw(_displaceTarget, Vector2.Zero, Color.White * 0.45f);
            _spriteBatch.End();
            _renderer.RelinquishSubRenderer();
        }

        public void DrawMaskParticles(GameTime gameTime)
        {
            DrawParticles(gameTime, _overworld.GetMap().GetMaskEmitters(_camera));
        }

        public void DrawFringe(ref RenderTarget2D target, GameTime gameTime)
        {
            var fringe = DrawTilemap(false);

            _renderer.RequestSubRenderer(target);
            _spriteBatch.Begin();
            _spriteBatch.Draw(fringe, Vector2.Zero, Color.White);
            _spriteBatch.End();
            _renderer.RelinquishSubRenderer();
        }

        public void DrawFringeParticles(GameTime gameTime)
        {
            DrawParticles(gameTime, _overworld.GetMap().GetFringeEmitters(_camera));
        }

        [System(RunOn.Draw, typeof(CPosition), typeof(CCollisionBound))]
        public void DebugDrawCollidableEntities(GameTime gameTime, GameObjectList gameObjects)
        {
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
                transformMatrix: _camera.Transform);
            
            DrawQuadtree(_overworld.GetMap().MaskEmitters);

            foreach (var collider in _overworld.GetMap().GetCollisions(_camera))
                DebugDraw(collider);

            foreach (var script in _overworld.GetMap().GetScripts(_camera))
                DebugDraw(script);

            foreach (var zone in _overworld.GetMap().GetZones(_camera))
                DebugDraw(zone);

            foreach (var emitter in _overworld.GetMap().GetMaskEmitters(_camera))
                DrawEmitter(emitter);

            foreach (var emitter in _overworld.GetMap().GetFringeEmitters(_camera))
                DrawEmitter(emitter);

            foreach (var gameObject in gameObjects)
                DebugDraw(gameObject);

            _spriteBatch.End();
        }

        private void DrawParticles(GameTime gameTime, IEnumerable<IEmitter> emitters)
        {
            foreach (var emitter in emitters)
            {
                emitter.Update(gameTime);
                emitter.Draw(_spriteBatch, gameTime);
            }
        }

        private RenderTarget2D DrawTilemap(bool isMask)
        {
            _tileMapFilter.SetSuperTileset(_superTileset);
            _tileMapFilter.SetTransform(_camera.Transform);
            _tileMapFilter.SetMode(isMask 
                ? TilemapFilter.Mode.Mask
                : TilemapFilter.Mode.Fringe);
            _tileMapFilter.Filter(ref _tilemapRenderTarget, _spriteBatch, _tempTileMap);
            return _tilemapRenderTarget;
        }

        private void DrawQuadtree<T>(Quadtree<T> quadtree)
        {
            _primitive.DrawBox(quadtree.Boundaries.X, quadtree.Boundaries.Y, quadtree.Boundaries.Width,
                quadtree.Boundaries.Height, Color.White * 0.5f);

            foreach (var child in quadtree.GetChildren())
                DrawQuadtree(child);
        }

        private void DrawEmitter(IEmitter emitter)
        {
            _primitive.DrawBox(emitter.Boundaries.X, emitter.Boundaries.Y, emitter.Boundaries.Width,
                emitter.Boundaries.Height, Color.HotPink);
        }

        private void DebugDraw(GameObject entity)
        {
            var position = entity.Components.Get<CPosition>();
            var bounds = entity.Components.Get<CCollisionBound>();
            var boxColor = entity.Components.Has<CSolid>() ? Color.Green : Color.White;

            // draw a box around the inflated boundaries
            var iTL = position.Position + bounds.InflatedBounds.TopLeft;
            _primitive.DrawBox(iTL.X, iTL.Y, bounds.InflatedBounds.Width, bounds.InflatedBounds.Height, Color.Red);

            // draw a box around the actual boundaries
            var bTL = position.Position + bounds.Bounds.TopLeft;
            _primitive.DrawBox(bTL.X, bTL.Y, bounds.Bounds.Width, bounds.Bounds.Height, Color.Yellow);

            // draw the points
            _primitive.DrawPolygon(position.Position, bounds.Points, boxColor);

            // draw the center
            var bCentre = position.Position + bounds.Center;
            _primitive.DrawBox(bCentre.X, bCentre.Y, 1, 1, Color.White);
        }
    }
}
