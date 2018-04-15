using System;
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

            MakeButton_OnClick(null, null);
        }

        private void MakeButton_OnClick(object sender, RoutedEventArgs e) {
            Console.WriteLine(ImageHelper.FindMinRect(_files.Select(f => new Bitmap(f))));
        }
    }
}