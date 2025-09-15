using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

class Passenger
{
    public string Name { get; set; }
    public string FlightNumber { get; set; }
    public bool HasTicket { get; set; }
    public bool PassedSecurity { get; set; }
    public bool IsOnBoard { get; set; }
}

class Flight
{
    public string FlightNumber { get; set; }
    public string Destination { get; set; }
    public int DepartureTime { get; set; }
    public string Status { get; set; } = "OnTime";
    public int Capacity { get; set; }
    public List<Passenger> PassengersOnBoard { get; set; } = new List<Passenger>();
}

class Airport
{
    public List<Flight> Flights = new List<Flight>();
    public List<Passenger> AllPassengers = new List<Passenger>();
    public Queue<Passenger> RegistrationQueue = new Queue<Passenger>();
    public Queue<Passenger> SecurityQueue = new Queue<Passenger>();

    Random rnd = new Random();
    int currentTime = 0;

    public void InitFlights()
    {
        Flights.Add(new Flight { FlightNumber = "PS101", Destination = "Kyiv", DepartureTime = 5, Capacity = 10 });
        Flights.Add(new Flight { FlightNumber = "PS202", Destination = "London", DepartureTime = 8, Capacity = 15 });
        Flights.Add(new Flight { FlightNumber = "PS303", Destination = "Warsaw", DepartureTime = 12, Capacity = 20 });
    }

    public void Tick()
    {
        currentTime++;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n--- Time: {currentTime} ---");
        Console.ResetColor();

 
        if (rnd.Next(100) < 30)
        {
            var flight = Flights[rnd.Next(Flights.Count)];
            var p = new Passenger
            {
                Name = "Passenger_" + rnd.Next(1000),
                FlightNumber = flight.FlightNumber
            };
            AllPassengers.Add(p);
            RegistrationQueue.Enqueue(p);
            Console.WriteLine($"New passenger {p.Name} arrived for flight {p.FlightNumber}");
        }

        for (int i = 0; i < 3; i++)
        {
            if (RegistrationQueue.Count > 0)
            {
                var p = RegistrationQueue.Dequeue();
                p.HasTicket = true;
                SecurityQueue.Enqueue(p);
                Console.WriteLine($"{p.Name} checked in for {p.FlightNumber}");
            }
        }

        
        for (int i = 0; i < 2; i++)
        {
            if (SecurityQueue.Count > 0)
            {
                var p = SecurityQueue.Dequeue();
                p.PassedSecurity = true;
                Console.WriteLine($"{p.Name} passed security");
            }
        }

        foreach (var f in Flights)
        {
            if (currentTime == f.DepartureTime - 2 && f.Status == "OnTime")
            {
                f.Status = "Boarding";
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Flight {f.FlightNumber} to {f.Destination} is now Boarding!");
                Console.ResetColor();
            }

            if (currentTime >= f.DepartureTime && f.Status != "Departed")
            {
                f.Status = "Departed";
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Flight {f.FlightNumber} departed!");
                Console.ResetColor();

                foreach (var p in f.PassengersOnBoard)
                {
                    AllPassengers.Remove(p);
                }
            }

            if (f.Status == "Boarding")
            {
                var ready = AllPassengers
                    .Where(p => p.FlightNumber == f.FlightNumber && p.PassedSecurity && !p.IsOnBoard)
                    .Take(5)
                    .ToList();

                foreach (var p in ready)
                {
                    if (f.PassengersOnBoard.Count < f.Capacity)
                    {
                        p.IsOnBoard = true;
                        f.PassengersOnBoard.Add(p);
                        Console.WriteLine($"{p.Name} boarded flight {f.FlightNumber}");
                    }
                }
            }
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Queue Registration: {RegistrationQueue.Count}, Security: {SecurityQueue.Count}");
        Console.ResetColor();

        foreach (var f in Flights)
        {
            Console.WriteLine($"{f.FlightNumber} to {f.Destination} - {f.Status}");
        }
    }
}

class Program
{
    static void Main()
    {
        Airport airport = new Airport();
        airport.InitFlights();

        while (true)
        {
            airport.Tick();
            Thread.Sleep(1000);
        }
    }
}
