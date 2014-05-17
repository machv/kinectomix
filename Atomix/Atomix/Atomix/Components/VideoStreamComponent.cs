using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Kinect;

namespace Atomix
{
    public enum VideoType { Color, Depth }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class VideoStreamComponent : DrawableGameComponent
    {
        private KinectChooser _chooser;
        Vector2 _offset;
        float _scale;
        Game _game;
        GraphicsDeviceManager _graphics;
        Texture2D _colorVideo;

        public VideoType Type { get; set; }

        public VideoStreamComponent(Game game, KinectChooser chooser, GraphicsDeviceManager graphics, Vector2 offset, float scale)
            : base(game)
        {
            _game = game;
            _chooser = chooser;
            _offset = offset;
            _scale = scale;
            _graphics = graphics;
        }

        public float Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public Vector2 RenderOffset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        /// <summary>
        /// The SpriteBatch used for rendering.
        /// </summary>
        private SpriteBatch spriteBatch;

        public override void Initialize()
        {
            base.Initialize();

            this.spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (_chooser.Sensor != null)
            {
                switch (Type)
                {
                    case VideoType.Color:
                        if(_chooser.Sensor.ColorStream.IsEnabled)
                        {
                            using (ColorImageFrame colorVideoFrame = _chooser.Sensor.ColorStream.OpenNextFrame(0))
                            {
                                if (colorVideoFrame != null)
                                {
                                    // Create array for pixel data and copy it from the image frame
                                    byte[] pixelData = new byte[colorVideoFrame.PixelDataLength];
                                    colorVideoFrame.CopyPixelDataTo(pixelData);

                                    // Convert RGBA to BGRA
                                    byte[] bgraPixelData = new byte[colorVideoFrame.PixelDataLength];
                                    for (int i = 0; i < pixelData.Length; i += 4)
                                    {
                                        bgraPixelData[i] = pixelData[i + 2];
                                        bgraPixelData[i + 1] = pixelData[i + 1];
                                        bgraPixelData[i + 2] = pixelData[i];
                                        bgraPixelData[i + 3] = 255; // The video comes with 0 alpha so it is transparent
                                    }

                                    _colorVideo = new Texture2D(_graphics.GraphicsDevice, colorVideoFrame.Width, colorVideoFrame.Height);
                                    _colorVideo.SetData(bgraPixelData);
                                }
                            }
                        }
                        break;
                    case VideoType.Depth:
                        if (_chooser.Sensor.DepthStream.IsEnabled)
                        {
                            using (DepthImageFrame depthFrame = _chooser.Sensor.DepthStream.OpenNextFrame(0))
                            {
                                if (depthFrame != null)
                                {
                                    DepthImagePixel[] depthPixels = new DepthImagePixel[_chooser.Sensor.DepthStream.FramePixelDataLength];
                                    byte[] colorPixels = new byte[_chooser.Sensor.DepthStream.FramePixelDataLength * sizeof(int)];

                                    // Copy the pixel data from the image to a temporary array
                                    depthFrame.CopyDepthImagePixelDataTo(depthPixels);

                                    // Get the min and max reliable depth for the current frame
                                    int minDepth = depthFrame.MinDepth;
                                    int maxDepth = depthFrame.MaxDepth;

                                    // Convert the depth to BGRA
                                    int colorPixelIndex = 0;
                                    for (int i = 0; i < depthPixels.Length; ++i)
                                    {
                                        // Get the depth for this pixel
                                        short depth = depthPixels[i].Depth;

                                        // To convert to a byte, we're discarding the most-significant
                                        // rather than least-significant bits.
                                        // We're preserving detail, although the intensity will "wrap."
                                        // Values outside the reliable depth range are mapped to 0 (black).

                                        // Note: Using conditionals in this loop could degrade performance.
                                        // Consider using a lookup table instead when writing production code.
                                        // See the KinectDepthViewer class used by the KinectExplorer sample
                                        // for a lookup table example.
                                        byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);

                                        // Write out blue byte
                                        colorPixels[colorPixelIndex++] = intensity;

                                        // Write out green byte
                                        colorPixels[colorPixelIndex++] = intensity;

                                        // Write out red byte                        
                                        colorPixels[colorPixelIndex++] = intensity;

                                        // Write alpha byte
                                        colorPixels[colorPixelIndex++] = 255;
                                    }

                                    _colorVideo = new Texture2D(_graphics.GraphicsDevice, depthFrame.Width, depthFrame.Height);
                                    _colorVideo.SetData(colorPixels);
                                }
                            }
                        }
                        break;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (_colorVideo != null)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(_colorVideo, new Rectangle((int)_offset.X, (int)_offset.Y, (int)(640 / _scale), (int)(480 / _scale)), Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
