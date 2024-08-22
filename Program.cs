using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions; //regex

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
                    string option = Validation.MenuValidation.GetValidOption();

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

            string username = Validation.SignUpValidation.GetValidUsername(users);
            string password = Validation.SignUpValidation.GetValidPassword();
            string email = Validation.SignUpValidation.GetValidEmail(users);

            var newUser = new User
            {
                Username = username,
                Password = password,
                Email = email
            };

            var validGenres = db.GetValidGenres();

            string[] genres;
            List<string> invalidGenres;

            do
            {
                Console.WriteLine("Select your favorite genres (e.g., Action, Comedy, Drama):");
                string inputGenres = Console.ReadLine();
                genres = inputGenres.Split(',').Select(g => g.Trim()).ToArray();

                invalidGenres = genres.Where(g => !validGenres.Contains(g, StringComparer.OrdinalIgnoreCase)).ToList();

                if (invalidGenres.Any())
                {
                    Console.WriteLine($"The following genres are not valid: {string.Join(", ", invalidGenres)}");
                    Console.WriteLine("Here are the available genres:");
                    Console.WriteLine(string.Join(", ", validGenres));
                }
            } while (invalidGenres.Any());

            foreach (string genre in genres)
            {
                newUser.AddGenrePreference(genre, 1);
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

            string username = Validation.LoginValidation.GetValidUsername();
            string password = Validation.LoginValidation.GetValidPassword();

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
            Console.WriteLine("3. Search for Shows");
            Console.WriteLine("4. Rate a Show");
            Console.WriteLine("5. Logout");

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
                    SearchShows();
                    break;
                case "4":
                    RateShow();
                    break;
                case "5":
                    loggedInUser = null;
                    break;
            }
        }

        static void SearchShows()
        {
            Console.Clear();
            Console.WriteLine("Search for Shows");
            Console.WriteLine("1. By Genre");
            Console.WriteLine("2. By Title");

            string option = Console.ReadLine();
            List<Database.DatabaseRecord> results = new List<Database.DatabaseRecord>();

            switch (option)
            {
                case "1":
                    Console.WriteLine("Enter Genre:");
                    string genre = Console.ReadLine();
                    results = db.FilterByGenre(new List<string> { genre });
                    break;
                case "2":
                    Console.WriteLine("Enter Title:");
                    string title = Console.ReadLine();
                    results = db.FilterByTitle(title);
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    return;
            }

            DisplaySearchResults(results);
        }

        static void DisplaySearchResults(List<Database.DatabaseRecord> results)
        {
            if (results.Count > 0)
            {
                Console.WriteLine("Search Results:");
                foreach (var show in results)
                {
                    Console.WriteLine($"{show.PrimaryTitle} ({show.StartYear}) - Genres: {string.Join(", ", show.Genres)}");
                }
            }
            else
            {
                Console.WriteLine("No shows found matching the criteria.");
            }
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
        }

        static void RateShow()
        {
            Console.Clear();
            Console.WriteLine("Rate a Show");
            Console.WriteLine("Enter the title of the show you want to rate:");
            string title = Console.ReadLine();

            // Fetch similar shows based on the entered title
            var similarShows = db.FilterByTitle(title);

            if (similarShows.Count == 0)
            {
                Console.WriteLine("No shows found with that title. Press any key to go back...");
                Console.ReadKey();
                return;
            }

            // Display similar shows and ask the user to select one
            Console.WriteLine("Select the show you want to rate:");
            for (int i = 0; i < similarShows.Count; i++)
            {
                var show = similarShows[i];
                Console.WriteLine($"{i + 1}. {show.PrimaryTitle} ({show.StartYear}) - Genres: {string.Join(", ", show.Genres)}");
            }

            Console.WriteLine("Enter the number of the show you want to rate:");
            if (int.TryParse(Console.ReadLine(), out int selectedShowIndex) && selectedShowIndex > 0 && selectedShowIndex <= similarShows.Count)
            {
                var selectedShow = similarShows[selectedShowIndex - 1];

                Console.WriteLine($"You selected: {selectedShow.PrimaryTitle} ({selectedShow.StartYear}) - Genres: {string.Join(", ", selectedShow.Genres)}");
                Console.WriteLine("Rate this show from 1 to 5 stars:");
                if (int.TryParse(Console.ReadLine(), out int rating) && rating >= 1 && rating <= 5)
                {
                    foreach (var genre in selectedShow.Genres)
                    {
                        loggedInUser.AddGenrePreference(genre, rating);
                    }

                    loggedInUser.WatchHistory.Add(selectedShow);
                    Console.WriteLine("Rating recorded. Press any key to go back...");
                }
                else
                {
                    Console.WriteLine("Invalid rating. Please enter a number between 1 and 5.");
                }
            }
            else
            {
                Console.WriteLine("Invalid selection. Press any key to go back...");
            }

            Console.ReadKey();
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

        public static class Validation
        {
            public static class MenuValidation
            {
                public static string GetValidOption()
                {
                    Console.WriteLine("1. Sign Up");
                    Console.WriteLine("2. Login");
                    Console.WriteLine("3. Exit");

                    string option = Console.ReadLine();

                    while (option != "1" && option != "2" && option != "3")
                    {
                        Console.WriteLine("Invalid option. Choose an option displayed above.");
                        option = Console.ReadLine();
                    }

                    return option;
                }
            }

            public static class SignUpValidation
            {
                public static string GetValidUsername(List<User> users)
                {
                    string username;
                    while (true)
                    {
                        Console.Write("Enter username: ");
                        username = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(username))
                        {
                            Console.WriteLine("Username cannot be empty.");
                        }
                        else if (users.Any(u => u.Username == username))
                        {
                            Console.WriteLine("Username already exists. Please choose a different one.");
                        }
                        else
                        {
                            break;
                        }
                    }
                    return username;
                }

                public static string GetValidEmail(List<User> users)
                {
                    string email;
                    var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);

                    while (true)
                    {
                        Console.Write("Enter email: ");
                        email = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(email))
                        {
                            Console.WriteLine("Email cannot be empty.");
                        }
                        else if (!emailRegex.IsMatch(email))
                        {
                            Console.WriteLine("Invalid email format. Please enter a valid email address.");
                        }
                        else if (users.Any(u => u.Email == email))
                        {
                            Console.WriteLine("Email is already in use. Please use a different one.");
                        }
                        else
                        {
                            break;
                        }
                    }
                    return email;
                }

                public static string GetValidPassword()
                {
                    string password;
                    var passwordRegex = new Regex(@"^(?=.*[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-])[A-Za-z\d!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]{8,}$");

                    while (true)
                    {
                        Console.Write("Enter password: ");
                        password = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(password))
                        {
                            Console.WriteLine("Password cannot be empty.");
                        }
                        else if (password.Length < 8)
                        {
                            Console.WriteLine("Password must be at least 8 characters long.");
                        }
                        else if (!passwordRegex.IsMatch(password))
                        {
                            Console.WriteLine("Password must contain at least one special character.");
                        }
                        else
                        {
                            break;
                        }
                    }
                    return password;
                }
            }

            public static class LoginValidation
            {
                public static string GetValidUsername()
                {
                    string username;
                    while (true)
                    {
                        Console.Write("Enter username: ");
                        username = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(username))
                        {
                            Console.WriteLine("Username cannot be empty.");
                        }
                        else
                        {
                            break;
                        }
                    }
                    return username;
                }

                public static string GetValidPassword()
                {
                    string password;
                    var passwordRegex = new Regex(@"^(?=.*[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-])[A-Za-z\d!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]{8,}$");

                    while (true)
                    {
                        Console.Write("Enter password: ");
                        password = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(password))
                        {
                            Console.WriteLine("Password cannot be empty.");
                        }
                        else if (password.Length < 8)
                        {
                            Console.WriteLine("Password must be at least 8 characters long.");
                        }
                        else if (!passwordRegex.IsMatch(password))
                        {
                            Console.WriteLine("Password must contain at least one special character.");
                        }
                        else
                        {
                            break;
                        }
                    }
                    return password;
                }
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
}