// <copyright company="Oswald MASKENS" file="SearchViewModel.cs">
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
using System.ComponentModel.DataAnnotations;
using HolmuskChallenge.Models;

namespace HolmuskChallenge.ViewModels.Fhir
{
    public class SearchViewModel
    {
        public class QueryViewModel
        {
            [Required]
            [StringLength(255, MinimumLength = 1)]
            [Display(Name = "Name", Prompt = "eg. Smith")]
            public string Name { get; set; }
        }

        public class PatientViewModel
        {
            [Display(Name = "Given name")]
            public string GivenName { get; set; }

            [Display(Name = "Family name")]
            public string FamilyName { get; set; }

            [Display(Name = "Date of birth")]
            public DateTimeOffset DateOfBirth { get; set; }

            public Gender Gender { get; set; }

            public string Comments { get; set; }

            public Patient ToPatientInfo()
            {
                return new Patient
                {
                    GivenName = this.GivenName,
                    FamilyName = this.FamilyName,
                    DateOfBirth = this.DateOfBirth,
                    Gender = this.Gender,
                    Comments = this.Comments
                };
            }
        }

        public QueryViewModel Query { get; set; }
        public IEnumerable<PatientViewModel> Patients { get; set; }
    }
}