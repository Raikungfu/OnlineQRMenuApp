using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineQRMenuApp.Models;

namespace OnlineQRMenuApp.Controllers
{
    public class CoffeeShopsController : BaseController
    {
        private readonly OnlineCoffeeManagementContext _context;

        public CoffeeShopsController(IWebHostEnvironment env, IConfiguration configuration, OnlineCoffeeManagementContext context) : base(env, configuration)
        {
            _context = context;
        }

        // GET: CoffeeShops
        public async Task<IActionResult> Index()
        {
            return View(await _context.CoffeeShops.ToListAsync());
        }

        // GET: CoffeeShops/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coffeeShop = await _context.CoffeeShops
                .FirstOrDefaultAsync(m => m.CoffeeShopId == id);
            if (coffeeShop == null)
            {
                return NotFound();
            }

            return View(coffeeShop);
        }

        // GET: CoffeeShops/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CoffeeShops/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CoffeeShopId,Name,Location,QRCode,PrimaryColor,SecondaryColor,Description,Slogan,Avatar,CoverImage,Hotline,Email,Website,Facebook,Instagram,Twitter,OpeningHours,UserId")] CoffeeShop coffeeShop)
        {
            if (ModelState.IsValid)
            {
                _context.Add(coffeeShop);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coffeeShop);
        }

        // GET: CoffeeShops/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coffeeShop = await _context.CoffeeShops.FindAsync(id);
            if (coffeeShop == null)
            {
                return NotFound();
            }
            return View(coffeeShop);
        }

        // POST: CoffeeShops/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CoffeeShopId,Name,Location,QRCode,PrimaryColor,SecondaryColor,Description,Slogan,Avatar,CoverImage,Hotline,Email,Website,Facebook,Instagram,Twitter,OpeningHours,UserId")] CoffeeShop coffeeShop)
        {
            if (id != coffeeShop.CoffeeShopId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(coffeeShop);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoffeeShopExists(coffeeShop.CoffeeShopId))
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
            return View(coffeeShop);
        }

        // GET: CoffeeShops/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coffeeShop = await _context.CoffeeShops
                .FirstOrDefaultAsync(m => m.CoffeeShopId == id);
            if (coffeeShop == null)
            {
                return NotFound();
            }

            return View(coffeeShop);
        }

        // POST: CoffeeShops/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coffeeShop = await _context.CoffeeShops.FindAsync(id);
            if (coffeeShop != null)
            {
                _context.CoffeeShops.Remove(coffeeShop);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CoffeeShopExists(int id)
        {
            return _context.CoffeeShops.Any(e => e.CoffeeShopId == id);
        }
    }
}
