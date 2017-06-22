using FaqWiki.Sitefinity.Frontend.Mvc.Models.Response;
using Ninject.Modules;

namespace FaqWiki.Sitefinity.Frontend
{
    /// <summary>
    /// This class is used to describe the bindings which will be used by the Ninject container when resolving classes
    /// </summary>
    public class InterfaceMappings : NinjectModule
    {
        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            Bind<IResponseModel>().To<ResponseModel>();
        }
    }
}