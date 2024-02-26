namespace HashSet
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //INavigator navigator = new Navigator();

            //RHashSet<Route> routes = new RHashSet<Route>();
            //HashSet<Route> routes1 = new HashSet<Route>();

            //Route route1 = new Route("1", 100, 2, true, new List<string> { "City1", "City2" });
            //Route route2 = new Route("2", 150, 3, true, new List<string> { "City1", "City3", "City4" });
            //Route route3 = new Route("3", 120, 1, true, new List<string> { "City2", "City4" });
            //Route route4 = new Route("4", 80, 4, false, new List<string> { "City3", "City1", "City4", "City4" });
            //Route route5 = new Route("5", 200, 5, false, new List<string> { "City4", "City1" });
            //Route route6 = new Route("6", 100, 2, true, new List<string> { "City1", "City2" });

            ////Route route6 = new Route("6", 200, 5, false, new List<string> { "City1", "City2", "City3", "City4" });
            ////Route route7 = new Route("7", 100, 2, true, new List<string> { "City1", "City2" });

            //navigator.AddRoute(route1);
            //navigator.AddRoute(route2);
            //navigator.AddRoute(route3);

            //navigator.RemoveRoute("3");


            //Console.WriteLine(HashHelpers.GetPrime(0));


            ////RLinkedList<int> list = new RLinkedList<int>();
            ////list.AddFirst(1);
            ////list.AddFirst(2);
            ////list.AddFirst(3);
            ////list.AddFirst(4);

            ////var list1 = list.ToList();
            ////for (int i = 0; i < list1.Count; i++)
            ////{
            ////    Console.Write(list1[i] + " ");
            ////}


            ////list.AddLast(5);
            ////list.AddLast(6);
            ////list.AddLast(7);

            ////list1 = list.ToList();
            ////Console.WriteLine();
            ////for (int i = 0; i < list1.Count; i++)
            ////{
            ////    Console.Write(list1[i] + " ");
            ////}
            ////HashSet<int> hashSet = new HashSet<int>();

            ////RHashSet<int> rHashSet = new RHashSet<int>();

            ////rHashSet.Add(1);
            ////rHashSet.Add(1);
            ////rHashSet.Add(26);
            ////rHashSet.Add(10);
            ////rHashSet.Add(1);
            ///

            INavigator navigator = new Navigator();

            Route route1 = new Route("1", 100, 2, true, new List<string> { "City1", "City2" });
            Route route2 = new Route("2", 150, 3, true, new List<string> { "City1", "City3", "City4" });
            Route route3 = new Route("3", 120, 1, true, new List<string> { "City2", "City4" });
            Route route4 = new Route("4", 80, 4, false, new List<string> { "City3", "City1", "City4", "City4" });
            Route route5 = new Route("5", 200, 5, false, new List<string> { "City4", "City1" });
            Route route6 = new Route("6", 200, 5, false, new List<string> { "City1", "City2", "City3", "City4" });
            Route route7 = new Route("7", 100, 2, true, new List<string> { "City1", "City2" });


            navigator.AddRoute(route1);
            navigator.AddRoute(route2);
            navigator.AddRoute(route3);
            navigator.AddRoute(route4);
            navigator.AddRoute(route5);
            navigator.AddRoute(route6);
            navigator.AddRoute(route7);


            Console.WriteLine("Size : " + navigator.Size());

            Console.WriteLine("\nGet route 1: ");

            PrintRoutes(navigator.GetRoute("1"));

            Console.WriteLine("\nAdd Popularity");
            navigator.ChooseRoute("1");

            PrintRoutes(navigator.GetRoute("1"));

            Console.WriteLine("\nContains route 1: " + navigator.contains(route1));

            Console.WriteLine("\nRemove route 1");
            navigator.RemoveRoute("1");

            Console.WriteLine("\nContains route 1: " + navigator.contains(route1));

            Console.WriteLine("\nSearch Routes (City1 to City4):");
            foreach (var route in navigator.searchRoutes("City1", "City4"))
            {
                Console.WriteLine($"Id: {route.Id}, Distance: {route.Distance}, Popularity: {route.Popularity}, Favorite: {route.IsFavorite}, Locations: {string.Join(", ", route.LocationPoints)}");
            }

            Console.WriteLine("\nFavorite Routes (Destination: City4):");
            foreach (var route in navigator.getFavoriteRoutes("City4"))
            {
                Console.WriteLine($"Id: {route.Id}, Distance: {route.Distance}, Popularity: {route.Popularity}, Favorite: {route.IsFavorite}, Locations: {string.Join(", ", route.LocationPoints)}");
            }

            Console.WriteLine("\nTop 3 Routes:");
            foreach (var route in navigator.getTop3Routes())
            {
                Console.WriteLine($"Id: {route.Id}, Distance: {route.Distance}, Popularity: {route.Popularity}, Favorite: {route.IsFavorite}, Locations: {string.Join(", ", route.LocationPoints)}");
            }

        }
        static void PrintRoutes(Route route)
        {
            Console.WriteLine($"Id: {route.Id}, Distance: {route.Distance}, Popularity: {route.Popularity}, Favorite: {route.IsFavorite}, Locations: [{string.Join(", ", route.LocationPoints)}]");
        }
    }
}
