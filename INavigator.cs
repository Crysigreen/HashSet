using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashSet
{
    internal interface INavigator
    {
        void AddRoute(Route route);
        void RemoveRoute(string id);
        bool contains(Route route);
        int Size();
        Route GetRoute(string id);
        void ChooseRoute(string id);
        IEnumerable<Route> searchRoutes(string startPoint, string endPoint);
        IEnumerable<Route> getFavoriteRoutes(string destinationPoint);
        IEnumerable<Route> getTop3Routes();
    }
}
