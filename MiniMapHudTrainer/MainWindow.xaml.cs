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

                        //assets path
                        string assetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");

                        //load images
                        string[] imageFiles = Directory.GetFiles(assetsPath);

                        foreach (string file in imageFiles)
                        {
                              cmbImages.Items.Add(Path.GetFileName(file));
                        } //end foreach

                        //voices path
                        string voicesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Voices");

                        //load voices
                        string[] voiceFiles = Directory.GetFiles(voicesPath);

                        foreach (string file in voiceFiles)
                        {
                              cmbVoices.Items.Add(Path.GetFileName(file));
                        } //end foreach

                        //select defaults
                        if (cmbImages.Items.Count > 0)
                        {
                              cmbImages.SelectedIndex = 0;
                        } //end if

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
                  int seconds = (int)sliderInterval.Value;

                  mainTimer.Interval = TimeSpan.FromSeconds(seconds);

                  mainTimer.Start();
            } //end btnStart_Click

            private void btnStop_Click(object sender, RoutedEventArgs e)
            {
                  mainTimer.Stop();
                  hideTimer.Stop();

                  imgMiniMap.Visibility = Visibility.Hidden;
            } //end btnStop_Click

            private void MainTimer_Tick(object sender, EventArgs e)
            {
                  mainTimer.Stop();

                  //make sure voice selected
                  if (cmbVoices.SelectedItem == null)
                  {
                        return;
                  } //end if

                  string voiceFileName = cmbVoices.SelectedItem.ToString().ToLower();

                  //show minimap image
                  if (voiceFileName.Contains("map"))
                  {
                        if (cmbImages.SelectedItem != null)
                        {
                              string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", cmbImages.SelectedItem.ToString());

                              imgMiniMap.Source = new BitmapImage(new Uri(imagePath));

                              imgMiniMap.Visibility = Visibility.Visible;
                        } //end if
                  }
                  else
                  {
                        //clear image
                        imgMiniMap.Source = null;

                        //show HUD text
                        if (voiceFileName.Contains("ammo"))
                        {
                              txtHudAlert.Text = "AMMO";
                        }
                        else if (voiceFileName.Contains("cash"))
                        {
                              txtHudAlert.Text = "CASH";
                        }
                        else if (voiceFileName.Contains("armor"))
                        {
                              txtHudAlert.Text = "ARMOR";
                        }
                        else
                        {
                              txtHudAlert.Text = "CHECK HUD";
                        } //end if

                        txtHudAlert.Visibility = Visibility.Visible;
                  } //end else

                  //play sound
                  string voicePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Voices", cmbVoices.SelectedItem.ToString());

                  player = new SoundPlayer(voicePath);
                  player.Play();

                  //restart hide timer
                  hideTimer.Stop();
                  hideTimer.Start();

                  mainTimer.Start();
            } //end MainTimer_Tick

            private void HideTimer_Tick(object sender, EventArgs e)
            {
                  hideTimer.Stop();

                  imgMiniMap.Visibility = Visibility.Hidden;

                  txtHudAlert.Visibility = Visibility.Hidden;
            } //end HideTimer_Tick

            private void cmbImages_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
            {
                  if (cmbImages.SelectedItem == null)
                  {
                        return;
                  } //end if

                  string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", cmbImages.SelectedItem.ToString());

                  imgMiniMap.Source = new BitmapImage(new Uri(imagePath));

                  imgMiniMap.Visibility = Visibility.Visible;
            }
      }
}