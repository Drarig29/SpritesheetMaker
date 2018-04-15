using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using static System.Console;

namespace SpritesheetMaker {

    public static class Program {

        private static void Main(string[] files) {
            if (files == null || files.Length == 0) return;

            WriteLine("Converting files to images...");
            var images = files.Select(f => new Bitmap(f)).ToArray();

            WriteLine("Finding the smallest rectangle which contains the image and the least transparency...");
            var rect = ImageHelper.FindMinRect(ref images);

            WriteLine("Making the spritesheet...");
            var bmp = MakeSpritesheet(images, rect);
            var dirName = Path.GetDirectoryName(files[0]);

            var filename = Path.Combine(dirName ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                (Path.GetFileName(dirName) ?? "result") + ".png");

            bmp.Save(filename);

            WriteLine("Spritesheet created!\n");
            WriteLine("Filename: " + filename);

            ReadLine();
        }

        private static Bitmap MakeSpritesheet(IReadOnlyList<Bitmap> images, Rectangle rect) {
            var sqrt = Math.Sqrt(images.Count);
            var countX = (int) Math.Ceiling(sqrt);
            var countY = (images.Count - images.Count % countX) / countX + (sqrt < countX ? 1 : 0);

            var ret = new Bitmap(countX * rect.Width, countY * rect.Height);

            using (var g = Graphics.FromImage(ret)) {
                for (var j = 0; j < countY; j++) {
                    for (var i = 0; i < countX; i++) {
                        g.DrawImage(images[j * countX + i].GetRegion(rect), i * rect.Width, j * rect.Height, rect.Width,
                            rect.Height);
                    }
                }
            }

            return ret;
        }

        public static void DrawTextProgressBar(int progress, int total) {
            CursorLeft = 0;
            Write("[");
            CursorLeft = 4;
            Write("%]");

            var value = (int) (100 * ((double) progress / total));

            if (progress < total) {
                CursorLeft = 1;
                Write(value);
            } else {
                CursorLeft = 0;
                WriteLine($"[{value}%]");
            }
        }
    }
}