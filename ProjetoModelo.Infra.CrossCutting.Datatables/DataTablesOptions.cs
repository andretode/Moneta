using System.Collections.Generic;

namespace Moneta.Infra.CrossCutting.Datatables
{
    public class DataTablesOptions
    {
        /// <summary>
        /// Allows mapping Database field names to translated properties.
        /// </summary>
        public Dictionary<string, string> SearchAliases { get; set; }

        public DataTablesOptions()
        {
            SearchAliases = new Dictionary<string, string>();
        }
    }    
}