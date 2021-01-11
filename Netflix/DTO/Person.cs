using System.Collections.Generic;

namespace Netflix.DTO
{
    public class Person
    {
        public string Name { get; set; }
        public List<string> MoviesWatched { get; set; } = new List<string>();

        public Person(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}