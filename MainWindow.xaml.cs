using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace odtwarzacz_audio_wpf {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow: Window {
		public MainWindow() {
			InitializeComponent();
		}

		private void playlistListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {

		}

        private void button1_Click(object sender, System.EventArgs e) {
            Stream myStream = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    if ((myStream = openFileDialog.OpenFile()) != null) {
                        using (myStream) {
                            // Insert code to read the stream here.
                        }
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show("Error: Could not read the file from disk. Original error: " + ex.Message);
                }
            }
        }
    }
}
