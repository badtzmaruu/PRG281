using System;
using System.Collections.Generic;
using System.Linq;

namespace Watt_2_Watch
{
    internal class Program
    {
        static List<User> users = new List<User>();
        static User loggedInUser = null;
        static Database db = new Database(Properties.Resources.MoviesDatabase);

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                if (loggedInUser == null)
                {
                    Console.WriteLine("1. Sign Up");
                    Console.WriteLine("2. Login");
                    Console.WriteLine("3. Exit");

                    string option = Console.ReadLine();

                    switch (option)
                    {
                        case "1":
                            SignUp();
                            break;
                        case "2":
                            Login();
                            break;
                        case "3":
                            return;
                    }
                }
                else
                {
                    MainMenu();
                }
            }
        }

        static void SignUp()
        {
            Console.Clear();
            Console.WriteLine("Sign Up");

            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            Console.Write("Enter email: ");
            string email = Console.ReadLine();

            var newUser = new User
            {
                Username = username,
                Password = password,
                Email = email
            };

            Console.WriteLine("Select your favorite genres (e.g., Action, Comedy, Drama):");
            string[] genres = Console.ReadLine().Split(',');

            foreach (string genre in genres)
            {
                newUser.AddGenrePreference(genre.Trim(), 1);
            }

            users.Add(newUser);
            loggedInUser = newUser;

            Console.WriteLine("Account created successfully! Press any key to continue...");
            Console.ReadKey();
        }

        static void Login()
        {
            Console.Clear();
            Console.WriteLine("Login");

            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            foreach (var user in users)
            {
                if (user.Username == username && user.Password == password)
                {
                    loggedInUser = user;
                    Console.WriteLine("Login successful! Press any key to continue...");
                    Console.ReadKey();
                    return;
                }
            }

            Console.WriteLine("Login failed. Press any key to try again...");
            Console.ReadKey();
        }

        static void MainMenu()
        {
            Console.Clear();
            Console.WriteLine("Main Menu");
            Console.WriteLine("1. View Profile");
            Console.WriteLine("2. Get Recommendations");
            Console.WriteLine("3. Logout");

            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    ViewProfile();
                    break;
                case "2":
                    GetRecommendations();
                    break;
                case "3":
                    loggedInUser = null;
                    break;
            }
        }

        static void ViewProfile()
        {
            Console.Clear();
            Console.WriteLine("Profile");
            Console.WriteLine("1. Display Details");
            Console.WriteLine("2. View Watch History");
            Console.WriteLine("3. Change Genre Preferences");
            Console.WriteLine("4. Back to Main Menu");

            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    DisplayDetails();
                    break;
                case "2":
                    ViewWatchHistory();
                    break;
                case "3":
                    ChangeGenrePreferences();
                    break;
                case "4":
                    return;
            }
        }

        static void DisplayDetails()
        {
            Console.Clear();
            Console.WriteLine("User Details");
            Console.WriteLine($"Username: {loggedInUser.Username}");
            Console.WriteLine($"Email: {loggedInUser.Email}");
            Console.WriteLine("Preferred Genres:");
            foreach (var genre in loggedInUser.PreferredGenres)
            {
                Console.WriteLine($"{genre.Key}: {genre.Value}");
            }
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
        }

        static void ViewWatchHistory()
        {
            Console.Clear();
            Console.WriteLine("Watch History:");
            foreach (var record in loggedInUser.WatchHistory)
            {
                Console.WriteLine($"{record.PrimaryTitle} ({record.StartYear}) - Genres: {string.Join(", ", record.Genres)}");
            }
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
        }

        static void ChangeGenrePreferences()
        {
            Console.Clear();
            Console.WriteLine("Change Genre Preferences");
            Console.WriteLine("Enter your new favorite genres (e.g., Action, Comedy, Drama):");

            string[] genres = Console.ReadLine().Split(',');
            var newPreferences = new Dictionary<string, int>();

            foreach (string genre in genres)
            {
                newPreferences[genre.Trim()] = 1;
            }

            loggedInUser.UpdateGenrePreferences(newPreferences);
            Console.WriteLine("Preferences updated! Press any key to go back...");
            Console.ReadKey();
        }

        static void GetRecommendations()
        {
            Console.Clear();
            Console.WriteLine("Recommended Shows:");

            var recommendedShows = new List<Database.DatabaseRecord>();

            foreach (var genre in loggedInUser.PreferredGenres.Keys)
            {
                var shows = db.FilterByGenre(new List<string> { genre });
                recommendedShows.AddRange(shows);
            }

            var distinctShows = recommendedShows.DistinctBy(show => show.ShowId).Take(10);

            foreach (var show in distinctShows)
            {
                Console.WriteLine($"{show.PrimaryTitle} ({show.StartYear}) - Genres: {string.Join(", ", show.Genres)}");
            }

            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
        }
    }
}
