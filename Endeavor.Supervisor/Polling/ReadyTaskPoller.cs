using Endeavor.Supervisor.Messaging;
using Endeavor.Supervisor.Persistence;
using Keryhe.Polling;
using Keryhe.Polling.Delay;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Endeavor.Supervisor.Polling
{
    public class ReadyTaskPoller : Poller<TaskToBeWorked>
    {
        private readonly ILogger<ReadyTaskPoller> _logger;
        private readonly IRepository _repository;

        public ReadyTaskPoller(IDelay delay, IRepository repository, ILogger<ReadyTaskPoller> logger)
            : base(delay, logger)
        {
            _logger = logger;
            _repository = repository;
        }

        protected override List<TaskToBeWorked> Poll()
        {
            _logger.LogDebug("Polling for ready tasks");

            List<TaskToBeWorked> results = _repository.GetTasksByStatus(StatusType.Ready);

            return results;
        }
    }
}
