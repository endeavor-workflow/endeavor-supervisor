using Endeavor.Supervisor.Messaging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using System.Text;

namespace Endeavor.Supervisor.Persistence
{
    public class SupervisorRepository : IRepository
    {
        private readonly IConnectionFactory _factory;

        public SupervisorRepository(IConnectionFactory factory)
        {
            _factory = factory;
        }

        public List<TaskToBeWorked> GetTasksByStatus(StatusType status)
        {
            string query = "SELECT t.ID AS TaskId, t.StepID, s.StepType FROM Task t INNER JOIN Step s ON s.ID = t.StepID WHERE t.StatusValue = @Status";

            var parameters = new DynamicParameters();
            parameters.Add("@Status", (int)status);

            using (var conn = _factory.Connection)
            {
                var tasks = conn.Query<TaskToBeWorked>(query, parameters, commandType: CommandType.Text);

                return tasks.AsList<TaskToBeWorked>();
            }       
        }

        public void UpdateTasksStatuses(List<long> taskIDs, StatusType status)
        {
            string tasks = string.Join(",", taskIDs);

            int statusValue = (int)status;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE Task SET StatusValue = ");
            sb.Append(statusValue.ToString());
            sb.Append(" WHERE ID in (");
            sb.Append(tasks);
            sb.Append(")");

            using (var conn = _factory.Connection)
            {
                conn.Execute(sb.ToString(), commandType: CommandType.Text);
            }
        }

        public void UpdateTaskStatus(long taskID, StatusType status)
        {
            int statusValue = (int)status;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE Task SET StatusValue = ");
            sb.Append(statusValue.ToString());
            sb.Append(" WHERE ID = ");
            sb.Append(taskID.ToString());

            using (var conn = _factory.Connection)
            {
                conn.Execute(sb.ToString(), commandType: CommandType.Text);
            }
        }
    }
}
