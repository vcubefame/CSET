//////////////////////////////// 
// 
//   Copyright 2020 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using CSETWeb_Api.BusinessLogic.Models;
using Hangfire.Server;

namespace CSETWeb_Api.Helpers
{
    public class HangfireLogger : ILogger
    {
        private PerformContext context;

        public HangfireLogger(PerformContext context)
        {
            this.context = context;
        }
    }
}

