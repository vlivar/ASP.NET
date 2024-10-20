using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models.Requests;
using PromoCodeFactory.WebHost.Models.Response;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Роли сотрудников
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RolesController
    {
        private readonly IRoleRepository _rolesRepository;

        public RolesController(IRoleRepository rolesRepository)
        {
            _rolesRepository = rolesRepository;
        }

        /// <summary>
        /// Получить все доступные роли сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<RoleItemResponse>> GetRolesAsync(CancellationToken cancellationToken)
        {
            var roles = await _rolesRepository.GetAllAsync(cancellationToken);

            var rolesModelList = roles.Select(x =>
                new RoleItemResponse()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                }).ToList();

            return rolesModelList;
        }

        /// <summary>
        /// Добавить новую роль для сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<RoleItemResponse> AddRolesAsync(CancellationToken cancellationToken, RoleRequest role)
        {
            var roleItem = new Role()
            {
                Name = role.Name,
                Description = role.Description,
                Id = Guid.NewGuid()
            };
            var newPole = await _rolesRepository.AddAsync(roleItem);
            await _rolesRepository.SaveChangesAsync();
            var rolesModel = 
                new RoleItemResponse()
                {
                    Id = newPole.Id,
                    Name = newPole.Name,
                    Description = newPole.Description
                };

            return rolesModel;
        }
    }
}
