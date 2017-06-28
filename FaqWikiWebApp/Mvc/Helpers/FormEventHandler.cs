using Telerik.Sitefinity;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.DynamicModules;
using Telerik.Sitefinity.Data.Linq.Dynamic;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Utilities.TypeConverters;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Lifecycle;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Versioning;
using IdentityModel.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Telerik.Sitefinity.Modules.Forms;
using Telerik.Sitefinity.Modules.Forms.Events;
using Telerik.Sitefinity.Workflow;

namespace SitefinityWebApp.Mvc.Helpers
{
    public static class FormEventHandler
    {
        public const string ClientId = "testApp";
        public const string ClientSecret = "secret";
        public const string TokenEndpoint = "http://localhost:5656/Sitefinity/Authenticate/OpenID/connect/token";
        public const string Username = "bivanova@progress.com";
        public const string Password = "password";
        public const string Scopes = "openid offline_access";
        public static readonly Dictionary<string, string> AdditionalParameters = new Dictionary<string, string>()
        {
            { "membershipProvider", "Default" }
        };
        private static TokenClient tokenClient;

        public static TokenResponse RequestToken()
        {
            //This is call to the token endpoint with the parameters that are set
            TokenResponse tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync(Username, Password, Scopes, AdditionalParameters).Result;

            if (tokenResponse.IsError)
            {
                throw new ApplicationException("Couldn't get access token. Error: " + tokenResponse.Error);
            }

            return tokenResponse;
        }

        public static TokenResponse RefreshToken(string refreshToken)
        {
            //This is call to the token endpoint that can retrieve new access and refresh token from the current refresh token
            return tokenClient.RequestRefreshTokenAsync(refreshToken).Result;
        }

        public static void Handler(FormSavedEvent eventInfo)
        {
            // Set the provider name for the DynamicModuleManager here. All available providers are listed in
            // Administration -> Settings -> Advanced -> DynamicModules -> Providers
            var providerName = String.Empty;

            // Set a transaction name and get the version manager
            var transactionName = "someTransactionName";
            var versionManager = VersionManager.GetManager(null, transactionName);

            DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager(providerName, transactionName);
            Type responseType = TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model.RFPResponses.Response");
            DynamicContent responseItem = dynamicModuleManager.CreateDataItem(responseType);
            
            // This is how values for the properties are set
            var controls = eventInfo.Controls;
            foreach (var control in controls)
            {
                if (!control.FieldName.StartsWith("Form"))
                {
                    responseItem.SetValue(control.FieldName.Replace("_0",""), control.Value);
                }
            }
            
            responseItem.SetString("UrlName", "myurl");
            responseItem.SetValue("Owner", SecurityManager.GetCurrentUserId());
            responseItem.SetValue("PublicationDate", DateTime.UtcNow);
            responseItem.SetWorkflowStatus(dynamicModuleManager.Provider.ApplicationName, "Draft ");

            // Create a version and commit the transaction in order changes to be persisted to data store
            versionManager.CreateVersion(responseItem, false);
            TransactionManager.CommitTransaction(transactionName);

            //Added by BB
            responseItem.SetWorkflowStatus(dynamicModuleManager.Provider.ApplicationName, "AwaitingApproval");
            //WorkflowManager.MessageWorkflow(responseItem.Id, responseType, providerName, "SendForApproval", false, new Dictionary<string, string>());


            TransactionManager.CommitTransaction(transactionName);
            //End added by BB

            //TODO use service call instead of API

            //var formId = eventInfo.FormId;
            //var controls = eventInfo.Controls;
            //var selectors = FormsManager.GetManager().GetForm(formId).Controls.Where(c => c.Caption.Equals("EntitySelector"));
            //if (selectors.Any()) {
            //    var entityName = selectors.FirstOrDefault().Properties.Where(p => p.Name.Equals("Settings")).FirstOrDefault().ChildProperties.Where(p => p.Name.Equals("Entity")).FirstOrDefault().Value;

            //    tokenClient = new TokenClient(TokenEndpoint, ClientId, ClientSecret, AuthenticationStyle.PostValues);
            //    TokenResponse tokenResponse = RequestToken();
            //    string accessToken = tokenResponse.AccessToken;
            //    string refreshToken = tokenResponse.RefreshToken;


            //    WebRequest wr = WebRequest.Create(DDEntityHelper.serviceURL + entityName.ToString().ToLower() + "s");
            //    wr.Method = "POST";
            //    string postData = "{\"";
            //        //+ entityMeta.dsName + "\":{\"" + entityMeta.ttName + "\":[{\"" + entityMeta.primaryKey + "\":" + (maxValue + 1);
            //    foreach(var control in controls){
            //        if (!control.FieldName.StartsWith("Form")) {
            //            if (!postData.Equals("{\"")) postData += ",\"";
            //            postData +=control.FieldName+"\":\""+control.Value+"\"";
            //        }
            //    }
            //    //add each property to body : ",\"Name\":\"Test4\"";
            //    postData += "}";
            //    byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            //    wr.ContentType = "application/json";
            //    wr.ContentLength = byteArray.Length;
            //    Stream dataStream = wr.GetRequestStream();
            //    dataStream.Write(byteArray, 0, byteArray.Length);
            //    dataStream.Close();
            //    wr.Headers.Add("Authorization", "Bearer " + accessToken);
            //    WebResponse wresponse = wr.GetResponse();

            //}

        }
    }
}