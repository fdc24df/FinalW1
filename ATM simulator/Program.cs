using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATMSimulator
{
    class Program
    {   
        static void Main(string[] args)
        {
            LoggerSetup.Initialize();
            ATM atm = new ATM();
            atm.Start();
            
            
        }
    }
}