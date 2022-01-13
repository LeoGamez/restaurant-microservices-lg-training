using IdentityModel;
using Mango.Services.Identity.DbContexts;
using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Mango.Services.Identity.Initializer
{
    public class DBInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DBInitializer(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {

            if (_roleManager.FindByNameAsync(SD.Admin).Result == null)
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Customer)).GetAwaiter().GetResult();
            }
            else
            {
                return;
            }

            ApplicationUser admin = new ApplicationUser()
            {
                UserName = "admin1_lg@gmail.com",
                Email = "admin1@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "1111111111",
                FirstName= "Ben",
                LastName="Admin"
            };

            _userManager.CreateAsync(admin, "Admin123*").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(admin,SD.Admin).GetAwaiter().GetResult();

            var temp1 =_userManager.AddClaimsAsync(admin, new Claim[] { 
                new Claim(JwtClaimTypes.Name,admin.FirstName+" ",admin.LastName),
                new Claim(JwtClaimTypes.GivenName,admin.FirstName),
                new Claim(JwtClaimTypes.FamilyName,admin.LastName),
                new Claim(JwtClaimTypes.Role, SD.Admin)
            }).Result;

            ApplicationUser customer = new ApplicationUser()
            {
                UserName = "customer1_lg@gmail.com",
                Email = "customer1@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "1112111111",
                FirstName = "Ben",
                LastName = "Customer"
            };

            _userManager.CreateAsync(customer, "Customer123*").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(customer,SD.Customer).GetAwaiter().GetResult();

            var temp2= _userManager.AddClaimsAsync(customer, new Claim[] {
                new Claim(JwtClaimTypes.Name,customer.FirstName+" ",customer.LastName),
                new Claim(JwtClaimTypes.GivenName,customer.FirstName),
                new Claim(JwtClaimTypes.FamilyName,customer.LastName),
                new Claim(JwtClaimTypes.Role, SD.Customer)
            }).Result;
        }
    }
}
