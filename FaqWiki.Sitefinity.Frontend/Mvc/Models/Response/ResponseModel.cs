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
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Model;
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
            if (page.HasValue)
            {
                currentPage = page.Value;
            }

            var responses = GetResponses();

            var totalCount = responses.Count();
            if(totalCount > 0)
            {
                totalCount = totalCount / ItemsPerPage;
            }

            var skip = 0;
            if (page.HasValue)
            {
                skip = page.Value * ItemsPerPage;
            }

            var viewModel = new ResponseListViewModel()
            {
                Responses = responses.Skip(skip).Take(ItemsPerPage).Select(item => new ResponseItemModel()
                {
                    Title = item.GetString("Title").ToString(),
                    Url = String.Format("{0}/{1}", url.TrimStart(new char[] { '~' }), item.UrlName),
                    Id = item.Id,
                    Requirement = item.GetString("Requirement").ToString(),
                    Response = item.GetString("Response").ToString(),
                    CurrentStatus = item.ApprovalWorkflowState,
                    Keywords = item.GetString("Keywords").ToString(),
                    LastModified = item.LastModified
                    //LastModifiedBy = GetUsersName(item.LastModifiedBy)
                }).ToList(),
                CurrentPage = currentPage,
                TotalPagesCount = totalCount
            };

            return viewModel;
        }

        public virtual ResponseViewModel GetViewModel(string itemUrl)
        {
            var response = GetItem(itemUrl);

            var viewModel = new ResponseViewModel()
            {
                Response = new ResponseItemModel()
                {
                    Title = response.GetString("Title").ToString(),
                    Requirement = response.GetString("Requirement").ToString(),
                    Id = response.Id,
                    Response = response.GetString("Response").ToString(),
                    CurrentStatus = response.ApprovalWorkflowState.ToString(),
                    Keywords = response.GetString("Keywords").ToString(),
                    LastModified = response.LastModified,
                    LastModifiedBy = GetUsersName(response.LastModifiedBy),
                    Url = response.UrlName
                }
            };

            return viewModel;
        }

        private string GetUsersName(Guid lastModifiedBy)
        {
            string name = "Unknown";

            if (lastModifiedBy != Guid.Empty)
            {
                var userManager = UserManager.GetManager();

                var user = userManager.GetUser(lastModifiedBy);

                if (user != null)
                {
                    UserProfileManager profileManager = UserProfileManager.GetManager();
                    var profile = profileManager.GetUserProfile<SitefinityProfile>(user);

                    if (profile != null)
                    {
                        name = String.Format("{0} {1}", profile.FirstName, profile.LastName);
                    }
                }
            }

            return name;
        }

        private DynamicContent GetItem(string itemUrl)
        {
            var providerName = String.Empty;

            // Set a transaction name
            var transactionName = "ResponseTransaction-" + new Guid().ToString();

            DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager(providerName, transactionName);
            Type responseType = TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model.RFPResponses.Response");

            return dynamicModuleManager.GetDataItems(responseType).Where(r => r.Status == ContentLifecycleStatus.Live && r.Visible == true && r.UrlName == itemUrl).FirstOrDefault();
        }

        private IQueryable<DynamicContent> GetResponses()
        {
            // Set the provider name for the DynamicModuleManager here. All available providers are listed in
            // Administration -> Settings -> Advanced -> DynamicModules -> Providers
            var providerName = String.Empty;

            // Set a transaction name
            var transactionName = "ResponseTransaction-" + new Guid().ToString();

            DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager(providerName, transactionName);
            Type responseType = TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model.RFPResponses.Response");

            // This is how we get the response items through filtering
            return dynamicModuleManager.GetDataItems(responseType).Where(r => r.Status == ContentLifecycleStatus.Live && r.Visible == true);
        }

        public bool IsEmpty()
        {
            return false;
        }
    }
}
