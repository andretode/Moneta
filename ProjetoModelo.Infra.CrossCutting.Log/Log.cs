using System;

namespace Moneta.Infra.CrossCutting.Log
{
    public class Log
    {
        public int LogId { get; set; }
        public string LogType { get; set; }
        public string Action { get; set; }
        public string User { get; set; }
        public string Machine { get; set; }
        public string IP { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
