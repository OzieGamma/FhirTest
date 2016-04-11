// <copyright company="Oswald MASKENS" file="IFhirClient.cs">
// Copyright 2014-2016 Oswald MASKENS
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
// 
//  http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
// </copyright>

using System.Collections.Generic;
using HolmuskChallenge.ViewModels.Fhir;

namespace HolmuskChallenge.Services.Fhir
{
    /// <summary>
    /// A client to access a <see href="https://www.hl7.org/fhir/">FHIR</see> server.
    /// </summary>
    public interface IFhirClient
    {
        /// <summary>
        /// Searches a patient based on his/her name.
        /// </summary>
        /// <param name="name">The name of the patient.</param>
        /// <returns>The patients matching the search query.</returns>
        IEnumerable<SearchViewModel.PatientViewModel> SearchPatientByNameAsync(string name);

        /// <summary>
        /// Gets patients from the FHIR server without any search query.
        /// </summary>
        /// <returns>The first patients returned by the server.</returns>
        IEnumerable<SearchViewModel.PatientViewModel> SearchAny();
    }
}