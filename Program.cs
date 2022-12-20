using System;
using System.Collections.Generic;
using System.Linq;
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

           2. The goal is to create a MarketingEngine class sending all events through the constructor as parameter and make it print the events that are happening in the same city as the customer. To do that, inside this class, create a SendCustomerNotifications method which will receive a customer as parameter and an Event parameter and will mock the the Notification Service API. DON’T MODIFY THIS METHOD, unless you want to add the price to the console.writeline for task 7. Add this ConsoleWriteLine inside the Method to mock the service. Inside this method you can add the code you need to run this task correctly but you cant modify the console writeline: Console.WriteLine($"{customer.Name} from {customer.City} event {e.Name} at {e.Date}");

            3. As part of a new campaign, we need to be able to let customers know about events that are coming up close to their next birthday. You can make a guess and add it to the MarketingEngine class if you want to. So we still want to keep how things work now, which is that we email customers about events in their city or the event closest to next customer's birthday, and then we email them again at some point during the year. The current customer, his birthday is on may. So it's already in the past. So we want to find the next one, which is 23. How would you like the code to be built? We don't just want functionality; we want more than that. We want to know how you plan to make that work. Please code it.

            4. The next requirement is to extend the solution to be able to send notifications for the five closest events to the customer. The interviewer here can paste a method to help you, or ask you to search it. We will attach a way to calculate the distance.

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
                new Event(1, "Phantom of the Opera", "New York",300, new DateTime(2023,12,23)),
                new Event(2, "Metallica", "Los Angeles",556, new DateTime(2023,12,02)),
                new Event(3, "Metallica", "New York",600, new DateTime(2023,12,06)),
                new Event(4, "Metallica", "Boston", 700,new DateTime(2023,10,23)),
                new Event(5, "LadyGaGa", "New York", 400,new DateTime(2023,09,20)),
                new Event(6, "LadyGaGa", "Boston", 900,new DateTime(2023,08,01)),
                new Event(7, "LadyGaGa", "Chicago", 200,new DateTime(2023,07,04)),
                new Event(8, "LadyGaGa", "San Francisco", 500,new DateTime(2023,07,07)),
                new Event(9, "LadyGaGa", "Washington", 750,new DateTime(2023,05,22)),
                new Event(10, "Metallica", "Chicago", 450,new DateTime(2023,01,01)),
                new Event(11, "Phantom of the Opera", "San Francisco", 530, new DateTime(2023,07,04)),
                new Event(12, "Phantom of the Opera", "Chicago", 890,new DateTime(2024,05,15))
            };

            var customer = new Customer()
            {
                Id = 1,
                Name = "John",
                City = "New York",
                BirthDate = new DateTime(1995, 05, 10)
            };
            MarketingEngine engine = new MarketingEngine(events);
            engine.SendEventsInSameCity(customer);
            engine.SendBirthDayEvent(customer, events, 5);
            engine.SendEventsInClosestCities(customer, events, 5);

        }

        public class MarketingEngine
        {
            private List<Event> _events;
            public static Dictionary<(string, string), int> Distances { get; } = new();
            public MarketingEngine(List<Event> events)
            {
                _events = events;
            }

            public void SendCustomerNotifications(Customer customer, Event e)
            {
                Console.WriteLine($"{customer.Name} from {customer.City} event {e.Name} at {e.Date} in {e.City}");
            }

            public void SendEventsInSameCity(Customer customer)
            {
                var eventsInSameCity = _events.Where(e => e.City == customer.City);
                foreach (var e in eventsInSameCity)
                {
                    this.SendCustomerNotifications(customer, e);
                }
            }

            public void SendBirthDayEvent(Customer customer, List<Event> events, int nEvents)
            {
                var nextBirthDayEvents = events.Where(e => (e.Date.Year == DateTime.Now.Year && e.Date.Month == customer.BirthDate.Month && e.Date.Day > customer.BirthDate.Day) || (e.Date.Year == DateTime.Now.Year && e.Date.Month > customer.BirthDate.Month) || (e.Date.Year > DateTime.Now.Year)).OrderBy(e => e.Date).Take(nEvents);
                foreach (var e in nextBirthDayEvents)
                {
                    SendCustomerNotifications(customer, e);
                }
            }

            public void SendEventsInClosestCities(Customer customer, List<Event> events, int nEvents)
            {
                var _events = events.OrderBy(e => GetDistance(customer.City, e.City)).Take(nEvents);
                foreach (var e in events)
                {
                    SendCustomerNotifications(customer, e);
                }
            }

            public void SendSortedEvents(Customer customer, Func<Event, int> func, int nEvents = 0)
            {
                var events = _events.OrderBy(func).Take(nEvents);
                foreach (var e in events)
                {
                    SendCustomerNotifications(customer, e);
                }
            }

            private static int GetDistance(string cityA, string cityB, int tries = 1)
            {
                if (Distances.TryGetValue((cityA, cityB), out var distance)) return distance;
                while (tries > 0)
                {
                    try
                    {
                        distance = AlphebiticalDistance(cityA, cityB);
                        break;
                    }
                    catch (TimeoutException e)
                    {
                        Console.WriteLine($"Got {e} with {cityA} and {cityB}");
                        if (tries < 2)
                            throw;
                    }
                }
                Distances[(cityA, cityB)] = distance;
                Distances[(cityB, cityA)] = distance;

                return distance;
            }

            private static int AlphebiticalDistance(string s, string t)
            {
                var result = 0;
                var i = 0;
                for (i = 0; i < Math.Min(s.Length, t.Length); i++)
                {
                    result += Math.Abs(s[i] - t[i]);
                }
                for (; i < Math.Max(s.Length, t.Length); i++)
                {
                    result += s.Length > t.Length ? s[i] : t[i];
                }

                return result;
            }
        }

        public class Event
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string City { get; set; }
            public decimal Price { get; set; }
            public DateTime Date { get; set; }

            public Event(int id, string name, string city, decimal price, DateTime date)
            {
                this.Id = id;
                this.Name = name;
                this.City = city;
                this.Price = price;
                this.Date = date;
            }
        }

        public class Customer
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string City { get; set; }
            public DateTime BirthDate { get; set; }
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

