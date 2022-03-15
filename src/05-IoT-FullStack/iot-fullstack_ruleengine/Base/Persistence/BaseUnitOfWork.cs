﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Base.Contracts.Persistence;
using Base.Contracts.Validation;
using Base.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Base.Persistence
{
    public class BaseUnitOfWork : IBaseUnitOfWork
    {
        public BaseApplicationDbContext BaseApplicationDbContext { get; init; }
        private bool _disposed;

        public BaseUnitOfWork(BaseApplicationDbContext dbContext)
        {
            BaseApplicationDbContext = dbContext;
            dbContext.Database.EnsureCreated();
            Sessions = new SessionRepository(dbContext);
            ApplicationUsers = new ApplicationUserRepository(dbContext);
        }

        public ISessionRepository Sessions { get; }
        public IApplicationUserRepository ApplicationUsers { get; }


        public async Task<int> SaveChangesAsync()
        {
            var entities = BaseApplicationDbContext.ChangeTracker.Entries()
                .Where(entity => entity.State == EntityState.Added || entity.State == EntityState.Modified)
                .Select(e => e.Entity)
                .ToArray();  // Geänderte Entities ermitteln
            foreach (var entity in entities)
            {
                var validationContext = new ValidationContext(entity, null, null);
                if (entity is IDatabaseValidatableObject) // UnitOfWork injizieren, wenn Interface implementiert ist
                {
                    validationContext.InitializeServiceProvider(serviceType => this);
                }
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(entity, validationContext, validationResults,
                    validateAllProperties: true);
                if (!isValid)
                {
                    var memberNames = new List<string>();
                    List<ValidationException> validationExceptions = new();
                    foreach (ValidationResult validationResult in validationResults)
                    {
                        validationExceptions.Add(new ValidationException(validationResult, null,
                                validationResult.MemberNames));
                        memberNames.AddRange(validationResult.MemberNames);
                    }
                    if (validationExceptions.Count == 1)  // eine Validationexception werfen
                    {
                        throw validationExceptions.Single();
                    }
                    else  // AggregateException mit allen ValidationExceptions als InnerExceptions werfen
                    {
                        throw new ValidationException($"Entity validation failed for {string.Join(", ", memberNames)}",
                            new AggregateException(validationExceptions));
                    }
                }
            }
            return await BaseApplicationDbContext.SaveChangesAsync();
        }

        public async Task DeleteDatabaseAsync() => await BaseApplicationDbContext.Database.EnsureDeletedAsync();
        public async Task MigrateDatabaseAsync() => await BaseApplicationDbContext.Database.MigrateAsync();
        public async Task CreateDatabaseAsync() => await BaseApplicationDbContext.Database.EnsureCreatedAsync();

        public void SetNoTracking()
        {
            BaseApplicationDbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }



        #region Dispose
        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    await BaseApplicationDbContext.DisposeAsync();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            BaseApplicationDbContext.Dispose();
            GC.SuppressFinalize(this);
        }


        #endregion

    }
}
