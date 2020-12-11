using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using OSIsoft.AF;
using OSIsoft.AF.Asset;

namespace OSIsoft_PI_Test
{
    class Program
    {

        private static string piSystemName = "jupiter001";
        private static string piSystemDatabase = "iON";
        private static string piSystemUser = "";
        private static string piSystemPass = "";
        private static string piSystemDomain = "";

        static void Main(string[] args)
        {
            var piSystem = new PISystems()[piSystemName];
            if (!piSystem.ConnectionInfo.IsConnected)
            {
                if (string.IsNullOrEmpty(piSystemUser))
                {
                    piSystem.Connect();
                }
                else
                {
                    var creds = new NetworkCredential(piSystemUser, piSystemPass, piSystemDomain);
                    piSystem.Connect(creds);
                }
            }

            if (piSystem.ConnectionInfo.IsConnected)
            {
                Console.WriteLine($"Connected to PI System '{piSystem.Name}' (GUID='{piSystem.ID}')");

                if (piSystem.Databases.Contains(piSystemDatabase))
                {
                    var afDatabase = piSystem.Databases[piSystemDatabase];

                    Console.WriteLine($"Connected to AF Database '{afDatabase.Name}' (GUID='{afDatabase.ID}')");

                    // Do stuff here...
                }
                else
                {
                    Console.WriteLine($"PI System Database '{piSystemDatabase}' does not exist.");
                }
            }
            else
            {
                Console.WriteLine($"Cannot connect to PI System '{piSystemName}'.");
            }


        }
    }
}
