using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace cpsc_471_project.Logging
{
    public class SqlMessage
    {
        public SqlMessage(
            EventId eventId,
            string commandText,
            string parameters,
            CommandType commandType,
            int commandTimeout,
            string elapsed
            )
        {
            EventId = eventId;
            CommandText = commandText;
            Parameters = parameters;
            CommandType = commandType;
            Elapsed = elapsed;
            CommandTimeout = commandTimeout;
        }
        public string Elapsed { get; }
        public int CommandTimeout { get; }
        public EventId EventId { get; }
        public string CommandText { get; }
        public string Parameters { get; }
        public CommandType CommandType { get; }
    }
}
