using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashSet
{
    public class Route
    {
        public string Id { get; set; }
        public double Distance { get; set; }
        public int Popularity { get; set; }
        public bool IsFavorite { get; set; }
        public List<string> LocationPoints { get; set; }

        public Route(string id, double distance, int popularity, bool isFavorite, List<string> locationPoints)
        {
            Id = id;
            Distance = distance;
            Popularity = popularity;
            IsFavorite = isFavorite;
            LocationPoints = locationPoints;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Route otherRoute = (Route)obj;

            return LocationPoints.First() == otherRoute.LocationPoints.First() &&
               LocationPoints.Last() == otherRoute.LocationPoints.Last() &&
               Distance == otherRoute.Distance;
        }


        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
