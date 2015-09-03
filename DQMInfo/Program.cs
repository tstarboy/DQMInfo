using System;
using System.Collections.Generic;
using DQMInfo.Data;

namespace DQMInfo
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var processor = new Processor();
            processor.MainMenu();
            return;
        }
    }
}
