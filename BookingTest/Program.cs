using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Script.Services;
using System.Web.Services;

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

    public class Program
    {
        [WebMethod]
        public static List<Option> GetSupplierOption (string url, string pickup, string dropoff)
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
                jsonData = readStream.ReadToEnd();
                var mySupplier = JsonConvert.DeserializeObject<Supplier>(jsonData);
                return mySupplier.Options;
            }
            catch(Exception e){}
            List<Option> o = new List<Option>();
            return o;
        }

        static void PrintAll(List<Option> o, string supplier, string pickup, string dropoff)
        {
            if(o.Count() == 0)
            {
                Console.WriteLine(supplier + "'s " + " Taxi's " + "is not available at the moment.");
                return;
            }
            Console.WriteLine("SEARCH RESULTS FOR " + supplier);
            Console.WriteLine("Pickup: " + pickup);
            Console.WriteLine("Dropoff: " + dropoff);
            foreach(Option opt in o)
            {
                Console.WriteLine("Option: car_type = " + opt.Car_type + "; price = " + opt.Price);
            }
        }

        static void PrintSupplier(List<Option> o)
        {
            int i = 0;
            while(o.Count() != 0)
            {
                if(i == 0)
                {
                    Console.WriteLine("The best options are:");
                    i++;
                }

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

            //Print the results for Dave's Taxi, Eric's Taxi or Jeff's Taxi:
            Console.WriteLine("Please choose which taxi results you want to get: ");
            Console.WriteLine("dave/jeff/eric/none");
            string taxi = Console.ReadLine();
            
            
            string name = "";

            switch (taxi)
            {
                case "dave":
                    {
                        name = "DAVE";
                        break;
                    }
                case "eric":
                    {
                        name = "ERIC";
                        break;
                    }
                case "jeff":
                    {
                        name = "JEFF";
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            //If there is a request to a specific taxi supplier, 
            //then print the options (obtained in maximum 2 secs) in descending order:
            int passengers = 0;
            List<Option> options = new List<Option>();
            List<Option> daveOptions = GetSupplierOption("https://techtest.rideways.com/dave", pickup, dropoff).OrderByDescending(o => o.Price).ToList();
            List<Option> ericOptions = GetSupplierOption("https://techtest.rideways.com/eric", pickup, dropoff).OrderByDescending(o => o.Price).ToList();
            List<Option> jeffOptions = GetSupplierOption("https://techtest.rideways.com/jeff", pickup, dropoff).OrderByDescending(o => o.Price).ToList();
            if (name != "")
            {

                Console.WriteLine("--------------------------------------------------------------------");
                //Sort and print the search results in descending order based on price.
                Console.WriteLine(name + "'s Taxis options:");
                switch(name)
                {
                    case "DAVE":
                        options = daveOptions;
                        break;
                    case "ERIC":
                        options = ericOptions;
                        break;
                    case "JEFF":
                        options = jeffOptions;
                        break;
                    default:
                        break;
                }
                PrintAll(options, name, pickup, dropoff);
                Console.WriteLine("--------------------------------------------------------------------");

                Console.WriteLine("Please enter the number of passengers.");
                Console.Write("passengers = ");
                passengers = Convert.ToInt32(Console.ReadLine());

                //Print only the options that have enough space for the number of passengers.
                List<Option> seatOptions = options.Where(o => carTypes[o.Car_type] >= passengers).ToList();
                if(seatOptions.Count() == 0)
                {
                    Console.WriteLine("There is no option for the number of passengers introduced.");
                }
                else
                {
                    Console.WriteLine("Options for the number of passenger introduced: ");
                }
                foreach (Option opt in seatOptions)
                {
                    Console.WriteLine("Option: car_type = " + opt.Car_type + "; price = " + opt.Price);
                }
                Console.WriteLine("--------------------------------------------------------------------");
               
            }

            //Get the cheapest option from Dave supplier (which also respects the number of
            //seats requested):
            Option dave = daveOptions.Find(o => o.Price == daveOptions.Min(x => x.Price) && carTypes[o.Car_type] >= passengers);

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
            Option eric = ericOptions.Find(o => o.Price == ericOptions.Min(x => x.Price) && carTypes[o.Car_type] >= passengers);
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
            Option jeff = jeffOptions.Find(o => o.Price == jeffOptions.Min(x => x.Price) && carTypes[o.Car_type] >= passengers);
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
            
            PrintSupplier(bestOptions);
            Console.WriteLine("--------------------------------------------------------------------");

            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
            
        }
        }
        
    }

