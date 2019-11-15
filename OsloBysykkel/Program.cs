using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsloBysykkel
{
    class Program
    {
        //private ProgramMode _programMode = ProgramMode.Undefined;
        private ProgramMode _programMode = ProgramMode.StatusInfoOgStatus;
        static void Main(string[] args)
        {
            try
            {
                var program = new Program();
                program.Run(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(Environment.NewLine + e.GetType() + " : " + e.Message);
            }
            finally
            {
                var t = 1;
            }

        }

        private void Run(string[] args)
        {
            ReadAndValidateArguments(args);

            switch (_programMode)
            {
                case ProgramMode.StatusInfoOgStatus:
                    {
                        Console.WriteLine("Henter stasjon info og status..");
                        var stasjonInfoStatus = new StasjonInfoOgStatus { };
                        stasjonInfoStatus.HentStasjonInfoOgStatus(args);
                        break;
                    }
                    //Her kan man legge til flere program mode i fremtiden
                
            }
        }

        private void ReadAndValidateArguments(string[] args)
        {
            if (args.Length == 0)
            {
                ShowHelp();
            }


            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower().Trim())
                {
                    case @"-stasjoninfostatus":
                        _programMode = ProgramMode.StatusInfoOgStatus;
                        break;
                        // Her kan man legge til flere mode i fremtiden
                }
            }

        }

        private string NL = Environment.NewLine;
        private string DNL = Environment.NewLine + Environment.NewLine;

        public void ShowHelp()
        {
            var message = new StringBuilder();
            message.Append(NL + "Bruk:" + NL);
            message.Append("  OsloBysykkel.exe modus" + DNL);
            message.Append("[Modus]" + DNL);
            message.Append("  -stasjoninfostatus: Henter stasjon navn og status." + NL);
            //eksempler kan legges inn her
            message.Append("" + NL);
            Console.WriteLine(message.ToString());
            //Environment.Exit(0);
        }
    }
    #region JSON Class
    public class StasjonList
    {
        public string station_id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
        public int capacity { get; set; }
    }

    public class StasjonInfoData
    {
        public List<StasjonList> stations { get; set; }
    }

    public class StasjonInfoRootObject
    {
        public int last_updated { get; set; }
        public int ttl { get; set; }
        public StasjonInfoData data { get; set; }
    }

    public class StasjonStatusList
    {
        public string station_id { get; set; }
        public int is_installed { get; set; }
        public int is_renting { get; set; }
        public int is_returning { get; set; }
        public int last_reported { get; set; }
        public int num_bikes_available { get; set; }
        public int num_docks_available { get; set; }
    }

    public class StasjonStatusData
    {
        public List<StasjonStatusList> stations { get; set; }
    }

    public class StasjonStatusRootObject
    {
        public int last_updated { get; set; }
        public int ttl { get; set; }
        public StasjonStatusData data { get; set; }
    }

    #endregion

}
