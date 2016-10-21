﻿using LibraryWebApp.Data;
using LibraryWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWebApp.Controllers
{
    public class LibraryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public LibraryController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IAuthorizationService authorizationService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _authorizationService = authorizationService;
        }

        #region Sections
        // GET: Sections
        public async Task<IActionResult> SectionIndex()
        {
            return View(await _context.Sections.ToListAsync());
        }

        // GET: Sections/Details/5
        public async Task<IActionResult> SectionDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var section = await _context.Sections.SingleOrDefaultAsync(m => m.SectionId == id);
            if (section == null)
            {
                return NotFound();
            }

            return View(section);
        }

        // GET: Sections/Create
        public IActionResult SectionCreate()
        {
            return View();
        }

        // POST: Sections/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SectionCreate([Bind("SectionId,Description,Name")] Section section)
        {
            if (ModelState.IsValid)
            {
                _context.Add(section);
                await _context.SaveChangesAsync();
                return RedirectToAction("SectionIndex");
            }
            return View(section);
        }

        // GET: Sections/Edit/5
        public async Task<IActionResult> SectionEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var section = await _context.Sections.SingleOrDefaultAsync(m => m.SectionId == id);
            if (section == null)
            {
                return NotFound();
            }
            return View(section);

        }

        // POST: Sections/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SectionEdit(int id, byte[] rowVersion, [Bind("SectionId,Description,Name")] Section sectionb)
        {
            if (id != sectionb.SectionId)
            {
                return NotFound();
            }

            var section = await _context.Sections.SingleOrDefaultAsync(m => m.SectionId == id);
            if (section == null)
            {
                Section deletedSection = new Section();
                await TryUpdateModelAsync(deletedSection);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The section was deleted by another user.");
                return View(deletedSection);
            }
            _context.Entry(section).Property("RowVersion").OriginalValue = rowVersion;

            if (await TryUpdateModelAsync<Section>(
                section,
                "",
                s => s.Name, s => s.Description))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("SectionIndex");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    // Using a NoTracking query means we get the entity but it is not tracked by the context
                    // and will not be merged with existing entities in the context.
                    var databaseEntity = await _context.Sections
                        .AsNoTracking()
                        .SingleAsync(d => d.SectionId == ((Section)exceptionEntry.Entity).SectionId);

                    var databaseEntry = _context.Entry(databaseEntity);

                    var databaseName = (string)databaseEntry.Property("Name").CurrentValue;
                    var proposedName = (string)exceptionEntry.Property("Name").CurrentValue;
                    if (databaseName != proposedName)
                    {
                        ModelState.AddModelError("Name", $"Current value: {databaseName}");
                    }
                    var databaseDescription = (string)databaseEntry.Property("Description").CurrentValue;
                    var proposedDescription = (string)exceptionEntry.Property("Description").CurrentValue;
                    if (databaseDescription != proposedDescription)
                    {
                        ModelState.AddModelError("Description", $"Current value: {databaseDescription}");
                    }
                    
                    ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                    + "was modified by another user after you got the original value. The "
                    + "edit operation was canceled and the current values in the database "
                    + "have been displayed. If you still want to edit this record, click "
                    + "the Save button again. Otherwise click the Back to List hyperlink.");
                    section.RowVersion = (byte[])databaseEntry.Property("RowVersion").CurrentValue;
                    ModelState.Remove("RowVersion");
                }
            }
            return View(section);
        }

        // GET: Sections/Delete/5
        public async Task<IActionResult> SectionDelete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }
            var section = await _context.Sections
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.SectionId == id);
            if (section == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("SectionIndex");
                }

                return NotFound();
            }
            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMessage"] = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }

            return View(section);

        }

        // POST: Sections/Delete/5
        [HttpPost, ActionName("SectionDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SectionDeleteConfirmed(int id)
        {
            try
            {
                if (await _context.Sections.AnyAsync(m => m.SectionId == id))
                {
                    var section = await _context.Sections.SingleOrDefaultAsync(m => m.SectionId == id);
                    _context.Sections.Remove(section);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("SectionIndex");

            }
            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("SectionDelete", new { id, concurrencyError = true });
            }

        }

        private bool SectionExists(int id)
        {
            return _context.Sections.Any(e => e.SectionId == id);
        }
        #endregion

        #region Titles

        // GET: Titles
        public async Task<IActionResult> TitleIndex()
        {
            var libraryContext = _context.Titles.Include(t => t.Section);
            return View(await libraryContext.ToListAsync());
        }

        // GET: Titles/Details/5
        public async Task<IActionResult> TitleDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var title = await _context.Titles.SingleOrDefaultAsync(m => m.TitleId == id);
            if (title == null)
            {
                return NotFound();
            }
            var numberOfItemsAvailable = _context.Items.Where(e => e.TitleId == title.TitleId).Count();
            return View(title);
        }

        // GET: Titles/Create
        public IActionResult TitleCreate()
        {
            ViewData["SectionId"] = new SelectList(_context.Sections, "SectionId", "Name");
            return View();
        }

        // POST: Titles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TitleCreate([Bind("TitleId,Annotation,Author,ISBN,Name,Publisher,SectionId,Type,Year")] Title title)
        {
            if (ModelState.IsValid)
            {
                _context.Add(title);
                await _context.SaveChangesAsync();
                return RedirectToAction("TitleIndex");
            }
            ViewData["SectionId"] = new SelectList(_context.Sections, "SectionId", "SectionId", title.SectionId);
            return View(title);
        }

        // GET: Titles/Edit/5
        public async Task<IActionResult> TitleEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var title = await _context.Titles.SingleOrDefaultAsync(m => m.TitleId == id);
            if (title == null)
            {
                return NotFound();
            }

            ViewData["SectionId"] = new SelectList(_context.Sections, "SectionId", "Name");
            return View(title);
        }

        // POST: Titles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TitleEdit(int id, byte[] rowVersion, [Bind("TitleId,Annotation,Author,ISBN,Name,Publisher,SectionId,Type,Year")] Title titlem)
        {
            if (id != titlem.TitleId)
            {
                return NotFound();
            }

            var title = await _context.Titles.SingleOrDefaultAsync(m => m.TitleId == id);
            if (title == null)
            {
                Title deletedTitle = new Title();
                await TryUpdateModelAsync(deletedTitle);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The title was deleted by another user.");
                return View(deletedTitle);
            }
            _context.Entry(title).Property("RowVersion").OriginalValue = rowVersion;

            if (await TryUpdateModelAsync<Title>(
                title,
                "",
                s => s.Name, s => s.Annotation, s => s.Author, s => s.Year, s => s.ISBN,
                s => s.Publisher, s => s.Type, s => s.SectionId))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("TitleIndex");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    // Using a NoTracking query means we get the entity but it is not tracked by the context
                    // and will not be merged with existing entities in the context.
                    var databaseEntity = await _context.Titles
                        .AsNoTracking()
                        .SingleAsync(d => d.TitleId == ((Title)exceptionEntry.Entity).TitleId);

                    var databaseEntry = _context.Entry(databaseEntity);

                    var databaseName = (string)databaseEntry.Property("Name").CurrentValue;
                    var proposedName = (string)exceptionEntry.Property("Name").CurrentValue;
                    if (databaseName != proposedName)
                    {
                        ModelState.AddModelError("Name", $"Current value: {databaseName}");
                    }
                    var databaseAnnotation = (string)databaseEntry.Property("Annotation").CurrentValue;
                    var proposedAnnotation = (string)exceptionEntry.Property("Annotation").CurrentValue;
                    if (databaseAnnotation != proposedAnnotation)
                    {
                        ModelState.AddModelError("Annotation", $"Current value: {databaseAnnotation}");
                    }
                    var databaseAuthor = (string)databaseEntry.Property("Author").CurrentValue;
                    var proposedAuthor = (string)exceptionEntry.Property("Author").CurrentValue;
                    if (databaseAuthor != proposedAuthor)
                    {
                        ModelState.AddModelError("Author", $"Current value: {databaseAuthor}");
                    }
                    var databaseYear = (short)databaseEntry.Property("Year").CurrentValue;
                    var proposedYear = (short)exceptionEntry.Property("Year").CurrentValue;
                    if (databaseYear != proposedYear)
                    {
                        ModelState.AddModelError("Year", $"Current value: {databaseYear:d}");
                    }
                    var databaseISBN = (string)databaseEntry.Property("ISBN").CurrentValue;
                    var proposedISBN = (string)exceptionEntry.Property("ISBN").CurrentValue;
                    if (databaseISBN != proposedISBN)
                    {
                        ModelState.AddModelError("ISBN", $"Current value: {databaseISBN}");
                    }

                    var databasePublisher = (string)databaseEntry.Property("Publisher").CurrentValue;
                    var proposedPublisher = (string)exceptionEntry.Property("Publisher").CurrentValue;
                    if (databasePublisher != proposedPublisher)
                    {
                        ModelState.AddModelError("Publisher", $"Current value: {databasePublisher}");
                    }
                    var databaseType = (string)databaseEntry.Property("Type").CurrentValue;
                    var proposedType = (string)exceptionEntry.Property("Type").CurrentValue;
                    if (databaseType != proposedType)
                    {
                        ModelState.AddModelError("Type", $"Current value: {databaseType}");
                    }
                    var databaseSectionId = (int)databaseEntry.Property("SectionId").CurrentValue;
                    var proposedSectionId = (int)exceptionEntry.Property("SectionId").CurrentValue;
                    if (databaseSectionId != proposedSectionId)
                    {
                        Section databaseSection = await _context.Sections.SingleAsync(i => i.SectionId == databaseSectionId);
                        ModelState.AddModelError("SectionID", $"Current value: {databaseSection.SectionId}");
                    }
                    ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                    + "was modified by another user after you got the original value. The "
                    + "edit operation was canceled and the current values in the database "
                    + "have been displayed. If you still want to edit this record, click "
                    + "the Save button again. Otherwise click the Back to List hyperlink.");
                    title.RowVersion = (byte[])databaseEntry.Property("RowVersion").CurrentValue;
                    ModelState.Remove("RowVersion");
                }
            }
            ViewData["SectionId"] = new SelectList(_context.Sections, "SectionId", "Name");
            return View(title);

        }

        public async Task<IActionResult> TitleDelete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            var title = await _context.Titles
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.TitleId == id);
            if (title == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("TitleIndex");
                }

                return NotFound();
            }
            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMessage"] = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }

            return View(title);
        }

        [HttpPost, ActionName("TitleDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TitleDelete(Title titlexx)
        {
            try
            {
                if (await _context.Titles.AnyAsync(m => m.TitleId == titlexx.TitleId))
                {
                    _context.Titles.Remove(titlexx);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("TitleIndex");

            }
            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("TitleDelete", new { concurrencyError = true, id = titlexx.TitleId });
            }

        }

        private bool TitleExists(int id)
        {
            return _context.Titles.Any(e => e.TitleId == id);
        }

        #endregion

        #region Items

        public async Task<IActionResult> ItemAddToFavourites(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items.SingleOrDefaultAsync(m => m.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost, ActionName("ItemAddToFavourites")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ItemAddToFavouritesConfirmed(int id)
        {

            var item = await _context.Items.SingleOrDefaultAsync(m => m.ItemId == id);
            var currentUser = User.Identity.Name;
            // _context.Users.First(e => e.Email == currentUser).Favourites.Add(new Favourite() { Url = "", Comment = ""});
            string url = "Library/ItemIndex/" + item.ItemId;
            return RedirectToAction("ItemIndex");

        }


        // GET: Items
        public async Task<IActionResult> ItemIndex()
        {
            var libraryContext = _context.Items.Include(i => i.Title).Include(e => e.Title.Section);
            return View(await libraryContext.ToListAsync());
        }

        // GET: Items/Details/5
        public async Task<IActionResult> ItemDetails(int? id)
        {

            var item = await _context.Items.SingleOrDefaultAsync(m => m.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        public IActionResult ItemCreate()
        {
            ViewData["TitleId"] = new SelectList(_context.Titles, "TitleId", "Name");
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ItemCreate([Bind("ItemId,Condition,CurrentLocation,Material,TitleId")] Item item)
        {
            if (ModelState.IsValid)
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction("ItemIndex");
            }
            ViewData["TitleId"] = new SelectList(_context.Titles, "TitleId", "TitleId", item.TitleId);
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> ItemEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items.SingleOrDefaultAsync(m => m.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }
            ViewData["TitleId"] = new SelectList(_context.Titles, "TitleId", "Name");
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ItemEdit(int id, byte[] rowVersion, [Bind("ItemId,Condition,CurrentLocation,Material,TitleId")] Item itemb)
        {
            //if (id != item.ItemId)
            //{
            //    return NotFound();
            //}

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update(item);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!ItemExists(item.ItemId))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //    return RedirectToAction("ItemIndex");
            //}

            if (id != itemb.ItemId)
            {
                return NotFound();
            }

            var item = await _context.Items.SingleOrDefaultAsync(m => m.ItemId == id);
            if (item == null)
            {
                Item deletedItem = new Item();
                await TryUpdateModelAsync(deletedItem);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The title was deleted by another user.");
                return View(deletedItem);
            }
            _context.Entry(item).Property("RowVersion").OriginalValue = rowVersion;

            if (await TryUpdateModelAsync<Item>(
                item,
                "",
                s => s.Condition, s => s.CurrentLocation, s => s.Material, s => s.TitleId))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("ItemIndex");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    // Using a NoTracking query means we get the entity but it is not tracked by the context
                    // and will not be merged with existing entities in the context.
                    var databaseEntity = await _context.Items
                        .AsNoTracking()
                        .SingleAsync(d => d.ItemId == ((Item)exceptionEntry.Entity).ItemId);

                    var databaseEntry = _context.Entry(databaseEntity);

                    
                    var databaseCondition = (string)databaseEntry.Property("Condition").CurrentValue;
                    var proposedCondition = (string)exceptionEntry.Property("Condition").CurrentValue;
                    if (databaseCondition != proposedCondition)
                    {
                        ModelState.AddModelError("Condition", $"Current value: {databaseCondition}");
                    }
                    var databaseMaterial = (string)databaseEntry.Property("Material").CurrentValue;
                    var proposedMaterial = (string)exceptionEntry.Property("Material").CurrentValue;
                    if (databaseMaterial != proposedMaterial)
                    {
                        ModelState.AddModelError("Material", $"Current value: {databaseMaterial}");
                    }
                 
                    var databaseCurrentLocation = (string)databaseEntry.Property("CurrentLocation").CurrentValue;
                    var proposedCurrentLocation = (string)exceptionEntry.Property("CurrentLocation").CurrentValue;
                    if (databaseCurrentLocation != proposedCurrentLocation)
                    {
                        ModelState.AddModelError("CurrentLocation", $"Current value: {databaseCurrentLocation}");
                    }
                    
                    var databaseTitleId = (int)databaseEntry.Property("TitleId").CurrentValue;
                    var proposedTitleId = (int)exceptionEntry.Property("TitleId").CurrentValue;
                    if (databaseTitleId != proposedTitleId)
                    {
                        Title databaseTitle = await _context.Titles.SingleAsync(i => i.TitleId == databaseTitleId);
                        ModelState.AddModelError("TitleId", $"Current value: {databaseTitle.TitleId}");
                    }
                    ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                    + "was modified by another user after you got the original value. The "
                    + "edit operation was canceled and the current values in the database "
                    + "have been displayed. If you still want to edit this record, click "
                    + "the Save button again. Otherwise click the Back to List hyperlink.");
                    item.RowVersion = (byte[])databaseEntry.Property("RowVersion").CurrentValue;
                    ModelState.Remove("RowVersion");
                }
            }
            ViewData["TitleId"] = new SelectList(_context.Titles, "TitleId", "Name");
            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> ItemDelete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items.AsNoTracking().SingleOrDefaultAsync(m => m.ItemId == id);
            if (item == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("ItemIndex");
                }
                return NotFound();
            }
            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMessage"] = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("ItemDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ItemDeleteConfirmed(int id)
        {
            try
            {
                if (await _context.Items.AnyAsync(m => m.ItemId == id))
                {
                    var item = await _context.Items.SingleOrDefaultAsync(m => m.ItemId == id);
                    _context.Items.Remove(item);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("ItemIndex");
            }
            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("ItemDelete", new { id, concurrencyError = true });
            }
        }

        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.ItemId == id);
        }
        #endregion
    }
}
