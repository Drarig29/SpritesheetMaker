using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;

namespace SpritesheetMaker {

    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
        }

        private string[] _files;

        private void FileList_OnDrop(object sender, DragEventArgs e) {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) {
                MessageBox.Show("This is not a good format!", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _files = (e.Data.GetData(DataFormats.FileDrop) as string[])?.OrderBy(file => file).ToArray();

            if (_files == null || _files.Any(file => !file.Contains(".png"))) {
                MessageBox.Show("Invalid files", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            FileList.ItemsSource = _files;
        }

        private Bitmap MakeSpritesheet(IReadOnlyList<Bitmap> images, Rectangle rect) {
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

        private void MakeButton_OnClick(object sender, RoutedEventArgs e) {
            if (_files == null || _files.Length == 0) return;

            var images = _files.Select(f => new Bitmap(f)).ToArray();
            var rect = ImageHelper.FindMinRect(ref images);

            for (var i = 0; i < images.Length; i++) {
                images[i] = ImageHelper.Crop(images[i], rect);
            }

            var bmp = MakeSpritesheet(images, rect);
            var dirName = Path.GetDirectoryName(_files[0]);

            var filename = Path.Combine(dirName ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                (Path.GetFileName(dirName) ?? "result") + ".png");

            bmp.Save(filename);
        }
    }
}