﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace SpritesheetMaker {

    public static class Program {

        private static void Main(string[] files) {
            if (files == null || files.Length == 0) return;

            var images = files.Select(f => new Bitmap(f)).ToArray();
            var rect = ImageHelper.FindMinRect(ref images);

            for (var i = 0; i < images.Length; i++) {
                images[i] = ImageHelper.Crop(images[i], rect);
            }

            var bmp = MakeSpritesheet(images, rect);
            var dirName = Path.GetDirectoryName(files[0]);

            var filename = Path.Combine(dirName ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                (Path.GetFileName(dirName) ?? "result") + ".png");

            bmp.Save(filename);

            Console.WriteLine($@"Le fichier {filename} a été créé.");
            Console.ReadLine();
        }

        private static Bitmap MakeSpritesheet(IReadOnlyList<Bitmap> images, Rectangle rect) {
            var sqrt = Math.Sqrt(images.Count);
            var countX = (int) Math.Ceiling(sqrt);
            var countY = (images.Count - images.Count % countX) / countX + (sqrt < countX ? 1 : 0);

            var ret = new Bitmap(countX * rect.Width, countY * rect.Height);

            using (var g = Graphics.FromImage(ret)) {
                for (var j = 0; j < countY; j++) {
                    for (var i = 0; i < countX; i++) {
                        g.DrawImage(images[j * countX + i], i * rect.Width, j * rect.Height, rect.Width, rect.Height);
                    }
                }
            }

            return ret;
        }
    }
}