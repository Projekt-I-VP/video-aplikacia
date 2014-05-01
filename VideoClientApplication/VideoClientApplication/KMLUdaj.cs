using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoClientApplication
{
    public class KMLUdaj
    {
        // key properties

        public TimeSpan UdajTime { get; /*private*/ set; }
        public string UdajLat { get; /*private*/ set; }
        public string UdajLon { get; /*private*/ set; }
        public string UdajBearing { get; /*private*/ set; }



        public KMLUdaj()
        {
            //prazdny konstruktor
        }

        // constructor
        public KMLUdaj(string lat, string lon, string bearing, TimeSpan time)
        {
            UdajLat = lat;
            UdajLon = lon;
            UdajBearing = bearing;
            UdajTime = time;
        }
    }
}
