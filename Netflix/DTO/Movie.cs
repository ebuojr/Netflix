using System;

namespace Netflix.DTO
{
    public class Movie
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int AgeRestriction { get; set; }
        public DateTime ReleaseDate { get; set; }
        public Genre Genre { get; set; }

        public Movie(string name, int ageRestriction, DateTime releaseDate, Genre genre)
        {
            Name = name;
            AgeRestriction = ageRestriction;
            ReleaseDate = releaseDate;
            Genre = genre;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}