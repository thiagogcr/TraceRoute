using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RouteCalc
{
    class Program
    {
        static void Main(string[] args)
        {
            Location locationA = new Location("A");
            Location locationB = new Location("B");
            Location locationC = new Location("C");
            Location locationD = new Location("D");
            Location locationE = new Location("E");
            Location locationF = new Location("F");
            Location locationG = new Location("G");
            Location locationH = new Location("H");

            RouteEngine engine = new RouteEngine();
            engine.Locations.Add(locationA);
            engine.Locations.Add(locationB);
            engine.Locations.Add(locationC);
            engine.Locations.Add(locationD);
            engine.Locations.Add(locationE);
            engine.Locations.Add(locationF);
            engine.Locations.Add(locationG);
            engine.Locations.Add(locationH);
            engine.Connections.Add( new Connection(locationA, locationB, 5));
            engine.Connections.Add( new Connection(locationA, locationF, 3));
            engine.Connections.Add( new Connection(locationB, locationC, 2));
            engine.Connections.Add( new Connection(locationB, locationG, 3));
            engine.Connections.Add( new Connection(locationC, locationH, 10));
            engine.Connections.Add( new Connection(locationC, locationD, 6));
            engine.Connections.Add( new Connection(locationD, locationE, 3));
            engine.Connections.Add( new Connection(locationE, locationF, 8));
            engine.Connections.Add( new Connection(locationF, locationG, 7));
            engine.Connections.Add( new Connection(locationG, locationH, 2));
            engine.Connections.Add( new Connection(locationH, locationE, 5));

            Console.WriteLine(engine.CalculateMinCost(locationF));
            
        }
    }
}
