﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Expense_Tracker.Models;

namespace Expense_Tracker.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Transactions.Include(t => t.Category);
            return View(await applicationDbContext.ToListAsync());
        }


        // GET: Transaction/AddorEdit
        public IActionResult AddorEdit(int id=0)
        {
            PopulateCategories();
            if(id==0)
            return View(new Transaction());
            else
            return View(_context.Transactions.Find(id));
        }

        // POST: Transaction/AddorEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddorEdit([Bind("TransactionId,CategoryId,Amount,Note,Date")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                if(transaction.TransactionId==0)
                _context.Add(transaction);
                else
                _context.Update(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateCategories();
            return View(transaction);
        }

        

        // GET: Transaction/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(m => m.TransactionId == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if(_context.Transactions == null)
            {
                return Problem("Entity set regarding Transactions is null");
            }
            var transaction = await _context.Transactions.FindAsync(id);
            if(transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        public void PopulateCategories()
        {
            var CategoryCollection = _context.Categories.ToList();
            Category Defaultcategory = new Category() { CategoryId = 0, Title = "Choose a new Category" };
            CategoryCollection.Insert(0,Defaultcategory);
            ViewBag.Categories = CategoryCollection;
        }
        
    }
}
