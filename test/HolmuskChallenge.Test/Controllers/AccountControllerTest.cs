// <copyright company="Oswald MASKENS" file="AccountControllerTest.cs">
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
using System.Threading.Tasks;
using FluentAssertions;
using HolmuskChallenge.Controllers;
using HolmuskChallenge.Models;
using HolmuskChallenge.Services.Email;
using HolmuskChallenge.ViewModels.Account;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Http.Features.Authentication;
using Microsoft.AspNet.Http.Features.Authentication.Internal;
using Microsoft.AspNet.Http.Internal;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace HolmuskChallenge.Test.Controllers
{
    public class AccountControllerTest
    {
        private ApplicationDbContext Context { get; }

        private UserManager<ApplicationUser> UserManager { get; }

        public SignInManager<ApplicationUser> SignInManager { get; }

        private const string JhonValidEmail = "jhon@example.com";
        private const string JhonValidPassword = "Password!0";
        private const string JhonValidPassword2 = "Password!2";
        private const string JhonInValidPassword = "abcd";

        public AccountControllerTest()
        {
            var services = new ServiceCollection();

            services
                .AddEntityFramework()
                .AddInMemoryDatabase()
                .AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase());

            services
                .AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Taken from https://github.com/aspnet/MusicStore/blob/dev/test/MusicStore.Test/ManageControllerTest.cs (and modified)
            // IHttpContextAccessor is required for SignInManager, and UserManager

            var context = new DefaultHttpContext();
            context.Features.Set<IHttpAuthenticationFeature>(new HttpAuthenticationFeature {Handler = new TestAuthHandler()});
            services.AddSingleton<IHttpContextAccessor>(h => new HttpContextAccessor {HttpContext = context});
            var serviceProvider = services.BuildServiceProvider();
            this.Context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            this.UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            this.SignInManager = serviceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
        }

        [Fact]
        public async Task LoginWithInvalidPasswordShouldDisplayError()
        {
            // Arrange
            var controller = new AccountController(this.UserManager, this.SignInManager, Substitute.For<ITemplatedEmailSender>(),
                Substitute.For<ILoggerFactory>());

            // Act
            var result = await controller.Login(new LoginViewModel {Email = JhonValidEmail, Password = JhonValidPassword});

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task LoginWithValidPasswordShouldRedirect()
        {
            // Arrange
            var templatedEmailSender = Substitute.For<ITemplatedEmailSender>();
            var creationResult = await this.UserManager.CreateAsync(new ApplicationUser {UserName = JhonValidEmail, Email = JhonValidEmail}, JhonValidPassword);
            creationResult.Succeeded.Should().BeTrue();

            var controller = new AccountController(this.UserManager, this.SignInManager, templatedEmailSender, Substitute.For<ILoggerFactory>())
            {
                Url = Substitute.For<IUrlHelper>()
            };
            controller.Url.IsLocalUrl(Arg.Any<string>()).Returns(true);

            // Act
            var result = await controller.Login(new LoginViewModel {Email = JhonValidEmail, Password = JhonValidPassword}, "https://localurl.url");

            // Assert
            result.Should().BeOfType<RedirectResult>();
        }

        [Fact]
        public async Task RegisterShouldRedisplayTheFormIfModelIsInvalid()
        {
            // Arrange
            var controller = new AccountController(this.UserManager, this.SignInManager, Substitute.For<ITemplatedEmailSender>(), Substitute.For<ILoggerFactory>());
            controller.ModelState.AddModelError("wrong", "model");


            // Act
            var result = await controller.Register(new RegisterViewModel());

            // Assert
            result.Should().BeOfType<ViewResult>();
        }
    }

    public class TestAuthHandler : IAuthenticationHandler
    {
        public void GetDescriptions(DescribeSchemesContext context)
        {
            context.Accept(new Dictionary<string, object> {{"Authentification", this}});
        }

        public Task AuthenticateAsync(AuthenticateContext context)
        {
            throw new NotImplementedException();
        }

        public Task ChallengeAsync(ChallengeContext context)
        {
            throw new NotImplementedException();
        }

        public Task SignInAsync(SignInContext context)
        {
            context.Accept();
            return Task.FromResult(0);
        }

        public Task SignOutAsync(SignOutContext context)
        {
            throw new NotImplementedException();
        }
    }
}