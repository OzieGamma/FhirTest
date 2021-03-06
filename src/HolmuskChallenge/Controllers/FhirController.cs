﻿// <copyright company="Oswald MASKENS" file="FhirController.cs">
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
    /// <summary>
    /// Controller for the fhir search functionality. Let's you import data from a FHIR server.
    /// </summary>
    [Authorize]
    public class FhirController : Controller
    {
        private readonly IFhirClient _fhirClient;

        public FhirController(IFhirClientFactory fhirClientFactory)
        {
            _fhirClient = fhirClientFactory.NewClient(new Uri("http://nprogram.azurewebsites.net/"));
        }

        /// <summary>
        /// Displays a list of patients gotten from the FHIR server.
        /// </summary>
        /// <param name="query">If provided, filter the search using this query.</param>
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

        /// <summary>
        /// Displays a summary and confirmation before importingg the patient.
        /// The actual import is POSTed to <see cref="Controller.Create(Patient, string)" />
        /// </summary>
        /// <param name="patient">The patient to confim import of.</param>
        [HttpGet]
        public ViewResult Import(Patient patient)
        {
            bool valid = this.ModelState.IsValid;
            this.ViewData["ReturnUrl"] = this.Url.Action("ImportSuccessfull");
            return View();
        }

        /// <summary>
        /// Displays a confirmation that the import was successfull.
        /// </summary>
        [HttpGet]
        public ViewResult ImportSuccessful() => View();
    }
}