using static Watt_2_Watch.Database;

namespace Watt_2_Watch
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Database db = new Database(Properties.Resources.MoviesDatabase);

            //List<DatabaseRecord> Shows = db.FilterByYearRange(2000, 2005);

            //Console.WriteLine($"First list has {Shows.Count} shows:\r\n\n");
            //foreach (DatabaseRecord rec in Shows)
            //{
            //    Console.WriteLine($"Year:{rec.StartYear} Title:{rec.OriginalTitle}.");
            //}

            //Shows = db.FilterByYearRange(Shows, 2000, 2001);

            //Console.WriteLine($"Second list has {Shows.Count} shows:\r\n\n");
            //foreach (DatabaseRecord rec in Shows)
            //{
            //    Console.WriteLine($"Year:{rec.StartYear} Title:{rec.OriginalTitle}.");
            //}

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
