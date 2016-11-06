using LibraryWebApp.Data;
using LibraryWebApp.Models;
using LibraryWebApp.Models.LibraryViewModels;
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

        private readonly IItemRepository _itemRepository;

        public LibraryController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IAuthorizationService authorizationService,
            ItemRepository itemRepository)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _authorizationService = authorizationService;
            _itemRepository = itemRepository;
        }

        #region Users
        // GET: ApplicationUsers
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UserIndex()
        {
            var users = await _context.Users.ToListAsync();
            var usersVM = new List<ApplicationUserViewModel>();
            foreach (var item in users)
            {
                usersVM.Add(new ApplicationUserViewModel(item));
            }
            return View(usersVM);
        }

        // GET: ApplicationUsers/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UserDetails(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            var applicationUserVM = new ApplicationUserViewModel(applicationUser);
            var r = await _userManager.GetRolesAsync(applicationUser);
            SelectList UserRoles = new SelectList(r);
            ViewData["UserRoles"] = UserRoles;

            return View(applicationUserVM);
        }

        // GET: ApplicationUsers/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UserEdit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            var applicationUserVM = new ApplicationUserViewModel(applicationUser);
            var listroles = System.Enum.GetValues(typeof(Models.Enum.LibraryRoles)).OfType<Models.Enum.LibraryRoles>();
            var r = await _userManager.GetRolesAsync(applicationUser);
            var sel = listroles.Where(e => e.ToString() == r.FirstOrDefault());
            SelectList UserRoles = new SelectList(listroles, sel.FirstOrDefault() );
            ViewData["UserRoles"] = UserRoles;
            return View(applicationUserVM);
        }

        // POST: ApplicationUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserEdit(string id, string Role, ApplicationUserViewModel applicationUserVM)
        {
            if (id != applicationUserVM.ApplicationUser.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                   var currentrole = await _userManager.GetRolesAsync(applicationUserVM.ApplicationUser);
                    if (!currentrole.Where(e => e == Role).Any())
                    {
                        var newRole = new IdentityUserRole<string>();
                        newRole.RoleId = _context.Roles.Where(e => e.Name == applicationUserVM.ApplicationUser.Role)?.FirstOrDefault()?.Id;
                        newRole.UserId = applicationUserVM.ApplicationUser.Id;
                        _context.UserRoles.Add(newRole);
                        _context.SaveChanges();
                    }

                    _context.Update(applicationUserVM.ApplicationUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationUserExists(applicationUserVM.ApplicationUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                var roleToRemove = await _userManager.GetRolesAsync(applicationUserVM.ApplicationUser);
                foreach (var item in roleToRemove.Where(e => e != Role))
                {
                    await _userManager.RemoveFromRoleAsync(applicationUserVM.ApplicationUser, item);
                }
                return RedirectToAction("UserIndex");
            }
            return View(applicationUserVM);
        }

        // GET: ApplicationUsers/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UserDelete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            var applicationUserVM = new ApplicationUserViewModel(applicationUser);
            return View(applicationUserVM);
        }

        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("UserDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserDeleteConfirmed(string id)
        {
            var applicationUser = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
            _context.Users.Remove(applicationUser);
            await _context.SaveChangesAsync();
            return RedirectToAction("UserIndex");
        }

        private bool ApplicationUserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }


        #endregion


        #region Favourites

        //GET: Favourites
        [Authorize(Roles = "Admin, Librarian, RegUser")]
        public async Task<IActionResult> FavouriteIndex()
        {
            var appuser = await _userManager.FindByNameAsync(User.Identity.Name);
            var appuserfav = await _context.Favourite.Where(e => e.User.UserName == appuser.UserName).ToListAsync();
            var favouritesVM = new List<Models.LibraryViewModels.FavouriteViewModel>();
            foreach (var item in appuserfav)
            {
                favouritesVM.Add(new Models.LibraryViewModels.FavouriteViewModel(item));
            }
            return View(favouritesVM);
        }

        // GET: Favourites/Details/5
        [Authorize(Roles = "Admin, Librarian, RegUser")]
        public async Task<IActionResult> FavouriteDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favourite = await _context.Favourite.SingleOrDefaultAsync(m => m.FavouriteId == id);
            if (favourite == null)
            {
                return NotFound();
            }
            var favouriteVM = new Models.LibraryViewModels.FavouriteViewModel(favourite);

            return View(favouriteVM);
        }

        // GET: Favourites/Edit/5
        [Authorize(Roles = "Admin, Librarian, RegUser")]
        public async Task<IActionResult> FavouriteEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favourite = await _context.Favourite.SingleOrDefaultAsync(m => m.FavouriteId == id);
            if (favourite == null)
            {
                return NotFound();
            }
            var favouriteVM = new Models.LibraryViewModels.FavouriteViewModel(favourite);
            return View(favouriteVM);
        }

        // POST: Favourites/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FavouriteEdit(int id, Models.LibraryViewModels.FavouriteViewModel favouriteVM)
        {

            if (id != favouriteVM.Favourite.FavouriteId)
            {
                return NotFound();
            }
            var favourite = await _context.Favourite.SingleOrDefaultAsync(m => m.FavouriteId == id);

            if (ModelState.IsValid)
            {
                try
                {
                    favourite.Comment = favouriteVM?.Favourite.Comment;
                    _context.Update(favourite);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FavouriteExists(favourite.FavouriteId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("FavouriteIndex");
            }
            return View(favouriteVM);
        }

        // GET: Favourites/Delete/5
        [Authorize(Roles = "Admin, Librarian, RegUser")]
        public async Task<IActionResult> FavouriteDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favourite = await _context.Favourite.SingleOrDefaultAsync(m => m.FavouriteId == id);
            if (favourite == null)
            {
                return NotFound();
            }
            var favouriteVM = new Models.LibraryViewModels.FavouriteViewModel(favourite);

            return View(favouriteVM);
        }

        // POST: Favourites/Delete/5
        [HttpPost, ActionName("FavouriteDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FavouriteDeleteConfirmed(int id)
        {
            var favourite = await _context.Favourite.SingleOrDefaultAsync(m => m.FavouriteId == id);
            _context.Favourite.Remove(favourite);
            await _context.SaveChangesAsync();
            return RedirectToAction("FavouriteIndex");
        }

        private bool FavouriteExists(int id)
        {
            return _context.Favourite.Any(e => e.FavouriteId == id);
        }
        #endregion

        #region Item Movement
        // GET: ItemMovements
        [Authorize(Roles = "Admin, Librarian")]
        public async Task<IActionResult> ItemMovementIndex()
        {
            var itemMovements = await _context.ItemMovements.Include(i => i.Item).Include(e => e.Item.Title).Include(p => p.Librarian).Include(u =>u.User).ToListAsync();
            var itemMovementsVM = new List<ItemMovementViewModel>();
            foreach (var item in itemMovements)
            {
                itemMovementsVM.Add(new ItemMovementViewModel(item));
            }
            return View(itemMovementsVM);
        }

        // GET: ItemMovements/Details/5
        [Authorize(Roles = "Admin, Librarian")]
        public async Task<IActionResult> ItemMovementDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemMovement = await _context.ItemMovements.SingleOrDefaultAsync(m => m.ItemMovementId == id);
            if (itemMovement == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = _context.Users.Where(e => e.Id == itemMovement.ApplicationUserId).FirstOrDefault().UserName;
            var titleId = _context.Items.Where(e => e.ItemId == itemMovement.ItemId).FirstOrDefault().TitleId;
            ViewData["ItemId"] = _context.Items.Where(e => e.ItemId == itemMovement.ItemId).FirstOrDefault().ItemId;
            ViewData["TitleName"] = _context.Titles.Where(e => e.TitleId == titleId).FirstOrDefault().Name;
            ViewData["LibrarianId"] = _context.Users.Where(e => e.Id == itemMovement.LibrarianId).FirstOrDefault().UserName;

            var itemMovementVM = new ItemMovementViewModel(itemMovement);
            return View(itemMovementVM);
        }

        // GET: ItemMovements/Create
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult ItemMovementCreate()
        {
            ViewData["ApplicationUserId"] = new SelectList(_userManager.Users.ToList(), "Id", "UserName");
            ViewData["ItemId"] = new SelectList(_context.Items.Include(e => e.Title), "ItemId", "ItemId");
            var listMovType = System.Enum.GetValues(typeof(Models.Enum.MovementType)).OfType<Models.Enum.MovementType>();
            ViewData["MovType"] = new SelectList(listMovType);
            return View();
        }

        // POST: ItemMovements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ItemMovementCreate(ItemMovementViewModel itemMovementVM)
        {
            if (ModelState.IsValid)
            {
                var librarian = _context.Users.Where(e => e.UserName == User.Identity.Name).FirstOrDefault();
                itemMovementVM.ItemMovement.LibrarianId = librarian.Id;
                _context.Add(itemMovementVM.ItemMovement);
                await _context.SaveChangesAsync();
                return RedirectToAction("ItemMovementIndex");
            }

            ViewData["ApplicationUserId"] = new SelectList(_userManager.Users.ToList(), "Id", "UserName", itemMovementVM.ItemMovement.ApplicationUserId);
            ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "Name", itemMovementVM.ItemMovement.ItemId);
            return View(itemMovementVM);
        }

        // GET: ItemMovements/Edit/5
        [Authorize(Roles = "Admin, Librarian")]
        public async Task<IActionResult> ItemMovementEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemMovement = await _context.ItemMovements.SingleOrDefaultAsync(m => m.ItemMovementId == id);
            if (itemMovement == null)
            {
                return NotFound();
            }
            ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "ItemId", itemMovement.ItemId);
            var itemMovementVM = new ItemMovementViewModel(itemMovement);
            return View(itemMovementVM);
        }

        // POST: ItemMovements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ItemMovementEdit(int id, ItemMovementViewModel itemMovementVM)
        {
            if (id != itemMovementVM.ItemMovement.ItemMovementId)
            {
                return NotFound();
            }
            var itemMov = await _context.ItemMovements.SingleOrDefaultAsync(m => m.ItemMovementId == id);
            if (itemMov == null)
            {
                ItemMovement deletedItem = new ItemMovement();
                await TryUpdateModelAsync(deletedItem);
                var deletedItemMovVM = new ItemMovementViewModel(deletedItem);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The item movement was deleted by another user.");
                return View(deletedItemMovVM);
            }
            _context.Entry(itemMov).Property("RowVersion").OriginalValue = itemMovementVM.ItemMovement.RowVersion;
            itemMov.Deadline = itemMovementVM.ItemMovement.Deadline;
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("ItemMovementIndex");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ModelState.Clear();
                    var exceptionEntry = ex.Entries.Single();
                    // Using a NoTracking query means we get the entity but it is not tracked by the context
                    // and will not be merged with existing entities in the context.
                    var databaseEntity = await _context.ItemMovements
                        .AsNoTracking()
                        .SingleAsync(d => d.ItemMovementId == ((ItemMovement)exceptionEntry.Entity).ItemMovementId);

                    var databaseEntry = _context.Entry(databaseEntity);


                    var databaseDeadline = (DateTime)databaseEntry.Property("Deadline").CurrentValue;
                    var proposedDeadline = (DateTime)exceptionEntry.Property("Deadline").CurrentValue;
                    if (databaseDeadline != proposedDeadline)
                    {
                        ModelState.AddModelError("ItemMovement.Deadline", $"Current value: {databaseDeadline}");
                    }
                    
                    ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                    + "was modified by another user after you got the original value. The "
                    + "edit operation was canceled and the current values in the database "
                    + "have been displayed. If you still want to edit this record, click "
                    + "the Save button again. Otherwise click the Back to List hyperlink.");
                    itemMovementVM.ItemMovement.RowVersion = (byte[])databaseEntry.Property("RowVersion").CurrentValue;
                }


            ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "ItemId", itemMovementVM.ItemMovement.ItemId);
            return View(itemMovementVM);
        }

        // GET: ItemMovements/Delete/5
        [Authorize(Roles = "Admin, Librarian")]
        public async Task<IActionResult> ItemMovementDelete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }
            var itemMovement = await _context.ItemMovements.AsNoTracking().SingleOrDefaultAsync(m => m.ItemMovementId == id);
            if (itemMovement == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("ItemMovementIndex");
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
            var itemMovementVM = new ItemMovementViewModel(itemMovement);

            ViewData["ApplicationUserId"] = _context.Users.Where(e => e.Id == itemMovement.ApplicationUserId).FirstOrDefault().UserName;
            var titleId = _context.Items.Where(e => e.ItemId == itemMovement.ItemId).FirstOrDefault().TitleId;
            ViewData["ItemId"] = _context.Items.Where(e => e.ItemId == itemMovement.ItemId).FirstOrDefault().ItemId;
            ViewData["TitleName"] = _context.Titles.Where(e => e.TitleId == titleId).FirstOrDefault().Name;
            ViewData["LibrarianId"] = _context.Users.Where(e => e.Id == itemMovement.LibrarianId).FirstOrDefault().UserName;

            return View(itemMovementVM);
        }

        // POST: ItemMovements/Delete/5
        [HttpPost, ActionName("ItemMovementDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ItemMovementDeleteConfirmed(int id)
        {
            try
            {
                if (await _context.ItemMovements.AnyAsync(m => m.ItemMovementId == id))
                {
                    var itemMovement = await _context.ItemMovements.SingleOrDefaultAsync(m => m.ItemMovementId == id);
                    _context.ItemMovements.Remove(itemMovement);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("ItemMovementIndex");
            }
            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("ItemMovementDelete", new { id, concurrencyError = true });
            }
        }

        private bool ItemMovementExists(int id)
        {
            return _context.ItemMovements.Any(e => e.ItemMovementId == id);
        }

        #endregion

        #region Sections
        // GET: Sections
        public async Task<IActionResult> SectionIndex()
        {
            var sections = await _context.Sections.ToListAsync();
            var sectionsVM = new List<SectionViewModel>();
            foreach (var item in sections)
            {
                sectionsVM.Add(new SectionViewModel(item));
            }
            return View(sectionsVM);
        }

        // GET: Sections/Details/5
        [Authorize(Roles = "Admin, Librarian")]
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
            var sectionVM = new SectionViewModel(section);
            return View(sectionVM);
        }

        // GET: Sections/Create
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult SectionCreate()
        {
            return View();
        }

        // POST: Sections/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SectionCreate(SectionViewModel sectionVM)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sectionVM.Section);
                await _context.SaveChangesAsync();
                return RedirectToAction("SectionIndex");
            }
            return View(sectionVM);
        }

        // GET: Sections/Edit/5
        [Authorize(Roles = "Admin, Librarian")]
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
            var sectionVM = new SectionViewModel(section);
            return View(sectionVM);

        }

        // POST: Sections/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SectionEdit(int id, SectionViewModel sectionVM)
        {
            if (id != sectionVM.Section.SectionId)
            {
                return NotFound();
            }

            var section = await _context.Sections.SingleOrDefaultAsync(m => m.SectionId == id);
            if (section == null)
            {
                Section deletedSection = new Section();
                await TryUpdateModelAsync(deletedSection);
                var deletedSectionVM = new SectionViewModel(deletedSection);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The section was deleted by another user.");
                return View(deletedSectionVM);
            }
            _context.Entry(section).Property("RowVersion").OriginalValue = sectionVM.Section.RowVersion;
            section.Description = sectionVM.Section.Description;
            section.Name = sectionVM.Section.Name;
           
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("SectionIndex");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ModelState.Clear();
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
                        ModelState.AddModelError("Section.Name", $"Current value: {databaseName}");
                    }
                    var databaseDescription = (string)databaseEntry.Property("Description").CurrentValue;
                    var proposedDescription = (string)exceptionEntry.Property("Description").CurrentValue;
                    if (databaseDescription != proposedDescription)
                    {
                        ModelState.AddModelError("Section.Description", $"Current value: {databaseDescription}");
                    }
                    
                    ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                    + "was modified by another user after you got the original value. The "
                    + "edit operation was canceled and the current values in the database "
                    + "have been displayed. If you still want to edit this record, click "
                    + "the Save button again. Otherwise click the Back to List hyperlink.");
                    sectionVM.Section.RowVersion = (byte[])databaseEntry.Property("RowVersion").CurrentValue;
                    ModelState.Remove("RowVersion");
                }
            return View(sectionVM);
        }

        // GET: Sections/Delete/5
        [Authorize(Roles = "Admin, Librarian")]
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
            var sectionVM = new SectionViewModel(section);
            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMessage"] = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }

            return View(sectionVM);

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
            var titles = await _context.Titles.Include(t => t.Section).ToListAsync();
            var titlesVM = new List<TitleViewModel>();
            foreach (var item in titles)
            {
                titlesVM.Add(new TitleViewModel(item));
            }
            return View(titlesVM);
        }

        // GET: Titles/Details/5
        [Authorize(Roles = "Admin, Librarian")]
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
            TitleViewModel titleVM = new TitleViewModel(title);
            return View(titleVM);
        }

        // GET: Titles/Create
        [Authorize(Roles = "Admin, Librarian")]
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
        public async Task<IActionResult> TitleCreate(TitleViewModel titleVM)
        {
            if (ModelState.IsValid)
            {
                _context.Add(titleVM.Title);
                await _context.SaveChangesAsync();
                return RedirectToAction("TitleIndex");
            }
            ViewData["SectionId"] = new SelectList(_context.Sections, "SectionId", "SectionId", titleVM.Title.SectionId);
            return View(titleVM);
        }

        // GET: Titles/Edit/5
        [Authorize(Roles = "Admin, Librarian")]
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
            TitleViewModel titleVM = new TitleViewModel(title);
            ViewData["SectionId"] = new SelectList(_context.Sections, "SectionId", "Name");
            return View(titleVM);
        }

        // POST: Titles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TitleEdit(int id, TitleViewModel titleVM)
        {
            if (id != titleVM.Title.TitleId)
            {
                return NotFound();
            }

            var title = await _context.Titles.SingleOrDefaultAsync(m => m.TitleId == id);
            if (title == null)
            {
                Title deletedTitle = new Title();
                await TryUpdateModelAsync(deletedTitle);
                var deletedTitleVM = new TitleViewModel(deletedTitle);
                ViewData["SectionId"] = new SelectList(_context.Sections, "SectionId", "Name");
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The title was deleted by another user.");
                return View(deletedTitleVM);
            }
            _context.Entry(title).Property("RowVersion").OriginalValue = titleVM.Title.RowVersion;
            title.Annotation = titleVM.Title.Annotation;
            title.Author = titleVM.Title.Author;
            title.ISBN = titleVM.Title.ISBN;
            title.Name = titleVM.Title.Name;
            title.Publisher = titleVM.Title.Publisher;
            title.SectionId = titleVM.Title.SectionId;
            title.Year = titleVM.Title.Year;
            title.Type = titleVM.Title.Type;
           
            try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("TitleIndex");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ModelState.Clear();
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
                        ModelState.AddModelError("Title.Name", $"Current value: {databaseName}");
                    }
                    var databaseAnnotation = (string)databaseEntry.Property("Annotation").CurrentValue;
                    var proposedAnnotation = (string)exceptionEntry.Property("Annotation").CurrentValue;
                    if (databaseAnnotation != proposedAnnotation)
                    {
                        ModelState.AddModelError("Title.Annotation", $"Current value: {databaseAnnotation}");
                    }
                    var databaseAuthor = (string)databaseEntry.Property("Author").CurrentValue;
                    var proposedAuthor = (string)exceptionEntry.Property("Author").CurrentValue;
                    if (databaseAuthor != proposedAuthor)
                    {
                        ModelState.AddModelError("Title.Author", $"Current value: {databaseAuthor}");
                    }
                    var databaseYear = (short)databaseEntry.Property("Year").CurrentValue;
                    var proposedYear = (short)exceptionEntry.Property("Year").CurrentValue;
                    if (databaseYear != proposedYear)
                    {
                        ModelState.AddModelError("Title.Year", $"Current value: {databaseYear:d}");
                    }
                    var databaseISBN = (string)databaseEntry.Property("ISBN").CurrentValue;
                    var proposedISBN = (string)exceptionEntry.Property("ISBN").CurrentValue;
                    if (databaseISBN != proposedISBN)
                    {
                        ModelState.AddModelError("Title.ISBN", $"Current value: {databaseISBN}");
                    }

                    var databasePublisher = (string)databaseEntry.Property("Publisher").CurrentValue;
                    var proposedPublisher = (string)exceptionEntry.Property("Publisher").CurrentValue;
                    if (databasePublisher != proposedPublisher)
                    {
                        ModelState.AddModelError("Title.Publisher", $"Current value: {databasePublisher}");
                    }
                    var databaseType = (string)databaseEntry.Property("Type").CurrentValue;
                    var proposedType = (string)exceptionEntry.Property("Type").CurrentValue;
                    if (databaseType != proposedType)
                    {
                        ModelState.AddModelError("Title.Type", $"Current value: {databaseType}");
                    }
                    var databaseSectionId = (int)databaseEntry.Property("SectionId").CurrentValue;
                    var proposedSectionId = (int)exceptionEntry.Property("SectionId").CurrentValue;
                    if (databaseSectionId != proposedSectionId)
                    {
                        Section databaseSection = await _context.Sections.SingleAsync(i => i.SectionId == databaseSectionId);
                        ModelState.AddModelError("Title.SectionID", $"Current value: {databaseSection.SectionId}");
                    }
                    ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                    + "was modified by another user after you got the original value. The "
                    + "edit operation was canceled and the current values in the database "
                    + "have been displayed. If you still want to edit this record, click "
                    + "the Save button again. Otherwise click the Back to List hyperlink.");
                    
                    titleVM.Title.RowVersion = (byte[])databaseEntry.Property("RowVersion").CurrentValue;
                }
            ViewData["SectionId"] = new SelectList(_context.Sections, "SectionId", "Name");

            return View(titleVM);

        }

        [Authorize(Roles = "Admin, Librarian")]
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
            var titleVM = new TitleViewModel(title);
            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMessage"] = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }

            return View(titleVM);
        }

        [HttpPost, ActionName("TitleDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TitleDelete(TitleViewModel titleVM)
        {
            try
            {
                if (await _context.Titles.AnyAsync(m => m.TitleId == titleVM.Title.TitleId))
                {
                    _context.Titles.Remove(titleVM.Title);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("TitleIndex");

            }
            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("TitleDelete", new { concurrencyError = true, id = titleVM.Title.TitleId });
            }
        }

        private bool TitleExists(int id)
        {
            return _context.Titles.Any(e => e.TitleId == id);
        }

        #endregion

        #region Items



        [Authorize(Roles = "Admin, Librarian, RegUser")]
        public async Task<IActionResult> ItemAddToFavourites(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items.SingleOrDefaultAsync(m => m.ItemId == id);
            var fav = new Models.LibraryViewModels.FavouriteViewModel();
            if (item == null)
            {
                return NotFound();
            }

            return View(fav);
        }

        [HttpPost, ActionName("ItemAddToFavourites")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ItemAddToFavouritesConfirmed(int id, Models.LibraryViewModels.FavouriteViewModel f)
        {
            var item = await _context.Items.SingleOrDefaultAsync(m => m.ItemId == id);
            var appuser = await _userManager.FindByNameAsync(User.Identity.Name);
            _context.Favourite.Add(new Favourite() { Comment = f?.Comment, User = appuser, ItemIndex = id.ToString()});
            _context.SaveChanges();
            return RedirectToAction("FavouriteIndex");

        }


        // GET: Items
        public async Task<IActionResult> ItemIndex()
        {
            var items = await _itemRepository.SelectAll();
            var itemsVM = new List<ItemViewModel>();
            foreach (var item in items)
            {
                itemsVM.Add(new ItemViewModel(item));
            }
            return View(itemsVM);
        }

        // GET: Items/Details/5
        [Authorize(Roles = "Admin, Librarian, RegUser")]
        public async Task<IActionResult> ItemDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var item =  await _itemRepository.SelectByID(id.ToString());
            if (item == null)
            {
                return NotFound();
            }
            var itemVM = new ItemViewModel(item);
            return View(itemVM);
        }

        // GET: Items/Create
        [Authorize(Roles = "Admin, Librarian")]
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
        public async Task<IActionResult> ItemCreate(ItemViewModel itemVM)
        {
            if (ModelState.IsValid)
            {
                 _itemRepository.Insert(itemVM.Item);
                await _itemRepository.Save();
                return RedirectToAction("ItemIndex");
            }
            ViewData["TitleId"] = new SelectList(_context.Titles, "TitleId", "TitleId", itemVM.Item.TitleId);
            return View(itemVM);
        }

        // GET: Items/Edit/5
        [Authorize(Roles = "Admin, Librarian")]
        public async Task<IActionResult> ItemEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _itemRepository.SelectByID(id.ToString());
            if (item == null)
            {
                return NotFound();
            }
            var itemVM = new ItemViewModel(item);
            ViewData["TitleId"] = new SelectList(_context.Titles, "TitleId", "Name");
            return View(itemVM);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ItemEdit(int id, byte[] rowVersion, ItemViewModel itemVM)
        {
            if (id != itemVM.Item.ItemId)
            {
                return NotFound();
            }
            var item = await _itemRepository.SelectByID(id.ToString());
            if (item == null)
            {
                Item deletedItem = new Item();
                await TryUpdateModelAsync(deletedItem);
                var deletedItemVM = new ItemViewModel(deletedItem);
                ViewData["TitleId"] = new SelectList(_context.Titles, "TitleId", "Name");
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The title was deleted by another user.");
                return View(deletedItemVM);
            }
            _context.Entry(item).Property("RowVersion").OriginalValue = itemVM.Item.RowVersion;
            item.Material = itemVM.Item.Material;
            item.Condition = itemVM.Item.Condition;
            item.CurrentLocation = itemVM.Item.CurrentLocation;
            item.TitleId = itemVM.Item.TitleId;
            try
            {
                await _itemRepository.Save();
                return RedirectToAction("ItemIndex");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ModelState.Clear();
                    var exceptionEntry = ex.Entries.Single();
                // Using a NoTracking query means we get the entity but it is not tracked by the context
                // and will not be merged with existing entities in the context.
                var databaseEntity = await _itemRepository.SelectByIDAsNoTracking(((Item)exceptionEntry.Entity).ItemId.ToString());

                var databaseEntry = _context.Entry(databaseEntity);

                    
                    var databaseCondition = (string)databaseEntry.Property("Condition").CurrentValue;
                    var proposedCondition = (string)exceptionEntry.Property("Condition").CurrentValue;
                    if (databaseCondition != proposedCondition)
                    {
                        ModelState.AddModelError("Item.Condition", $"Current value: {databaseCondition}");
                    }
                    var databaseMaterial = (string)databaseEntry.Property("Material").CurrentValue;
                    var proposedMaterial = (string)exceptionEntry.Property("Material").CurrentValue;
                    if (databaseMaterial != proposedMaterial)
                    {
                        ModelState.AddModelError("Item.Material", $"Current value: {databaseMaterial}");
                    }
                 
                    var databaseCurrentLocation = (string)databaseEntry.Property("CurrentLocation").CurrentValue;
                    var proposedCurrentLocation = (string)exceptionEntry.Property("CurrentLocation").CurrentValue;
                    if (databaseCurrentLocation != proposedCurrentLocation)
                    {
                        ModelState.AddModelError("Item.CurrentLocation", $"Current value: {databaseCurrentLocation}");
                    }
                    
                    var databaseTitleId = (int)databaseEntry.Property("TitleId").CurrentValue;
                    var proposedTitleId = (int)exceptionEntry.Property("TitleId").CurrentValue;
                    if (databaseTitleId != proposedTitleId)
                    {
                        Title databaseTitle = await _context.Titles.SingleAsync(i => i.TitleId == databaseTitleId);
                        ModelState.AddModelError("Item.TitleId", $"Current value: {databaseTitle.TitleId}");
                    }
                    ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                    + "was modified by another user after you got the original value. The "
                    + "edit operation was canceled and the current values in the database "
                    + "have been displayed. If you still want to edit this record, click "
                    + "the Save button again. Otherwise click the Back to List hyperlink.");
                    itemVM.Item.RowVersion = (byte[])databaseEntry.Property("RowVersion").CurrentValue;
            }

            ViewData["TitleId"] = new SelectList(_context.Titles, "TitleId", "Name");
            return View(itemVM);
        }

        // GET: Items/Delete/5
        [Authorize(Roles = "Admin, Librarian")]
        public async Task<IActionResult> ItemDelete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }
            var item = await _itemRepository.SelectByID(id.ToString());
            if (item == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("ItemIndex");
                }
                return NotFound();
            }
            var itemVM = new ItemViewModel(item);
            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMessage"] = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }

            return View(itemVM);
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
                    var item = await _itemRepository.SelectByID(id.ToString());
                    await _itemRepository.Delete(item.ItemId.ToString());
                    await _itemRepository.Save();
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
