using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.Execution;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineQRMenuApp.Models;
using OnlineQRMenuApp.Service;

namespace OnlineQRMenuApp.Controllers
{
    public class TokenRequest
    {
        public string Token { get; set; }
    }

    public class UsersController : BaseController
    {
        private readonly OnlineCoffeeManagementContext _context;
        private readonly TokenService _tokenService;

        public UsersController(IWebHostEnvironment env, IConfiguration configuration, OnlineCoffeeManagementContext context, TokenService tokenService) : base(env, configuration)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        [HttpGet("Admin/Login")]
        public async Task<IActionResult> AdminLogin()
        {
            return View();
        }

        [HttpGet("Coffee-Shop/Login")]
        public async Task<IActionResult> CoffeeShopLogin()
        {
            return View();
        }

        [HttpGet("Coffee-Shop/Register")]
        public async Task<IActionResult> CoffeeShopRegister()
        {
            return View();
        }

        [HttpPost("Coffee-Shop/Login")]
        public async Task<IActionResult> CoffeeShopLogin([Bind("Email,Password")] User m)
        {
            var member = await ValidateMemberAsync(m.Email, m.Password);
            if (member == null)
            {
                ViewData["ErrorMessage"] = "ID/Password not correct!";
                return View();
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, member.Email),
        new Claim(ClaimTypes.NameIdentifier, member.UserId.ToString()),
        new Claim(ClaimTypes.Role, member.UserType),
    };
            var users = await _context.Users.ToListAsync();

            foreach (var user in users)
            {
                user.UserType = "CoffeeShopManager";
            }

            await _context.SaveChangesAsync();
            var token = _tokenService.GenerateToken(member);

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return Json(new { token, success = true });
        }

        [HttpPost("login-token")]
        public async Task<IActionResult> LoginToken([FromBody] TokenRequest tokenRequest)
        {
            if (string.IsNullOrEmpty(tokenRequest?.Token))
            {
                return BadRequest(new { message = "Token không hợp lệ." });
            }

            var principal = _tokenService.ValidateToken(tokenRequest.Token);
            if (principal == null)
            {
                return Unauthorized(new { message = "Token không hợp lệ hoặc đã hết hạn." });
            }

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmail = principal.FindFirst(ClaimTypes.Name)?.Value;
            var userRole = principal.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
            {
                return Unauthorized(new { message = "Token không chứa thông tin người dùng." });
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, userEmail),
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim(ClaimTypes.Role, userRole)
    };


            var newToken = _tokenService.GenerateToken(new User
            {
                Email = userEmail,
                UserId = int.Parse(userId),
                UserType = userRole
            });

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return Ok(new { token = newToken, success = true });
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private async Task<User> ValidateMemberAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var member = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
            if (member == null)
            {
                return null;
            }

            return member;
        }


        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,FullName,Email,Password,Address,UserType")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,FullName,Email,Password,Address,UserType")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
