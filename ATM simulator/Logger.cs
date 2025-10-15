using NLog;

namespace ATMSimulator
{
    public static class LoggerSetup
    {
        public static Logger? Logger { get; private set; }


        public static void Initialize()
        {   
            LogManager.Setup().LoadConfigurationFromFile("Nlog.config");
            
            Logger = LogManager.GetLogger("ATMSimulator");
        }
    }
}