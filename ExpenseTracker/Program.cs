using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            MainMenu();



        }

        static void MainMenu(){
            Console.WriteLine("Welcome to Expense Tracker");
            bool flag = true;
            while(flag){
                Console.WriteLine("What would you like to do? ");
                Console.WriteLine("1. Add Expense ");
                Console.WriteLine("2. View Expenses ");
                Console.WriteLine("3. Filter by Date ");
                Console.WriteLine("4. Delete Expense ");
                Console.WriteLine("5. Exit ");
                int choice = 0;

                try{
                    choice = Convert.ToInt32(Console.ReadLine());
                }catch(Exception e){
                    Console.WriteLine("Invalid Choice. Please try again");
                }

                switch (choice)
                {
                    case 1:
                        AddExpense();
                        break;
                    case 2:
                        ViewExpenses();
                        break;
                    case 3:
                        FilterByDate();
                        break;
                    case 4:
                        DeleteExpense();
                        break;
                    case 5: 
                        flag = false;
                        Console.WriteLine("Goodbye!");
                        break; 
                    default:
                        Console.WriteLine("Invalid Choice");
                        break;
                }
            }

        }
        //User provides, Amount, category, date (default to today if not given) 
        //syntax: 28.30, Groceries, 2020-10-10
        static void AddExpense(){
            bool flag = true;
            List<string> expenses = new List<string>();
            while(flag){
                Console.WriteLine("Enter the expense details in the following format: ");
                Console.WriteLine("Amount Category Date (23.50 Groceries 2020-10-10)");
                string input = Console.ReadLine();
                
                //Check if input is null or empty
                if(string.IsNullOrEmpty(input)){
                    Console.WriteLine("Invalid input. Please try again");
                    continue;
                }
                string[] expenseDetails = input.Split(' ');

                if(expenseDetails.Length != 3){
                    Console.WriteLine("Invalid input. Please try again");
                }else{
                    try {
                        
                        double amount;
                        if(!double.TryParse(expenseDetails[0], out amount)){
                            Console.WriteLine("Invalid input. Please try again");
                            continue;
                        }
                        DateTime date; 
                        if(!DateTime.TryParse(expenseDetails[2], out date)){
                            Console.WriteLine("Invalid input. Please try again");
                            continue;
                        }
                        string category = expenseDetails[1];
                        Console.WriteLine("Amount: " + amount);
                        Console.WriteLine("Category: " + category);
                        Console.WriteLine("Date: " + date);
                        Console.WriteLine("Do you want to add another expense? (Y/N)");
                        string response = Console.ReadLine();
                        expenses.Add(input);

                        if(response.ToUpper() == "N"){
                            flag = false;
                            SaveExpenses(expenses);
                        }
                    } catch (Exception e){
                        Console.WriteLine("Invalid input. Please try again");
                    }
    
                }

            }
            
        }

        static void SaveExpenses(List<string> expenses){
            string filePath = "expenses.txt"; 
            try{
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, true)){
                    foreach(string expense in expenses){
                        file.WriteLine(expense);
                    }

                }
                 Console.WriteLine("Expenses saved successfully!");


            }catch(Exception ex){
                Console.WriteLine("An error occurred while saving expenses: " + ex.Message);
            }

        }

        static void ViewExpenses(){
            string filePath = "expenses.txt";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("No expenses have been recorded yet.");
                return;
            }

            try
            {
                string[] lines = File.ReadAllLines(filePath);

                Console.WriteLine("Here are your recorded expenses:");
                Console.WriteLine("--------------------------------");

                foreach (string line in lines)
                {
                    Console.WriteLine(line);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
            }

        }

        static void DeleteExpense(){
            Console.WriteLine("Enter the expense to delete:");
            string expense = Console.ReadLine();

            string filePath = "expenses.txt";

            if (!File.Exists(filePath))
            {
                Console.WriteLine("No expenses have been recorded yet.");
                return;
            }

            try
            {
                string[] lines = File.ReadAllLines(filePath);

                List<string> newLines = new List<string>();

                foreach (string line in lines)
                {
                    if (line != expense)
                    {
                        newLines.Add(line);
                    }
                }

                //overwrite the file with the new lines
                File.WriteAllLines(filePath, newLines);

                Console.WriteLine("Expense deleted successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
            }
        }
        //want to add filtering all expenses in a given month
        static void FilterByDate(){
            Console.WriteLine("Enter the date to filter by (yyyy-mm-dd):");
            string date = Console.ReadLine();

            string filePath = "expenses.txt";

            if (!File.Exists(filePath))
            {
                Console.WriteLine("No expenses have been recorded yet.");
                return;
            }

            try
            {
                string[] lines = File.ReadAllLines(filePath);

                Console.WriteLine("Here are your recorded expenses for " + date + ":");
                Console.WriteLine("--------------------------------");

                foreach (string line in lines)
                {
                    string[] parts = line.Split(' ');

                    if (parts.Length >= 3 && parts[2] == date)
                    {
                        Console.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading the file: {ex.Message}");

            }
        }

        
    }
}