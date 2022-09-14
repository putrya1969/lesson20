using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LinqLesson
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<Person> persons = new List<Person>();
            var options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;
            options.Converters.Add(new JsonStringEnumConverter());
            options.Converters.Add(new DateTimeOffsetConverterUsingDateTimeParse());

            using (StreamReader streamReader = new StreamReader("data.json"))
            {
                var content = streamReader.ReadToEnd();
                persons = JsonSerializer.Deserialize<IEnumerable<Person>>(content, options);
            }
            
            //find out who is located farthest north / south / west / east using latitude/ longitude data
            var farthestNorth = persons.Where(p => p.Latitude == persons.Max(p => p.Latitude)).FirstOrDefault();
            var farthestSouth = persons.Where(p => p.Latitude == persons.Min(p => p.Latitude)).FirstOrDefault();
            var farthestWest = persons.Where(p => p.Longitude == persons.Max(p => p.Longitude)).FirstOrDefault();
            var farthestEast = persons.Where(p => p.Longitude == persons.Min(p => p.Longitude)).FirstOrDefault();
            
            //find max and min distance between 2 persons
            var pairWirhMaxDist = persons.SelectMany(p1 => persons.Select(p2 => new { Person1 = p1, Person2 = p2 }))
                .Where(p => p.Person1 != p.Person2)
                .Select(d => new PairWithDistance(d.Person1, d.Person2, (GetDistanceInMiles(d.Person1, d.Person2))))
                .OrderByDescending(d => d.Distance)
                .FirstOrDefault()
                .ToString();
            Console.WriteLine($"Maximum distance between\n{pairWirhMaxDist}");
            var pairWirhMinDist = persons.SelectMany(p1 => persons.Select(p2 => new { Person1 = p1, Person2 = p2 }))
                .Where(p => p.Person1 != p.Person2)
                .Select(d => new PairWithDistance(d.Person1, d.Person2, (GetDistanceInMiles(d.Person1, d.Person2))))
                .OrderBy(d => d.Distance)
                .FirstOrDefault()
                .ToString();
            Console.WriteLine($"Minimum distance between\n{pairWirhMinDist}");
            
            //find 2 persons whos ‘about’ have the most same words
            var twoPersonWithLongestAbout = persons.OrderByDescending(p => p.About.WordsCount()).Take(2);

            //find persons with same friends(compare by friend’s name)
            var grooupByFriends = persons
                .SelectMany(person => person.Friends, (person, friend) => new { FriendName = friend.Name, PersonName = person.Name })
                .GroupBy(pf => pf.FriendName)
                .Where(pf => pf.Count() > 1)
                .ToList();
            foreach (var commonFriend in grooupByFriends)
            {
                Console.WriteLine($"{commonFriend.Key} is common friend for:");
                foreach (var person in commonFriend)
                {
                    Console.WriteLine(person.PersonName);
                }
                Console.WriteLine();
            }
        }

        public static double GetDistanceInMiles(Person person1, Person person2)
        {
            return 3963.0 * Math.Acos((Math.Sin(person1.Latitude) * Math.Sin(person2.Latitude)) + Math.Cos(person1.Latitude) * Math.Cos(person2.Latitude) * Math.Cos(person2.Longitude - person1.Longitude));
        }
    }

    public static class StrintExtension
    {
        public static int WordsCount(this string source)
        {
            return source.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }
    }

    public class DateTimeOffsetConverterUsingDateTimeParse : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(typeToConvert == typeof(DateTimeOffset));
            return DateTimeOffset.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private static readonly string _format = "yyyy-MM-dd'T'HH:mm:ss.fff";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.ParseExact(reader.GetString(), _format, System.Globalization.CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToUniversalTime().ToString(_format));
    }
}
