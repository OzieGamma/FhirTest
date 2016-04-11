// <copyright company="Oswald MASKENS" file="PatientController.cs">
// Copyright 2014-2016 Oswald MASKENS
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
// 
//  http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
// </copyright>

using System.Linq;
using System.Threading.Tasks;
using HolmuskChallenge.Helpers;
using HolmuskChallenge.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;

namespace HolmuskChallenge.Controllers
{
    /// <summary>
    /// Handels the CRUD operations for a <see cref="Patient" />
    /// </summary>
    [Authorize]
    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays a list of <see cref="Patient" />
        /// </summary>
        public async Task<IActionResult> Index()
        {
            return View(await _context.Patients.Where(_ => _.Deleted == false).ToListAsync());
        }

        /// <summary>
        /// Displays the details of 1 <see cref="Patient" />
        /// </summary>
        /// <param name="id">The id of the <see cref="Patient" /> to display.</param>
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Patient patientInfo = await _context.Patients.Where(_ => _.Deleted == false).SingleAsync(m => m.Id == id);
            if (patientInfo == null)
            {
                return HttpNotFound();
            }

            return View(patientInfo);
        }

        /// <summary>
        /// Displays the Create view for a  <see cref="Patient" />
        /// </summary>
        public IActionResult Create()
        {
            this.ViewData["ReturnUrl"] = this.Url.Action("Index");
            return View();
        }

        /// <summary>
        /// Creates a  <see cref="Patient" /> if the passed Model is valid. 
        /// Otherwise redisplays the create form.
        /// </summary>
        /// <param name="patient">The <see cref="Patient"/> to create.</param>
        /// <param name="returnUrl">The url to redirect to once the <see cref="Patient"/> was created.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient, string returnUrl = null)
        {
            if (this.ModelState.IsValid)
            {
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
                return this.RedirectToLocal(returnUrl);
            }

            return View(patient);
        }

        /// <summary>
        /// Displays the Edit view for a <see cref="Patient" />.
        /// </summary>
        /// <param name="id">The id of the <see cref="Patient" /> to edit.</param>
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Patient patientInfo = await _context.Patients.Where(_ => _.Deleted == false).SingleAsync(m => m.Id == id);
            if (patientInfo == null)
            {
                return HttpNotFound();
            }
            return View(patientInfo);
        }

        /// <summary>
        /// Edits a <see cref="Patient" /> and redirct to the list of Patients. <see cref="Index" />
        /// </summary>
        /// <param name="patient">The edited <see cref="Patient" />.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Patient patient)
        {
            if (this.ModelState.IsValid && !patient.Deleted)
            {
                _context.Update(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(patient);
        }

        /// <summary>
        /// Displays a confirmation page for deleting a <see cref="Patient" />.
        /// </summary>
        /// <param name="id">The id of the <see cref="Patient" /> to delete.</param>
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Patient patientInfo = await _context.Patients.Where(_ => _.Deleted == false).SingleAsync(m => m.Id == id);
            if (patientInfo == null)
            {
                return HttpNotFound();
            }

            return View(patientInfo);
        }

        /// <summary>
        /// Deletes a <see cref="Patient" /> and redirects to the list of <see cref="Patient" />. <see cref="Index"/>
        /// </summary>
        /// <param name="id">The id of the <see cref="Patient" /> to delete.</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            Patient patientInfo = await _context.Patients.SingleAsync(m => m.Id == id);
            _context.Patients.Remove(patientInfo);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}