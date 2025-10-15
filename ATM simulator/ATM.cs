using ATM_simulator;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ATMSimulator
{
    public class ATM
    {
        private static readonly Logger Logger = LogManager.GetLogger(nameof(ATM));

        private List<Account> accounts;
        private List<Transaction> transactions;
        private Account? currentAccount ;
        
        public ATM()
        {
            accounts = DataStore.LoadAccounts();
            transactions = DataStore.LoadTransactions();
            
        }

        
        public void Start()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Enter the card number:");
                    string? cardNumber = Console.ReadLine()?.Trim();
                    if(cardNumber == null)
                    {
                        Console.WriteLine("The card number cannot be empty.");
                        continue;
                    }
                    if(cardNumber.Length != 16 || !cardNumber.All(char.IsDigit))
                    {
                        Console.WriteLine("The card number must contain exactly 16 digits and only digits.");
                        Logger.Warn($"Invalid card number format: {cardNumber}");
                        continue;
                    }



                    Console.WriteLine("Enter the expiration date (MM/YY):");
                    string? expiry = Console.ReadLine()?.Trim();
                    if (expiry == null)
                        {
                        Console.WriteLine("The expiration date cannot be empty.");
                        continue;
                    }
                    if (!DateTime.TryParseExact(expiry, "MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out DateTime expDate) || expDate < DateTime.Now)
                    {
                        Console.WriteLine("Invalid validity period or card has expired.");
                        Logger.Warn($"Invalid or expired card: {cardNumber} with expiry {expiry}");
                        continue;
                    }



                    Console.WriteLine("Please Provide PIN:");
                    string? pin = Console.ReadLine()?.Trim();
                    if(pin == null)
                    {
                        Console.WriteLine("PIN cannot be empty.");
                        continue;
                    }
                    if(pin.Length != 4 || !pin.All(char.IsDigit))
                    {
                        Console.WriteLine("The PIN must contain exactly 4 digits and only digits.");
                        Logger.Warn($"Invalid PIN format for card: {cardNumber}");
                        continue;
                    }
                    currentAccount = accounts.FirstOrDefault(a => a.CardNumber == cardNumber && a.ExpiryDate == expiry && a.Pin == pin);
                    if (currentAccount == null)
                    {
                        currentAccount = new Account(cardNumber, expiry, pin, 0);
                        accounts.Add(currentAccount);
                        Console.WriteLine(accounts.Count);
                        DataStore.SaveAccounts(accounts);
                        Logger.Info($"Created new Account!");

                    }
                    


                    Logger.Info($"Successful login for card: {cardNumber}");
                    break;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Unexpected error in ATM loop.");
                    Console.WriteLine("Error! Please try again.");
                }
                
            }
            

            bool stayLoggedIn = true;
            while (stayLoggedIn)
            {
                ShowMenu();
                string? choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        Withdraw();
                        break;
                    case "2":
                        Deposit();
                        break;
                    case "3":
                        ViewBalance();
                        break;
                    case "4":
                        Last5Transactions();
                        break;
                    case "5":
                        ChangePin();
                        break;
                    case "6":
                        CurrencyConversion();
                        break;
                    case "0":
                        stayLoggedIn = false;
                        Console.WriteLine("Log out of the system.");
                        Logger.Info($"Logout for card: {currentAccount.CardNumber}");
                        break;
                    default:
                        Console.WriteLine("wrong choice.");
                        break;
                }
            }
        }

        

      
        
        private void ShowMenu()
        {
            Console.WriteLine("\nChoose an action:");
            Console.WriteLine("1. Withdraw money");
            Console.WriteLine("2.Deposit money");
            Console.WriteLine("3. View balance");
            Console.WriteLine("4. Last 5 transactions");
            Console.WriteLine("5. Change PIN");
            Console.WriteLine("6. Currency conversion");
            Console.WriteLine("0. Exit");
        }

        
        private void ViewBalance()
        {
            if (currentAccount == null)
            {
                Console.WriteLine("No account is currently logged in.");
                return;
            }
            Console.WriteLine($"your balance: {currentAccount.Balance}");
            LogTransaction("View balance", 0);
        }

        
        private void Withdraw()
        {   
            Console.WriteLine("Enter the withdrawal amount:");
            if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0 && amount <= currentAccount?.Balance)
            {
                currentAccount.Balance -= amount;
                DataStore.SaveAccounts(accounts);
                Console.WriteLine("The operation was successful.");
                LogTransaction("withdrawal of money", -amount);
            }
            else
            {
                Console.WriteLine("Incorrect amount or insufficient balance.");
            }
        }

        
        private void Deposit()
        {
            if (currentAccount == null)
            {
                Console.WriteLine("No account is currently logged in.");
                return;
            }
            Console.WriteLine("Enter the amount to deposit:");
            if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
            {
                currentAccount.Balance += amount;

                DataStore.SaveAccounts(accounts);
                Console.WriteLine("The operation was successful.");
                LogTransaction("depositing money", amount);
            }
            else
            {
                Console.WriteLine("Incorrect amount.");
            }
        }

        
        private void Last5Transactions()
        {
            if (currentAccount == null)
            {
                Console.WriteLine("No account is currently logged in.");
                return;
            }
            var last5 = transactions.Where(t => t.CardNumber == currentAccount.CardNumber)
                                    .OrderByDescending(t => t.Date)
                                    .Take(5);
            if (!last5.Any())
            {
                Console.WriteLine("No transactions found.");
                return;
            }
            foreach (var t in last5)
            {
                Console.WriteLine($"{t.Date:yyyy-MM-dd HH:mm:ss} - {t.Type}: {t.Amount}");
            }
            LogTransaction("View last 5 transactions", 0);
        }

        
        private void ChangePin()
        {
            if (currentAccount == null)
            {
                Console.WriteLine("No account is currently logged in.");
                return;
            }
            Console.WriteLine("Enter new PIN (4 digits): ");
            string? newPin = Console.ReadLine()?.Trim();
            if (newPin?.Length == 4 && int.TryParse(newPin, out _))
            {
                currentAccount.Pin = newPin;
                DataStore.SaveAccounts(accounts);
                Console.WriteLine("PIN changed successfully.");
                LogTransaction("Change PIN", 0);
            }
            else
            {
                Console.WriteLine("Invalid PIN format.");
            }
        }

        
        private void CurrencyConversion()
        {
            const decimal rate = 2.7m;
            Console.WriteLine("Enter the amount in USD: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal usd) && usd > 0)
            {
                decimal gel = usd * rate;
                Console.WriteLine($"{usd} USD = {gel} GEL (კურსი: 1 USD = {rate} GEL)");
                LogTransaction("Currency conversion", usd);
            }
            else
            {
                Console.WriteLine("Incorrect amount.");
            }
        }

        
        private void LogTransaction(string type, decimal amount)
        {
            if (currentAccount == null)
            {
                Console.WriteLine("No account is currently logged in.");
                return;
            }
            transactions.Add(new Transaction(currentAccount.CardNumber, type, amount));
            
                        

            DataStore.SaveTransactions(transactions);
            Logger.Info($"{type} for card {currentAccount.CardNumber}: {amount}");
        }
    }
}