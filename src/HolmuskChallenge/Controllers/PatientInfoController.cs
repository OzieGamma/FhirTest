// <copyright company="Oswald MASKENS" file="PatientInfoController.cs">
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
using HolmuskChallenge.Models;
using HolmuskChallenge.ViewModels;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;

namespace HolmuskChallenge.Controllers
{
    [Authorize]
    public class PatientInfoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientInfoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Patient
        public async Task<IActionResult> Index()
        {
            return View(await _context.Patients.Where(_ => _.Deleted == false).ToListAsync());
        }

        // GET: Patient/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var patientInfo = await _context.Patients.Where(_ => _.Deleted == false).SingleAsync(m => m.Id == id);
            if (patientInfo == null)
            {
                return HttpNotFound();
            }

            return View(patientInfo);
        }

        // GET: Patient/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Patient/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient)
        {
            if (this.ModelState.IsValid)
            {
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            
            return View(patient);
        }

        // GET: Patient/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var patientInfo = await _context.Patients.Where(_ => _.Deleted == false).SingleAsync(m => m.Id == id);
            if (patientInfo == null)
            {
                return HttpNotFound();
            }
            return View(patientInfo);
        }

        // POST: Patient/Edit/5
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

        // GET: Patient/Delete/5
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var patientInfo = await _context.Patients.Where(_ => _.Deleted == false).SingleAsync(m => m.Id == id);
            if (patientInfo == null)
            {
                return HttpNotFound();
            }

            return View(patientInfo);
        }

        // POST: Patient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var patientInfo = await _context.Patients.SingleAsync(m => m.Id == id);
            _context.Patients.Remove(patientInfo);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}