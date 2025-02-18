using System;
using System.Collections.Generic;

namespace FinanceFrenzy.Models;

public class TaxCalculator
{
    // Maryland, Virginia, and DC state progressive tax brackets
    public static readonly Dictionary<string, List<(double, double)>> ProgressiveStateBrackets = new()
    {
        { "Maryland", new List<(double, double)>
            {
                (1000, 0.02),  
                (2000, 0.03),   
                (3000, 0.04),   
                (100000, 0.0475), 
                (125000, 0.05),  
                (150000, 0.0525), 
                (250000, 0.055),  
                (double.MaxValue, 0.0575) 
            }
        },

        { "Virginia", new List<(double, double)>
            {
                (3000, 0.02),   
                (5000, 0.03),   
                (17000, 0.05),  
                (double.MaxValue, 0.0575) 
            }
        },

        { "District of Columbia", new List<(double, double)>
            {
                (10000, 0.04),   
                (40000, 0.06),  
                (60000, 0.065),  
                (250000, 0.085), 
                (500000, 0.0925), 
                (1000000, 0.0975), 
                (double.MaxValue, 0.1075) 
            }
        }
    };


    // Calculate state tax based on progressive brackets
    private static double CalculateProgressiveStateTax(double income, string state)
    {
        

        var brackets = ProgressiveStateBrackets[state];
        double totalTax = 0.0;
        double previousBracket = 0.0;

        //go through each bracket and calculate the tax
        foreach (var (bracket, rate) in brackets)
        {
            //if above tax the entire bracket
            if (income > bracket)
            {
                totalTax += (bracket - previousBracket) * rate;
                previousBracket = bracket;
            }
            //if below tax the remaining income
            else
            {
                totalTax += (income - previousBracket) * rate;
                break;
            }
        }

        return totalTax;
    }

    // Federal progressive tax brackets for single and married 
    public static readonly Dictionary<string, List<(double, double)>> FederalTaxBrackets = new()
    {
        { "Single", new List<(double, double)>
            {
                (11600, 0.10),
                (47150, 0.12),
                (100525, 0.22),
                (191950, 0.24),
                (243725, 0.32),
                (609350, 0.35),
                (double.MaxValue, 0.37)
            }
        },

        { "Married", new List<(double, double)>
            {
                (23200, 0.10),
                (94300, 0.12),
                (201050, 0.22),
                (383900, 0.24),
                (487450, 0.32),
                (731200, 0.35),
                (double.MaxValue, 0.37)
            }
        }
    };


     public static double CalculateFederalTax(double income, string filingStatus)
    {
        var brackets = FederalTaxBrackets[filingStatus];
        double totalTax = 0.0;
        double previousBracket = 0.0;

        //uses the same logic as the state tax
        foreach (var (bracket, rate) in brackets)
        {
            if (income > bracket)
            {
                totalTax += (bracket - previousBracket) * rate;
                previousBracket = bracket;
            }
            else
            {
                totalTax += (income - previousBracket) * rate;
                break;
            }
        }
        return totalTax;
    }

    //calculates the social security tax
    public static double CalculateSocialSecurityTax(double income)
    {
        double socialSecurityLimit = 168600;
        return Math.Min(income, socialSecurityLimit) * 0.062;
    }


    public static double CalculateTakeHomePay(double income, string state, string filingStatus)
    {
        double federalTax = CalculateFederalTax(income, filingStatus);
        double stateTax = CalculateProgressiveStateTax(income, state);
        double socialSecurityTax = CalculateSocialSecurityTax(income);

        double totalTax = federalTax + stateTax + socialSecurityTax;
        double takeHomePay = income - totalTax;

        return takeHomePay;
    }
}
