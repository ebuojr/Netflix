using Netflix.DTO;
using System.Linq;
using Raven.Client.Documents.Indexes;

namespace Netflix.Indexes
{
    public class LatestMovieByGenre : AbstractIndexCreationTask<Movie>
    {
        public LatestMovieByGenre()
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

            Reduce = movies => from movie in movies
                group movie by movie.Genre
                into groupedByGenre
                select new
                {
                    latestMovie = groupedByGenre.OrderByDescending(a => a.ReleaseDate).FirstOrDefault()
                }
                into obj
                select new
                {
                    obj.latestMovie.Id,
                    obj.latestMovie.Name,
                    obj.latestMovie.AgeRestriction,
                    obj.latestMovie.Genre,
                    obj.latestMovie.ReleaseDate
                };

        }
    }
}