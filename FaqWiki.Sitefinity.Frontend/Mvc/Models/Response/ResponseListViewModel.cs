using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Modules.GenericContent;

namespace FaqWiki.Sitefinity.Frontend.Mvc.Models.Response
{
    public class ResponseListViewModel
    {
        public ResponseListViewModel()
            {
                Responses = new List<ResponseItemModel>();
            }

        public int CurrentPage { get; set; }

        public int? TotalPagesCount { get; set; }

        public List<ResponseItemModel> Responses { get; set; }
    }
}
