namespace DTApp.API.Helper
{
    public class UserParams
    {
        private const int _maxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get { return _pageSize;}
            set { _pageSize = (value > _maxPageSize? _maxPageSize : value);}
        }

        public int UserId { get; set; }
        public string  Gender { get; set; }

        public int MinimumAge { get; set; } = 18;

        public int MaximumAge { get; set; } = 99;

        public string OrderBy { get; set; }

        public bool Likers { get; set; }

        public bool Likees { get; set; }
        
    }
}