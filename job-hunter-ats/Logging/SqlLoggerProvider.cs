using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cpsc_471_project.Logging
{
    public class SqlLoggerProvider : ILoggerProvider
    {
        private ILogger logger;

        public SqlLoggerProvider(ILogger logger)
        {
            this.logger = logger;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return logger;
        }

        public void Dispose()
        {

        }
    }
}
