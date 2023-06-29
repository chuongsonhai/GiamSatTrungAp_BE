using Newtonsoft.Json;
using System.Collections.Generic;

namespace EVN.Core
{
    public class BaseFilterRequest : BaseRequest
    {
        [JsonProperty("Filter")]
        public BaseFilter Filter { get; set; }
    }

    public class BaseFilter
    {
        public int status { get; set; } = -1;
        public string keyword { get; set; } = string.Empty;
        public int? deptid { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
    }

    //Search base
    public class BaseRequest
    {
        public virtual Paginator Paginator { get; set; } = new Paginator();
        public virtual Sorting Sorting { get; set; } = new Sorting();
        public virtual SelectedRowIds SelectedRowIds { get; set; } = new SelectedRowIds();
        public virtual Grouping Grouping { get; set; } = new Grouping();        
        public virtual string SearchTerm { get; set; } = "";
    }

    public class Paginator
    {
        public int page { get; set; }
        public int pageSize { get; set; }
        public int total { get; set; }
        public List<object> pageSizes { get; set; } = new List<object>();
    }

    public class Sorting
    {
        public string column { get; set; }
        public string direction { get; set; }
    }

    public class SelectedRowIds
    {
        public List<int> lstIds { get; set; } = new List<int>();
    }

    public class Grouping
    {
        public SelectedRowIds selectedRowIds { get; set; }
        public List<object> itemIds { get; set; } = new List<object>();
    }
}
