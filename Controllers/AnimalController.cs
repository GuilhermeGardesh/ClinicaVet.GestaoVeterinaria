﻿using ClinicaVet.GestaoVeterinaria.Constantes;
using ClinicaVet.GestaoVeterinaria.Extensions;
using ClinicaVet.GestaoVeterinaria.Interfaces;
using ClinicaVet.GestaoVeterinaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClinicaVet.GestaoVeterinaria.Controllers
{
    [ClaimsControllerAuthorize(AreasConstantes.ANIMAL)]
    public class AnimalController : Controller
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IProprietarioRepository _proprietarioRepository;

        public AnimalController(
            IAnimalRepository animalRepository
            , IProprietarioRepository proprietarioRepository)
        {
            _animalRepository = animalRepository;
            _proprietarioRepository = proprietarioRepository;
        }

        // GET: AnimalController
        [ClaimsAuthorize(AreasConstantes.ANIMAL, PermissoesConstantes.LER)]
        public ActionResult Index()
        {
            var animais = _animalRepository.ObterAnimaisProprietarios();
            return View(animais);
        }

        // GET: AnimalController/Details/5
        [ClaimsAuthorize(AreasConstantes.ANIMAL, PermissoesConstantes.LER)]
        public ActionResult Details(int idAnimal)
        {
            var animal = _animalRepository.ObterPorId(idAnimal);
            return View(animal);
        }

        // GET: AnimalController/Create
        [ClaimsAuthorize(AreasConstantes.ANIMAL, PermissoesConstantes.CRIAR)]
        public ActionResult Create()
        {
            ViewBag.proprietarios = new SelectList(_proprietarioRepository.ObterTodos(), "Id", "Nome");
            return View();
        }

        // POST: AnimalController/Create
        [ClaimsAuthorize(AreasConstantes.ANIMAL, PermissoesConstantes.CRIAR)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Animal animal)
        {
            try
            {
                _animalRepository.Inserir(animal);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        // GET: AnimalController/Edit/5
        [ClaimsAuthorize(AreasConstantes.ANIMAL, PermissoesConstantes.EDITAR)]
        public ActionResult Edit(int idAnimal)
        {
            var animal = _animalRepository.ObterPorId(idAnimal);
            return View(animal);
        }

        // POST: AnimalController/Edit/5
        [ClaimsAuthorize(AreasConstantes.ANIMAL, PermissoesConstantes.EDITAR)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Animal animal)
        {
            try
            {
                _animalRepository.Atualizar(animal);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AnimalController/Delete/5
        [ClaimsAuthorize(AreasConstantes.ANIMAL, PermissoesConstantes.EXCLUIR)]
        public ActionResult Delete(int idAnimal)
        {
            var animal = _animalRepository.ObterPorId(idAnimal);
            return View(animal);
        }

        // POST: AnimalController/Delete/5
        [ClaimsAuthorize(AreasConstantes.ANIMAL, PermissoesConstantes.EXCLUIR)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int idAnimal)
        {
            try
            {
                var animal = _animalRepository.ObterPorId(idAnimal);

                _animalRepository.Deletar(animal);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
