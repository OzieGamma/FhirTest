// <copyright company="Oswald MASKENS" file="Patient.cs">
// Copyright 2014-2016 Oswald MASKENS
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
// 
//  http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;

namespace HolmuskChallenge.Models
{
    public class Patient
    {
        [Required, Key]
        public long Id { get; set; }

        [Required]
        public bool Deleted { get; set; } = false;

        [Required]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "The first name should be at least 1 and maximum 255 characters long")]
        [Display(Name = "First name")]
        public string GivenName { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "The last name should be at least 1 and maximum 255 characters long")]
        [Display(Name = "Last name")]
        public string FamilyName { get; set; }

        [Required]
        [Display(Name = "Date of birth")]
        public DateTimeOffset DateOfBirth { get; set; }

        [Required]
        public Gender Gender { get; set; }

        public string Comments { get; set; }

        [Required]
        [Display(Name = "Created at")]
        public DateTimeOffset CreatedAt { get; set; }

        [Required]
        [Display(Name = "Last modified at")]
        public DateTimeOffset LastModifiedAt { get; set; }
    }
}