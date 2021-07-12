using System;

namespace IAccess.Models
{
    public class SearchResultViewModel
    {
        public Guid Id { get; set; }
        public string StringContent { get; set; }
        public int MatchTimes { get; set; }
    }
}
