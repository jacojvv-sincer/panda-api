using System.Collections.Generic;

namespace Panda.API.ViewModels
{
    public class PaginatedContentViewModel<T>
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public List<T> Items { get; set; }
    }
}