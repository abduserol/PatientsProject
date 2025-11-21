using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PatientsProject.App.Models;
using PatientsProject.Core.App.Services.MVC;

namespace PatientsProject.Mvc.Controllers;

public class UsersController : Controller
{
    private readonly IService<UserRequest, UserResponse> _userService;
    private readonly IService<RoleRequest, RoleResponse> _roleService;
    private readonly IService<GroupRequest, GroupResponse> _groupService;

    public UsersController(
        IService<UserRequest, UserResponse> userService,
        IService<RoleRequest, RoleResponse> roleService,
        IService<GroupRequest, GroupResponse> groupService)
    {
        _userService = userService;
        _roleService = roleService;
        _groupService = groupService;
    }

    // Dropdown ve ListBox doldurma metodu
    // selectedRoleIds: Hata durumunda veya Edit sayfasında seçili rolleri korumak için
    private void SetViewData(List<int>? selectedRoleIds = null)
    {
        // 1. GRUPLAR (Dropdown - Tek Seçim)
        var groups = _groupService.List();
        ViewBag.GroupId = new SelectList(groups, "Id", "Title");

        // 2. ROLLER (ListBox - Çoklu Seçim)
        var roles = _roleService.List();
        ViewBag.RoleIds = new MultiSelectList(roles, "Id", "Name", selectedRoleIds);
    }

    private void SetTempData(string message, string key = "Message")
    {
        TempData[key] = message;
    }

    // GET: Users
    public IActionResult Index()
    {
        var list = _userService.List();
        return View(list);
    }

    // GET: Users/Create
    public IActionResult Create()
    {
        SetViewData();
        return View();
    }

    // POST: Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(UserRequest user)
    {
        if (ModelState.IsValid)
        {
            var response = _userService.Create(user);
            if (response.IsSuccessful)
            {
                SetTempData(response.Message);
                // Detay sayfasına veya Listeye yönlendirebiliriz
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", response.Message);
        }

        // Hata varsa kullanıcının seçtiği rolleri geri gönderiyoruz
        SetViewData(user.RoleIds);
        return View(user);
    } // GET: Users/Details/5

    public IActionResult Details(int id)
    {
        // Servisten ilgili kullanıcının detaylarını (UserResponse) çekiyoruz
        var item = _userService.Item(id);

        // Eğer kullanıcı bulunamazsa listeye geri dön
        if (item == null)
        {
            SetTempData("User not found!", "Message");
            return RedirectToAction(nameof(Index));
        }

        return View(item);
    }

// GET: Users/Edit/5
    public IActionResult Edit(int id)
    {
        // Servisten kullanıcının mevcut bilgilerini ve rollerini getir
        var item = _userService.Edit(id);

        if (item == null)
        {
            SetTempData("User not found!", "Message");
            return RedirectToAction(nameof(Index));
        }

        // Dropdownları doldur. 
        // item.RoleIds parametresini göndererek kullanıcının rollerini seçili hale getiriyoruz.
        SetViewData(item.RoleIds);

        return View(item);
    }

    // POST: Users/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(UserRequest user)
    {
        if (ModelState.IsValid)
        {
            var response = _userService.Update(user);
            if (response.IsSuccessful)
            {
                SetTempData(response.Message);
                return RedirectToAction(nameof(Index)); // Veya Details'e yönlendir
            }

            ModelState.AddModelError("", response.Message);
        }

        // Hata durumunda seçili rolleri koru
        SetViewData(user.RoleIds);
        return View(user);
    }

    // GET: Users/Delete/5
    public IActionResult Delete(int id)
    {
        // Silme onay sayfasına gitmeden önce, silinecek veriyi gösteriyoruz
        var item = _userService.Item(id);

        if (item == null)
        {
            SetTempData("User not found!", "Message");
            return RedirectToAction(nameof(Index));
        }

        return View(item);
    }

    // POST: Users/Delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        // Silme işlemini gerçekleştiriyoruz
        var response = _userService.Delete(id);

        // İşlem sonucunu TempData ile View'a taşıyoruz
        SetTempData(response.Message);

        // Listeye geri dönüyoruz
        return RedirectToAction(nameof(Index));
    }
}