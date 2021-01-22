using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace cpsc_471_project.Logging
{
    public class SqlLogger : ILogger
    {
        private const string PATH = @"..\logs\sql_log.txt";

        public SqlLogger()
        {
            if (!Directory.Exists(@"..\logs"))
            {
                Directory.CreateDirectory(@"..\logs");
            }
        }

        ~SqlLogger()
        {
            
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return default;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            // ID of important SQL events
            if (eventId != 20101)
            {
                return;
            }

            if (state is IReadOnlyList<KeyValuePair<string, object>> valueList)
            {
                string sql = valueList.FirstOrDefault(k => k.Key == "commandText").Value.ToString();
                var regex = new Regex(@"job_hunter_ats\.Controllers\.\w+\.\w+\(.*\)");
                var matches = regex.Match(Environment.StackTrace);
                
                if (matches.Length > 0)
                {
                    Match match = matches.NextMatch();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(match.Value);

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(sql);

                    Console.WriteLine();

                    using (StreamWriter sw = File.AppendText(PATH))
                    {
                        sw.WriteLine(match.Value);
                        sw.WriteLine(sql);
                        sw.WriteLine();
                    }
                }
            }
        }
    }
}
