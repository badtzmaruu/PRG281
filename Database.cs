using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Watt_2_Watch
{
    internal class Database
    {
        #region Constructor
        /// <summary>
        /// Turns the Movies Batabse text file into accessible records.
        /// </summary>
        /// <param name="DatabaseFile"></param>
        public Database(string DatabaseFile)
        {
            string[] lines = DatabaseFile.Split('\n');

            bool FirstLine = true;

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                
                if (!FirstLine)
                {
                    string[] fields = line.Split('\t');

                    bool isAdult = false;

                    if (fields[4] == "1") isAdult = true;

                    if (fields[5] == "\\N") fields[5] = "0";
                    if (fields[6] == "\\N") fields[6] = "0";
                    if (fields[7] == "\\N") fields[7] = "0";

                    DatabaseRecord Record = new DatabaseRecord()
                    {
                        ShowId = fields[0],
                        TitleType = fields[1],
                        PrimaryTitle = fields[2],
                        OriginalTitle = fields[3],
                        IsAdult = isAdult,
                        StartYear = Convert.ToInt32(fields[5]),
                        EndYear = Convert.ToInt32(fields[6]),
                        RuntimeMinutes = Convert.ToInt32(fields[7]),
                        Genres = fields[8].Split(',').ToList(),
                    };

                    Records.Add(Record);
                }
                else FirstLine = false;
            }
        }
        #endregion

        #region Classes and records
        /// <summary>
        /// Database record definition.
        /// </summary>
        public record DatabaseRecord
        {
            /// <summary>
            /// IMDB show identifier.
            /// </summary>
            public string ShowId { get; init; }
            public string TitleType { get; init; }
            public string PrimaryTitle { get; init; }
            public string OriginalTitle { get; init; }
            public bool IsAdult { get; init; }
            public int StartYear { get; init; }
            public int EndYear { get; init; }
            public int RuntimeMinutes { get; init; }
            public List<string> Genres { get; init; }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Database records.
        /// </summary>
        private List<DatabaseRecord> Records { get; init; } = new List<DatabaseRecord>();
        #endregion

        #region Methods
        /// <summary>
        /// Return a list of shows that aired beween a range of dates.
        /// </summary>
        /// <param name="StartYear">Starting year of air dates.</param>
        /// <param name="EndYear">Ending year of air dates.</param>
        /// <returns>A list of shows that aired between the start and ending air dates.</returns>
        public List<DatabaseRecord> FilterByYearRange(int StartYear, int EndYear)
        {
            List<DatabaseRecord> Newlist = new List<DatabaseRecord>();
            foreach (DatabaseRecord rec in Records)
            {
                if (rec.TitleType == "tvSeries" || rec.TitleType == "movie" || rec.TitleType == "short" || rec.TitleType == "tvMiniSeries" || rec.TitleType == "tvSpecial")
                {
                    if ((rec.StartYear >= StartYear) && (rec.StartYear <= EndYear)) Newlist.Add(rec);           
                }   
            }
            return Newlist;
        }
        /// <summary>
        /// Return a list of shows that aired beween a range of dates from a provided list.
        /// </summary>
        /// <param name="RecordList">List to process.</param>
        /// <param name="StartYear">Starting year of air dates.</param>
        /// <param name="EndYear">Ending year of air dates.</param>
        /// <returns>A list of shows that aired between the start and ending air dates.</returns>
        public List<DatabaseRecord> FilterByYearRange(List<DatabaseRecord> RecordList, int StartYear, int EndYear)
        {
            List<DatabaseRecord> Newlist = new List<DatabaseRecord>();
            foreach (DatabaseRecord rec in RecordList)
            {
                if (rec.TitleType == "tvSeries" || rec.TitleType == "movie" || rec.TitleType == "short" || rec.TitleType == "tvMiniSeries" || rec.TitleType == "tvSpecial")
                {
                    if ((rec.StartYear >= StartYear) && (rec.StartYear <= EndYear))  Newlist.Add(rec);   
                }
            }
            return Newlist;
        }
        /// <summary>
        /// Returns a list of shows that match or partially match a list of genres.
        /// </summary>
        /// <param name="genres">Show genre types.</param>
        /// <returns>A list of shows that match provided genres.</returns>
        public List<DatabaseRecord> FilterByGenre(List<string> genres)
        {
            List<DatabaseRecord> Newlist = new List<DatabaseRecord>();

            foreach (DatabaseRecord rec in Records)
            {
                if (rec.TitleType == "tvSeries" || rec.TitleType == "movie" || rec.TitleType == "short" || rec.TitleType == "tvMiniSeries" || rec.TitleType == "tvSpecial")
                {
                    if (rec.Genres.Any(genre => genres.Contains(genre, StringComparer.OrdinalIgnoreCase))) Newlist.Add(rec);
                }
            }
            return Newlist;
        }
        /// <summary>
        /// Returns a list of shows that match or partially match a list of genres from a provided list.
        /// </summary>
        /// <param name="RecordList">List to process.</param>
        /// <param name="genres">Show genre types.</param>
        /// <returns>A list of shows that match provided genres.</returns>
        public List<DatabaseRecord> FilterByGenre(List<DatabaseRecord> RecordList, List<string> genres)
        {
            List<DatabaseRecord> Newlist = new List<DatabaseRecord>();

            foreach (DatabaseRecord rec in Records)
            {
                if (rec.TitleType == "tvSeries" || rec.TitleType == "movie" || rec.TitleType == "short" || rec.TitleType == "tvMiniSeries" || rec.TitleType == "tvSpecial")
                {
                    if (rec.Genres.Any(genre => genres.Contains(genre, StringComparer.OrdinalIgnoreCase)))
                        Newlist.Add(rec);
                }
            }
            return Newlist;
        }
        /// <summary>
        /// Returns a list of shows that match or partially match a title name.
        /// </summary>
        /// <param name="title">The title to be searched.</param>
        /// <returns>A list of shows that match or are similar to a title.</returns>
        public List<DatabaseRecord> FilterByTitle(string title)
        {
            List<DatabaseRecord> Newlist = new List<DatabaseRecord>();

            foreach (DatabaseRecord rec in Records)
            {
                if (rec.TitleType == "tvSeries" || rec.TitleType == "movie" || rec.TitleType == "short" || rec.TitleType == "tvMiniSeries" || rec.TitleType == "tvSpecial")
                {
                    if (rec.PrimaryTitle.Contains(title, StringComparison.OrdinalIgnoreCase) || rec.OriginalTitle.Contains(title, StringComparison.OrdinalIgnoreCase)) Newlist.Add(rec);
                }
            }
            return Newlist;
        }
        /// <summary>
        /// Returns a list of shows that match or partially match a title name from a provided list.
        /// </summary>
        /// <param name="RecordList">List to process</param>
        /// <param name="title">The title to be searched.</param>
        /// <returns>A list of shows that match or are similar to a title from a provided list.</returns>
        public List<DatabaseRecord> FilterByTitle(List<DatabaseRecord> RecordList, string title)
        {
            List<DatabaseRecord> Newlist = new List<DatabaseRecord>();

            foreach (DatabaseRecord rec in Records)
            {
                if (rec.TitleType == "tvSeries" || rec.TitleType == "movie" || rec.TitleType == "short" || rec.TitleType == "tvMiniSeries" || rec.TitleType == "tvSpecial")
                {
                    if (rec.PrimaryTitle.Contains(title, StringComparison.OrdinalIgnoreCase) || rec.OriginalTitle.Contains(title, StringComparison.OrdinalIgnoreCase))
                        Newlist.Add(rec);
                }
            }
            return Newlist;
        }
        /// <summary>
        /// Returns a list of shows with a duration between a range of run times.
        /// </summary>
        /// <param name="minDuration">Shortest duration.</param>
        /// <param name="maxDuration">Longest duration.</param>
        /// <returns>A list of shows that match a range of run times.</returns>
        public List<DatabaseRecord> FilterByDuration(int minDuration, int maxDuration)
        {
            List<DatabaseRecord> Newlist = new List<DatabaseRecord>();
            foreach (DatabaseRecord rec in Records)
            {
                if (rec.TitleType == "tvSeries" || rec.TitleType == "movie" || rec.TitleType == "short" || rec.TitleType == "tvMiniSeries" || rec.TitleType == "tvSpecial")
                {
                    if (rec.RuntimeMinutes >= minDuration && rec.RuntimeMinutes <= maxDuration) Newlist.Add(rec);
                }
            }
            return Newlist;
        }
        /// <summary>
        /// Returns a list of shows with a duration between a range of run times from a provided list.
        /// </summary>
        /// <param name="RecordList">List to process.</param>
        /// <param name="minDuration">Shortest duration.</param>
        /// <param name="maxDuration">Longest duration.</param>
        /// <returns>A list of shows that match a range of run times from a provided list.</returns>
        public List<DatabaseRecord> FilterByDuration(List<DatabaseRecord> RecordList, int minDuration, int maxDuration)
        {
            List<DatabaseRecord> Newlist = new List<DatabaseRecord>();
            foreach (DatabaseRecord rec in Records)
            {
                if (rec.TitleType == "tvSeries" || rec.TitleType == "movie" || rec.TitleType == "short" || rec.TitleType == "tvMiniSeries" || rec.TitleType == "tvSpecial")
                {
                    if (rec.RuntimeMinutes >= minDuration && rec.RuntimeMinutes <= maxDuration)
                        Newlist.Add(rec);
                }
            }
            return Newlist;
        }
        #endregion
    }
}
