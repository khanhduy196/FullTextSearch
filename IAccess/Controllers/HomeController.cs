using CsvHelper;
using IAccess.Models;
using IAccess.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace IAccess.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Search all the characters in the [String Content] column to find out the records which contain the keyword
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Search(string keyword = "")
        {
            var result = new List<SearchResultViewModel>() { };
            int totalOccurrences = 0;
            string value = string.Empty;
            if (!string.IsNullOrEmpty(keyword))
            {
                ISearchService searchService = new SearchService(keyword);

                using (var reader = new StreamReader(@"wwwroot/Testing.csv"))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        value = csv.GetField("Value");
                        totalOccurrences = searchService.GetTotalOccurrences(value);
                        if(totalOccurrences > 0)
                        {
                            result.Add(new SearchResultViewModel {
                                Id = csv.GetField<Guid>("Id"),
                                StringContent = value,
                                MatchTimes = totalOccurrences
                            });
                        }
                    }
                }
            }

            return Ok(result);
        }

        /// <summary>
        /// Create a CSV called [Testing] in the local drive
        /// Insert the 100K records into this CSV.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Insert()
        {
            if (System.IO.File.Exists(@"wwwroot/Testing.csv"))
                System.IO.File.Delete(@"wwwroot/Testing.csv");

            var records = new List<CsvFileItem> { };
            for (int i = 0; i < 100000; i++)
            {
                records.Add(new CsvFileItem
                {
                    Id = Guid.NewGuid(),
                    Value = RandomString(1000)
                });

            }

            using (var writer = new StreamWriter(@"wwwroot/Testing.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
            }

            return Ok();
        }

        /// <summary>
        /// Home page
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Create String Content
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789 ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
