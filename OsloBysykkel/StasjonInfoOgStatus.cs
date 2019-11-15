using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
namespace OsloBysykkel
{
    public class StasjonInfoOgStatus
    {
        private static string _stasjonInfoEndepunktUrl = (ConfigurationManager.AppSettings["stasjon_info_endepunkt"]); //hent fra konfig fila
        private static string _stasjonStatusEndepunktUrl = (ConfigurationManager.AppSettings["stasjon_status_endepunkt"]); //hent fra konfig fila
        private static string _hovedArbeidKatalog = (ConfigurationManager.AppSettings["hoved_arbeidkatalog"]); //hent fra konfig fila

        public void HentStasjonInfoOgStatus(string[] args)
        {
            string osloBysykkelKatalog = _hovedArbeidKatalog + "\\" + "OsloBysykkel" + "\\"; // Katalog med navn OsloBysykkel på C drive (default)
            string stasjonInfoOgStatusKatalog = osloBysykkelKatalog + "\\" + "StasjonInfoOgStatus" + "\\"; // Katalog med navn stasjonInfoOgStatusKatalog under OsloBysykkel

            // Lag osloBysykkelKatalog hvis ikke finnes fra før
            if (!Directory.Exists(osloBysykkelKatalog))
            {
                Directory.CreateDirectory(osloBysykkelKatalog);
            }

            // Lag stasjonInfoOgStatusKatalog hvis ikke finnes fra før
            if (!Directory.Exists(stasjonInfoOgStatusKatalog))
            {
                Directory.CreateDirectory(stasjonInfoOgStatusKatalog);
            }
            // Tekst fila med header
            string tekstFilePath = stasjonInfoOgStatusKatalog + @"OsloBysykkel_stasjon_info_status" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + @".txt";
            string[] header = { "StasjonNavn;AntallTilgjengeligLåser;LedigeSykkler" };
            System.IO.File.WriteAllLines(tekstFilePath, header, Encoding.Unicode);
            //Kall mot API
            StasjonInfoRootObject outStasjonInfo = HentStasjonInfo(); //Henter stasjon informasjon
            StasjonStatusRootObject outStasjonStatus = HentStasjonStatus(); // Henter Status
            KombinerLister(); //Kombinerer objekt lister

            StasjonInfoRootObject HentStasjonInfo()
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(_stasjonInfoEndepunktUrl);
                httpWReq.Method = "GET";
                HttpWebResponse httpWResp = (HttpWebResponse)httpWReq.GetResponse();
                StasjonInfoRootObject jsonResponse = null;
                try
                {
                    //Test the connection
                    if (httpWResp.StatusCode == HttpStatusCode.OK)
                    {
                        Stream responseStream = httpWResp.GetResponseStream();
                        string jsonString = null;

                        //Set jsonString using a stream reader
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            // Kunne kanskje brukt Regex istedenfor
                            jsonString = reader.ReadToEnd().Replace("\\u00f8", "ø").Replace("\\u00d8", "Ø").Replace("\\u00e5", "å").Replace("\\u00e6", "æ").Replace("\\u00e9", "é").Replace("\\u00fc", "ü");
                            reader.Close();
                        }

                        //Deserialize our JSON
                        JavaScriptSerializer sr = new JavaScriptSerializer();
                        //Trim er kanskje ikke helt nødvendig
                        jsonResponse = sr.Deserialize<StasjonInfoRootObject>(jsonString.Trim('"'));
                    }


                    else
                    {
                        FailComponent(httpWResp.StatusCode.ToString());

                    }
                }
                //Output JSON parsing error
                catch (Exception e)
                {
                    FailComponent(e.ToString());
                }
                return jsonResponse;

            }
            StasjonStatusRootObject HentStasjonStatus()
            {
                HttpWebRequest httpStatsjonStatusReq = (HttpWebRequest)WebRequest.Create(_stasjonStatusEndepunktUrl);
                httpStatsjonStatusReq.Method = "GET";
                HttpWebResponse httpStatsjonStatusResp = (HttpWebResponse)httpStatsjonStatusReq.GetResponse();
                StasjonStatusRootObject jsonStatsjonStatusResponse = null;
                try
                {
                    //Test the connection
                    if (httpStatsjonStatusResp.StatusCode == HttpStatusCode.OK)
                    {
                        Stream responseStream = httpStatsjonStatusResp.GetResponseStream();
                        string jsonStatsjonStatusString = null;

                        //Set jsonString using a stream reader
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            jsonStatsjonStatusString = reader.ReadToEnd().Replace("\\", "");
                            reader.Close();
                        }

                        //Deserialize our JSON
                        JavaScriptSerializer sr = new JavaScriptSerializer();
                        //Trim er kanskje ikke helt nødvendig
                        jsonStatsjonStatusResponse = sr.Deserialize<StasjonStatusRootObject>(jsonStatsjonStatusString.Trim('"'));
                        //jsonResponse = sr.Deserialize(jsonString);
                    }


                    else
                    {
                        FailComponent(httpStatsjonStatusResp.StatusCode.ToString());

                    }
                }
                //Output JSON parsing error
                catch (Exception e)
                {
                    FailComponent(e.ToString());
                }
                return jsonStatsjonStatusResponse;

            }
            void FailComponent(string errorMsg)
            {
                bool fail = false;

            }

            void KombinerLister()
            {
               
                var query = from stations in outStasjonInfo.data.stations
                            from status in outStasjonStatus.data.stations
                            where stations.station_id == status.station_id
                            select new { stations.name,status.num_docks_available, status.num_bikes_available };

                Console.WriteLine("Starter lagring av resultater til text fil..");
                foreach (var item in query)
                {
                    System.IO.StreamWriter tekstwriter = new System.IO.StreamWriter(tekstFilePath, true, Encoding.Unicode);
                    tekstwriter.Write(item.name);
                    tekstwriter.Write(";");
                    tekstwriter.Write(item.num_docks_available);
                    tekstwriter.Write(";");
                    tekstwriter.Write(item.num_bikes_available);
                    tekstwriter.WriteLine("");
                    tekstwriter.Close();
                    
                }
                Console.WriteLine("Ferdig lagret resultater i text fil.");
            }
        }
    }
}
