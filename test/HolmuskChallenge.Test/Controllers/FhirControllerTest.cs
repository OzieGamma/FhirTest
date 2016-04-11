// <copyright company="Oswald MASKENS" file="FhirControllerTest.cs">
// Copyright 2014-2016 Oswald MASKENS
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
// 
//  http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
// </copyright>

using System;
using FluentAssertions;
using HolmuskChallenge.Controllers;
using HolmuskChallenge.Services.Fhir;
using HolmuskChallenge.ViewModels.Fhir;
using Microsoft.AspNet.Mvc;
using NSubstitute;
using Xunit;

namespace HolmuskChallenge.Test.Controllers
{
    public class FhirControllerTest
    {
        [Fact]
        public void ImportSuccessfulShouldReturnAView()
        {
            // Arrange
            var controller = new FhirController(Substitute.For<IFhirClientFactory>());

            // Act
            ViewResult result = controller.ImportSuccessful();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void CallSearchWithNullShouldSearchAny()
        {
            // Arrange
            var fhirClientFactory = Substitute.For<IFhirClientFactory>();
            var fhirClient = Substitute.For<IFhirClient>();

            fhirClientFactory.NewClient(Arg.Any<Uri>()).Returns(fhirClient);

            var controller = new FhirController(fhirClientFactory);

            // Act
            ViewResult result = controller.Search(null);

            // Assert
            fhirClient.Received(1).SearchAny();
            fhirClient.Received(0).SearchPatientByNameAsync(Arg.Any<string>());
        }

        [Fact]
        public void CallSearchWithValidQueryShouldSearchByName()
        {
            // Arrange
            var fhirClientFactory = Substitute.For<IFhirClientFactory>();
            var fhirClient = Substitute.For<IFhirClient>();

            fhirClientFactory.NewClient(Arg.Any<Uri>()).Returns(fhirClient);

            var controller = new FhirController(fhirClientFactory);

            // Act
            ViewResult result = controller.Search(new SearchViewModel.QueryViewModel {Name = "Rose"});

            // Assert
            fhirClient.Received(0).SearchAny();
            fhirClient.Received(1).SearchPatientByNameAsync(Arg.Any<string>());
            fhirClient.Received(1).SearchPatientByNameAsync("Rose");
        }
    }
}