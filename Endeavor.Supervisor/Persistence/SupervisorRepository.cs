using Endeavor.Supervisor.Messaging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Keryhe.Persistence;

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

            var parameters = new Dictionary<string,object>();
            parameters.Add("@Status", (int)status);

            using (var conn = _factory.Connection)
            {
                List<TaskToBeWorked> tasks = new List<TaskToBeWorked>();

                var results = conn.ExecuteQuery(query, CommandType.Text, parameters);
                foreach(Dictionary<string, object> result in results)
                {
                    TaskToBeWorked task = new TaskToBeWorked(result);
                    tasks.Add(task);
                }

                return tasks;
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
                conn.ExecuteNonQuery(sb.ToString(), CommandType.Text);
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
                conn.ExecuteNonQuery(sb.ToString(), CommandType.Text);
            }
        }
    }
}
