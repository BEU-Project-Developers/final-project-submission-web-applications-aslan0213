using HotelManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers
{
	public class AccountController : Controller
	{
		private static List<User> users = new List<User>();
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Login(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);
				if (user != null)
				{
					HttpContext.Session.SetInt32("UserId", user.Id);
					return RedirectToAction("Index", "Home");
				}
				ModelState.AddModelError("", "Invalid email or password.");
			}
			return View(model);
		}
		public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				if (model.Password != model.ConfirmPassword)
				{
					ModelState.AddModelError("", "Passwords do not match.");
					return View(model);
				}

				if (users.Any(u => u.Email == model.Email))
				{
					ModelState.AddModelError("", "Email already exists.");
					return View(model);
				}

				var user = new User
				{
					Id = users.Count + 1,
					FullName = model.FullName,
					Email = model.Email,
					Password = model.Password
				};
				users.Add(user);
				return RedirectToAction("Login");
			}
			return View(model);
		}

		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Login");
		}
	}
}
