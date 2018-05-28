using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace Avinode
{
    class Program
    {
        private static void Main()
        {
            // Get xml file and active path from console
            Console.WriteLine("Write File Path Here");
            string filePath = Console.ReadLine();
            Console.WriteLine("Write Active Path Here");
            string activePath = Console.ReadLine();
            XDocument xmlFile = XDocument.Load(filePath);


            // Query xml for active paths
            var foundActives = from item in xmlFile.Descendants("item")
                          where (string)item.Element("path").Attribute("value") == activePath
                          select item.Element("displayName").Value;

            // Query xml for all items
            var allItems = xmlFile.Descendants("item")
                .Select(x => new
                {
                    displayName = x.Element("displayName").Value,
                    path = x.Element("path").Attribute("value").Value
                })
                .ToList();

            // Initialize list for all actives (children and parents)
            List<string> allActives = new List<string>();

            foreach (string name in foundActives)
            {
                // Hacky try catch to check for grandparents
                List<string> currentActives = new List<string>();
                try
                {
                    var actives = xmlFile.Descendants("item")
                   .Where(x => x.Element("displayName").Value == name)
                   .Select(x => new
                   {
                       parentName = x.Parent.Parent.Parent.Parent.Element("displayName").Value
                   })
                   .ToList();
                    foreach (var active in actives)
                    {
                        currentActives.Add(active.parentName);
                    }
                }
                catch{}
                // Hacky try catch to check for parents
                try
                {
                    var actives = xmlFile.Descendants("item")
                    .Where(x => x.Element("displayName").Value == name)
                    .Select(x => new
                    {
                        parentName = x.Parent.Parent.Element("displayName").Value
                    })
                    .ToList();
                    foreach(var active in actives)
                    {
                        currentActives.Add(active.parentName);
                    }
                }
                catch
                {
                    var actives = xmlFile.Descendants("item")
                    .Where(x => x.Element("displayName").Value == name)
                    .Select(x => new
                    {
                        name = x.Element("displayName").Value
                    })
                    .ToList();
                    foreach (var active in actives)
                    {
                        currentActives.Add(active.name);
                    }
                }
                // Check if I've already added it to the allActives list, then add it
                foreach (var active in currentActives)
                {
                    if(!allActives.Exists(x => x == active))
                    {
                        allActives.Add(active);
                    }
                }
                if (!allActives.Exists(x => x == name))
                {
                    allActives.Add(name);
                }
            }

            

            // Loop to check if item is active, and display the results.
            foreach (var item in allItems)
            {
                bool active = false;
                foreach (var activeItem in allActives)
                {
                    if(item.displayName == activeItem)
                    {
                        active = true;
                    }
                }
                Console.WriteLine(item.displayName);
                Console.WriteLine(item.path);
                if(active)
                {
                    Console.WriteLine("[ACTIVE]");
                }
            }
            Console.WriteLine("-------------------");
            Console.WriteLine("Press Enter to Exit");
            Console.ReadLine();
        }
    }
}

// For Testing

// C:\Users\Adam\Desktop\Avinode\Menu1.xml
// /Requests/OpenQuotes.aspx
// /Requests/Quotes/CreateQuote.aspx

// C:\Users\Adam\Desktop\Avinode\Menu2.xml
// /TWR/AircraftSearch.aspx