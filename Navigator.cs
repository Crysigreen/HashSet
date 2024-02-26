using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashSet
{
    internal class Navigator : INavigator
    {
        private readonly HashSet<Route> routesHashSet1;
        private readonly RHashSet<Route> routesHashSet;

        public Navigator()
        {
            routesHashSet1 = new HashSet<Route>();
            routesHashSet = new RHashSet<Route>();
        }

        public void AddRoute(Route route)
        {
            //routesHashSet.Add(route);
            foreach (var rout in routesHashSet)
            {

                var t = Equals(route, rout);
                if (t)
                {
                    Console.WriteLine(t);
                    return;
                }
                
            }

            routesHashSet.Add(route);
        }

        public void RemoveRoute(string routeId)
        {
            Route routeToRemove = null;

            foreach (var route in routesHashSet)
            {
                if (route.Id == routeId)
                {
                    routeToRemove = route;
                    break;
                }
            }

            if (routeToRemove != null)
            {
                routesHashSet.Remove(routeToRemove);
            }
        }

        public bool contains(Route route)
        {
            return routesHashSet.Contains(route);
        }

        public int Size()
        {
            return routesHashSet.Count;
        }

        public Route GetRoute(string routeId)
        {
            foreach (var route in routesHashSet)
            {
                if (route.Id == routeId)
                {
                    return route;
                }
            }

            return null;
        }

        public void ChooseRoute(string routeId)
        {
            foreach (var route in routesHashSet)
            {
                if (route.Id == routeId)
                {
                    route.Popularity++;
                    break;
                }
            }
        }



        public IEnumerable<Route> searchRoutes(string startPoint, string endPoint)
        {
            var matchingRoutes = routesHashSet
                .Where(r => r.LocationPoints.First() == startPoint && r.LocationPoints.Last() == endPoint)
                .ToList();

            if (matchingRoutes.Count == 0)
            {
                return Enumerable.Empty<Route>();
            }

            return matchingRoutes
                .OrderBy(r => CalculateDistance(startPoint, endPoint, r.LocationPoints))
                .ThenByDescending(r => r.Popularity)
                .ThenByDescending(r => r.IsFavorite)
                .ToList();
        }

        // Метод для расчета расстояния между точками в маршруте
        private int CalculateDistance(string startPoint, string endPoint, List<string> routePoints)
        {
            int startIndex = routePoints.IndexOf(startPoint);
            int endIndex = routePoints.IndexOf(endPoint);

            if (startIndex == -1 || endIndex == -1)
            {
                // Неверные точки, вернуть максимальное значение для отсечения
                return int.MaxValue;
            }

            return Math.Abs(endIndex - startIndex);
        }

        public IEnumerable<Route> getFavoriteRoutes(string destinationPoint)
        {
            return routesHashSet
                .Where(r => r.IsFavorite && r.LocationPoints.Contains(destinationPoint) && !r.LocationPoints.First().Equals(destinationPoint))
                .OrderBy(r => r.Distance)
                .ThenByDescending(r => r.Popularity)
                .ToList();
        }

        public IEnumerable<Route> getTop3Routes()
        {
            return routesHashSet
                .OrderByDescending(r => r.Popularity)
                .ThenBy(r => r.Distance)
                .ThenBy(r => r.LocationPoints.Count)
                .Take(3)
                .ToList();
        }


    }
}
