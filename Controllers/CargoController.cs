﻿using ClinicaVet.GestaoVeterinaria.Constantes;
using ClinicaVet.GestaoVeterinaria.Extensions;
using ClinicaVet.GestaoVeterinaria.Models.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ClinicaVet.GestaoVeterinaria.Controllers
{
    [ClaimsControllerAuthorize(AreasConstantes.GERENCIAR_USUARIOS)]
    public class CargoController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public CargoController(
            RoleManager<IdentityRole> roleManager, 
            UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [ClaimsAuthorize(AreasConstantes.GERENCIAR_USUARIOS, PermissoesConstantes.LER)]
        public IActionResult Index()
        {
            return View(_roleManager.Roles);
        }

        [ClaimsAuthorize(AreasConstantes.GERENCIAR_USUARIOS, PermissoesConstantes.CRIAR)]
        public IActionResult Create() => View();

        [ClaimsAuthorize(AreasConstantes.GERENCIAR_USUARIOS, PermissoesConstantes.CRIAR)]
        [HttpPost]
        public async Task<IActionResult> Create([Required] string name)
        {
            if (ModelState.IsValid)
            {
                await _roleManager.CreateAsync(new IdentityRole(name));
                return RedirectToAction("Index");
            }
            return View(name);
        }

        [ClaimsAuthorize(AreasConstantes.GERENCIAR_USUARIOS, PermissoesConstantes.EXCLUIR)]
        public IActionResult Delete() => View();

        [ClaimsAuthorize(AreasConstantes.GERENCIAR_USUARIOS, PermissoesConstantes.EXCLUIR)]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "No role found");
            return View("Index", _roleManager.Roles);
        }

        [ClaimsAuthorize(AreasConstantes.GERENCIAR_USUARIOS, PermissoesConstantes.ATUALIZAR)]
        public async Task<IActionResult> Update(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            var members = new List<IdentityUser>();
            var nonMembers = new List<IdentityUser>();
            foreach (var user in _userManager.Users)
            {
                var list = await _userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
                list.Add(user);
            }
            return View(new RoleEditDTO
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            });
        }

        [HttpPost]
        public async Task<IActionResult> Update(RoleModification model)
        {
            if (ModelState.IsValid)
            {
                foreach (string userId in model.AddIds ?? new string[] { })
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                        await _userManager.AddToRoleAsync(user, model.RoleName);
                }
                foreach (string userId in model.DeleteIds ?? new string[] { })
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                        await _userManager.RemoveFromRoleAsync(user, model.RoleName);
                }
            }

            if (ModelState.IsValid)
                return RedirectToAction(nameof(Index));
            else
                return await Update(model.RoleId);
        }
    }
}
