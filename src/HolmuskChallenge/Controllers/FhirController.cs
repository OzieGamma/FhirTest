// <copyright company="Oswald MASKENS" file="FhirController.cs">
// Copyright 2014-2016 Oswald MASKENS
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
// 
//  http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
// </copyright>

using System;
using HolmuskChallenge.Models;
using HolmuskChallenge.Services.Fhir;
using HolmuskChallenge.ViewModels.Fhir;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

namespace HolmuskChallenge.Controllers
{
    [Authorize]
    public class FhirController : Controller
    {
        private readonly IFhirClient _fhirClient;

        public FhirController(IFhirClientFactory fhirClientFactory)
        {
            _fhirClient = fhirClientFactory.NewClient(new Uri("http://nprogram.azurewebsites.net/"));
        }

        [HttpGet]
        public ViewResult Search([FromQuery] SearchViewModel.QueryViewModel query)
        {
            if (!this.ModelState.IsValid || (query == null))
            {
                return View(new SearchViewModel
                {
                    Query = new SearchViewModel.QueryViewModel(),
                    Patients = _fhirClient.SearchAny()
                });
            }

            var patients = _fhirClient.SearchPatientByNameAsync(query.Name);
            return View(new SearchViewModel {Query = query, Patients = patients});
        }

        [HttpGet]
        public ViewResult Import(Patient patient)
        {
            bool valid = this.ModelState.IsValid;
            this.ViewData["ReturnUrl"] = this.Url.Action("ImportSuccessfull");
            return View();
        }

        [HttpGet]
        public ViewResult ImportSuccessful() => View();
    }
}