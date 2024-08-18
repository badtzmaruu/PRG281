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


            Console.ReadKey();
        }
    }
}
