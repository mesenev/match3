using System;

namespace match
{
    public class Boulder
    {
        public float SPEED = 0.3f * Game1.GLOBAL_SPEED;

        public int X;
        public int Y;
        private int creationTime;
        public int Direction;

        public float Rotation { get { return (Environment.TickCount - creationTime) * 0.000001f; } }

        public Boulder (int x, int y, int dir)
        {
            X = x;
            Y = y;
            Direction = dir;
            creationTime = Environment.TickCount;

        }

        public int GetXcoord ()
        {
            float _ = X;
            if (Direction == 0)
                _ = X + (Environment.TickCount - creationTime) * SPEED;
            if (Direction == 2)
                _ = X - (Environment.TickCount - creationTime) * SPEED;
            if (_ > 800 || _ < Game1.INDENT)
                GameProcess.Boulders.Remove (this);
            return (int)Math.Round (_);

        }

        public int GetYcoord ()
        {
            float _ = Y;
            if (Direction == 1)
                _ = Y + (Environment.TickCount - creationTime) * SPEED;
            if (Direction == 3)
                _ = Y - (Environment.TickCount - creationTime) * SPEED;
            if (_ > 800 || _ < Game1.INDENT)
                GameProcess.Boulders.Remove (this);
            return (int)Math.Round (_);
        }
    }
}

