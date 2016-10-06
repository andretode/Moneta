using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;
using System.Xml.XPath;
using Newtonsoft.Json;
using Moneta.Application.Interfaces;
using Moneta.Application.ViewModels;
using Moneta.Domain.Entities;

namespace Moneta.Services.WebAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ContasController : ApiController
    {
        private readonly IContaAppService _contaApp;

        public ContasController(IContaAppService contaApp)
        {
            _contaApp = contaApp;
        }

        // GET: api/Contas
        public IEnumerable<ContaViewModel> Get()
        {
            return _contaApp.GetAll();
        }

        // POST: api/Contas
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Contas/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/Contas/5
        public void Delete(int id)
        {
        }
    }

    public class SysDataTablePager<T>
    {
        public string sEcho { get; set; }
        public int iTotalRecords { get; set; }
        public int iTotalDisplayRecords { get; set; }
        public List<T> aaData { get; set; }
    }

    public class DataTablesParam
    {
        public int iDisplayStart { get; set; }
        public int iDisplayLength { get; set; }
        public int iColumns { get; set; }
        public string sSearch { get; set; }
        public bool bEscapeRegex { get; set; }
        public int iSortingCols { get; set; }
        public int sEcho { get; set; }
        public List<bool> bSortable { get; set; }
        public List<bool> bSearchable { get; set; }
        public List<string> sSearchColumns { get; set; }
        public List<int> iSortCol { get; set; }
        public List<string> sSortDir { get; set; }
        public List<bool> bEscapeRegexColumns { get; set; }

        public DataTablesParam()
        {
            bSortable = new List<bool>();
            bSearchable = new List<bool>();
            sSearchColumns = new List<string>();
            iSortCol = new List<int>();
            sSortDir = new List<string>();
            bEscapeRegexColumns = new List<bool>();
        }
    }
}