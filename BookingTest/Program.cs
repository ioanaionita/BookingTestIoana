using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BookingTest
{
    public class Supplier
    {
        public string Supplier_id { get; set; }
        public string Pickup { get; set; }
        public string Dropoff { get; set; }
        public List<Option> Options { get; set; }
    }

    public class Option
    {
        public string Supplier_Id { get; set; }
        public string Car_type { get; set; }
        public int Price { get; set; }
    }

    class Program
    {
        static List<Option> getSupplierOption (string url, string pickup, string dropoff)
        {
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["pickup"] = pickup;
            query["dropoff"] = dropoff;
            uriBuilder.Query = query.ToString();

            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(uriBuilder.Uri);
            //Give the Taxis supplier API 2 seconds (2000 miliseconds) to respond:
            wrGETURL.Timeout = 2000;

            Stream objStream;
            StreamReader readStream;
            string jsonData;
            try
            {
                objStream = wrGETURL.GetResponse().GetResponseStream();
                readStream = new StreamReader(objStream, Encoding.UTF8);
                Console.WriteLine("Search finished.");
                jsonData = readStream.ReadToEnd();
                var mySupplier = JsonConvert.DeserializeObject<Supplier>(jsonData);
                return mySupplier.Options;
            }
            catch(Exception e){}
            List<Option> o = new List<Option>();
            return o;
        }

        static void printAll(List<Option> o, string supplier, string pickup, string dropoff)
        {
            if(o.Count() == 0)
            {
                Console.WriteLine(supplier + " Taxi's " + "is not available at the moment.");
                return;
            }
            Console.WriteLine("SEARCH RESULTS FOR " + supplier);
            Console.WriteLine("Pickup: " + pickup);
            Console.WriteLine("Dropoff: " + dropoff);
            foreach(Option opt in o)
            {
                Console.WriteLine("Option: car_type = " + opt.Car_type + "; price = " + opt.Price);
            }
            Console.WriteLine("--------------------------------------------------------------------");
        }

        static void printSupplier(List<Option> o)
        {
            while(o.Count() != 0)
            {
                Option opt = o.Find(x => x.Price == o.Min(y => y.Price));
                Console.WriteLine(opt.Car_type + " - " + opt.Supplier_Id + " - " + opt.Price);
                o.RemoveAll(x => x.Price == o.Min(y => y.Price));
            }
        }

        static void Main(string[] args)
        {
            Dictionary<string, int> carTypes = new Dictionary<string, int>();
            carTypes.Add("STANDARD", 4);
            carTypes.Add("EXECUTIVE", 4);
            carTypes.Add("LUXURY", 4);
            carTypes.Add("PEOPLE_CARRIER", 6);
            carTypes.Add("LUXURY_PEOPLE_CARRIER", 6);
            carTypes.Add("MINIBUS", 16);

            Console.WriteLine("Please enter the pickup latitude and longitude.");
            Console.Write("pickup (latitude,longitude) = ");
            string pickup = Console.ReadLine();
            Console.WriteLine("Please enter the dropoff latitude and longitude.");
            Console.Write("dropoff (latitude,longitude) = ");
            string dropoff = Console.ReadLine();
            Console.WriteLine("Please enter the number of passengers.");
            Console.Write("passengers = ");
            int passengers = Convert.ToInt32(Console.ReadLine());

            string url;
            url = "https://techtest.rideways.com/dave";
            List<Option> daveOptions = getSupplierOption(url, pickup, dropoff);
            //Printed the supplier obtained after the search:
            Console.WriteLine("--------------------------------------------------------------------");
            printAll(daveOptions, "DAVE", pickup, dropoff);

            Option dave = new Option();
            Option eric = new Option();
            Option jeff = new Option();
            
            //Sort and print the search results in descending order based on price.
            List<Option> options = daveOptions.OrderByDescending(o => o.Price).ToList();
            Console.WriteLine("--------------------------------------------------------------------");
            Console.WriteLine("SORTED RESULTS (Dave's Taxis):");
            printAll(options, "DAVE", pickup, dropoff);

            //Print only the options that have enough space for the number of passengers.
            Console.WriteLine("Options for the number of passenger introduced: ");
            List<Option> seatOptions = options.Where(o => carTypes[o.Car_type] >= passengers).ToList();
            foreach (Option opt in seatOptions)
            {
                Console.WriteLine("Option: car_type = " + opt.Car_type + "; price = " + opt.Price);
            }
            Console.WriteLine("--------------------------------------------------------------------");

            //Get the cheapest option from Dave supplier (which also respects the number of
            //seats requested):
            dave = options.Find(o => o.Price == options.Min(x => x.Price) && carTypes[o.Car_type] >= passengers);
           
        //I should have the cheapest option from Dave (considering the number of passengers):
        //If there is no option (the number of passengers may be too big, dave == null.
        //Otherwise, dave = the option with the lowest price.
        if (dave != null)
        {
            Console.WriteLine("The cheapest option from Dave's Taxis is: " + dave.Car_type + " - " + "Dave" + " - " + dave.Price);
        }
        else
        {
            Console.WriteLine("Dave's Taxi does not have any convenient options.");
        }

        Console.WriteLine("--------------------------------------------------------------------");

        //Get cheapest option from Eric's Taxis
        string urlEric;
        urlEric = "https://techtest.rideways.com/eric";
        var ericSupplier = getSupplierOption(urlEric, pickup, dropoff);
        eric = ericSupplier.Find(e => e.Price == ericSupplier.Min(x => x.Price) && carTypes[e.Car_type] >= passengers);
        if (eric != null)
        {
            Console.WriteLine("The cheapest option from Eric's Taxis is: " + eric.Car_type + " - " + "Eric" + " - " + eric.Price);
                    
        }
        else
        {
            Console.WriteLine("Eric's Taxi does not have any convenient options.");
        }

        Console.WriteLine("--------------------------------------------------------------------");

        //Get cheapest option from Jeff's Taxis
        string urlJeff;
        urlJeff = "https://techtest.rideways.com/jeff";
        var jeffSupplier = getSupplierOption(urlJeff, pickup, dropoff);
        jeff = jeffSupplier.Find(j => j.Price == jeffSupplier.Min(x => x.Price) && carTypes[j.Car_type] >= passengers);
        if (jeff != null)
        {
            Console.WriteLine("The cheapest option from Jeff's Taxis is: " + jeff.Car_type + " - " + "Jeff" + " - " + jeff.Price);
        }
        else
        {
            Console.WriteLine("Jeff's Taxi does not have any convenient options.");
        }
        Console.WriteLine("--------------------------------------------------------------------");

            //Printing the options (cheapest ones and only one option taken per car type):
            List<Option> bestOptions = new List<Option>();
            if (dave != null)
            {
                dave.Supplier_Id = "Dave";
                bestOptions.Add(dave);
            }
            if (eric != null)
            {
                eric.Supplier_Id = "Eric";
                bestOptions.Add(eric);
            }
            if (jeff != null)
            {
                jeff.Supplier_Id = "Jeff";
                bestOptions.Add(jeff);
            }
            
            printSupplier(bestOptions);
            Console.WriteLine("--------------------------------------------------------------------");

            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
        }
        }
        
    }

