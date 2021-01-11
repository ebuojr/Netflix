using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Netflix.DTO;
using Netflix.Indexes;
using Raven.Client.Documents;

namespace Netflix
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var documentStore = CreateStore())
            {
                using (var session = documentStore.OpenSession())
                {
                    session.Store(new Movie("Harry Potter", 12, DateTime.Now.AddDays(-100), Genre.Adventure));
                    session.Store(new Movie("Cars", 6, DateTime.Now.AddDays(-200), Genre.Cartoon));
                    session.Store(new Movie("Smølferne", 8, DateTime.Now.AddDays(-300), Genre.Cartoon));
                    session.Store(new Movie("T2", 15, DateTime.Now.AddDays(-400), Genre.Action));

                    session.SaveChanges();

                    //Always in sync, never stale
                    var loadedFromDocumentStore = session.Load<Movie>("movies/12-A");
                    var multipleLoadedFromDocumentStore = session.Load<Movie>(new []{"movies/12-A", "movies/10-A" });

                    //Search dynamic index
                    var searchedByName = session.Query<Movie>().FirstOrDefault(a => a.Name == "T2");
                    //Static index
                    var searchedByNameInSpecificIndex = session.Query<Movie, MovieIndex>().FirstOrDefault(a => a.Name == "T2");

                    //session.Store(new Person("Per"));
                    //session.Store(new Person("Julie"));
                    //session.Store(new Person("Trine"));
                    //session.Store(new Person("Bo"));
                    //session.SaveChanges();

                    var newestMovieInEachGenre = session.Query<Movie, LatestMovieByGenre>().ToArray();

                    var harryPotter = session.Query<Movie>().FirstOrDefault(a => a.Name == "Harry Potter");
                    var cars = session.Query<Movie>().FirstOrDefault(a => a.Name == "Cars");
                    var smølferne = session.Query<Movie>().FirstOrDefault(a => a.Name == "Smølferne");
                    var t2 = session.Query<Movie>().FirstOrDefault(a => a.Name == "T2");

                    var per = session.Query<Person>().FirstOrDefault(a => a.Name == "Per");
                    var julie = session.Query<Person>().FirstOrDefault(a => a.Name == "Julie");
                    var trine = session.Query<Person>().FirstOrDefault(a => a.Name == "Trine");
                    var bo = session.Query<Person>().FirstOrDefault(a => a.Name == "Bo");

                    //Delete entity
                    //session.Delete(bo);
                    //session.SaveChanges();

                    //per.MoviesWatched.Add(harryPotter.Id);
                    //per.MoviesWatched.Add(cars.Id);

                    //julie.MoviesWatched.Add(harryPotter.Id);
                    //julie.MoviesWatched.Add(t2.Id);

                    //trine.MoviesWatched.Add(smølferne.Id);
                    //trine.MoviesWatched.Add(cars.Id);

                    //bo.MoviesWatched.Add(smølferne.Id);

                    session.SaveChanges();


                    List<Person> people = new List<Person>();
                    string[] names = people.Select(p => p.Name).ToArray();
                    string[] allWatchedMovies = people.SelectMany(a => a.MoviesWatched).ToArray();

                    var recommendation = session.Query<MovieRecommendation, MovieRecommenderIndex>()
                        .FirstOrDefault(a => a.ForMovie == t2.Id);
                    var recommendedMovies = session.Load<Movie>(recommendation.Recommendations);
                }
            }
        }

        private static IDocumentStore CreateStore()
        {
            IDocumentStore store = new DocumentStore()
            {
                // Define the cluster node URLs (required)
                Urls = new[] { "http://localhost:8080", 
                    /*some additional nodes of this cluster*/ },

                // Set conventions as necessary (optional)
                Conventions =
                {
                    MaxNumberOfRequestsPerSession = 100,
                    UseOptimisticConcurrency = true
                },

                // Define a default database (optional)
                Database = "netflix",

                // Define a client certificate (optional)
                //Certificate = new X509Certificate2("C:\\path_to_your_pfx_file\\cert.pfx"),

                // Initialize the Document Store
            }.Initialize();

            store.ExecuteIndex(new MovieIndex());
            store.ExecuteIndex(new LatestMovieByGenre());
            store.ExecuteIndex(new MovieRecommenderIndex());
            return store;
        }
    }
}
