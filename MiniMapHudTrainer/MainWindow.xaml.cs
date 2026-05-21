using System;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MiniMapHudTrainer
{
      public partial class MainWindow : Window
      {
            
            private DispatcherTimer mainTimer;       //main alert timer
            private DispatcherTimer hideTimer;       //hide image timer

            private SoundPlayer player;
            private DispatcherTimer countdownTimer;
           
            private int countdownSeconds;
            private Random random = new Random();

            public MainWindow()
            {
                  try
                  {
                        InitializeComponent();

                        //create timers
                        mainTimer = new DispatcherTimer();
                        mainTimer.Tick += MainTimer_Tick;

                        hideTimer = new DispatcherTimer();
                        hideTimer.Interval = TimeSpan.FromSeconds(3);
                        hideTimer.Tick += HideTimer_Tick;

                        countdownTimer = new DispatcherTimer();
                        countdownTimer.Interval = TimeSpan.FromSeconds(1);
                        countdownTimer.Tick += CountdownTimer_Tick;
                        //assets path
                        string assetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");

                        
                        //voices path
                        string voicesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Voices");

                        //load voices
                        string[] voiceFiles = Directory.GetFiles(voicesPath);

                        foreach (string file in voiceFiles)
                        {
                              cmbVoices.Items.Add(Path.GetFileName(file));
                        } //end foreach

                        
                        if (cmbVoices.Items.Count > 0)
                        {
                              cmbVoices.SelectedIndex = 0;
                        } //end if

                        txtSliderValue.Text = sliderInterval.Value.ToString();
                  }
                  catch (Exception ex)
                  {
                        MessageBox.Show(ex.ToString());
                  } //end catch
            }
            private void sliderInterval_ValueChanged(
                object sender,
                RoutedPropertyChangedEventArgs<double> e)
            {
                  if (txtSliderValue != null)
                  {
                        txtSliderValue.Text =
                            ((int)sliderInterval.Value).ToString();
                  } //end if
            } //end sliderInterval_ValueChanged

            private void btnStart_Click(object sender, RoutedEventArgs e)
            {
                  countdownSeconds = (int)sliderInterval.Value;

                  txtCountdown.Text = countdownSeconds.ToString();

                  mainTimer.Interval = TimeSpan.FromSeconds(countdownSeconds);

                  mainTimer.Start();

                  countdownTimer.Start();
            } //end btnStart_Click

            private void btnStop_Click(object sender, RoutedEventArgs e)
            {
                  mainTimer.Stop();
                  hideTimer.Stop();
                  countdownTimer.Stop();

                  imgMiniMap.Visibility = Visibility.Hidden;

                  txtHudAlert.Visibility = Visibility.Hidden;
            } //end btnStop_Click
            private void CountdownTimer_Tick(object sender, EventArgs e)
            {
                  countdownSeconds--;

                  if (countdownSeconds < 0)
                  {
                        countdownSeconds = 0;
                  } //end if

                  txtCountdown.Text = countdownSeconds.ToString();
            } //end CountdownTimer_Tick

            
            private void HideTimer_Tick(object sender, EventArgs e)
            {
                  hideTimer.Stop();

                  imgMiniMap.Visibility = Visibility.Hidden;

                  txtHudAlert.Visibility = Visibility.Hidden;
            } //end HideTimer_Tick

            
            private void MainTimer_Tick(object sender, EventArgs e)
            {
                  mainTimer.Stop();

                  if (cmbVoices.SelectedItem == null)
                  {
                        return;
                  } //end if

                  //random voice selection
                  if (chkRandomVoice.IsChecked == true)
                  {
                        int randomIndex = random.Next(cmbVoices.Items.Count);

                        cmbVoices.SelectedIndex = randomIndex;
                  } //end if

                  string voiceFileName = cmbVoices.SelectedItem.ToString().ToLower();

                  string imageName = "map.png";

                  //determine image from filename
                  if (voiceFileName.Contains("ammo"))
                  {
                        imageName = "ammo.png";
                  }
                  else if (voiceFileName.Contains("cash"))
                  {
                        imageName = "cash.png";
                  }
                  else if (voiceFileName.Contains("armor"))
                  {
                        imageName = "armor.png";
                  }
                  else if (voiceFileName.Contains("map"))
                  {
                        imageName = "map.png";
                  } //end if

                  //load image
                  string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", imageName);

                  imgMiniMap.Source = new BitmapImage(new Uri(imagePath));

                  imgMiniMap.Visibility = Visibility.Visible;

                  txtHudAlert.Visibility = Visibility.Hidden;

                  //play sound
                  string voicePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Voices", cmbVoices.SelectedItem.ToString());

                  player = new SoundPlayer(voicePath);

                  player.Play();

                  //restart hide timer
                  hideTimer.Stop();
                  hideTimer.Start();

                  //reset countdown
                  countdownSeconds = (int)sliderInterval.Value;

                  txtCountdown.Text = countdownSeconds.ToString();

                  mainTimer.Start();
            } //end MainTimer_Tick
      }
}