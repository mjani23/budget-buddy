using SQLite;
using System.IO;
using Microsoft.Maui.Storage;

namespace FinanceFrenzy.Models
{
    public static class DatabaseHelper
    {
        private static string dbPath = Path.Combine(FileSystem.AppDataDirectory, "FinanceFrenzy.db");

        //this function creates and initializes the database
        public static void InitializeDatabase()
        {
            using (var db = new SQLiteConnection(dbPath))
            {
                db.CreateTable<UserInfo>();
                db.CreateTable<BudgetCategory>();
                db.CreateTable<Expense>();

                // Ensure "Miscellaneous" category exists
                var miscCategory = db.Table<BudgetCategory>().FirstOrDefault(c => c.Category == "Miscellaneous");
                if (miscCategory == null)
                {
                    db.Insert(new BudgetCategory { Category = "Miscellaneous", Amount = 100.0, CanBeDeleted = false });
                }
            }
        }


        //save an expense into the database
        public static void SaveExpense(Expense expense)
        {
            using (var db = new SQLiteConnection(dbPath))
            {
                db.Insert(expense);
            }
        }

        //load an expense into the database
        public static List<Expense> LoadExpenses()
        {
            using (var db = new SQLiteConnection(dbPath))
            {
                db.CreateTable<Expense>();
                return db.Table<Expense>().ToList();
            }
        }


        //delete an expense from the database
        public static void DeleteExpense(Expense expense)
        {
            using (var db = new SQLiteConnection(dbPath))
            {
                db.Delete(expense);
            }

            // If no more expenses exist for the category, remove it from BudgetCategory
            using (var db = new SQLiteConnection(dbPath))
            {
                bool categoryExists = db.Table<Expense>().Any(e => e.Category == expense.Category);
                if (!categoryExists)
                {
                    var categoryToDelete = db.Table<BudgetCategory>().FirstOrDefault(c => c.Category == expense.Category);
                    if (categoryToDelete != null)
                    {
                        db.Delete(categoryToDelete);
                    }
                }
            }
        }



        //adds a new budget category to the database or updates it if it already exists
        public static void SaveBudgetCategory(BudgetCategory category)
        {
            using (var db = new SQLiteConnection(dbPath))
            {
                // Check if the category already exists
                var existingCategory = db.Table<BudgetCategory>().FirstOrDefault(c => c.Category == category.Category);
                if (existingCategory == null)
                {
                    //insert new one
                    db.Insert(category);
                }
                else
                {
                    //update it
                    existingCategory.Amount = category.Amount;
                    db.Update(existingCategory);
                }
            }
        }
        //loads all budget categories from the database
        public static List<BudgetCategory> LoadBudgetCategories()
        {
            using (var db = new SQLiteConnection(dbPath))
            {
                db.CreateTable<BudgetCategory>();
                return db.Table<BudgetCategory>().ToList();
            }
        }


        public static void DeleteBudgetCategory(BudgetCategory category)
        {
            using (var db = new SQLiteConnection(dbPath))
            {
                db.Delete(category);
            }
        }

        //loads user info
        public static void SaveIncomeData(double income)  
        {
            using (var db = new SQLiteConnection(dbPath))
            {
                var userInfo = db.Table<UserInfo>().FirstOrDefault();
                if (userInfo == null)
                {
                    userInfo = new UserInfo { Income = income };
                    db.Insert(userInfo);
                }
                else
                {
                    userInfo.Income = income;
                    db.Update(userInfo);
                }
            }
        }

        //loads income 
        public static double LoadIncomeData()
        {
            using (var db = new SQLiteConnection(dbPath))
            {
                var userInfo = db.Table<UserInfo>().FirstOrDefault();

                // Log what is being retrieved
                if (userInfo == null)
                {
                    Console.WriteLine("🔍 DEBUG: No income data found in DB.");
                    return 0.0;
                }
                else
                {
                    Console.WriteLine($"🔍 DEBUG: Loaded Income from DB = {userInfo.Income}");
                    return userInfo.Income;
                }
            }
        }


        //gets user info from database
        public static async Task<UserInfo> GetUserAsync(int id)
        {
            using (var db = new SQLiteConnection(dbPath))
            {
                return db.Table<UserInfo>().FirstOrDefault(u => u.Id == id);
            }
        }

        //updates user info in database
        public static async Task UpdateUserAsync(UserInfo user)
        {
            using (var db = new SQLiteConnection(dbPath))
            {
                db.Update(user);
            }
        }

        // Saves take-home pay to the database
        public static void SaveTakeHomePay(double takeHomePay)
        {
            using (var db = new SQLiteConnection(dbPath))
            {
                var userInfo = db.Table<UserInfo>().FirstOrDefault();
                if (userInfo == null)
                {
                    userInfo = new UserInfo { TakeHomePay = takeHomePay };
                    db.Insert(userInfo);
                }
                else
                {
                    userInfo.TakeHomePay = takeHomePay;
                    db.Update(userInfo);
                }
            }
        }

        // Loads take-home pay from the database
        public static double LoadTakeHomePay()
        {
            using (var db = new SQLiteConnection(dbPath))
            {
                var userInfo = db.Table<UserInfo>().FirstOrDefault();
                return userInfo?.TakeHomePay ?? 0.0; 
            }
        }

    }
}
