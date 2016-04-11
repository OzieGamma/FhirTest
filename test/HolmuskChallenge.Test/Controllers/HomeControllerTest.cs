// <copyright company="Oswald MASKENS" file="HomeControllerTest.cs">
// Copyright 2014-2016 Oswald MASKENS
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
// 
//  http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
// </copyright>

using FluentAssertions;
using HolmuskChallenge.Controllers;
using Microsoft.AspNet.Mvc;
using Xunit;

namespace HolmuskChallenge.Test.Controllers
{
    public class HomeControllerTest
    {
        [Fact]
        public void IndexShouldRedirectToPatientIndex()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            IActionResult result = controller.Index();

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectResult = (RedirectToActionResult)result;

            redirectResult.ControllerName.Should().Be("Patient");
            redirectResult.ActionName.Should().Be("Index");
        }
    }
}