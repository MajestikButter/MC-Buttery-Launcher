using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MC_Buttery_Launcher
{
    public class Logger
    {
        public readonly string logFile;

        public Logger(string logFilePath, string logFileName) {
            logFilePath = Path.GetFullPath(logFilePath);

            if (!Directory.Exists(logFilePath))
            {
                Directory.CreateDirectory(logFilePath);
            }

            logFile = logFilePath + "/" + logFileName;
        }

        public void Info(string msg)
        {
            msg = "[Info] " + msg;
            Console.WriteLine(msg);
            File.AppendAllText(logFile, msg);
        }

        public void Error(string msg)
        {
            msg = "[Error] " + msg;
            Console.Error.WriteLine(msg);
            File.AppendAllText(logFile, msg);
        }
    }
}