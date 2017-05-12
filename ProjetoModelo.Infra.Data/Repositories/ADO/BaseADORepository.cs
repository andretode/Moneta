using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Moneta.Infra.Data.Repositories.ADO
{
    public class BaseADORepository
    {
        public Database Connection
        {
            get
            {
                return new DatabaseProviderFactory().Create("Moneta");
            }
        }
    }
}
