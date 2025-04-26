namespace Agile_Aggregator.Domain.Models
{
    public class FilterParams
    {
        public DateTime? From { get; set; }
        public string? Category { get; set; }
        public string? SortBy { get; set; }
 

            public string? City { get; set; }

            public string? ZipCode { get; set; }


            public double? Latitude { get; set; }

 
            public double? Longitude { get; set; }



        public string? Owner { get; set; }


        public string? Repo { get; set; }
        /// <summary>
        /// full-text search across all articles
        /// </summary>
        public string? Query { get; set; }

        /// <summary>
        /// comma-separated source identifiers (e.g. "bbc-news,the-verge")
        /// </summary>
        public string? Sources { get; set; }

        /// <summary>
        /// e.g. "technology", "business"
        /// </summary>


        /// <summary>
        /// ISO 2-letter country code (e.g. "us", "gr")
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// max articles to return (default 5)
        /// </summary>
        public int PageSize { get; set; } = 5;
    }
    }


