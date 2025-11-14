using System.Threading;
using System.Threading.Tasks;
using DomainTravelAgency.Models;

namespace DALtravelagency.Interfaces
{
    /// <summary>
    /// Репозиторий для работы с аккаунтами (пользователями).
    /// Наследуется от базового CRUD-репозитория.
    /// </summary>
    public interface IAccountStorage : IBaseStorage<User>
    {
        /// <summary>
        /// Проверить, свободен ли email.
        /// </summary>
        Task<bool> IsEmailFreeAsync(string email, CancellationToken ct = default);

        /// <summary>
        /// Найти пользователя по email (для логина).
        /// </summary>
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    }
}
