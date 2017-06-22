using Telerik.Sitefinity.DynamicModules.Model;

namespace FaqWiki.Sitefinity.Frontend.Mvc.Models.Response
{
    public interface IResponseModel
    {
        int ItemsPerPage { get; set; }

        ResponseListViewModel GetListViewModel(int? page, string url);
        ResponseViewModel GetViewModel(DynamicContent response);
        bool IsEmpty();
    }
}