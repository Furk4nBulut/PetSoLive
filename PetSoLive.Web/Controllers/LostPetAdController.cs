using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using PetSoLive.Core.Interfaces;

namespace PetSoLive.Web.Controllers;

public class LostPetAdController : Controller
{
    private readonly ILostPetAdService _lostPetAdService;
    private readonly IEmailService _emailService;
    private readonly IUserService _userService;
    private readonly IStringLocalizer<LostPetAdController> _localizer;

    // Constructor Dependency Injection
    public LostPetAdController(ILostPetAdService lostPetAdService, 
        IUserService userService,
        IEmailService emailService,
        IStringLocalizer<LostPetAdController> localizer)
    {
        _lostPetAdService = lostPetAdService;
        _userService = userService;
        _emailService = emailService;
        _localizer = localizer;
    }

    // Oturum kontrolü metodu
    private IActionResult RedirectToLoginIfNotLoggedIn()
    {
        if (HttpContext.Session.GetString("Username") == null)
        {
            return RedirectToAction("Login", "Account");
        }
        return null;
    }
    // Yeni bir GET metodu ekleyin, AJAX isteğini karşılayacak
    public IActionResult GetDistrictsByCity(string city)
    {
        // GetDistrictsByCity metodunu çağırarak ilgili ilçeleri alıyoruz
        var districts = CityList.GetDistrictsByCity(city);
    
        // Dönen ilçeleri JSON formatında döndürüyoruz
        return Json(districts);
    }


    // GET: /LostPetAd/Create
    public IActionResult Create()
    {
        // Oturum kontrolü, giriş yapmamış kullanıcıyı yönlendir
        var redirectResult = RedirectToLoginIfNotLoggedIn();
        if (redirectResult != null) return redirectResult;

        // Şehir listesini ViewData ile view'a gönderiyoruz
        ViewData["Cities"] = CityList.Cities;

        // Başlangıçta district listesi boş olacak
        ViewData["Districts"] = new List<string>(); 

        return View();
    }
    // Kayıp ilanı oluşturma işlemi
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LostPetAd lostPetAd, string city, string district)
    {
        var redirectResult = RedirectToLoginIfNotLoggedIn();
        if (redirectResult != null) return redirectResult;

        if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(district))
        {
            TempData["ErrorMessage"] = "City and District are required.";
            ViewData["Cities"] = CityList.Cities;
            ViewData["Districts"] = new List<string>();
            return View(lostPetAd);
        }

        lostPetAd.LastSeenCity = city;
        lostPetAd.LastSeenDistrict = district;

        var username = HttpContext.Session.GetString("Username");
        var user = await _userService.GetUserByUsernameAsync(username);
        lostPetAd.UserId = user.Id;

        // Kaybolan ilanı kaydedelim
        await _lostPetAdService.CreateLostPetAdAsync(lostPetAd, city, district);

        TempData["SuccessMessage"] = "The lost pet ad has been created successfully, and notifications have been sent.";

        return RedirectToAction("Index");
    }

    // GET: /LostPetAd/Index
    public async Task<IActionResult> Index()
    {
        var lostPetAds = await _lostPetAdService.GetAllLostPetAdsAsync();
        if (lostPetAds == null)
        {
            TempData["ErrorMessage"] = "Could not retrieve lost pet ads. Please try again later.";
            lostPetAds = new List<LostPetAd>();
        }

        return View(lostPetAds); 
    }

    // GET: /LostPetAd/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var lostPetAd = await _lostPetAdService.GetLostPetAdByIdAsync(id);
        if (lostPetAd == null)
        {
            TempData["ErrorMessage"] = "Lost Pet Ad not found.";
            return RedirectToAction("Index");
        }

        var currentUser = HttpContext.Session.GetString("Username");
        ViewBag.CurrentUser = currentUser;
        return View(lostPetAd);
    }
    

// GET: /LostPetAd/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var redirectResult = RedirectToLoginIfNotLoggedIn();
        if (redirectResult != null) return redirectResult;

        var lostPetAd = await _lostPetAdService.GetLostPetAdByIdAsync(id);
        if (lostPetAd == null)
        {
            TempData["ErrorMessage"] = "Lost Pet Ad not found.";
            return RedirectToAction("Index");
        }

        // Check if the current user is the owner of the ad
        var currentUser = HttpContext.Session.GetString("Username");
        if (lostPetAd.User == null || lostPetAd.User.Username != currentUser)
        {
            TempData["ErrorMessage"] = "You do not have permission to edit this ad.";
            return RedirectToAction("Index");
        }

        // Populate city and district lists
        ViewData["Cities"] = CityList.Cities;
        ViewData["Districts"] = lostPetAd.LastSeenCity != null ?  GetDistrictsByCity(lostPetAd.LastSeenCity) : new List<string>();

        return View(lostPetAd);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LostPetAd updatedLostPetAd, string city, string district)
    {
        var redirectResult = RedirectToLoginIfNotLoggedIn();
        if (redirectResult != null) return redirectResult;

        if (id != updatedLostPetAd.Id)
        {
            TempData["ErrorMessage"] = "Invalid ad ID.";
            return RedirectToAction("Index");
        }

        var lostPetAd = await _lostPetAdService.GetLostPetAdByIdAsync(id);
        if (lostPetAd == null)
        {
            TempData["ErrorMessage"] = "Lost Pet Ad not found.";
            return RedirectToAction("Index");
        }

        // Check if the current user is the owner of the ad
        var currentUser = HttpContext.Session.GetString("Username");
        if (lostPetAd.User == null || lostPetAd.User.Username != currentUser)
        {
            TempData["ErrorMessage"] = "You do not have permission to edit this ad.";
            return RedirectToAction("Index");
        }

        // Update the properties of the lost pet ad
        lostPetAd.PetName = updatedLostPetAd.PetName;
        lostPetAd.Description = updatedLostPetAd.Description;
        lostPetAd.LastSeenCity = city;
        lostPetAd.LastSeenDistrict = district;
        lostPetAd.ImageUrl = updatedLostPetAd.ImageUrl;
        lostPetAd.LastSeenDate = updatedLostPetAd.LastSeenDate;

        try
        {
            // Update the lost pet ad
            await _lostPetAdService.UpdateLostPetAdAsync(lostPetAd);
            TempData["SuccessMessage"] = "The lost pet ad has been updated successfully.";
            return RedirectToAction("Details", new { id = lostPetAd.Id });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"An error occurred while updating the lost pet ad: {ex.Message}";
            return RedirectToAction("Index");
        }
    }




    // GET: /LostPetAd/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var lostPetAd = await _lostPetAdService.GetLostPetAdByIdAsync(id);

        if (lostPetAd == null)
        {
            TempData["ErrorMessage"] = "Lost Pet Ad not found.";
            return RedirectToAction("Index");
        }

        // Check if the current user is the owner of the ad
        var currentUser = HttpContext.Session.GetString("Username");

        if (lostPetAd.User == null)
        {
            TempData["ErrorMessage"] = "The user associated with this ad is not found.";
            return RedirectToAction("Index");
        }

        if (lostPetAd.User.Username != currentUser)
        {
            TempData["ErrorMessage"] = "You do not have permission to delete this ad.";
            return RedirectToAction("Index");
        }
        TempData["DeleteMessage"] = _localizer["DeleteConfirmation"];
        return View(lostPetAd);
    }

    // POST: /LostPetAd/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var lostPetAd = await _lostPetAdService.GetLostPetAdByIdAsync(id);

        if (lostPetAd == null)
        {
            TempData["ErrorMessage"] = "Lost Pet Ad not found.";
            return RedirectToAction("Index");
        }

        // Check if the current user is the owner of the ad
        var currentUser = HttpContext.Session.GetString("Username");

        if (lostPetAd.User == null)
        {
            TempData["ErrorMessage"] = "The user associated with this ad is not found.";
            return RedirectToAction("Index");
        }

        if (lostPetAd.User.Username != currentUser)
        {
            TempData["ErrorMessage"] = "You do not have permission to delete this ad.";
            return RedirectToAction("Index");
        }

        await _lostPetAdService.DeleteLostPetAdAsync(lostPetAd);
        TempData["SuccessMessage"] = "The lost pet ad has been deleted successfully.";
        return RedirectToAction("Index");
    }
    
}