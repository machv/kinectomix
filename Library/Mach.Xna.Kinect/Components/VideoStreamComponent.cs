using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Kinect;

namespace Mach.Xna.Kinect
{
    /// <summary>
    /// Type of stream from the Kinect sensor.
    /// </summary>
    public enum VideoStream
    {
        /// <summary>
        /// Color video stream.
        /// </summary>
        Color,
        /// <summary>
        /// Depth video stream.
        /// </summary>
        Depth
    }

    /// <summary>
    /// Shows video stream from the Kinect sensor.
    /// </summary>
    public class VideoStreamComponent : DrawableGameComponent
    {
        private Texture2D _videoFrame;
        private VideoStream _streamType;
        private float _renderingScale;
        private Vector2 _renderingOffset;
        private SpriteBatch _spriteBatch;
        private VisualKinectManager _chooser;
        private Game _game;
        private GraphicsDevice _graphicsDevice;
        private Rectangle _position;

        /// <summary>
        /// Gets current video frame from the Kinect sensor.
        /// </summary>
        /// <returns>Current video frame from the Kinect sensor.</returns>
        public Texture2D VideoFrame
        {
            get { return _videoFrame; }
        }
        /// <summary>
        /// Gets or sets <see cref="VideoStream"/> type used to display.
        /// </summary>
        /// <returns><see cref="VideoStream"/> type used to display.</returns>
        public VideoStream StreamType
        {
            get { return _streamType; }
            set { _streamType = value; }
        }
        /// <summary>
        /// Gets or sets in what scale should be video frames rendered.
        /// </summary>
        /// <returns>Scale which is used to render video frame.</returns>
        public float RenderingScale
        {
            get { return _renderingScale; }
            set { _renderingScale = value; }
        }
        /// <summary>
        /// Gets or sets the position where will be video frames rendered on the screen.
        /// </summary>
        /// <returns>Position where will be video frames rendered on the screen.</returns>
        public Vector2 RenderingOffset
        {
            get { return _renderingOffset; }
            set { _renderingOffset = value; }
        }

        /// <summary>
        /// Creates new instance of <see cref="VisualKinectManager"/>.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        /// <param name="chooser"><see cref="VisualKinectManager"/> which will be providing video stream.</param>
        public VideoStreamComponent(Game game, VisualKinectManager chooser)
            : base(game)
        {
            _game = game;
            _chooser = chooser;
            _graphicsDevice = game.GraphicsDevice;
        }

        /// <summary>
        /// Initializes required objects for this component.
        /// </summary>
        public override void Initialize()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            base.Initialize();
        }

        /// <summary>
        /// Updates data from the Kinect sensor for selected <see cref="VideoStream"/> type.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Update(GameTime gameTime)
        {
            if (_chooser.Sensor != null)
            {
                switch (StreamType)
                {
                    case VideoStream.Color:
                        if (_chooser.Sensor.ColorStream.IsEnabled)
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
                                        bgraPixelData[i + 3] = 255;
                                    }

                                    _videoFrame = new Texture2D(_graphicsDevice, colorVideoFrame.Width, colorVideoFrame.Height);
                                    _videoFrame.SetData(bgraPixelData);

                                    _position = new Rectangle((int)_renderingOffset.X, (int)_renderingOffset.Y, (int)(colorVideoFrame.Width * _renderingScale), (int)(colorVideoFrame.Height * _renderingScale));
                                }
                            }
                        }
                        break;
                    case VideoStream.Depth:
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

                                    _videoFrame = new Texture2D(_graphicsDevice, depthFrame.Width, depthFrame.Height);
                                    _videoFrame.SetData(colorPixels);

                                    _position = new Rectangle((int)_renderingOffset.X, (int)_renderingOffset.Y, (int)(depthFrame.Width * _renderingScale), (int)(depthFrame.Height * _renderingScale));
                                }
                            }
                        }
                        break;
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws current output from the Kinect sensor of selected <see cref="VideoStream"/> type.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        {
            if (_videoFrame != null)
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(_videoFrame, _position, Color.White);
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
