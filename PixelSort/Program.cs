using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace PixelSort
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Path.GetFullPath(args[0]);
            string directory = string.Join(".", path.Split('.').Where(p => !path.EndsWith(p)));
            string plain_filename = Path.GetFileNameWithoutExtension(path);

            Directory.CreateDirectory(directory);

            Bitmap bmp = new Bitmap(path);
            var pixels = ReadPixels(bmp);
            var ordered = pixels.OrderBy(p => CalculateLuminance(p.Value)).ToList();

            // set these two values

            int frame_rate = TryGetArgument(60, 1, args);
            int seconds = TryGetArgument(10, 2, args);

            int total_frames = frame_rate * seconds;

            double pixels_per_frame = ordered.Count / total_frames;

            int frames_saved = 0;

            int console_x = Console.CursorLeft;
            int console_y = Console.CursorTop;

            for(int x = 0; x < bmp.Width; x++)
            {
                for(int y = 0; y < bmp.Height; y++)
                {
                    int index = (x * bmp.Height) + y;
                    var point = ordered[index].Key;

                    while(((point.X * bmp.Height) + point.Y) < index)
                    {
                        // this keeps tracking previously-swapped pixels
                        point = ordered[((point.X * bmp.Height) + point.Y)].Key;
                    }

                    SwapPixels(bmp, new Point(x, y), point);

                    if(index % pixels_per_frame == 0)
                    {
                        bmp.Save(Path.Combine(directory, string.Format("{0}-{1}.png", plain_filename, frames_saved++)), System.Drawing.Imaging.ImageFormat.Png);
                        Console.SetCursorPosition(console_x, console_y);
                        Console.WriteLine("{0} out of {1} frames, {2:0.00}% done.", frames_saved, total_frames, (frames_saved / (double)total_frames) * 100);
                    }
                }
            }

            Console.WriteLine("Done.");
        }

        static int TryGetArgument(int def, int index, string[] args)
        {
            if(args.Count() > index)
            {
                int temp = 0;
                if (int.TryParse(args[index], out temp))
                    return temp;
            }

            return def;
        }

        static void SwapPixels(Bitmap bmp, Point first, Point second)
        {
            // no XOR autism here
            Color temp = bmp.GetPixel(first.X, first.Y);
            bmp.SetPixel(first.X, first.Y, bmp.GetPixel(second.X, second.Y));
            bmp.SetPixel(second.X, second.Y, temp);
        }

        static Dictionary<Point, Color> ReadPixels(Bitmap bmp)
        {
            var ret = new Dictionary<Point, Color>();

            for(int x = 0; x < bmp.Width; x++)
            {
                for(int y = 0; y < bmp.Height; y++)
                {
                    ret[new Point(x, y)] = bmp.GetPixel(x, y);
                }
            }

            return ret;
        }

        static double CalculateLuminance(Color color)
        {
            // ITU BT.601
            return (0.299 * color.R) + (0.587 * color.G) + (0.114 * color.B);
        }
    }

    struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
