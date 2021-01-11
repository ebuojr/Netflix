using Netflix.DTO;
using System.Linq;
using Raven.Client.Documents.Indexes;

namespace Netflix.Indexes
{
    public class MovieRecommenderIndex : AbstractIndexCreationTask<Person, MovieRecommendation>
    {
        public MovieRecommenderIndex()
        {
            Map = people => from person in people
                from movie in person.MoviesWatched
                let watchedWith = person.MoviesWatched.Where(m => m != movie)
                select new
                {
                    ForMovie = movie,
                    Recommendations = watchedWith.ToArray()
                };

            Reduce = recommendations => from recommendation in recommendations
                group recommendation by recommendation.ForMovie
                into grouped
                select new
                {
                    ForMovie = grouped.Key,
                    Recommendations = grouped.SelectMany(a => a.Recommendations)
                        .OrderByDescending(a => grouped.SelectMany(b => b.Recommendations)
                            .Count(c => c == a)).Distinct().ToArray()
                };
        }
    }
}