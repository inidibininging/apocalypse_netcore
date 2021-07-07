using System;
using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Core.Utilities;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Client.Services
{
    public class PlanetBitmapService
    {
        private List<Vector2> GeneratedDots = new();

        public PlanetBitmapService()
        {
            DefaultRiverPalette = RollTheDice(3) ? "getRiverBlue" : "getDevilRed";
            DefaultLandscapePalette =
                RollTheDice(3) ? "getUnison" : Randomness.Instance.TrueOrFalse() ? "getGreen" : "getDirt";
            Thickness = Randomness.Instance.From(1, 10);
        }

        public Dictionary<string, Func<Color>> PaletteGenerators { get; set; } = new();
        public int Radius { get; set; }

        public float DiameterOutside => Radius / 2f;

        public float DiameterSquaredOutside => DiameterOutside * DiameterOutside;

        public float DiameterInside => (Radius - Thickness) / 2f;

        public float DiameterSquaredInside => DiameterInside * DiameterInside;

        public int Thickness { get; set; } = 1;
        public string DefaultRiverPalette { get; set; }
        public string DefaultLandscapePalette { get; set; }

        private int GetPixelIndex(int x, int y)
        {
            return y * Radius + x;
        }

        private Vector2 GetVector(int x, int y)
        {
            return new(x - MathF.Abs(DiameterOutside),
                y - MathF.Abs(DiameterOutside));
        }

        public Color[] GetPixels()
        {
            var pixels = new Color[Radius * Radius];
            var atmosphereColor = new Color
            {
                R = (byte) Randomness.Instance.From(20, 255),
                G = (byte) Randomness.Instance.From(20, 255),
                B = (byte) Randomness.Instance.From(20, 255),
                A = (byte) Randomness.Instance.From(10, 255)
            };
            var shiningPoint = Randomness.Instance.From(0, Radius);
            for (var y = 0; y < Radius; y++)
            for (var x = 0; x < Radius; x++)
            {
                var pixelIndex = GetPixelIndex(x, y);
                var circlePos = GetVector(x, y);

                //this is outside the circle
                if (circlePos.LengthSquared() <= DiameterSquaredInside)
                {
                    pixels[pixelIndex] = ProcessInline(x, y);
                    if (RollTheDice(2) && x > shiningPoint - (shiningPoint + Randomness.Instance.From(10, 80)) &&
                        x < shiningPoint + Randomness.Instance.From(10, 80) &&
                        y > shiningPoint - (shiningPoint + Randomness.Instance.From(10, 80)) &&
                        y < shiningPoint + Randomness.Instance.From(10, 80))
                        pixels[pixelIndex] = AddMask(pixels[pixelIndex], (byte) Randomness.Instance.From(20, 60));
                    continue;
                }

                if (circlePos.LengthSquared() <= DiameterSquaredOutside)
                {
                    pixels[pixelIndex] = atmosphereColor;
                    // pixels[pixelIndex].R -= 40;
                    // pixels[pixelIndex].G -= 40;
                    // pixels[pixelIndex].B -= 40;
                    continue;
                }

                // outline
                pixels[pixelIndex] = Color.Transparent;
            }

            return Reprocess(pixels);
        }

        private Color AddMask(Color color, byte val)
        {
            color.R += val;
            color.G += val;
            color.B += val;
            return color;
        }

        private IEnumerable<Vector2> GenerateALine()
        {
            var values = Randomness.Instance.From(2, 5 * 10);
            for (var i = 0; i < values; i++)
                yield return new Vector2(Randomness.Instance.From(0, Radius - Randomness.Instance.From(0, Radius / 2)),
                    Randomness.Instance.From(0,
                        Radius - Randomness.Instance.From(0, Radius / 2)));
        }

        //https://www.geeksforgeeks.org/bresenhams-line-generation-algorithm/
        private List<Vector2> Bresenham(int x1, int y1, int x2, int y2)
        {
            var linePoints = new List<Vector2>();
            var m_new = 2 * (y2 - y1);
            var slope_error_new = m_new - (x2 - x1);

            for (int x = x1, y = y1; x <= x2; x++)
            {
                //Console.Write("(" + x + "," + y + ")\n");
                linePoints.Add(new Vector2(x, y));
                // Add slope to increment angle formed
                slope_error_new += m_new;

                // Slope error reached limit, time to
                // increment y and update slope error.
                if (slope_error_new >= 0)
                {
                    y++;
                    slope_error_new -= 2 * (x2 - x1);
                }
            }

            return linePoints;
        }

        //CUDOOOOS TO THIS ONE: http://ericw.ca/notes/bresenhams-line-algorithm-in-csharp.html
        private IEnumerable<Point> GetPointsOnLine(int x0, int y0, int x1, int y1)
        {
            var steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                int t;
                t = x0; // swap x0 and y0
                x0 = y0;
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

            var dx = x1 - x0;
            var dy = Math.Abs(y1 - y0);
            var error = dx / 2;
            var ystep = y0 < y1 ? 1 : -1;
            var y = y0;
            for (var x = x0; x <= x1; x++)
            {
                yield return new Point(steep ? y : x, steep ? x : y);
                error = error - dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
        }

        private Color ProcessInline(int x, int y)
        {
            if (GeneratedDots.Count == 0)
                GeneratedDots = GenerateALine().ToList();
            //GeneratedDots.ForEach(lp => //Console.WriteLine($"{lp.X};{lp.Y}"));

            var currentCursor = 0;
            if (GeneratedDots.Count() % 2 > 0)
                //Console.WriteLine("UPS");
                while (currentCursor + 1 < GeneratedDots.Count())
                {
                    currentCursor += 1;

                    var firstX = GeneratedDots.ElementAt(currentCursor - 1);
                    var secondX = GeneratedDots.ElementAt(currentCursor);

                    var x1 = (int) MathF.Abs(firstX.X);
                    var x2 = (int) MathF.Abs(secondX.X);

                    var y1 = (int) MathF.Abs(firstX.Y);
                    var y2 = (int) MathF.Abs(secondX.Y);

                    if (GetPointsOnLine(x1, y1, x2, y2).Any(v =>
                        v.X + 12 > x &&
                        v.X - 12 < x
                        &&
                        v.Y + 12 > y &&
                        v.Y - 12 < y && Randomness.Instance.TrueOrFalse()
                    ))
                        return PaletteGenerators[DefaultRiverPalette]();

                    firstX = Vector2.Zero;
                    secondX = Vector2.Zero;
                }

            if (RollTheDice(10))
                return RollTheDice(5) ? PaletteGenerators[DefaultLandscapePalette]() :
                    Randomness.Instance.TrueOrFalse() ? PaletteGenerators["getUnison"]() :
                    PaletteGenerators["getYellowish"]();
            return PaletteGenerators[DefaultLandscapePalette]();
        }

        private bool RollTheDice(int times)
        {
            var dices = new List<bool>();
            for (var diceTimes = 0; diceTimes < times; diceTimes++) dices.Add(Randomness.Instance.TrueOrFalse());
            return dices.All(dice => dice);
        }

        private Color ProcessOutline(int x, int y)
        {
            return Color.Transparent;
        }

        private Color[] Reprocess(Color[] pixels)
        {
            var chooseChannel = Randomness.Instance.From(0, 200);
            var randomMask = Randomness.Instance.From(0, 90);

            if (chooseChannel > 50 && chooseChannel < 100)
                return pixels.Select(pixel =>
                {
                    if (pixel == Color.Transparent)
                        return pixel;
                    pixel.R += (byte) randomMask;
                    pixel.G += (byte) (randomMask / 2);
                    return pixel;
                }).ToArray();
            if (chooseChannel > 100 && chooseChannel < 150)
                return pixels.Select(pixel =>
                {
                    if (pixel == Color.Transparent)
                        return pixel;
                    pixel.G += (byte) randomMask;
                    pixel.B += (byte) (randomMask / 2);
                    return pixel;
                }).ToArray();
            if (chooseChannel > 150 && chooseChannel < 200)
                return pixels.Select(pixel =>
                {
                    if (pixel == Color.Transparent)
                        return pixel;
                    pixel.B += (byte) randomMask;
                    pixel.R += (byte) (randomMask / 2);
                    return pixel;
                }).ToArray();
            return pixels;
        }
    }
}