// <copyright company="Oswald MASKENS" file="RazorTemplatedEmailSender.cs">
// Copyright 2014-2016 Oswald MASKENS
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
// 
//  http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
// </copyright>

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Infrastructure;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.ViewEngines;
using Microsoft.AspNet.Mvc.ViewFeatures;

namespace HolmuskChallenge.Services.Email
{
    /// <summary>
    /// Default implementation of ITemplatedEmailSender
    /// 
    /// Uses the Razor engine to render email templates.
    /// The templates should be stored under ~/EmailTemplates/{Controller}/{TemplateName}.
    /// </summary>
    public class RazorTemplatedEmailSender : ITemplatedEmailSender
    {
        private readonly IEmailSender _emailSender;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly IHttpContextAccessor _httpContextAccessoraccessor;
        private readonly IActionContextAccessor _actionContextAccessor;

        public RazorTemplatedEmailSender(IEmailSender emailSender,
            ICompositeViewEngine viewEngine,
            IModelMetadataProvider modelMetadataProvider,
            IHttpContextAccessor httpContextAccessoraccessor,
            IActionContextAccessor actionContextAccessor)
        {
            _emailSender = emailSender;
            _viewEngine = viewEngine;
            _modelMetadataProvider = modelMetadataProvider;
            _httpContextAccessoraccessor = httpContextAccessoraccessor;
            _actionContextAccessor = actionContextAccessor;
        }

        private ActionContext ActionContext => _actionContextAccessor.ActionContext;

        /// <inheritdoc/>
        public async Task SendEmailAsync(string template, string email, string subject, object model)
        {
            // If it is not absolute, use ~/EmailTemplates/{Controller}/{TemplateName}
            var message = await RenderTemplate(ResolveTemplatePath(template), model);
            await _emailSender.SendEmailAsync(email, subject, message);
        }

        /// <summary>
        /// Renders a Razor template
        /// </summary>
        private async Task<string> RenderTemplate(string resolvedTemplate, object model)
        {
            var viewData = new ViewDataDictionary(_modelMetadataProvider, new ModelStateDictionary()) { Model = model };

            using (var sw = new StringWriter())
            {
                var viewResult = _viewEngine.FindPartialView(this.ActionContext, resolvedTemplate);

                var viewContext = new ViewContext(
                    this.ActionContext,
                    viewResult.View,
                    viewData,
                    new TempDataDictionary(_httpContextAccessoraccessor, new SessionStateTempDataProvider()),
                    sw,
                    new HtmlHelperOptions());

                await viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// If a template path is not absolute (~/path/to/template),
        /// Resolves it to ~/EmailTemplates/{controller}/{template}
        /// </summary>
        /// <param name="template">The template name</param>
        /// <returns>The resolved template path</returns>
        private string ResolveTemplatePath(string template)
        {
            var controller = this.ActionContext.RouteData.Values["controller"];
            return template.StartsWith("~") ? template : $"~/EmailTemplates/{controller}/{template}";
        }
    }
}