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
        public string Car_type { get; set; }
        public int Price { get; set; }
    }
    class Program
    {                    

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
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["pickup"] = pickup;
            query["dropoff"] = dropoff;
            uriBuilder.Query = query.ToString();
           
            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(uriBuilder.Uri);
            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();

            StreamReader readStream = new StreamReader(objStream, Encoding.UTF8);

            Console.WriteLine("Search finished.");
            string jsonData = readStream.ReadToEnd();
            var mySupplier = JsonConvert.DeserializeObject<Supplier>(jsonData);

            //Printed the supplier obtained after the search:
            Console.WriteLine("--------------------------------------------------------------------");
            Console.WriteLine("SEARCH RESULTS:");
            Console.WriteLine("Supplier_id: " + mySupplier.Supplier_id);
            Console.WriteLine("Pickup: " + mySupplier.Pickup);
            Console.WriteLine("Dropoff: " + mySupplier.Dropoff);
            foreach (Option opt in mySupplier.Options)
            {
                Console.WriteLine("Option: car_type = " + opt.Car_type + "; price = " + opt.Price);
            }
            Console.WriteLine("--------------------------------------------------------------------");

            //Sort and print the search results in descending order based on price.
            List<Option> options = mySupplier.Options.OrderByDescending(o => o.Price).ToList();
            Console.WriteLine("SORTED RESULTS:");
            foreach(Option opt in options)
            {
                Console.WriteLine("Option: car_type = " + opt.Car_type + "; price = " + opt.Price);
            }
            Console.WriteLine("--------------------------------------------------------------------");

            //Print only the options that have enough space for the number of passengers. 
            Console.WriteLine("Options for the number of passenger introduced: ");
            foreach(Option opt in options)
            {
                int optPassengers = carTypes[opt.Car_type];
                if(optPassengers >= passengers)
                {
                    Console.WriteLine("Option: car_type = " + opt.Car_type + "; price = " + opt.Price);
                }

            }
            Console.WriteLine("--------------------------------------------------------------------");

            //

            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
            objStream.Close();
            readStream.Close();
        }

        
    }
}
