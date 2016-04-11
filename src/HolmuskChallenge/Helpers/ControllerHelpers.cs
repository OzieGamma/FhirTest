// <copyright company="Oswald MASKENS" file="ControllerHelpers.cs">
// Copyright 2014-2016 Oswald MASKENS
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
// 
//  http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
// </copyright>

using HolmuskChallenge.Controllers;
using Microsoft.AspNet.Mvc;

namespace HolmuskChallenge.Helpers
{
    /// <summary>
    /// Contains helpers methods for ASP.NET MVC Core 1 <see cref="Controller"/>.
    /// </summary>
    public static class ControllerHelpers
    {
        /// <summary>
        /// Forces an Url to be local.
        /// </summary>
        /// <param name="controller">The <see cref="Controller"/> to extend.</param>
        /// <param name="url">The url to test.</param>
        /// <returns>The url if it's local, otherwise a link to the home page.</returns>
        public static string ForceLocal(this Controller controller, string url)
        {
            if (controller.Url.IsLocalUrl(url))
            {
                return url;
            }
            return controller.Url.Action(nameof(HomeController.Index), "Home");
        }

        /// <summary>
        /// Creates an <see cref="RedirectResult"/> based on <see cref="ForceLocal"/>
        /// </summary>
        /// <param name="controller">The <see cref="Controller"/> to extend.</param>
        /// <param name="url">The url to used (if local).</param>
        public static IActionResult RedirectToLocal(this Controller controller, string url) => new RedirectResult(ForceLocal(controller, url));
    }
}