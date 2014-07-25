using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Mach.Xna
{
    /// <summary>
    /// Allows drawing of the animated texture.
    /// </summary>
    public class AnimatedTexture
    {
        private int _frameCount;
        private Texture2D _texture;
        private float _timePerFrame;
        private int _frameIndex;
        private double _totalElapsed;
        private bool _isPaused;
        private float _rotation;
        private float _scale;
        private Vector2 _origin;

        /// <summary>
        /// Gets the current frame index.
        /// </summary>
        /// <value>
        /// The index of the current frame.
        /// </value>
        public int Frame
        {
            get { return _frameIndex; }
        }
        /// <summary>
        /// Gets or sets the angle (in radians) to rotate the sprite about its center..
        /// </summary>
        /// <value>
        /// The angle (in radians) to rotate the sprite about its center.
        /// </value>
        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }
        /// <summary>
        /// Gets or sets the scale factor.
        /// </summary>
        /// <value>
        /// The scale factor.
        /// </value>
        public float Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }
        /// <summary>
        /// Gets or sets the sprite origin; the default is (0,0) which represents the upper-left corner.
        /// </summary>
        /// <value>
        /// The sprite origin; the default is (0,0) which represents the upper-left corner.
        /// </value>
        public Vector2 Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }
        /// <summary>
        /// Gets a value indicating whether the animation is paused.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the animation is paused; otherwise, <c>false</c>.
        /// </value>
        public bool IsPaused
        {
            get { return _isPaused; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedTexture"/> class.
        /// </summary>
        /// <param name="origin">The sprite origin; the default is (0,0) which represents the upper-left corner.</param>
        /// <param name="rotation">Specifies the angle (in radians) to rotate the sprite about its center.</param>
        /// <param name="scale">Scale factor.</param>
        public AnimatedTexture(Vector2 origin, float rotation, float scale)
        {
            Origin = origin;
            Rotation = rotation;
            Scale = scale;
        }
        /// <summary>
        /// Loads the specified asset from the <see cref="ContentManager"/>.
        /// </summary>
        /// <param name="content">The content manager.</param>
        /// <param name="asset">The name of the asset to load.</param>
        /// <param name="frameCount">How many frames does this asset have.</param>
        /// <param name="framesPerSec">The frames per sec.</param>
        public void Load(ContentManager content, string asset, int frameCount, int framesPerSec)
        {
            Texture2D texture = content.Load<Texture2D>(asset);

            Load(texture, frameCount, framesPerSec);
        }

        /// <summary>
        /// Loads the specified texture.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="frameCount">How many frames does this asset have.</param>
        /// <param name="framesPerSec">The frames per sec.</param>
        public void Load(Texture2D texture, int frameCount, int framesPerSec)
        {
            _frameCount = frameCount;
            _texture = texture;
            _timePerFrame = (float)1 / framesPerSec;
            _frameIndex = 0;
            _totalElapsed = 0;
            _isPaused = false;
        }

        /// <summary>
        /// Updates the frame by the elapsed time.
        /// </summary>
        /// <param name="elapsed">The elapsed.</param>
        public void UpdateFrame(double elapsed)
        {
            if (_isPaused)
                return;

            _totalElapsed += elapsed;
            if (_totalElapsed > _timePerFrame)
            {
                _frameIndex++;
                _frameIndex = _frameIndex % _frameCount;
                _totalElapsed -= _timePerFrame;
            }
        }

        /// <summary>
        /// Draws the frame at specified position on the screen.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch that will be used for rendering.</param>
        /// <param name="screenPosition">The position on the screen.</param>
        public void DrawFrame(SpriteBatch spriteBatch, Vector2 screenPosition)
        {
            DrawFrame(spriteBatch, _frameIndex, screenPosition);
        }

        /// <summary>
        /// Draws the selected frame at specified position on the screen.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch that will be used for rendering.</param>
        /// <param name="frame">The frame index to draw.</param>
        /// <param name="screenPososition">The screen position.</param>
        public void DrawFrame(SpriteBatch spriteBatch, int frame, Vector2 screenPososition)
        {
            int frameWidth = _texture.Width / _frameCount;
            Rectangle sourcerect = new Rectangle(frameWidth * frame, 0, frameWidth, _texture.Height);
            spriteBatch.Draw(_texture, screenPososition, sourcerect, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Resets the animation.
        /// </summary>
        public void Reset()
        {
            _frameIndex = 0;
            _totalElapsed = 0f;
        }

        /// <summary>
        /// Stops the animation.
        /// </summary>
        public void Stop()
        {
            Pause();
            Reset();
        }

        /// <summary>
        /// Resumes animation.
        /// </summary>
        public void Play()
        {
            _isPaused = false;
        }

        /// <summary>
        /// Pauses animation.
        /// </summary>
        public void Pause()
        {
            _isPaused = true;
        }
    }
}
