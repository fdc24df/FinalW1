using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using NLog;

namespace ATM_simulator
{
    public static class DataStore
    {
        private static readonly Logger Logger = LogManager.GetLogger(nameof(DataStore));

        private static string AccountsFile = "data/accounts.json";
        private static string TransactionsFile = "data/transactions.json";

        public static List<Account> LoadAccounts()
        {
            try
            {
                if (!File.Exists(AccountsFile))
                {
                    Logger.Warn("Accounts file not found, returning empty list.");
                    return new List<Account>();
                }
                var json = File.ReadAllText(AccountsFile);
                return JsonSerializer.Deserialize<List<Account>>(json) ?? new List<Account>();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error loading accounts.");
                return new List<Account>();
            }
        }

        public static void SaveAccounts(List<Account> accounts)
        {
            try
            {
                var json = JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true });
                Directory.CreateDirectory(Path.GetFullPath("data"));
                File.WriteAllText(AccountsFile, json);
                Console.WriteLine("SAVED!!!");
                Console.WriteLine(AccountsFile, json);
                Logger.Info("Accounts saved successfully.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error saving accounts.");
            }
        }

        public static List<Transaction> LoadTransactions()
        {
            try
            {
                if (!File.Exists(TransactionsFile))
                {
                    Logger.Warn("Transactions file not found, returning empty list.");
                    return new List<Transaction>();
                }
                var json = File.ReadAllText(TransactionsFile);
                return JsonSerializer.Deserialize<List<Transaction>>(json) ?? new List<Transaction>();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error loading transactions.");
                return new List<Transaction>();
            }
        }

        public static void SaveTransactions(List<Transaction> transactions)
        {
            try
            {
                var json = JsonSerializer.Serialize(transactions, new JsonSerializerOptions { WriteIndented = true });
                Directory.CreateDirectory("data");
                File.WriteAllText(TransactionsFile, json);
                Logger.Info("Transactions saved successfully.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error saving transactions.");
            }
        }
    }
}