namespace HashSet
{
    internal class Program
    {
        static void Main(string[] args)
        {
            INavigator navigator = new Navigator();

            RHashSet<Route> routes = new RHashSet<Route>();
            HashSet<Route> routes1 = new HashSet<Route>();

            Route route1 = new Route("1", 100, 2, true, new List<string> { "City1", "City2" });
            Route route2 = new Route("2", 150, 3, true, new List<string> { "City1", "City3", "City4" });
            Route route3 = new Route("3", 120, 1, true, new List<string> { "City2", "City4" });
            Route route4 = new Route("4", 80, 4, false, new List<string> { "City3", "City1", "City4", "City4" });
            Route route5 = new Route("5", 200, 5, false, new List<string> { "City4", "City1" });
            Route route6 = new Route("6", 100, 2, true, new List<string> { "City1", "City2" });

            //Route route6 = new Route("6", 200, 5, false, new List<string> { "City1", "City2", "City3", "City4" });
            //Route route7 = new Route("7", 100, 2, true, new List<string> { "City1", "City2" });

            routes.Add(route1);
            routes1.Add(route1);
            routes.Add(route6);
            routes1.Add(route6);
            routes.Add(route2);
            routes1.Add(route2);
            routes.Add(route3);
            routes1.Add(route3);
            Console.WriteLine(routes.Contains(route1));
            routes.Remove(route6);
            routes1.Remove(route6);

            routes.Clear();
            routes1.Clear();

            
            Console.WriteLine(HashHelpers.GetPrime(0));


            //RLinkedList<int> list = new RLinkedList<int>();
            //list.AddFirst(1);
            //list.AddFirst(2);
            //list.AddFirst(3);
            //list.AddFirst(4);

            //var list1 = list.ToList();
            //for (int i = 0; i < list1.Count; i++)
            //{
            //    Console.Write(list1[i] + " ");
            //}


            //list.AddLast(5);
            //list.AddLast(6);
            //list.AddLast(7);

            //list1 = list.ToList();
            //Console.WriteLine();
            //for (int i = 0; i < list1.Count; i++)
            //{
            //    Console.Write(list1[i] + " ");
            //}
            //HashSet<int> hashSet = new HashSet<int>();

            //RHashSet<int> rHashSet = new RHashSet<int>();

            //rHashSet.Add(1);
            //rHashSet.Add(1);
            //rHashSet.Add(26);
            //rHashSet.Add(10);
            //rHashSet.Add(1);
        }
    }
}
