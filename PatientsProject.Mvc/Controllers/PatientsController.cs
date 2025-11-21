using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PatientsProject.App.Models;
using PatientsProject.Core.App.Services.MVC;

namespace PatientsProject.Mvc.Controllers;

public class PatientsController : Controller
    {
        private readonly IService<PatientRequest, PatientResponse> _patientService;
        private readonly IService<DoctorRequest, DoctorResponse> _doctorService;
        private readonly IService<UserRequest, UserResponse> _userService;
        private readonly IService<GroupRequest, GroupResponse> _groupService;

        public PatientsController(
            IService<PatientRequest, PatientResponse> patientService,
            IService<DoctorRequest, DoctorResponse> doctorService,
            IService<UserRequest, UserResponse> userService,
            IService<GroupRequest, GroupResponse> groupService)
        {
            _patientService = patientService;
            _doctorService = doctorService;
            _userService = userService;
            _groupService = groupService;
        }

        private void SetViewData(List<int>? selectedDoctorIds = null, int? selectedUserId = null, int? selectedGroupId = null)
        {
            var doctors = _doctorService.List();
            ViewBag.Doctors = new MultiSelectList(doctors, "Id", "DoctorName", selectedDoctorIds);

            var users = _userService.List();
            ViewBag.UserId = new SelectList(users, "Id", "UserName", selectedUserId);

            var groups = _groupService.List();
            ViewBag.GroupId = new SelectList(groups, "Id", "Title", selectedGroupId);
        }

        private void SetTempData(string message, string key = "Message")
        {
            TempData[key] = message;
        }

        public IActionResult Index()
        {
            var list = _patientService.List();
            return View(list);
        }
        public IActionResult Details(int id)
        {
            var item = _patientService.Item(id);
            if (item == null)
            {
                SetTempData("Patient not found!");
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        public IActionResult Create()
        {
            SetViewData(); 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PatientRequest patient)
        {
            if (ModelState.IsValid)
            {
                var response = _patientService.Create(patient);
                if (response.IsSuccessful)
                {
                    SetTempData(response.Message);
                    return RedirectToAction(nameof(Details), new { id = response.Id }); 
                }
                ModelState.AddModelError("", response.Message);
            }
            
            SetViewData(patient.DoctorIds, patient.UserId, patient.GroupId); 
            return View(patient);
        }

        public IActionResult Edit(int id)
        {
            var item = _patientService.Edit(id);
            if (item == null)
            {
                SetTempData("Patient not found!");
                return RedirectToAction(nameof(Index));
            }

            SetViewData(item.DoctorIds, item.UserId, item.GroupId); 
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(PatientRequest patient)
        {
            if (ModelState.IsValid)
            {
                var response = _patientService.Update(patient);
                if (response.IsSuccessful)
                {
                    SetTempData(response.Message);
                    return RedirectToAction(nameof(Details), new { id = response.Id });
                }
                ModelState.AddModelError("", response.Message);
            }
            
            SetViewData(patient.DoctorIds, patient.UserId, patient.GroupId);
            return View(patient);
        }

        public IActionResult Delete(int id)
        {
            var item = _patientService.Item(id);
            if (item == null)
            {
                SetTempData("Patient not found!");
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var response = _patientService.Delete(id);
            SetTempData(response.Message);
            return RedirectToAction(nameof(Index));
        }
    }