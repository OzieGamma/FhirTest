// <copyright company="Oswald MASKENS" file="DefaultFhirClient.cs">
// Copyright 2014-2016 Oswald MASKENS
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
// 
//  http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using HolmuskChallenge.Models;
using HolmuskChallenge.ViewModels.Fhir;
using Patient = Hl7.Fhir.Model.Patient;

namespace HolmuskChallenge.Services.Fhir
{
    public class DefaultFhirClient : IFhirClient
    {
        private readonly FhirClient _client;

        public DefaultFhirClient(Uri endPoint)
        {
            _client = new FhirClient(endPoint);
        }

        private IEnumerable<SearchViewModel.PatientViewModel> Search(SearchParams searchParams)
        {
            var result = _client.Search(searchParams, "Patient");
            return result.Entry.Select(_ => _.Resource as Patient).Select(ParseFhirPatientToPatientInfo);
        }

        private SearchViewModel.PatientViewModel ParseFhirPatientToPatientInfo(Patient fhirPatient)
        {
            return new SearchViewModel.PatientViewModel
            {
                GivenName = fhirPatient.Name.First().Given.First(),
                FamilyName = fhirPatient.Name.First().Family.First(),
                DateOfBirth = DateTimeOffset.Parse(fhirPatient.BirthDate),
                Gender = fhirPatient.Gender == AdministrativeGender.Male
                    ? Gender.Male
                    : fhirPatient.Gender == AdministrativeGender.Female
                        ? Gender.Female
                        : Gender.Other,
                Comments = string.Join(Environment.NewLine, fhirPatient.FhirComments)
            };
        }

        public IEnumerable<SearchViewModel.PatientViewModel> SearchAny() => Search(new SearchParams());

        public IEnumerable<SearchViewModel.PatientViewModel> SearchPatientByNameAsync(string name)
        {
            var searchParams = new SearchParams
            {
                Parameters = {new Tuple<string, string>("name", name)}
            };

            return Search(searchParams);
        }
    }
}