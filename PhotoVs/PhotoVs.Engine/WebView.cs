using ImpromptuNinjas.UltralightSharp.Enums;
using ImpromptuNinjas.UltralightSharp.Safe;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Graphics;
using PhotoVs.Engine.Graphics.Filters;
using Renderer = ImpromptuNinjas.UltralightSharp.Safe.Renderer;

namespace PhotoVs.Engine
{
    public class WebView
    {
        private Renderer _renderer;
        private Session _session;
        private View _view;

        private IRenderer _rn;
        private byte[] _pixels;
        private RenderTarget2D _renderTarget;
        private RenderTarget2D _finalRenderTarget;

        public Texture2D Texture
        {
            get
            {
                Update();
                return _finalRenderTarget;
            }
        }

        private SimpleFilter _filter;
        private readonly SpriteBatch _spriteBatch;

        public WebView(IRenderer renderer, IAssetLoader assetLoader, SpriteBatch spriteBatch, int width, int height, string homePage)
        {
            _rn = renderer;
            _spriteBatch = spriteBatch;
            _filter = new SimpleFilter(renderer, spriteBatch, assetLoader.Get<Effect>("shaders/bgra_to_rgba.fx"));

            InitialiseUltralight();

            Resize(width, height);
            Navigate(homePage);
        }

        private void InitialiseUltralight()
        {
            AppCore.EnablePlatformFontLoader();
            AppCore.EnablePlatformFileSystem("content");

            _renderer = new Renderer(Config());
            _session = new Session(_renderer, false, "pvr");
            _view = new View(_renderer, 800, 600, true, _session);
        }

        private Config Config()
        {
            var config = new Config();
            config.SetCachePath("cache");
            config.SetResourcePath("resources");
            config.SetUseGpuRenderer(false);
            config.SetEnableImages(true);
            config.SetEnableJavaScript(true);
            config.SetDeviceScale(1);
            config.SetFontHinting(FontHinting.Monochrome);

            return config;
        }

        public void Resize(int width, int height)
        {
            _view.Resize((uint) width, (uint) height);

            _renderTarget?.Dispose();
            _renderTarget = null;

            _finalRenderTarget?.Dispose();
            _finalRenderTarget = null;

            _renderTarget = _rn.CreateRenderTarget(width, height);
            _finalRenderTarget = _rn.CreateRenderTarget(width, height);
        }

        public void Navigate(string uri)
        {
            _view.LoadUrl(uri);
            _view.Focus();
        }

        private void Update()
        {
            _renderer.Update();
            _renderer.Render();

            var surface = _view.GetSurface();
            var dirtyBounds = surface.GetDirtyBounds();
            if (!dirtyBounds.IsEmpty())
            {
                surface.ClearDirtyBounds();

                var bitmap = surface.GetBitmap();
                var pixels = bitmap.LockPixels();
                var size = (int) (bitmap.GetWidth() * bitmap.GetHeight() * bitmap.GetBpp());

                if (_pixels == null || _pixels.Length != size)
                    _pixels = new byte[size];

                /*unsafe
            {
                fixed (byte* dest = &_pixels[0])
                {
                    // srcPtr and destPtr are IntPtr's pointing to valid memory locations
                    // size is the number of long (normally 4 bytes) to copy
                    byte* src = (long*) pixels;
                    for (int i = 0; i < size / sizeof(long); i++)
                    {
                        dest[i] = src[i];
                    }
                }
            }*/

                unsafe
                {
                    fixed (void* dest = &_pixels[0])
                    {
                        System.Buffer.MemoryCopy(pixels.ToPointer(), dest, size, size);
                    }
                }

                //Marshal.Copy(pixels, _pixels, 0, size);
                bitmap.UnlockPixels();

                _renderTarget.SetData(_pixels);
                _filter.Filter(ref _finalRenderTarget, _spriteBatch, _renderTarget);
            }
        }
    }
}