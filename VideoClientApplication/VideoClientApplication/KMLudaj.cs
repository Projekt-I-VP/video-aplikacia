using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualnePrechadzky
{
    class KMLudaj
    {

        // key properties
        public Int32 UdajId { get; /*private*/ set; }
        public string UdajLat { get; /*private*/ set; }
        public string UdajLon { get; /*private*/ set; }
        public string UdajAltitude { get; /*private*/ set; }      
        public string UdajBearing { get; /*private*/ set; }
        public string UdajSpeed { get; /*private*/ set; }
        public string UdajDistance { get; /*private*/ set; }
        public string UdajTime { get; /*private*/ set; }


            public KMLudaj()
            {
                //prazdny konstruktor
            }

            // constructor
            public KMLudaj(Int32 udajId, string lat, string lon, string altitude, string bearing, string speed, string distance, string time)
            {
                UdajId = udajId;
                UdajLat = lat;
                UdajLon = lon;
                UdajAltitude = altitude;
                UdajBearing = bearing;
                UdajSpeed = speed;
                UdajDistance = distance;
                UdajTime = time;
            }

    }
}
