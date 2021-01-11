using Netflix.DTO;
using System.Linq;
using Raven.Client.Documents.Indexes;

namespace Netflix.Indexes
{
    public class MovieIndex : AbstractIndexCreationTask<Movie>
    {
        public MovieIndex()
        {
            Map = movies => from movie in movies
                select new
                {
                    movie.Id,
                    movie.Name,
                    movie.AgeRestriction,
                    movie.Genre,
                    movie.ReleaseDate
                };
        }
    }
}