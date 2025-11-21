using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PatientsProject.App.Models;
using PatientsProject.Core.App.Services.MVC;

namespace PatientsProject.Mvc.Controllers;

public class DoctorsController : Controller
    {
        private readonly IService<DoctorRequest, DoctorResponse> _doctorService;
        private readonly IService<BranchRequest, BranchResponse> _branchService;
        
        // YENİ EKLENENLER: User ve Group servisleri
        private readonly IService<UserRequest, UserResponse> _userService;
        private readonly IService<GroupRequest, GroupResponse> _groupService;

        public DoctorsController(
            IService<DoctorRequest, DoctorResponse> doctorService,
            IService<BranchRequest, BranchResponse> branchService,
            IService<UserRequest, UserResponse> userService,
            IService<GroupRequest, GroupResponse> groupService)
        {
            _doctorService = doctorService;
            _branchService = branchService;
            _userService = userService;
            _groupService = groupService;
        }

        private void SetViewData()
        {
            // 1. Branş Listesi
            var branches = _branchService.List();
            ViewBag.BranchId = new SelectList(branches, "Id", "Title");

            // 2. User Listesi (Kullanıcı Adı görünsün, ID arka planda tutulsun)
            var users = _userService.List();
            ViewBag.UserId = new SelectList(users, "Id", "UserName");

            // 3. Group Listesi (Grup Başlığı görünsün)
            var groups = _groupService.List();
            ViewBag.GroupId = new SelectList(groups, "Id", "Title");
        }

        private void SetTempData(string message, string key = "Message")
        {
            TempData[key] = message;
        }

        public IActionResult Index()
        {
            var list = _doctorService.List();
            return View(list);
        }

        public IActionResult Details(int id)
        {
            var item = _doctorService.Item(id);
            return View(item);
        }

        public IActionResult Create()
        {
            SetViewData(); // Tüm dropdownları doldur
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DoctorRequest doctor)
        {
            if (ModelState.IsValid)
            {
                var response = _doctorService.Create(doctor);
                if (response.IsSuccessful)
                {
                    SetTempData(response.Message);
                    return RedirectToAction(nameof(Details), new { id = response.Id });
                }
                ModelState.AddModelError("", response.Message);
            }
            SetViewData();
            return View(doctor);
        }

        public IActionResult Edit(int id)
        {
            var item = _doctorService.Edit(id);
            if (item == null)
                return RedirectToAction(nameof(Index));

            SetViewData();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(DoctorRequest doctor)
        {
            if (ModelState.IsValid)
            {
                var response = _doctorService.Update(doctor);
                if (response.IsSuccessful)
                {
                    SetTempData(response.Message);
                    return RedirectToAction(nameof(Details), new { id = response.Id });
                }
                ModelState.AddModelError("", response.Message);
            }
            SetViewData();
            return View(doctor);
        }

        public IActionResult Delete(int id)
        {
            var item = _doctorService.Item(id);
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var response = _doctorService.Delete(id);
            SetTempData(response.Message);
            return RedirectToAction(nameof(Index));
        }
    }