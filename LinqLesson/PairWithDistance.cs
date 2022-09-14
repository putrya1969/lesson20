using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqLesson
{
    internal class PairWithDistance
    {
        public Person FirstPerson { get; set; }
        public Person SecondPerson { get; set; }
        public double Distance { get; set; }
        public PairWithDistance(Person first, Person second, double distance)
        {
            FirstPerson = first;
            SecondPerson = second;
            Distance = distance;
        }

        public override string ToString()
        {
            return $"{FirstPerson.Name} and {SecondPerson.Name} equals {Distance} miles";
        }
    }
}
