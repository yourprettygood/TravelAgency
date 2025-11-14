using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DALtravelagency.Interfaces;
using DomainTravelAgency.Models;
using Microsoft.EntityFrameworkCore;

namespace DALtravelagency.Storage
{
    /// <summary>
    /// Реализация репозитория аккаунтов (пользователей).
    /// </summary>
    public sealed class UserStorage : IAccountStorage
    {
        private readonly AppDbContext _db;

        public UserStorage(AppDbContext db)
        {
            _db = db;
        }

        // ===== Реализация IBaseStorage<User> =====

        public Task<List<User>> GetAllAsync()
        {
            return _db.Users
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<User?> GetByIdAsync(long id)
        {
            return _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User> AddAsync(User user)
        {
            if (user.CreatedAt == default)
                user.CreatedAt = DateTime.UtcNow;

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task UpdateAsync(User user)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var existing = await _db.Users.FindAsync(id);
            if (existing != null)
            {
                _db.Users.Remove(existing);
                await _db.SaveChangesAsync();
            }
        }

        // ===== Дополнительные методы IAccountStorage =====

        public async Task<bool> IsEmailFreeAsync(string email, CancellationToken ct = default)
        {
            var normalized = email.Trim().ToLowerInvariant();

            return !await _db.Users
                .AsNoTracking()
                .AnyAsync(u => u.Email.ToLower() == normalized, ct);
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            var normalized = email.Trim().ToLowerInvariant();

            return _db.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalized, ct);
        }
    }
}
