using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM_simulator
{
    
    
        public class Account
        {
            public string CardNumber { get; set; }
            public string ExpiryDate { get; set; }
            public string Pin { get; set; }
            public decimal Balance { get; set; } 
            public Account(string cardNumber, string expiryDate, string pin, decimal balance)
            {
                CardNumber = cardNumber;
                ExpiryDate = expiryDate;
                Pin = pin;
                Balance = balance;
            }
        }

        public class Transaction
        {   
            public string CardNumber { get; set; }
            public string Type { get; set; }
            public decimal Amount { get; set; } 
            public DateTime Date { get; set; } 

            public Transaction(string cardNumber, string type, decimal amount)
            {
                Date = DateTime.Now; 
                CardNumber = cardNumber;
                Type = type;
                Amount = amount;

            }
        }

        
}

