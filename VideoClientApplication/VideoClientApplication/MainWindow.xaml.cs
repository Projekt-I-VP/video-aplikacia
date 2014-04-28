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
using System.Threading;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Threading;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace VideoClientApplication
{

    public class Alpha
    {

        // This method that will be called when the thread is started
        public void Beta()
        {
            while (true)
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var odpoved = client.GetStringAsync("http://localhost:50435/api/values");
                Console.WriteLine(odpoved.Result);
                Thread.Sleep(1000);
            }
        }
    };

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool myVideoIsPlaying = false;
        bool myVideoLoaded = false;
        Alpha oAlpha = null;
        Thread oThread = null;
        Thread vlakno = null;

        public MainWindow()
        {
            InitializeComponent();
            speedRatioSlider.Minimum = 0.0;
            speedRatioSlider.Maximum = 2.5;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                myVideoLoaded = false;

                // Open document
                string filename = dlg.FileName;

                FileInfo videoFile = new FileInfo(dlg.FileName);

                if (videoFile.Exists)
                {
                    myMediaElement.Source = new Uri(videoFile.FullName);

                    myVideoLoaded = true;
                }

            }
        }

        private void Element_MediaOpened(object sender, EventArgs e)
        {
            timelineSlider.Maximum = myMediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
        }

        private void Element_MediaEnded(object sender, EventArgs e)
        {
            myMediaElement.Stop();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if (myVideoLoaded)
            {
                if (myVideoIsPlaying)
                {
                    myMediaElement.Stop();

                    // Request that oThread be stopped
                    //oThread.Abort();

                    // Wait until oThread finishes. Join also has overloads
                    // that take a millisecond interval or a TimeSpan object.
                    //oThread.Join();
                    vlakno.Abort();
                    vlakno.Join();
                    Console.WriteLine();
                    Console.WriteLine("Alpha.Beta has finished");

                    myPlayStopButton.Content = "Play";
                }
                else
                {
                    //creating new thread
                    /*
                    oAlpha = new Alpha();

                    // Create the thread object, passing in the Alpha.Beta method
                    // via a ThreadStart delegate. This does not start the thread.
                    oThread = new Thread(new ThreadStart(oAlpha.Beta));

                    // Start the thread
                    oThread.Start();

                    // Spin for a while waiting for the started thread to become
                    // alive:
                    while (!oThread.IsAlive) ;
                    */
                    vlakno = StartTheThread(this);
                    myMediaElement.Play();
                    myPlayStopButton.Content = "Stop";
                    timelineSlider.Value = myMediaElement.Position.TotalMilliseconds;
                }

                myVideoIsPlaying = !myVideoIsPlaying;
            }
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            if (myVideoLoaded)
            {
                if (myVideoIsPlaying)
                {
                    myMediaElement.Pause();
                    myPauseButton.Content = "Continue";
                }
                else
                {
                    myMediaElement.Play();
                    myPauseButton.Content = "Pause";
                }

                myVideoIsPlaying = !myVideoIsPlaying;
            }
        }

        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            if (myVideoLoaded)
            {
                if (myVideoIsPlaying)
                {
                    myMediaElement.SpeedRatio = myMediaElement.SpeedRatio + 0.1;
                }
            }
        }
        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            if (myVideoLoaded)
            {
                if (myVideoIsPlaying)
                {
                    myMediaElement.SpeedRatio = myMediaElement.SpeedRatio - 0.1;
                }
            }
        }

        private void ChangeMediaSpeedRatio(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            myMediaElement.SpeedRatio = (double)speedRatioSlider.Value;
        }

        // Jump to different parts of the media (seek to).  
        private void SeekToMediaPosition(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            int SliderValue = (int)timelineSlider.Value;

            // Overloaded constructor takes the arguments days, hours, minutes, seconds, miniseconds. 
            // Create a TimeSpan with miliseconds equal to the slider value.
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
            myMediaElement.Position = ts;
        }

        public Thread StartTheThread(MainWindow parameter)
        {
            var thread = new Thread(() => RealStart(parameter));
            thread.Start();
            return thread;
        }

        private static void RealStart(MainWindow window)
        {
            Console.WriteLine("spustilo sa nove vlakno");
            while (true)
            {
                string time = "no";
                string clientName = "";
                window.myMediaElement.Dispatcher.Invoke(DispatcherPriority.Normal,
    (ThreadStart)delegate { 
        time = window.myMediaElement.Position.ToString();
        clientName = window.myClientName.Text;
    }
);

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var odpoved = client.GetStringAsync("http://localhost:50435/api/values?clientName=" +clientName+"&time=" + time);
                //Console.WriteLine(odpoved.Result);
                //window.labelZVlakna.Dispatcher.Invoke(() => window.labelZVlakna.Content = odpoved.Result);
                //Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(odpoved.Result);
                List<KeyValuePair<string, string>> values = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<KeyValuePair<string, string>>>(odpoved.Result);
                window.labelZVlakna.Dispatcher.Invoke(() => window.textBox1.Text = values[0].ToString());
                Thread.Sleep(1000);
            }
        }
    }
}
