using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Mach.Xna
{
    public class AnimatedTexture
    {
        private int framecount;
        private Texture2D myTexture;
        private float TimePerFrame;
        private int frame;
        private double TotalElapsed;
        private bool Paused;

        public int Frame { get { return frame; } }

        public float Rotation, Scale;
        public Vector2 Origin;
        public AnimatedTexture(Vector2 origin, float rotation, float scale)
        {
            Origin = origin;
            Rotation = rotation;
            Scale = scale;
        }
        public void Load(ContentManager content, string asset, int frameCount, int framesPerSec)
        {
            Texture2D texture = content.Load<Texture2D>(asset);

            Load(texture, frameCount, framesPerSec);
        }

        public void Load(Texture2D texture, int frameCount, int framesPerSec)
        {
            framecount = frameCount;
            myTexture = texture;
            TimePerFrame = (float)1 / framesPerSec;
            frame = 0;
            TotalElapsed = 0;
            Paused = false;
        }

        // class AnimatedTexture
        public void UpdateFrame(double elapsed)
        {
            if (Paused)
                return;

            TotalElapsed += elapsed;
            if (TotalElapsed > TimePerFrame)
            {
                frame++;
                // Keep the Frame between 0 and the total frames, minus one.
                frame = frame % framecount;
                TotalElapsed -= TimePerFrame;
            }
        }

        // class AnimatedTexture
        public void DrawFrame(SpriteBatch batch, Vector2 screenPos)
        {
            DrawFrame(batch, frame, screenPos);
        }
        public void DrawFrame(SpriteBatch batch, int frame, Vector2 screenPos)
        {
            int FrameWidth = myTexture.Width / framecount;
            Rectangle sourcerect = new Rectangle(FrameWidth * frame, 0,
                FrameWidth, myTexture.Height);
            batch.Draw(myTexture, screenPos, sourcerect, Color.White,
                Rotation, Origin, Scale, SpriteEffects.None, 0);
        }

        public bool IsPaused
        {
            get { return Paused; }
        }
        public void Reset()
        {
            frame = 0;
            TotalElapsed = 0f;
        }
        public void Stop()
        {
            Pause();
            Reset();
        }
        public void Play()
        {
            Paused = false;
        }
        public void Pause()
        {
            Paused = true;
        }
    }
}
