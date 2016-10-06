
using System.Collections.Generic;

namespace Moneta.Infra.CrossCutting.Datatables
{
    public class DataTablesData<T>
    {
        public int sEcho { get; set; }
        public int iTotalRecords { get; set; }
        public int iTotalDisplayRecords { get; set; }
        public List<T> aaData { get; set; }

    }

}