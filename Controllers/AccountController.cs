using HotelManagementSystem.Models;
using HotelManagementSystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelManagementSystem.Controllers
{
	public class AccountController : Controller
	{
		private readonly IUserService _userService;

		public AccountController(IUserService userService)
		{
			_userService = userService;
		}

		public IActionResult Login()
		{
			try
			{
				return View();
			}
			catch (Exception)
			{
				return View("Error");
			}
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var user = await _userService.AuthenticateAsync(model.Email, model.Password);
					if (user != null)
					{
						var claims = new List<Claim>
						{
							new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
							new Claim(ClaimTypes.Name, user.FullName),
							new Claim(ClaimTypes.Email, user.Email),
							new Claim(ClaimTypes.Role, user.Role)
						};

						var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
						var authProperties = new AuthenticationProperties
						{
							IsPersistent = model.RememberMe
						};

						await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
							new ClaimsPrincipal(claimsIdentity), authProperties);

						// Store user ID in session as well for backward compatibility
						HttpContext.Session.SetInt32("UserId", user.Id);

						// Redirect based on role
						if (user.Role == "Admin")
							return RedirectToAction("Index", "Admin");
						else
							return RedirectToAction("Index", "Home");
					}
					ModelState.AddModelError("", "Invalid email or password.");
				}
				return View(model);
			}
			catch (Exception)
			{
				ModelState.AddModelError("", "An error occurred during login. Please try again.");
				return View(model);
			}
		}

		public IActionResult Register()
		{
			try
			{
				return View();
			}
			catch (Exception)
			{
				return View("Error");
			}
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					if (await _userService.EmailExistsAsync(model.Email))
					{
						ModelState.AddModelError("Email", "This email is already registered.");
						return View(model);
					}

					var user = await _userService.RegisterAsync(model);
					if (user != null)
					{
						TempData["Success"] = "Registration successful! Please login with your credentials.";
						return RedirectToAction("Login");
					}
					ModelState.AddModelError("", "Registration failed. Please try again.");
				}
				return View(model);
			}
			catch (Exception)
			{
				ModelState.AddModelError("", "An error occurred during registration. Please try again.");
				return View(model);
			}
		}

		[Authorize]
		public async Task<IActionResult> Logout()
		{
			try
			{
				await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
				HttpContext.Session.Clear();
				return RedirectToAction("Index", "Home");
			}
			catch (Exception)
			{
				return View("Error");
			}
		}

		[Authorize]
		public async Task<IActionResult> Profile()
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
				var user = await _userService.GetUserByIdAsync(userId);
				return View(user);
			}
			catch (Exception)
			{
				return View("Error");
			}
		}

		public IActionResult AccessDenied()
		{
			try
			{
				return View();
			}
			catch (Exception)
			{
				return View("Error");
			}
		}
	}
}