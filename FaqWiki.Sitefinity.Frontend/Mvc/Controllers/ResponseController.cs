using System;
using System.ComponentModel;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Modules.Pages.Configuration;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web.UI;
using Telerik.Sitefinity.DynamicModules.Model;
using FaqWiki.Sitefinity.Frontend.Mvc.Models.Response;
using Telerik.Sitefinity.Web;

namespace FaqWiki.Sitefinity.Frontend.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "Response_MVC", Title = "Response", SectionName = ToolboxesConfig.ContentToolboxSectionName, CssClass = ResponseController.WidgetIconCssClass)]
    public class ResponseController : Controller, ICustomWidgetVisualizationExtended
    {
        #region Properties

        /// <summary>
        /// Gets the Card widget model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual IResponseModel Model
        {
            get
            {
                if (this.model == null)
                    this.model = ControllerModelFactory.GetModel<IResponseModel>(this.GetType());

                return this.model;
            }
        }
        public string EmptyLinkText
        {
            get
            {
                return "No results";
            }
        }

        /// <summary>
        /// Gets or sets the name of the template that widget will be displayed.
        /// </summary>
        /// <value></value>
        public string TemplateName
        {
            get
            {
                return this.templateName;
            }

            set
            {
                this.templateName = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether widget is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if widget has no image selected; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return this.Model.IsEmpty();
            }
        }

        public string WidgetCssClass
        {
            get
            {
                return WidgetIconCssClass;
            }
        }


        /// <summary>
        /// Gets the is design mode.
        /// </summary>
        /// <value>The is design mode.</value>
        protected virtual bool IsDesignMode
        {
            get
            {
                return SystemManager.IsDesignMode;
            }
        }

        #endregion

        #region Actions

        public ActionResult Index(int? page)
        {
            if (this.IsEmpty)
            {
                return new EmptyResult();
            }
            var node = SiteMapBase.GetActualCurrentNode();
            var viewModel = this.Model.GetListViewModel(page, node.Url);
            ViewBag.RedirectPageUrlTemplate = node.Url.TrimStart(new char[] { '~' }) + "/{0}";

            return View(this.TemplateName, viewModel);
        }

        public ActionResult Details(DynamicContent response)
        {
            var viewModel = this.Model.GetViewModel(response);
            var node = SiteMapBase.GetActualCurrentNode();
            ViewBag.ListPageUrl = node.Url.TrimStart(new char[] { '~' });

            return View("Detail",viewModel);
        }
        

        public ActionResult Edit(DynamicContent response)
        {
            var viewModel = this.Model.GetViewModel(response);
            var node = SiteMapBase.GetActualCurrentNode();
            ViewBag.ListPageUrl = node.Url.TrimStart(new char[] { '~' });

            return View("Edit", viewModel);
        }

        #endregion

        #region Private fields and constants

        internal const string WidgetIconCssClass = "sfMvcIcn";
        private IResponseModel model;

        private string templateName = "List";

        #endregion
    }
}
