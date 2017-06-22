using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Telerik.Sitefinity.DynamicModules;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules.GenericContent;
using Telerik.Sitefinity.Utilities.TypeConverters;

namespace FaqWiki.Sitefinity.Frontend.Mvc.Models.Response
{
    public class ResponseModel : IResponseModel
    {
        private int itemsPerPage = 20;
        public int ItemsPerPage
        {
            get { return itemsPerPage; }
            set { itemsPerPage = value; }
        }

        public virtual ResponseListViewModel GetListViewModel(int? page, string url)
        {
            var currentPage = 0;

            if(page.HasValue)
            {
                currentPage = page.Value;
            }

            var responses = GetResponses(currentPage);

            var viewModel = new ResponseListViewModel()
            {
                Responses = responses.Select(item => new ResponseItemModel()
                {
                    Title = item.GetString("Title").ToString(),
                    Url = String.Format("{0}/{1}",url.TrimStart(new char[] { '~' }), item.UrlName)
                }).ToList(),
                CurrentPage = currentPage,
                TotalPagesCount = 100
            };

            return viewModel;
        }

        public virtual ResponseViewModel GetViewModel(DynamicContent response)
        {

            var viewModel = new ResponseViewModel()
            {
                Title = response.GetString("Title").ToString(),
                Requirement = response.GetString("Requirement").ToString()
            };


            return viewModel;
        }

        private IQueryable<DynamicContent> GetResponses(int? page)
        {
            // Set the provider name for the DynamicModuleManager here. All available providers are listed in
            // Administration -> Settings -> Advanced -> DynamicModules -> Providers
            var providerName = String.Empty;

            // Set a transaction name
            var transactionName = "ResponseTransaction-" + new Guid().ToString();

            DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager(providerName, transactionName);
            Type responseType = TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model.RFPResponses.Response");

            var skip = 0;

            if (page.HasValue)
            {
                skip = page.HasValue ? 0 : page.Value * ItemsPerPage;
            }

            // This is how we get the response items through filtering
            return dynamicModuleManager.GetDataItems(responseType).Where(r => r.Status == ContentLifecycleStatus.Live && r.Visible == true).Skip(skip).Take(this.ItemsPerPage);
        }

        public bool IsEmpty()
        {
            return false;
        }
    }
}
