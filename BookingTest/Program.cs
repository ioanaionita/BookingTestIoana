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
            //Give Dave's Taxis supplier API 2 seconds (2000 miliseconds) to respond:
            wrGETURL.Timeout = 2000;

            Stream objStream;
            StreamReader readStream;
            string jsonData;
            int price, i;
            Option dave = new Option();
            Option eric = new Option();
            Option jeff = new Option();
            try
            {
                objStream = wrGETURL.GetResponse().GetResponseStream();
                readStream = new StreamReader(objStream, Encoding.UTF8);
                Console.WriteLine("Search finished.");
                jsonData = readStream.ReadToEnd();
                var mySupplier = JsonConvert.DeserializeObject<Supplier>(jsonData);

                //Printed the supplier obtained after the search:
                Console.WriteLine("--------------------------------------------------------------------");
                Console.WriteLine("SEARCH RESULTS (from Dave's Taxis):");
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
                Console.WriteLine("SORTED RESULTS (Dave's Taxis):");
                foreach (Option opt in options)
                {
                    Console.WriteLine("Option: car_type = " + opt.Car_type + "; price = " + opt.Price);
                }
                Console.WriteLine("--------------------------------------------------------------------");

                //Print only the options that have enough space for the number of passengers. 
                Console.WriteLine("Options for the number of passenger introduced: ");
                foreach (Option opt in options)
                {
                    int optPassengers = carTypes[opt.Car_type];
                    if (optPassengers >= passengers)
                    {
                        Console.WriteLine("Option: car_type = " + opt.Car_type + "; price = " + opt.Price);
                    }

                }
                Console.WriteLine("--------------------------------------------------------------------");

                //Get the cheapest option from Dave supplier:
                price = 0;
                i = 1;
                foreach (Option opt in options)
                {
                    if (i == 1)
                    {
                        price = opt.Price;
                        i = 0;
                    }
                    int optPassengers = carTypes[opt.Car_type];
                    if (optPassengers >= passengers)
                    {
                        if (opt.Price <= price)
                        {
                            price = opt.Price;
                            dave = opt;
                        }
                    }
                }
                //I should have the cheapest option from Dave (considering the number of passengers):
                //If there is no option (the number of passengers may be too big, dave == null.
                //Otherwise, dave = the option with the lowest price.
                if (dave != null)
                {
                    Console.WriteLine("The cheapest option from Dave's Taxis is: " + dave.Car_type + " - " + "Dave" + " - " + dave.Price);
                    Console.WriteLine("--------------------------------------------------------------------");
                }  
                objStream.Close();
                readStream.Close();
            }
            catch (Exception e)
            { }
                //Get cheapest option from Eric's Taxis
                string urlEric;
                urlEric = "https://techtest.rideways.com/eric";
                var uriBuilderEric = new UriBuilder(urlEric);
                uriBuilderEric.Query = query.ToString();

                WebRequest wrGETURLEric;
                wrGETURLEric = WebRequest.Create(uriBuilderEric.Uri);
                //Give Eric's Taxis supplier API 2 seconds (2000 miliseconds) to respond:
                wrGETURLEric.Timeout = 2000;
            try
            {
                objStream = wrGETURLEric.GetResponse().GetResponseStream();

                readStream = new StreamReader(objStream, Encoding.UTF8);

                jsonData = readStream.ReadToEnd();
                var ericSupplier = JsonConvert.DeserializeObject<Supplier>(jsonData);
                price = 0;
                i = 1;
                foreach (Option opt in ericSupplier.Options)
                {
                    if (i == 1)
                    {
                        price = opt.Price;
                        i = 0;
                    }
                    int optPassengers = carTypes[opt.Car_type];
                    if (optPassengers >= passengers)
                    {
                        if (opt.Price <= price)
                        {
                            price = opt.Price;
                            eric = opt;
                        }
                    }
                }
                if (eric != null)
                {
                    Console.WriteLine("The cheapest option from Eric's Taxis is: " + eric.Car_type + " - " + "Eric" + " - " + eric.Price);
                    Console.WriteLine("--------------------------------------------------------------------");
                }
                objStream.Close();
                readStream.Close();
            }
            catch (Exception ex)
            { }
                    //Get cheapest option from Jeff's Taxis
                    string urlJeff;
                    urlJeff = "https://techtest.rideways.com/jeff";
                    var uriBuilderJeff = new UriBuilder(urlJeff);
                    uriBuilderJeff.Query = query.ToString();

                    WebRequest wrGETURLJeff;
                    wrGETURLJeff = WebRequest.Create(uriBuilderJeff.Uri);
                    //Give Jeff's Taxis supplier API 2 seconds (2000 miliseconds) to respond:
                    wrGETURLJeff.Timeout = 2000;
            try
            {
                objStream = wrGETURLJeff.GetResponse().GetResponseStream();

                readStream = new StreamReader(objStream, Encoding.UTF8);

                jsonData = readStream.ReadToEnd();
                var jeffSupplier = JsonConvert.DeserializeObject<Supplier>(jsonData);
                price = 0;
                i = 1;
                foreach (Option opt in jeffSupplier.Options)
                {
                    if (i == 1)
                    {
                        price = opt.Price;
                        i = 0;
                    }
                    int optPassengers = carTypes[opt.Car_type];
                    if (optPassengers >= passengers)
                    {
                        if (opt.Price <= price)
                        {
                            price = opt.Price;
                            jeff = opt;
                        }
                    }
                }
                if (jeff != null)
                {
                    Console.WriteLine("The cheapest option from Jeff's Taxis is: " + jeff.Car_type + " - " + "Jeff" + " - " + jeff.Price);
                    Console.WriteLine("--------------------------------------------------------------------");
                }
                objStream.Close();
                readStream.Close();
            }
            catch (Exception exp) { }


            bool daveB = false;
            bool ericB = false;
            bool jeffB = false;

            if(dave != null)
            {
                if(eric == null || dave.Car_type != eric.Car_type)
                {
                    if(jeff == null || dave.Car_type != jeff.Car_type)
                    {
                        daveB = true;
                    }
                    else
                    {
                        if(dave.Price < jeff.Price)
                        {
                            daveB = true;
                        }
                        else
                        {
                            daveB = false;
                        }
                    }
                }
                else
                {
                    if(dave.Price < eric.Price)
                    {
                        daveB = true;
                    }
                    else
                    {
                        daveB = false;
                    }
                }
            }
            if(eric != null)
            {
                if(dave == null || dave.Car_type != eric.Car_type)
                {
                    if(jeff == null || jeff.Car_type != eric.Car_type)
                    {
                        ericB = true;
                    }
                    else
                    {
                        if(eric.Price < jeff.Price)
                        {
                            ericB = true;
                        }
                        else
                        {
                            ericB = false;
                        }
                    }
                }
                else
                {
                    if(eric.Price < dave.Price)
                    {
                        ericB = true;
                    }
                    else
                    {
                        ericB = false;
                    }
                }
            }
            if (daveB)
            {
                Console.WriteLine("The cheapest option from Dave's Taxis is: " + dave.Car_type + " - " + "Dave" + " - " + dave.Price);
            }
            if (ericB)
            {
                Console.WriteLine("The cheapest option from Eric's Taxis is: " + eric.Car_type + " - " + "Eric" + " - " + eric.Price);
            }
            if (jeffB)
            {
                Console.WriteLine("The cheapest option from Jeff's Taxis is: " + jeff.Car_type + " - " + "Jeff" + " - " + jeff.Price);
            }

            Console.WriteLine("--------------------------------------------------------------------");

            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
        }
        }
        
    }

