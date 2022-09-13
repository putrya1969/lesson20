using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Diagnostics;
using System.Linq;

namespace LinqLesson
{
    class Program
    {

            /*find out who is located farthest north/south/west/east using latitude/longitude data
              find max and min distance between 2 persons
              find 2 persons whos ‘about’ have the most same words
              find persons with same friends (compare by friend’s name)*/
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
                persons = JsonSerializer.Deserialize<IEnumerable<Person>>(content,options);
            }
            //find out who is located farthest north / south / west / east using latitude/ longitude data
            var farthestNorth = persons.Where(p => p.Latitude == persons.Max(p => p.Latitude)).FirstOrDefault();
            var farthestSouth = persons.Where(p => p.Latitude == persons.Min(p => p.Latitude)).FirstOrDefault();
            var farthestWest = persons.Where(p => p.Longitude == persons.Max(p => p.Longitude)).FirstOrDefault();
            var farthestEast = persons.Where(p => p.Longitude == persons.Min(p => p.Longitude)).FirstOrDefault();
            //find max and min distance between 2 persons


            //find 2 persons whos ‘about’ have the most same words
            var twoPersonWithLongestAbout = persons.OrderByDescending(p => p.About.WordsCount()).Take(2);

            //find persons with same friends(compare by friend’s name)
            var listFriends = persons.SelectMany(p => p.Friends).ToList();
        }

        private double GetDistanceInMiles(double lat1, double long1, double lat2, double long2)
        {
            return 3963.0 * Math.Acos((Math.Sin(lat1) * Math.Sin(lat2)) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(long2-long1));
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

    class MyClass : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
