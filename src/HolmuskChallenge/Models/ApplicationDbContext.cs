// <copyright company="Oswald MASKENS" file="ApplicationDbContext.cs">
// Copyright 2014-2016 Oswald MASKENS
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
// 
//  http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
// </copyright>

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking;

namespace HolmuskChallenge.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Patient> Patients { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        /// <inheritdoc />
        public override int SaveChanges() => SaveChanges(true);

        /// <inheritdoc />
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            InterceptSaveChanges();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        /// <inheritdoc />
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
            => await SaveChangesAsync(true, cancellationToken);

        /// <inheritdoc />
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            InterceptSaveChanges();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        ///     Intercepts changes made to models before they are saved in the database.
        /// </summary>
        private void InterceptSaveChanges()
        {
            InterceptAddChanges();
            InterceptModifiedChanges();
            InterceptDeletedChanges();
        }

        /// <summary>
        ///     Adds CreatedAt and LastModifiedAt on new PatientInfos
        /// </summary>
        private void InterceptAddChanges()
        {
            var addedPatients =
                this.ChangeTracker.Entries()
                    .Where(x => x.State == EntityState.Added)
                    .Select(_ => _.Entity)
                    .OfType<Patient>()
                    .ToList();

            foreach (Patient addedPatient in addedPatients)
            {
                addedPatient.CreatedAt = DateTimeOffset.UtcNow;
                addedPatient.LastModifiedAt = DateTimeOffset.UtcNow;
                addedPatient.DateOfBirth = addedPatient.DateOfBirth.ToUniversalTime();
            }
        }

        /// <summary>
        ///     Updates LastModifiedAt on modified PatientInfos
        /// </summary>
        private void InterceptModifiedChanges()
        {
            var modifiedPatients =
                this.ChangeTracker.Entries()
                    .Where(x => x.State == EntityState.Modified)
                    .Select(_ => _.Entity)
                    .OfType<Patient>()
                    .ToList();

            foreach (Patient modifiedPatient in modifiedPatients)
            {
                modifiedPatient.LastModifiedAt = DateTimeOffset.UtcNow;
                modifiedPatient.DateOfBirth = modifiedPatient.DateOfBirth.ToUniversalTime();
            }
        }

        /// <summary>
        /// Prevents deletion of PatientInfos. Simply marks them with the Delete Flag
        /// </summary>
        private void InterceptDeletedChanges()
        {
            var deletedPatients =
                this.ChangeTracker.Entries()
                    .Where(_ => _.State == EntityState.Deleted)
                    .ToList();

            foreach (EntityEntry deletedPatient in deletedPatients)
            {
                var entityAsPatient = deletedPatient.Entity as Patient;

                if (entityAsPatient != null)
                {
                    deletedPatient.State = EntityState.Modified;
                    entityAsPatient.Deleted = true;
                }
            }
        }
    }
}