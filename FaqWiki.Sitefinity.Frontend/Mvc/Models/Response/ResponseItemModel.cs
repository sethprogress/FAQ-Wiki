using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Modules.GenericContent;

namespace FaqWiki.Sitefinity.Frontend.Mvc.Models.Response
{
    public class ResponseItemModel
    {
        public string Title { get; set; }
        public string Requirement { get; set; }

        [DynamicLinksContainer]
        public string Response { get; set; }
        public string Keywords { get; set; }
        public DateTime LastModified { get; set; }
        public string LastModifiedBy { get; set; }
        public string CurrentStatus { get; set; }

        public string Url { get; set; }
    }
}
