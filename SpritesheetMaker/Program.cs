using System;
using System.Drawing;
using System.IO;
using System.Linq;
using static System.Console;

namespace SpritesheetMaker {

    public static class Program {

        private static int _width;
        private static int _height;

        private static void Main(string[] args) {
            if (args == null || args.Length == 0) return;

            FileAttributes attr0 = 0; //impossible value

            foreach (var arg in args) {
                var attr = File.GetAttributes(arg);

                if (attr0 == 0) {
                    attr0 = attr;
                    continue;
                }

                if (attr != attr0) {
                    throw new ArgumentException("The selection has to be only files or folder, not both.");
                }
            }

            WriteLine("Width? (0 for auto)");
            _width = Convert.ToInt32(ReadLine());

            WriteLine("Height? (0 for auto)");
            _height = Convert.ToInt32(ReadLine());

            //we know there are only folder or only files in args
            if ((attr0 & FileAttributes.Directory) == FileAttributes.Directory) {
                for (var i = 0; i < args.Length; i++) {
                    ProcessImages(new DirectoryInfo(args[i]).GetFiles().Select(f => new Bitmap(f.FullName)).ToArray(),
                        Path.Combine(
                            Path.GetDirectoryName(args[i]) ??
                            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                            Path.GetFileName(args[i]) + ".png"), i + 1);
                }
            } else {
                var dirName = Path.GetDirectoryName(args[0]);

                ProcessImages(args.Select(f => new Bitmap(f)).ToArray(),
                    Path.Combine(dirName ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                        (Path.GetFileName(dirName) ?? "result") + ".png"));
            }

            ReadLine();
        }

        private static void ProcessImages(Bitmap[] images, string resultFile, int folderProgress = -1) {
            if (folderProgress > -1) {
                WriteLine($"Folder {folderProgress} ({Path.GetFileNameWithoutExtension(resultFile)}):\n");
            }

            WriteLine("Finding the smallest rectangle which contains the image and the least transparency...");
            var rect = ImageHelper.FindMinRect(ref images);

            if (_width == 0) {
                _width = rect.Width;
            }

            if (_height == 0) {
                _height = rect.Height;
            }

            WriteLine("\nMaking the spritesheet...");
            var bmp = MakeSpritesheet(ref images, rect);

            bmp.Save(resultFile);

            WriteLine("Spritesheet created!\n");
            WriteLine("Filename: " + resultFile);

            if (folderProgress > -1) {
                WriteLine("\n----------------------\n");
            }
        }

        private static Bitmap MakeSpritesheet(ref Bitmap[] images, Rectangle rect) {
            var sqrt = Math.Sqrt(images.Length);
            var columnCount = (int) Math.Ceiling(sqrt);
            var lineCount = (images.Length - images.Length % columnCount) / columnCount;

            if (sqrt < columnCount && columnCount * lineCount < images.Length) {
                lineCount++;
            }

            var ret = new Bitmap(columnCount * _width, lineCount * _height);

            using (var g = Graphics.FromImage(ret)) {
                for (var j = 0; j < lineCount; j++) {
                    for (var i = 0; i < columnCount; i++) {
                        var curr = j * columnCount + i;
                        if (curr >= images.Length) break;

                        g.DrawImage(images[curr].GetRegion(rect), i * _width, j * _height, _width,
                            _height);
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