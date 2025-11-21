using Microsoft.AspNetCore.Mvc;
using PatientsProject.App.Models;
using PatientsProject.Core.App.Services.MVC;

namespace PatientsProject.Mvc.Controllers;

public class BranchesController : Controller
    {
        // Service injections:
        private readonly IService<BranchRequest, BranchResponse> _branchService;

        public BranchesController(IService<BranchRequest, BranchResponse> branchService)
        {
            _branchService = branchService;
        }
        private void SetTempData(string message, string key = "Message")
        {
            TempData[key] = message;
        }

        public IActionResult Index()
        {
            var list = _branchService.List();
            return View(list); 
        }

        public IActionResult Details(int id)
        {
            var item = _branchService.Item(id);
            return View(item); 
        }

        public IActionResult Create()
        {
            return View(); 
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(BranchRequest branch)
        {
            if (ModelState.IsValid) 
            {
                // Insert item service logic:
                var response = _branchService.Create(branch);
                if (response.IsSuccessful)
                {
                    SetTempData(response.Message); 
                    return RedirectToAction(nameof(Details), new { id = response.Id }); 
                }
                ModelState.AddModelError("", response.Message); 
            }
            return View(branch); 
        }

        public IActionResult Edit(int id)
        {
            // Get item to edit service logic:
            var item = _branchService.Edit(id);
            if (item == null) // Kayıt bulunamazsa listeye dön
                return RedirectToAction(nameof(Index));

            return View(item); 
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(BranchRequest branch)
        {
            if (ModelState.IsValid) 
            {
                var response = _branchService.Update(branch);
                if (response.IsSuccessful)
                {
                    SetTempData(response.Message); 
                    return RedirectToAction(nameof(Details), new { id = response.Id }); 
                }
                ModelState.AddModelError("", response.Message); 
            }
            return View(branch); 
        }

        public IActionResult Delete(int id)
        {
            // Get item to delete service logic:
            var item = _branchService.Item(id);
            return View(item); 
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            // Delete item service logic:
            var response = _branchService.Delete(id);
            SetTempData(response.Message); 
            return RedirectToAction(nameof(Index)); 
        }
    }