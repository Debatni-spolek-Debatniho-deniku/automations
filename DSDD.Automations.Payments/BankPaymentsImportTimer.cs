using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DSDD.Automations.Payments
{
    public class BankPaymentsImportTimer
    {
        public BankPaymentsImportTimer(ILogger<BankPaymentsImportTimer> logger)
        {
            _logger = logger;
        }

        //[Function(nameof(BankPaymentsImportTimer))]
        //public void Run([TimerTrigger("0 0 1 * * * *")] TimerInfo myTimer)
        //{
        //    _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

        //    if (myTimer.ScheduleStatus is not null)
        //    {
        //        _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        //    }
        //}

        private readonly ILogger<BankPaymentsImportTimer> _logger;
    }
}
