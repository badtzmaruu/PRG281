﻿using System;
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
            bool firstLine = true;

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                if (!firstLine)
                {
                    string[] fields = line.Split('\t');
                    bool isAdult = fields.Length > 4 && fields[4] == "1";

                    // Checks to see if there is data for the field.
                    int.TryParse(fields.Length > 5 && fields[5] != "\\N" ? fields[5] : "0", out int startYear);
                    int.TryParse(fields.Length > 6 && fields[6] != "\\N" ? fields[6] : "0", out int endYear);
                    int.TryParse(fields.Length > 7 && fields[7] != "\\N" ? fields[7] : "0", out int runtimeMinutes);

                    if (fields.Length > 8)
                    {
                        DatabaseRecord Record = new DatabaseRecord()
                        {
                            ShowId = fields[0],
                            TitleType = fields[1],
                            PrimaryTitle = fields[2],
                            OriginalTitle = fields[3],
                            IsAdult = isAdult,
                            StartYear = startYear,
                            EndYear = endYear,
                            RuntimeMinutes = runtimeMinutes,
                            Genres = fields[8].Split(',').ToList(),
                        };

                        Records.Add(Record);
                    }
                }
                else
                    firstLine = false;
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

        private IEnumerable<DatabaseRecord> FilterByType(IEnumerable<DatabaseRecord> records)
        {
            return records.Where(rec => rec.TitleType == "tvSeries" || rec.TitleType == "movie" || rec.TitleType == "short" || rec.TitleType == "tvMiniSeries" || rec.TitleType == "tvSpecial");
        }

        public List<DatabaseRecord> FilterByYearRange(int startYear, int endYear)
        {
            return FilterByType(Records).Where(rec => rec.StartYear >= startYear && rec.StartYear <= endYear).ToList();
        }

        public List<DatabaseRecord> FilterByYearRange(List<DatabaseRecord> recordList, int startYear, int endYear)
        {
            return FilterByType(recordList).Where(rec => rec.StartYear >= startYear && rec.StartYear <= endYear).ToList();
        }

        public List<DatabaseRecord> FilterByGenre(List<string> genres)
        {
            return FilterByType(Records).Where(rec => rec.Genres.Any(genre => genres.Contains(genre, StringComparer.OrdinalIgnoreCase))).ToList();
        }

        public List<DatabaseRecord> FilterByGenre(List<DatabaseRecord> recordList, List<string> genres)
        {
            return FilterByType(recordList).Where(rec => rec.Genres.Any(genre => genres.Contains(genre, StringComparer.OrdinalIgnoreCase))).ToList();
        }

        public List<DatabaseRecord> FilterByTitle(string title)
        {
            return FilterByType(Records).Where(rec => rec.PrimaryTitle.Contains(title, StringComparison.OrdinalIgnoreCase) || rec.OriginalTitle.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<DatabaseRecord> FilterByTitle(List<DatabaseRecord> recordList, string title)
        {
            return FilterByType(recordList).Where(rec => rec.PrimaryTitle.Contains(title, StringComparison.OrdinalIgnoreCase) || rec.OriginalTitle.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<DatabaseRecord> FilterByDuration(int minDuration, int maxDuration)
        {
            return FilterByType(Records).Where(rec => rec.RuntimeMinutes >= minDuration && rec.RuntimeMinutes <= maxDuration).ToList();
        }

        public List<DatabaseRecord> FilterByDuration(List<DatabaseRecord> recordList, int minDuration, int maxDuration)
        {
            return FilterByType(recordList).Where(rec => rec.RuntimeMinutes >= minDuration && rec.RuntimeMinutes <= maxDuration).ToList();
        }

        public List<DatabaseRecord> FilterByType(string showType)
        {
            return Records.Where(rec => rec.TitleType == showType).ToList();
        }

        public List<DatabaseRecord> FilterByType(List<DatabaseRecord> recordList, string showType)
        {
            return recordList.Where(rec => rec.TitleType == showType).ToList();
        }
        #endregion
    }
}
