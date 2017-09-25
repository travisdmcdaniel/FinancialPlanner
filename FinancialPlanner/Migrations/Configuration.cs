namespace FinancialPlanner.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using FinancialPlanner.Helpers;

    internal sealed class Configuration : DbMigrationsConfiguration<FinancialPlanner.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(FinancialPlanner.Models.ApplicationDbContext context)
        {

            #region Seed Roles
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Administrator"))
            {
                roleManager.Create(new IdentityRole { Name = "Administrator" });
            }

            if (!context.Roles.Any(r => r.Name == "Head Of Household"))
            {
                roleManager.Create(new IdentityRole { Name = "Head Of Household" });
            }

            if (!context.Roles.Any(r => r.Name == "Member Of Household"))
            {
                roleManager.Create(new IdentityRole { Name = "Member Of Household" });
            }

            if (!context.Roles.Any(r => r.Name == "Guest"))
            {
                roleManager.Create(new IdentityRole { Name = "Guest" });
            }
            #endregion

            #region Seed Users
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "travis.d.mcdaniel@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "travis.d.mcdaniel@gmail.com",
                    Email = "travis.d.mcdaniel@gmail.com",
                    FirstName = "Travis",
                    LastName = "McDaniel",
                    Protected = true,
                    DisplayName = "TravisMcDaniel"
                }, "Abc&123!");
            }

            if (!context.Users.Any(u => u.Email == "eddard@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "eddardstark@mailinator.com",
                    Email = "eddardstark@mailinator.com",
                    FirstName = "Eddard",
                    LastName = "Stark",
                    Protected = true,
                    DisplayName = "EddardStark"
                }, "Abc&123!");
            }

            if (!context.Users.Any(u => u.Email == "catelynstark@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "catelynstark@mailinator.com",
                    Email = "catelynstark@mailinator.com",
                    FirstName = "Catelyn",
                    LastName = "Stark",
                    Protected = true,
                    DisplayName = "CatelynStark"
                }, "Abc&123!");
            }

            if (!context.Users.Any(u => u.Email == "robbstark@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "robbstark@mailinator.com",
                    Email = "robbstark@mailinator.com",
                    FirstName = "Robb",
                    LastName = "Stark",
                    Protected = true,
                    DisplayName = "RobbStark"
                }, "Abc&123!");
            }

            if (!context.Users.Any(u => u.Email == "theongreyjoy@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "theongreyjoy@mailinator.com",
                    Email = "theongreyjoy@mailinator.com",
                    FirstName = "Theon",
                    LastName = "Greyjoy",
                    Protected = true,
                    DisplayName = "TheonGreyjoy"
                }, "Abc&123!");
            }

            if (!context.Users.Any(u => u.Email == "ashagreyjoy@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "ashagreyjoy@mailinator.com",
                    Email = "ashagreyjoy@mailinator.com",
                    FirstName = "Asha",
                    LastName = "Greyjoy",
                    Protected = true,
                    DisplayName = "AshaGreyjoy"
                }, "Abc&123!");
            }
            #endregion

            #region Give Seeded Users Roles
            var tempId = userManager.FindByEmail("travis.d.mcdaniel@gmail.com").Id;
            userManager.AddToRole(tempId, "Administrator");
            tempId = userManager.FindByEmail("eddardstark@mailinator.com").Id;
            userManager.AddToRole(tempId, "Head Of Household");
            tempId = userManager.FindByEmail("catelynstark@mailinator.com").Id;
            userManager.AddToRole(tempId, "Member Of Household");
            tempId = userManager.FindByEmail("robbstark@mailinator.com").Id;
            userManager.AddToRole(tempId, "Member Of Household");
            tempId = userManager.FindByEmail("theongreyjoy@mailinator.com").Id;
            userManager.AddToRole(tempId, "Guest");
            tempId = userManager.FindByEmail("ashagreyjoy@mailinator.com").Id;
            userManager.AddToRole(tempId, "Guest");
            #endregion

            #region Seeded Household
            Household wfHousehold = new Household { Name = "Winterfell", Description = "Winter Is Coming", Key = Utilities.GenHouseholdKey(), Password = "TheNorthRemembers" };
            Household afpHousehold = new Household { Name = "abacus financial planner", Description = "abacus financial planner", Key = Utilities.GenHouseholdKey(), Password = "Abc&123!", Id = 777 };
            #endregion

            #region Add Seeded Users To Household
            wfHousehold.Users.Add(userManager.FindByEmail("eddardstark@mailinator.com"));
            wfHousehold.Users.Add(userManager.FindByEmail("catelynstark@mailinator.com"));
            wfHousehold.Users.Add(userManager.FindByEmail("robbstark@mailinator.com"));
            #endregion

           

            #region Seed BudgetStatuses
            BudgetStatus bsUnder = new BudgetStatus { Name = "Under Budget" };
            BudgetStatus bsOver = new BudgetStatus { Name = "Over Budget" };
            BudgetStatus bsPar = new BudgetStatus { Name = "On Par" };
            BudgetStatus bsNoInfo = new BudgetStatus { Name = "No Status" };
            context.BudgetStatuses.AddOrUpdate(
                s => s.Name,
                bsUnder,
                bsPar,
                bsOver,
                bsNoInfo
                );
            #endregion


            #region Seed Budgets
            //Budget budget1 = new Budget { Name = "Entertainment" };
            //Budget budget2 = new Budget { Name = "Groceries" };
            //Budget budget3 = new Budget { Name = "Utilities" };
            //Budget budget4 = new Budget { Name = "Automobile" };
            //Budget budget5 = new Budget { Name = "Dining Out" };
            Budget budget6 = new Budget { Name = "abacus financial planner", Id = 666, Description = "Default Budget Items For abacus financial planner", Date = DateTime.Now, Month = DateTime.Now.Month, Year = DateTime.Now.Year, AmountAgainst = 0m, AmountBudgeted = 0m, Monthly = true, HouseholdId = 777 };

            context.Budgets.AddOrUpdate(
                b => b.Name,
                budget6);

            BudgetItem budgetItem = new BudgetItem { Name = "Abacus - Overdraft Fee", BudgetId = 666, Date = DateTime.Now, Year = DateTime.Now.Year, Month = DateTime.Now.Month };

            context.BudgetItems.AddOrUpdate(
                bi => bi.Name,
                budgetItem
                );
            #endregion

            #region Complete Household Creation
            context.Households.AddOrUpdate(
                p => p.Name,
                wfHousehold,
                afpHousehold
                );
            #endregion
        }
    }
}
