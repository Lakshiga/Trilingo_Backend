using System.Collections.Generic;

namespace TES_Learning_App.Application_Layer.DTOs.Progress.Response
{
    public class PagedProgressResponse
    {
        public IEnumerable<ProgressDto> Data { get; set; } = new List<ProgressDto>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}


