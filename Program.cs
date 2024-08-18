using static Watt_2_Watch.Database;

namespace Watt_2_Watch
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Database db = new Database(Properties.Resources.MoviesDatabase);

            List<DatabaseRecord>  Shows = db.FilterByGenre(["horror"]);

            Console.WriteLine($"Shows found: {Shows.Count}\r\n\n");
            foreach (DatabaseRecord rec in Shows)
            {
                Console.WriteLine($"Title:{rec.OriginalTitle} Genre: {rec.Genres}.");
            }

            var user = new Dictionary<string, object>
            {
                { "Name", "John" },
                { "Surname", "Doe" },
                { "PreferredGenres", new Dictionary<string, int> { { "Action", 5 }, { "Comedy", 7 }, { "Drama", 4} } }
            };

            var user2 = new Dictionary<string, object>
            {
                { "Name", "John" },
                { "Surname", "Doe" },
                { "PreferredGenres", new Dictionary<string, int> { { "Documentary", 5 }, { "Animation", 7 }, { "Romance", 4} } }
            };

            var engine2 = new recommendationEngine(user2, db);
            engine2.RecommendShows();

            var engine = new recommendationEngine(user, db);
            engine.RecommendShows();


            Console.ReadKey();
        }
    }
}
