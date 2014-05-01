using SharpKml.Dom;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace VideoClientApplication
{
    class KMLReader
    {
        public FileInfo file;

        public KMLReader(string full_path)
        {
            /*file = new FileInfo("@"+full_path);*/
            file = new FileInfo(@"C:\Users\jDzama\Documents\Visual Studio 2012\Projects\kvideudata.kml");
        }

        public void  parsuj(out List<System.TimeSpan> listCasov, out List<KMLUdaj> listUdajov)
        {
            System.TimeSpan zaciatocnyCas = new System.TimeSpan();
            Boolean prvyUdaj = true;
            listCasov = new List<System.TimeSpan>();
            listUdajov = new List<KMLUdaj>();

            if (!file.Exists)
            {
                Console.WriteLine("Subor som nenasiel.");
            }
            else
            {
                System.IO.TextReader reader = new StreamReader(file.FullName);
                /*while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null) break;
                    Console.WriteLine(line);
                }*/
                KmlFile kmlFile = KmlFile.Load(reader);

                Kml kml = kmlFile.Root as Kml;
                Console.WriteLine(
                    kmlFile.Root.ToString());

                foreach (var extendedData in kml.Flatten().OfType<ExtendedData>())
                {
                    foreach (var schemaData in extendedData.SchemaData)
                    {
                        KMLUdaj udaj = new KMLUdaj();
                        //vytvorim novy udaj, kde nahadzem hodnoty
                        foreach (SimpleData sd in schemaData.SimpleData)
                        {
                            string szName = sd.Name;
                            string szDataValue = sd.Text;
                            /*Console.Write(szName + " = ");
                            Console.WriteLine(szDataValue);*/

                            switch (szName)
                            {
                                case "FID":
                                    if (Convert.ToInt32(szDataValue) == 0)
                                    {
                                        prvyUdaj = true;
                                    }
                                    break;
                                case "Lat":
                                    udaj.UdajLat = szDataValue;
                                    break;
                                case "Lon":
                                    udaj.UdajLon = szDataValue;
                                    break;
                                case "Bearing":
                                    udaj.UdajBearing = szDataValue;
                                    break;
                                case "UTC_Time":
                                    if (prvyUdaj)
                                    {
                                        zaciatocnyCas = System.TimeSpan.Parse(szDataValue);
                                        prvyUdaj = false;
                                    }
                                    udaj.UdajTime = System.TimeSpan.Parse(szDataValue) - zaciatocnyCas;
                                    listCasov.Add(udaj.UdajTime);
                                    break;
                                default:
                                    break;
                            }
                            //pridal som jeden udaj
                        }
                        //mam hodnoty v udaj, mozem to hadzat do pola po tomto napr...
                        listUdajov.Add(udaj);
                    }

                }
            }
        }


        /*public KMLudaj parsujUvod()
        {

            if (!file.Exists)
            {
                Console.WriteLine("Subor som nenasiel.");
            }
            else
            {
                System.IO.TextReader reader = new StreamReader(file.FullName);
                /*while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null) break;
                    Console.WriteLine(line);
                }
                KmlFile kmlFile = KmlFile.Load(reader);

                Kml kml = kmlFile.Root as Kml;
                Console.WriteLine(
                    kmlFile.Root.ToString());

                foreach (var extendedData in kml.Flatten().OfType<ExtendedData>())
                {
                    foreach (var schemaData in extendedData.SchemaData)
                    {
                        KMLudaj udaj = new KMLudaj();
                        //vytvorim novy udaj, kde nahadzem hodnoty
                        foreach (SimpleData sd in schemaData.SimpleData)
                        {
                            string szName = sd.Name;
                            string szDataValue = sd.Text;
                            /*Console.Write(szName + " = ");
                            Console.WriteLine(szDataValue);

                            switch (szName)
                            {
                                case "FID":
                                    udaj.UdajId = Convert.ToInt32(szDataValue);
                                    break;
                                case "Lat":
                                    if (udaj.UdajId > 0)
                                        udaj.UdajLat = szDataValue;
                                    break;
                                case "Lon":
                                    if (udaj.UdajId > 0)
                                        udaj.UdajLon = szDataValue;
                                    break;
                                case "Altitude":
                                    if (udaj.UdajId > 0)
                                        udaj.UdajAltitude = szDataValue;
                                    break;
                                case "Bearing":
                                    udaj.UdajBearing = szDataValue;
                                    break;
                                case "Speed":
                                    udaj.UdajSpeed = szDataValue;
                                    break;
                                case "Distance":
                                    if (udaj.UdajId > 0)
                                        udaj.UdajDistance = szDataValue;
                                    break;
                                case "UTC_Time":
                                    if (udaj.UdajId > 0)
                                        udaj.UdajTime = szDataValue;
                                    break;
                                default:
                                    break;
                            }
                            //pridal som jeden udaj
                        }
                        //vraciam hodnoty z prvej sady udajov (zmenim na presnejsie z prvych sad - rychlost, bearing a pod.)
                        if (udaj.UdajId >= 10)
                        {
                            udaj.UdajId = 0;
                            return udaj;
                        }
                    }

                }
            }
            return null;
        }*/
    }
}
