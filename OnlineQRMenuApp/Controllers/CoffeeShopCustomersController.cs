using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineQRMenuApp.Models;

namespace OnlineQRMenuApp.Controllers
{
    public class CoffeeShopCustomersController : Controller
    {
        private readonly OnlineCoffeeManagementContext _context;

        public CoffeeShopCustomersController(OnlineCoffeeManagementContext context)
        {
            _context = context;
        }

        // GET: CoffeeShopCustomers
        public async Task<IActionResult> Index()
        {
            var onlineCoffeeManagementContext = _context.CoffeeShopCustomers.Include(c => c.CoffeeShop).Include(c => c.User);
            return View(await onlineCoffeeManagementContext.ToListAsync());
        }

        // GET: CoffeeShopCustomers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coffeeShopCustomer = await _context.CoffeeShopCustomers
                .Include(c => c.CoffeeShop)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CoffeeShopCustomerId == id);
            if (coffeeShopCustomer == null)
            {
                return NotFound();
            }

            return View(coffeeShopCustomer);
        }

        // GET: CoffeeShopCustomers/Create
        public IActionResult Create()
        {
            ViewData["CoffeeShopId"] = new SelectList(_context.CoffeeShops, "CoffeeShopId", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email");
            return View();
        }

        // POST: CoffeeShopCustomers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CoffeeShopCustomerId,CoffeeShopId,UserId,JoinedDate")] CoffeeShopCustomer coffeeShopCustomer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(coffeeShopCustomer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CoffeeShopId"] = new SelectList(_context.CoffeeShops, "CoffeeShopId", "Name", coffeeShopCustomer.CoffeeShopId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", coffeeShopCustomer.UserId);
            return View(coffeeShopCustomer);
        }

        // GET: CoffeeShopCustomers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coffeeShopCustomer = await _context.CoffeeShopCustomers.FindAsync(id);
            if (coffeeShopCustomer == null)
            {
                return NotFound();
            }
            ViewData["CoffeeShopId"] = new SelectList(_context.CoffeeShops, "CoffeeShopId", "Name", coffeeShopCustomer.CoffeeShopId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", coffeeShopCustomer.UserId);
            return View(coffeeShopCustomer);
        }

        // POST: CoffeeShopCustomers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CoffeeShopCustomerId,CoffeeShopId,UserId,JoinedDate")] CoffeeShopCustomer coffeeShopCustomer)
        {
            if (id != coffeeShopCustomer.CoffeeShopCustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(coffeeShopCustomer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoffeeShopCustomerExists(coffeeShopCustomer.CoffeeShopCustomerId))
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
            ViewData["CoffeeShopId"] = new SelectList(_context.CoffeeShops, "CoffeeShopId", "Name", coffeeShopCustomer.CoffeeShopId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", coffeeShopCustomer.UserId);
            return View(coffeeShopCustomer);
        }

        // GET: CoffeeShopCustomers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coffeeShopCustomer = await _context.CoffeeShopCustomers
                .Include(c => c.CoffeeShop)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CoffeeShopCustomerId == id);
            if (coffeeShopCustomer == null)
            {
                return NotFound();
            }

            return View(coffeeShopCustomer);
        }

        // POST: CoffeeShopCustomers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coffeeShopCustomer = await _context.CoffeeShopCustomers.FindAsync(id);
            if (coffeeShopCustomer != null)
            {
                _context.CoffeeShopCustomers.Remove(coffeeShopCustomer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CoffeeShopCustomerExists(int id)
        {
            return _context.CoffeeShopCustomers.Any(e => e.CoffeeShopCustomerId == id);
        }
    }
}
