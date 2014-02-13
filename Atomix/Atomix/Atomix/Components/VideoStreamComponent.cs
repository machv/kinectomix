using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Kinect;


namespace Atomix
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class VideoStreamComponent : DrawableGameComponent
    {
        private KinectChooser _chooser;
        Vector2 _offset;
        Game _game;
        GraphicsDeviceManager _graphics;
        Texture2D _colorVideo;

        public VideoStreamComponent(Game game, KinectChooser chooser, GraphicsDeviceManager graphics, Vector2 offset)
            : base(game)
        {
            _game = game;
            _chooser = chooser;
            _offset = offset;
            _graphics = graphics;
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
            if (_chooser.Sensor != null && _chooser.Sensor.ColorStream.IsEnabled)
            {
                using (ColorImageFrame colorVideoFrame = _chooser.Sensor.ColorStream.OpenNextFrame(0))
                {
                    if (colorVideoFrame != null)
                    {
                        // Create array for pixel data and copy it from the image frame
                        Byte[] pixelData = new Byte[colorVideoFrame.PixelDataLength];
                        colorVideoFrame.CopyPixelDataTo(pixelData);

                        // Convert RGBA to BGRA
                        Byte[] bgraPixelData = new Byte[colorVideoFrame.PixelDataLength];
                        for (int i = 0; i < pixelData.Length; i += 4)
                        {
                            bgraPixelData[i] = pixelData[i + 2];
                            bgraPixelData[i + 1] = pixelData[i + 1];
                            bgraPixelData[i + 2] = pixelData[i];
                            bgraPixelData[i + 3] = (Byte)255; // The video comes with 0 alpha so it is transparent
                        }

                        _colorVideo = new Texture2D(_graphics.GraphicsDevice, colorVideoFrame.Width, colorVideoFrame.Height);
                        _colorVideo.SetData(bgraPixelData);
                    }
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (_colorVideo != null)
            {
                spriteBatch.Begin();
                int scale = 2;
                spriteBatch.Draw(_colorVideo, new Rectangle((int)_offset.X, (int)_offset.Y, 640 / scale, 480 / scale), Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
