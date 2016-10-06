using System;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace Moneta.Infra.CrossCutting.Datatables
{
    public class DataTablesModelBinder
    {
        public object BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            DataTablesParam obj = new DataTablesParam();
            var request = actionContext.ActionArguments;

            obj.iDisplayStart = Convert.ToInt32(request["iDisplayStart"]);
            obj.iDisplayLength = Convert.ToInt32(request["iDisplayLength"]);
            obj.iColumns = Convert.ToInt32(request["iColumns"]);
            obj.sSearch = request["sSearch"].ToString();
            obj.bEscapeRegex = Convert.ToBoolean(request["bEscapeRegex"]);
            obj.iSortingCols = Convert.ToInt32(request["iSortingCols"]);
            obj.sEcho = int.Parse(request["sEcho"].ToString());

            for (int i = 0; i < obj.iColumns; i++)
            {
                obj.bSortable.Add(Convert.ToBoolean(request["bSortable_" + i]));
                obj.bSearchable.Add(Convert.ToBoolean(request["bSearchable_" + i]));
                obj.sSearchColumns.Add(request["sSearch_" + i].ToString());
                obj.bEscapeRegexColumns.Add(Convert.ToBoolean(request["bEscapeRegex_" + i]));
                obj.iSortCol.Add(Convert.ToInt32(request["iSortCol_" + i]));
                obj.sSortDir.Add(request["sSortDir_" + i].ToString());
            }
            return obj; 
        }
    }
}