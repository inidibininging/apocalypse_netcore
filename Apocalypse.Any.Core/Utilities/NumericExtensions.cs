using Apocalypse.Any.Core.Behaviour;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Core.Utilities
{
    public static class NumericExtensions
    {
        public static double ToRadians(this double val)
        {
            return (Math.PI / 180) * val;
        }

        public static IEnumerable<Vector2> DrawLineOfVectors(this Vector2 a, Vector2 b)
        {
            var x0 = (int)MathF.Round(a.X);
            var y0 = (int)MathF.Round(a.Y);
            var x1 = (int)MathF.Round(b.X);
            var y1 = (int)MathF.Round(b.Y);

            bool steep = MathF.Abs(y1 - y0) > MathF.Abs(x1 - x0);
            if (steep)
            {
                int t;
                t = x0; // swap x0 and y0
                x0 = y1;
                y0 = t;
                t = x1; // swap x1 and y1
                x1 = y1;
                y1 = t;
            }
            if (x0 > x1)
            {
                int t;
                t = x0; // swap x0 and x1
                x0 = x1;
                x1 = t;
                t = y0; // swap y0 and y1
                y0 = y1;
                y1 = t;
            }
            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                yield return new Vector2((steep ? y : x), (steep ? x : y));
                error = error - dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
            yield break;
        }

        //Copy Monkey: https://stackoverflow.com/questions/2049582/how-to-determine-if-a-point-is-in-a-2d-triangle
        public static bool PointInTriangle(Vector2 p, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            var s = p0.Y * p2.X - p0.X * p2.Y + (p2.Y - p0.Y) * p.X + (p0.X - p2.X) * p.Y;
            var t = p0.X * p1.Y - p0.Y * p1.X + (p0.Y - p1.Y) * p.X + (p1.X - p0.X) * p.Y;

            if ((s < 0) != (t < 0))
                return false;

            var A = -p1.Y * p2.X + p0.Y * (p2.X - p1.X) + p0.X * (p1.Y - p2.Y) + p1.X * p2.Y;
            if (A < 0.0)
            {
                s = -s;
                t = -t;
                A = -A;
            }
            return s > 0 && t > 0 && (s + t) <= A;
        }

        public static float ToOppositeAngle(this float val)
        {
            return ((val + 180) % 360);
        }

        public static float ToRadians(this float val)
        {
            return (float)((Math.PI / 180) * val);
        }

        public static TimeSpan Seconds(this int val)
        {
            return TimeSpan.FromSeconds(val);
        }

        public static TimeSpan Minutes(this int val)
        {
            return TimeSpan.FromMinutes((double)Convert.ChangeType(val, TypeCode.Double));
        }

        public static TimeSpan Hours(this int val)
        {
            return TimeSpan.FromHours(val);
        }

        public static TimeSpan Days(this int val)
        {
            return TimeSpan.FromDays(val);
        }

        public static TimeSpan Seconds(this double val) => TimeSpan.FromSeconds(val);

        public static TimeSpan Hours(this double val) => TimeSpan.FromHours(val);

        public static TimeSpan Minutes(this double val) => TimeSpan.FromMinutes(val);

        public static TimeSpan Days(this double val) => TimeSpan.FromDays(val);

        public static IUpdateableLite Do(this TimeSpan val, Action action, bool loop = false) => new GameTimeTimer(val) { Loop = loop }.Do(action);

        //public static void AddTimeBehaviour(this IGameObjectDictionary subject, TimeSpan time,Action action)
        //{
        //    subject.Add($"{Guid.NewGuid().ToString()}_GameTimeTimerBehaviourWrapper",
        //                new UpdateableBehaviourWrapper(new GameTimeTimer(time).Do(action)));
        //}

        public static void AddUpdateable(this IGameObjectDictionary subject, IUpdateableLite updateable)
        {
            subject.Add($"{Guid.NewGuid().ToString()}_UpdateableBehaviourWrapper",
                        new UpdateableBehaviourWrapper(updateable));
        }
    }
}