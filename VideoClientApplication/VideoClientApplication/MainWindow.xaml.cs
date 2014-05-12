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
using System.Collections.ObjectModel;
using System.Device.Location;

namespace VideoClientApplication
{

    /*public class Alpha
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
    };*/

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class MainWindow : Window
    {
        bool myVideoIsPlaying = false;
        bool myVideoLoaded = false;

        SortedList<System.TimeSpan,KMLUdaj> listUdajov;
        List<System.TimeSpan> keysList;
        List<KMLUdaj> valuesList;
        GeoCoordinate myCoordinates = new GeoCoordinate();

       // Alpha oAlpha = null;
        //Thread oThread = null;
        Thread vlakno = null;
        ObservableCollection<KeyValuePair<string, string>> predList = new ObservableCollection<KeyValuePair<string, string>>();
        ObservableCollection<KeyValuePair<string, string>> zaList = new ObservableCollection<KeyValuePair<string, string>>();

        public MainWindow()
        {
            InitializeComponent();
            speedRatioSlider.Minimum = 0.0;
            speedRatioSlider.Maximum = 2.5;

            ListBoxZa.ItemsSource = zaList;
            ListBoxPred.ItemsSource = predList;
        }

        public void processServerData(List<KeyValuePair<string, string>> data)
        {
            //Console.WriteLine(data.Count);

            predList.Clear();
            zaList.Clear();

            TimeSpan cas = myMediaElement.Position;
            labelMyTime.Content = cas.Hours + " : " + cas.Minutes + " : " + cas.Seconds;

            int myKIndex = keysList.BinarySearch(myMediaElement.Position);
            myKIndex = myKIndex >= 0 ? myKIndex : ~myKIndex;

            //Console.WriteLine(Convert.ToDouble("21,225890"));
            Console.WriteLine("                 "+valuesList[myKIndex].UdajLon.Trim().Replace(".",","));
            myCoordinates.Longitude = Convert.ToDouble(valuesList[myKIndex].UdajLon.Trim().Replace(".", ","));
            myCoordinates.Latitude = Convert.ToDouble(valuesList[myKIndex].UdajLat.Trim().Replace(".", ","));
            myCoordinates.Course = Convert.ToDouble(valuesList[myKIndex].UdajBearing.Trim().Replace(".", ","));

            foreach (KeyValuePair<string, string> klient in data)
            {
                if (klient.Key.Equals(myClientName.Text))
                {
                    continue;
                }
                if (TimeSpan.Parse(klient.Value) > cas)
                {
                    zaList.Add(klient);
                }
                else
                {
                    predList.Add(klient);
                }

                int kIndex = keysList.BinarySearch(TimeSpan.Parse(klient.Value));
                kIndex = kIndex >= 0 ? kIndex : ~kIndex;

                GeoCoordinate coordinate = new GeoCoordinate(Convert.ToDouble(valuesList[kIndex].UdajLat.Trim().Replace(".", ",")),
                    Convert.ToDouble(valuesList[kIndex].UdajLon.Trim().Replace(".", ",")));
                coordinate.Course = Convert.ToDouble(valuesList[kIndex].UdajBearing.Trim().Replace(".", ","));

                double distance = myCoordinates.GetDistanceTo(coordinate);
                
                Console.WriteLine("index udaja " + klient.Key + " je " + kIndex 
                    + " latitude: " + valuesList[kIndex].UdajLat 
                    + " longitude: " + valuesList[kIndex].UdajLon
                    + " vzdialenost ku mne je " + distance
                    + " bearing: "  + valuesList[kIndex].UdajBearing
                    + " vidno ma " + isVisible(distance, coordinate));
            }

            //data.ForEach(predList.Add);
            //data.ForEach(zaList.Add);
        }

        private Boolean isVisible(double distance, GeoCoordinate coordinate)
        {
            Console.Write(" natocenie..." + myCoordinates.Course);
            return (distance < 100/*???*/) && 
                (Math.Abs(coordinate.Course - BearingTo(coordinate.Latitude, coordinate.Longitude))<80/2 
                || ((coordinate.Course + BearingTo(coordinate.Latitude, coordinate.Longitude) % 360)<80/2)/*FOV*/);
        }

        public double BearingTo(double lat, double lng)
        {
            double lat1 = DegreeToRadian(myCoordinates.Latitude);
            double lat2 = DegreeToRadian(lat);
            double dLon = DegreeToRadian(lng) - (DegreeToRadian(myCoordinates.Longitude));

            double y = Math.Sin(dLon) * Math.Cos(lat2);
            double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
            double brng = Math.Atan2(y, x);

            return (RadianToDegree(brng) + 360) % 360;
        }

        /*public double bearing()
        {
            double myx = 1;
            double myy = 2;
            double kux = 3;
            double kuy = 4;


            if (myx == kux) myx = myx + 1;
            if (kuy == myy) myy = myy + 1;

            if (myx >= kux)
            {
                if (myy >= kuy)
                {
                    double bearing = Math.Atan(Math.Abs(kux - myx) / Math.Abs(kuy - myy)); //+180
                }
                else
                {
                    double bearing = Math.Atan(Math.Abs(kuy - myy) / Math.Abs(myx - kux)); //+270
                }
            }
            else
            {
                if (myy >= kuy)
                {
                    double bearing = Math.Atan(Math.Abs(kuy - myy) / Math.Abs(myx - kux)); //+90
                }
                else
                {
                    double bearing = Math.Atan(Math.Abs(kux - myx) / Math.Abs(kuy - myy)); //+0
                }
            }

            return 0.0;
        }*/

        public double DegreeToRadian(double angle) { return Math.PI * angle / 180.0; }

        public double RadianToDegree(double angle) { return 180.0 * angle / Math.PI; }

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

                    nacitajKml(dlg);
                }

            }
        }

        private void nacitajKml(Microsoft.Win32.OpenFileDialog dlg)
        {
            string kmlName = dlg.FileName + ".kml";
            KMLReader kmlreader = new KMLReader(kmlName);
            listUdajov = kmlreader.parsuj();
            keysList = new List<System.TimeSpan>(listUdajov.Keys);
            valuesList = new List<KMLUdaj>(listUdajov.Values);
            //Console.WriteLine(listUdajov.Values[0].UdajLat + listUdajov.Values[0].UdajTime);
            //Console.WriteLine("toto kmlko je uz sparsovane: "+kmlName);
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
                
                List<KeyValuePair<string, string>> m = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(odpoved.Result);

                //Console.WriteLine(m.Count);

                // Bad Boys


                window.labelZVlakna.Dispatcher.Invoke(() => window.processServerData(m));
                //window.labelZVlakna.Dispatcher.Invoke(() => window.labelZVlakna.Content = odpoved.Result);
                //Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(odpoved.Result);
                //List<KeyValuePair<string, string>> values = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<KeyValuePair<string, string>>>(odpoved.Result);
                


                //window.labelZVlakna.Dispatcher.Invoke(() => window.textBox1.Text = values[0].ToString());
                Thread.Sleep(1000);
            }
        }
    }
}
