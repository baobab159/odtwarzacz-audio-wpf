using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace odtwarzacz_audio_wpf {
	public partial class WindowSuperMusicPlayer: Window {
        String[] files, paths;
        private MediaPlayer mediaPlayer = new MediaPlayer();
        bool loadedPlaylist = false;
        int trackIndex, songDuration, MediaPlayerSliderValue; 

        public WindowSuperMusicPlayer() {
			InitializeComponent();
            try {
                videoPlayer.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\nature on boat.mp4");  //chooses eye keeper source

                videoPlayer.ScrubbingEnabled = true;    //shows video preview
                videoPlayer.Play();
                videoPlayer.Stop();
                
                ButtonPlay.Visibility = Visibility.Visible;
                ButtonPause.Visibility = Visibility.Hidden;
            }
            catch (FileNotFoundException e) {
                MessageBox.Show($"The music visualiser file was not found: '{e}'");
            }
        }

        private void ButtonSelectSongs_Click(object sender, RoutedEventArgs e) {
            selectSongs();
        }

        private void playNextTrack() {
            if (trackIndex < PlaylistListBox.Items.Count - 1) {
                trackIndex += 1;
                mediaPlayer.Open(new Uri(paths[trackIndex]));
                mediaPlayer.Play();
                TextBlockSongName.Text = PlaylistListBox.Items[trackIndex].ToString();
                videoPlayer.Position = TimeSpan.FromSeconds(1);
                ButtonNext.IsEnabled = true;
            } else {
                ButtonNext.Opacity = 0.5;        //sets button transparency due to end of the playlist
                ButtonNext.IsEnabled = false;
            }
        }

        private void playPreviousTrack() {
            if (trackIndex < PlaylistListBox.Items.Count - 1) {
                trackIndex -= 1;
                mediaPlayer.Open(new Uri(paths[trackIndex]));
                mediaPlayer.Play();
                TextBlockSongName.Text = PlaylistListBox.Items[trackIndex].ToString();
                videoPlayer.Position = TimeSpan.FromSeconds(1);
                ButtonNext.IsEnabled = true;
            }
        }

        public void selectSongs() {             //puts the chosen songs to the playlistListBox
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic); // "C:\\Users\\admin\\Music";
            openFileDialog.Filter = "music files (*.wav)|*.wav|music files (*.mp3)|*.mp3|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

			openFileDialog.Multiselect = true;
			if (openFileDialog.ShowDialog() == true) {
				try {
					files = openFileDialog.SafeFileNames;    //saves the track names in the tracks array
					paths = openFileDialog.FileNames;        //saves the path tracks in the tracks array

					for (int i = 0; i < files.Length; i++)
						PlaylistListBox.Items.Add(files[i]);

					loadedPlaylist = true;
                    }
				catch (Exception ex) {
                    MessageBox.Show("Error: Could not read the file from disk. Original error: " + ex.Message);
                }
            }
        }

        #region media controlling
        private void ButtonPrevious_MouseDown(object sender, MouseButtonEventArgs e) {
            try {
                if (trackIndex > 0)
                    trackIndex -= 1;

                mediaPlayer.Open(new Uri(paths[trackIndex]));
                videoPlayer.Position = TimeSpan.FromSeconds(1);
                videoPlayer.Play();
                mediaPlayer.Play();
                TextBlockSongName.Text = PlaylistListBox.Items[trackIndex].ToString();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ButtonBackwards_MouseDown(object sender, MouseButtonEventArgs e) { //sets 10 seconds rewinds in the media and video players
            TimeSpan ts = new TimeSpan(0, 0, 0, (int)(mediaPlayer.Position.Seconds-10));    
            mediaPlayer.Position = ts;
            videoPlayer.Position = ts;
        }

        private void ButtonPause_MouseDown(object sender, MouseButtonEventArgs e) {
            if (loadedPlaylist==true) {
                videoPlayer.Pause();
                mediaPlayer.Pause();
                ButtonPlay.Visibility = Visibility.Visible;
                ButtonPause.Visibility = Visibility.Hidden;
            }
        }

        private void ButtonPlay_MouseDown(object sender, MouseButtonEventArgs e) {
            if (loadedPlaylist == true) {
                videoPlayer.Play();
                mediaPlayer.Play();
                ButtonPlay.Visibility = Visibility.Hidden;
                ButtonPause.Visibility = Visibility.Visible;
            } else MessageBox.Show("You can add some songs obviously :)");
        }

        private void ButtonForwards_MouseDown(object sender, MouseButtonEventArgs e) {
            TimeSpan ts = new TimeSpan(0, 0, 0, (int)(mediaPlayer.Position.Seconds + 10));
            mediaPlayer.Position = ts;
            videoPlayer.Position = ts;
        }

        private void ButtonNext_MouseDown(object sender, MouseButtonEventArgs e) {
            try {
                playNextTrack();
            }
            catch(Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }

    //forwards or backwards the media and video players according to position of the slider
        private void MediaPlayerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {       
            videoPlayer.Position = TimeSpan.FromSeconds(MediaPlayerSlider.Value);
            mediaPlayer.Position = TimeSpan.FromSeconds(MediaPlayerSlider.Value);
            TextBlockCurrentTimeSign.Text = TimeSpan.FromSeconds(MediaPlayerSlider.Value).ToString(@"m\:ss");
        }

        /*private void SeekToMediaPosition(object sender, RoutedPropertyChangedEventArgs<double> args) {
            MediaPlayerSliderValue = (int)MediaPlayerSlider.Value;
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, MediaPlayerSliderValue);
            mediaPlayer.Position = ts;
            videoPlayer.Position = ts;
        }*/
        # endregion

        private void timer_Tick(object sender, EventArgs e) {       //
            try {
                if ((mediaPlayer.Source != null) && (mediaPlayer.NaturalDuration.HasTimeSpan)) {
                    MediaPlayerSlider.Minimum = 0;
                    MediaPlayerSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                    MediaPlayerSlider.Value = mediaPlayer.Position.TotalSeconds;
                    songDuration = (int)(mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds);
                    TextBlockSongDuration.Text = TimeSpan.FromSeconds(songDuration).ToString(@"m\:ss");

                    if (MediaPlayerSlider.Maximum== MediaPlayerSlider.Value) //if end of the track the media player plays the next track
                        playNextTrack();
                }
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        #region other components

        private void PlaylistListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e) {  //plays the selected track
            try {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += timer_Tick;
                timer.Start();

                mediaPlayer.Play();
                videoPlayer.Play();
                ButtonPlay.Visibility = Visibility.Hidden;
                ButtonPause.Visibility = Visibility.Visible;
                trackIndex = PlaylistListBox.SelectedIndex;
                TextBlockSongName.Text = PlaylistListBox.Items[trackIndex].ToString();
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void PictureWorld_MouseDown(object sender, MouseButtonEventArgs e) {    //enters web browser to search the video clip of music that is currently played
            try {
                WindowWebBrowser windowWebBrowser = new WindowWebBrowser();
                windowWebBrowser.WebBrowserURLAddress.Text = "https://www.bing.com/videos/search?q=" + TextBlockSongName.Text;
                windowWebBrowser.WebBrowser.Source = new Uri("https://www.bing.com/videos/search?q=" + TextBlockSongName.Text);
                windowWebBrowser.Show();
                mediaPlayer.Pause();
                videoPlayer.Pause();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void PlaylistListBox_SelectionChanged(object sender, SelectionChangedEventArgs e){
            mediaPlayer.Open(new Uri(paths[PlaylistListBox.SelectedIndex]));
        }
        
        private void PlaylistListBox_Drop(object sender, DragEventArgs e) {
            string filename = (string)((DataObject)e.Data).GetFileDropList()[0];
            PlaylistListBox.SelectionMode = SelectionMode.Extended;
            selectSongs();
        }
		#endregion

		#region menu
		private void MenuItemContact_Click(object sender, RoutedEventArgs e) {
            WindowWebBrowser windowWebBrowser = new WindowWebBrowser();
            windowWebBrowser.Show();
            windowWebBrowser.WebBrowserURLAddress.Text = "https://elearning.pwsip.edu.pl/user/profile.php?id=469";
            windowWebBrowser.WebBrowser.Source=new Uri (windowWebBrowser.WebBrowserURLAddress.Text);
        }

        private void MenuItemEyeKeeper_Click(object sender, RoutedEventArgs e) {
            if (MenuItemEyeKeeper.IsChecked)
                videoPlayer.Visibility = Visibility.Visible;
            else
                videoPlayer.Visibility = Visibility.Hidden;
        }

        private void MenuItemNewPlaylist_Click(object sender, RoutedEventArgs e) {
            PlaylistListBox.Items.Clear();
            selectSongs();
        }

        private void MenuItemErrors_Click(object sender, RoutedEventArgs e) {
            WindowWebBrowser windowWebBrowser = new WindowWebBrowser();
            windowWebBrowser.Show();
            windowWebBrowser.WebBrowserURLAddress.Text = "https://google.com";
            windowWebBrowser.WebBrowser.Source = new Uri(windowWebBrowser.WebBrowserURLAddress.Text);
        }
		#endregion
		
        private void ButtonPlay_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {

        }

        private void Window_Closed(object sender, EventArgs e) {
            Environment.Exit(0);
        }
    }
}
