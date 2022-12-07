using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using static TicketsConsole.Program;

/*

Let's say we're running a small entertainment business as a start-up. This means we're selling tickets to live events on a website. An email campaign service is what we are going to make here. We're building a marketing engine that will send notifications (emails, text messages) directly to the client and we'll add more features as we go.

Please, instead of debuging with breakpoints, debug with "Console.Writeline();" for each task because the Interview will be in Coderpad and in that platform you cant do Breakpoints.

*/
namespace TicketsConsole
{
    internal class Program
    {

        static void Main(string[] args)
        {
            /*

            1. You can see here a list of events, a customer object. Try to understand the code, make it compile. 

            2. The goal is to create a MarketingEngine class sending all events through the constructor as parameter and make it print the events that are happening 
            in the same city as the customer. To do that, inside this class, create a SendCustomerNotifications method which will receive a customer as parameter and
             an Event parameter and will mock the the Notification Service API. DON’T MODIFY THIS METHOD, unless you want to add the price to the console.writeline for task 7. 
             Add this ConsoleWriteLine inside the Method to mock the service. Inside this method you can add the code you need to run this task correctly but you cant modify 
             the console writeline: Console.WriteLine($"{customer.Name} from {customer.City} event {e.Name} at {e.Date}");

            3. As part of a new campaign, we need to be able to let customers know about events that are coming up close to their next birthday. You can make a guess and 
            add it to the MarketingEngine class if you want to. So we still want to keep how things work now, which is that we email customers about events in their city 
            or the event closest to next customer's birthday, and then we email them again at some point during the year. The current customer, his birthday is on may. 
            So it's already in the past. So we want to find the next one, which is 23. How would you like the code to be built? We don't just want functionality; we want more 
            than that. We want to know how you plan to make that work. Please code it.

            4. The next requirement is to extend the solution to be able to send notifications for the five closest events to the customer. The interviewer here can paste a 
            method to help you, or ask you to search it. We will attach a way to calculate the distance.
            public record City(string Name, int X, int Y);
            |public static readonly IDictionary<string, City> Cities = new Dictionary<string, City>()
        {
            { "New York", new City("New York", 3572, 1455) },
            { "Los Angeles", new City("Los Angeles", 462, 975) },
            { "San Francisco", new City("San Francisco", 183, 1233) },
            { "Boston", new City("Boston", 3778, 1566) },
            { "Chicago", new City("Chicago", 2608, 1525) },
            { "Washington", new City("Washington", 3358, 1320) },
        };
var customerCityInfo = Cities.Where(c => c.Key == city).Single().Value;
var distance = Math.Abs(customerCityInfo.X - eventCityInfo.X) + Math.Abs(customerCityInfo.Y - eventCityInfo.Y);

            5. If the calculation of the distances is an API call which could fail or is too expensive, how will you improve the code written in 4? Think in caching the data which could be code it as a dictionary. You need to store the distances between two cities. Example:

            New York - Boston => 400 
            Boston - Washington => 540
            Boston - New York => Should not exist because "New York - Boston" is already stored and the distance is the same. 

            6. If the calculation of the distances is an API call which could fail, what can be done to avoid the failure? Think in HTTPResponse Answers: Timeoute, 404, 403. How can you handle that exceptions? Code it.

            7.  If we also want to sort the resulting events by other fields like price, etc. to determine whichones to send to the customer, how would you implement it? Code it.
            */

            var events = new List<Event>{
                new Event(1, "Phantom of the Opera", "New York", new DateTime(2023,12,23), 4500),
                new Event(2, "Metallica", "Los Angeles", new DateTime(2023,12,02), 800),
                new Event(3, "Metallica", "New York", new DateTime(2023,12,06), 400),
                new Event(4, "Metallica", "Boston", new DateTime(2023,10,23), 2500),
                new Event(5, "LadyGaGa", "New York", new DateTime(2023,09,20), 1600),
                new Event(6, "LadyGaGa", "Boston", new DateTime(2023,08,01), 900),
                new Event(7, "LadyGaGa", "Chicago", new DateTime(2023,07,04), 600),
                new Event(8, "LadyGaGa", "San Francisco", new DateTime(2023,07,07), 300),
                new Event(9, "LadyGaGa", "Washington", new DateTime(2023,05,22), 1200),
                new Event(10, "Metallica", "Chicago", new DateTime(2023,01,01), 500),
                new Event(11, "Phantom of the Opera", "San Francisco", new DateTime(2023,07,04), 400),
                new Event(12, "Phantom of the Opera", "Chicago", new DateTime(2024,05,15), 800),
            };

            var customer = new Customer()
            {
                Id = 1,
                Name = "John",
                City = "New York",
                BirthDate = new DateTime(1995, 05, 10)
            };
            MarketingEngine marketingEngine = new MarketingEngine(events);
            foreach (var e in events)
            {
                marketingEngine.SendCustomerNotifications(customer, e);
            }
        }

        public class MarketingEngine
        {
            List<Event> _events;
            List<Event> cachedEvents = new List<Event>();
            public MarketingEngine(List<Event> events)
            {
                _events = events;
            }
            public void SendCustomerNotifications(Customer customer, Event e)
            {
                List<Event> customerEvents = new List<Event>();
                if (customer.BirthDate.Month == e.Date.Month && e.Date.Day >= customer.BirthDate.Day && e.Date.Year == DateTime.Now.Year)
                {
                    //send email for customer about event that's close to his/her birthday this year
                    //Console.WriteLine($"{customer.Name} from {customer.City} event {e.Name} at {e.Date} - BirthDay");
                    customerEvents.Add(e);
                }
                else if (customer.BirthDate.Month == e.Date.Month && e.Date.Year == (DateTime.Now.Year + 1))
                {
                    //send email for customer about event that's close to his/her birthday next year
                    //Console.WriteLine($"{customer.Name} from {customer.City} event {e.Name} at {e.Date} - BirthDay");
                    customerEvents.Add(e);
                }
                if (customer.City == e.City && e.Date > DateTime.Now)
                {
                    //send email for customer about future events in his/her city
                    //Console.WriteLine($"{customer.Name} from {customer.City} event {e.Name} at {e.Date} - Same City");
                    customerEvents.Add(e);
                }

                //considered as external api call
                List<Event> topFiveEventsInClosestCities = new List<Event>();
                try
                {
                    if (cachedEvents.Count > 0)
                        topFiveEventsInClosestCities = cachedEvents;
                    else
                    {
                        topFiveEventsInClosestCities = this.GetFiveEventsInClosestCities(customer, _events);
                        cachedEvents = topFiveEventsInClosestCities;
                    }
                }
                catch (WebException ex)
                {
                    //log issue using loggers
                    Console.WriteLine($"{ex.Status}: API call failed due to: {ex.Message}");
                    topFiveEventsInClosestCities = new List<Event>();
                }

                if (topFiveEventsInClosestCities.FirstOrDefault(_e => _e.Id == e.Id) != null)
                {
                    //send email for customer about events that will happen in 5 closest cities
                    //Console.WriteLine($"{customer.Name} from {customer.City} event {e.Name} at {e.Date} - Closest City");
                    customerEvents.Add(e);
                }

                //sort events by price
                //SortEventsByPrice(customerEvents);

                SendNotification(customer, customerEvents.OrderBy(e => e.Price).ToList());
            }

            private void SendNotification(Customer customer, List<Event> events)
            {
                foreach (var e in events)
                {
                    Console.WriteLine($"{customer.Name} from {customer.City} event {e.Name} at {e.Date} - Closest City");
                }
            }

            private List<Event> SortEventsByPrice(List<Event> events)
            {
                return events.OrderBy(e => e.Price).ToList();
            }

            private List<Event> GetFiveEventsInClosestCities(Customer customer, List<Event> events)
            {
                var customerCityInfo = Cities.Where(c => c.Key == customer.City).Single().Value;
                var distances = events.Select(e => Math.Abs(customerCityInfo.X - Cities.Where(c => c.Key == e.City).Single().Value.X) + Math.Abs(customerCityInfo.Y - Cities.Where(c => c.Key == e.City).Single().Value.Y)).OrderBy(e => e).Take(5).ToList();
                return events.Select(e => e)
                             .OrderBy(e => Math.Abs(customerCityInfo.X - Cities.Where(c => c.Key == e.City).Single().Value.X) + Math.Abs(customerCityInfo.Y - Cities.Where(c => c.Key == e.City).Single().Value.Y))
                             .Take(5)
                             .ToList();
            }

            private static readonly IDictionary<string, City> Cities = new Dictionary<string, City>()
            {
                { "New York", new City("New York", 3572, 1455) },
                { "Los Angeles", new City("Los Angeles", 462, 975) },
                { "San Francisco", new City("San Francisco", 183, 1233) },
                { "Boston", new City("Boston", 3778, 1566) },
                { "Chicago", new City("Chicago", 2608, 1525) },
                { "Washington", new City("Washington", 3358, 1320) },
            };
        }

        public class Event
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string City { get; set; }
            public DateTime Date { get; set; }
            public double Price { get; set; }

            public Event(int id, string name, string city, DateTime date, double price)
            {
                this.Id = id;
                this.Name = name;
                this.City = city;
                this.Date = date;
                this.Price = price;
            }
        }

        public class Customer
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string City { get; set; }
            public DateTime BirthDate { get; set; }
        }

        public class City
        {
            public string Name { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public City(string name, int x, int y)
            {
                this.Name = name;
                this.X = y;
                this.Y = y;
            }
        }


        /*-------------------------------------
        Coordinates are roughly to scale with miles in the USA
           2000 +----------------------+  
                |                      |  
                |                      |  
             Y  |                      |  
                |                      |  
                |                      |  
                |                      |  
                |                      |  
             0  +----------------------+  
                0          X          4000
        ---------------------------------------*/
    }
}
