using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace odtwarzacz_audio_wpf {
	/// <summary>
	/// Interaction logic for WindowWebBrowser.xaml
	/// </summary>
	public partial class WindowWebBrowser : Window {
		public WindowWebBrowser() {
			InitializeComponent();
		}

		private void WebBrowserURLAddress_TextChanged(object sender, TextChangedEventArgs e) {

		}

		private void ButtonGo_Click(object sender, RoutedEventArgs e) {
			try {
				WebBrowser.Source = new Uri(WebBrowserURLAddress.Text);
				//WebBrowser.NavigateToString(WebBrowserURLAddress.Text+"hope");
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void WebBrowser_Loaded(object sender, RoutedEventArgs e) {
			//WebBrowser.NavigateToString("Paste this phrase in loaded websites: " + WebBrowserURLAddress.ToString());
			try {
				WebBrowser.NavigateToString(WebBrowserURLAddress.Text);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void Window_Closed(object sender, EventArgs e) {
			
		}
	}
}