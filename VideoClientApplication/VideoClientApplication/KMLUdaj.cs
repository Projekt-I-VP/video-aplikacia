using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoClientApplication
{
    class KMLudaj
    {
        // key properties

        public TimeSpan UdajTime { get; /*private*/ set; }
        public string UdajLat { get; /*private*/ set; }
        public string UdajLon { get; /*private*/ set; }
        public string UdajBearing { get; /*private*/ set; }



        public KMLudaj()
        {
            //prazdny konstruktor
        }

        // constructor
        public KMLudaj(string lat, string lon, string bearing, TimeSpan time)
        {
            UdajLat = lat;
            UdajLon = lon;
            UdajBearing = bearing;
            UdajTime = time;
        }
    }
}
