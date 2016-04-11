// <copyright company="Oswald MASKENS" file="SendGridConfig.cs">
// Copyright 2014-2016 Oswald MASKENS
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
// 
//  http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
// </copyright>

using System.Net.Mail;

namespace HolmuskChallenge.Config
{
    /// <summary>
    /// Respresent the configuration of a <see href="https://sendgrid.com">SendGrid Account</see>
    /// </summary>
    public class SendGridConfig
    {
        /// <summary>
        /// The <see href="https://sendgrid.com/docs/User_Guide/Settings/api_keys.html">SendGrid Api Key</see>
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The email address used to send emails. 
        /// </summary>
        public string FromEmail { get; set; }

        /// <summary>
        /// The name used to send email.
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// Gets the <see cref="MailAddress"/> buildt using <see cref="FromEmail"/> and <see cref="FromName"/>.
        /// </summary>
        public MailAddress From => new MailAddress(this.FromEmail, this.FromName);
    }
}