using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoursePro.Data;
using CoursePro.Models;

namespace CoursePro.Controllers
{
    public class TextBookController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TextBookController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TextBook
        public async Task<IActionResult> Index()
        {
            return View(await _context.TextBooks.ToListAsync());
        }

        // GET: TextBook/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var textBook = await _context.TextBooks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (textBook == null)
            {
                return NotFound();
            }

            return View(textBook);
        }

        // GET: TextBook/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TextBook/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ImageFileData,ImageContentType")] TextBook textBook)
        {
            if (ModelState.IsValid)
            {
                _context.Add(textBook);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(textBook);
        }

        // GET: TextBook/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var textBook = await _context.TextBooks.FindAsync(id);
            if (textBook == null)
            {
                return NotFound();
            }
            return View(textBook);
        }

        // POST: TextBook/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,ImageFileData,ImageContentType")] TextBook textBook)
        {
            if (id != textBook.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(textBook);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TextBookExists(textBook.Id))
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
            return View(textBook);
        }

        // GET: TextBook/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var textBook = await _context.TextBooks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (textBook == null)
            {
                return NotFound();
            }

            return View(textBook);
        }

        // POST: TextBook/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var textBook = await _context.TextBooks.FindAsync(id);
            _context.TextBooks.Remove(textBook);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TextBookExists(int id)
        {
            return _context.TextBooks.Any(e => e.Id == id);
        }
    }
}
