using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Endeavor.Supervisor.Persistence
{
    public interface IConnectionFactory
    {
        IDbConnection Connection { get; }
    }
}
