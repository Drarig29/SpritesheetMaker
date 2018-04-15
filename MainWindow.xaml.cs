using System.IO;
using System.Linq;
using System.Windows;

namespace SpritesheetMaker {

    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
        }

        private void FileList_OnDrop_Drop(object sender, DragEventArgs e) {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) {
                MessageBox.Show("This is not a good format!", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var files = (e.Data.GetData(DataFormats.FileDrop) as string[])?.Select(Path.GetFileName)
                .OrderBy(file => file).ToArray();

            if (files == null || files.Any(file => !file.Contains(".png"))) {
                MessageBox.Show("Invalid files", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            FileList.ItemsSource = files;
        }

        private void MakeButton_OnClick(object sender, RoutedEventArgs e) {
            
        }
    }
}