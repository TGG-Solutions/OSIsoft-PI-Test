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

        private static string piSystemName = "JUPITER001";
        private static string piSystemDatabase = "iON";
        private static string piSystemUser = "rkirk.adm";
        private static string piSystemPass = "GXj2N7ee6J7TkPdK";
        private static string piSystemDomain = "tgg";

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
                    object sysCookie = piSystem.GetFindChangedItemsCookie(searchSandbox: false);

                    for (int i = 0; i < 100; i++)
                    {
                        Console.WriteLine("Checking for changes...");

                        List<AFChangeInfo> list = new List<AFChangeInfo>();
                        int resultsPerPage = 1000;
                        while (true)
                        {
                            var results = piSystem.FindChangedItems(false, true, resultsPerPage, sysCookie, out sysCookie);
                            if ((results?.Count ?? 0) == 0) break;
                            list.AddRange(results);
                        }

                        // Refresh objects that have been changed.
                        AFChangeInfo.Refresh(piSystem, list);

                        // Find the objects that have been changed.
                        foreach (AFChangeInfo info in list)
                        {
                            try
                            {
                                AFObject myObj = info.FindObject(piSystem, true);
                                Console.WriteLine("Found changed object: {0}", myObj.ToString());
                                Console.WriteLine("Identity: " + myObj.Identity.ToString());
                                Console.WriteLine("Action = " + info.Action);
                                Console.WriteLine("Change Time = " + info.ChangeTime);
                                Console.WriteLine("Value Updated = " + info.IsValueUpdate);
                                Console.WriteLine("Parent ID = " + info.ParentID);
                                Console.WriteLine("Parent = " + ((info.ParentID == Guid.Empty) ? "NONE" : AFElement.FindElement(piSystem, info.ParentID).GetPath()));
                                Console.WriteLine(" ");
                            }
                            catch { }
                        }

                        System.Threading.Thread.Sleep(5000);
                    }

                    
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
