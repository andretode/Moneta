using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Moneta.Infra.CrossCutting.Identity
{
    [Table("AspNetClients")]
    public class Client
    {
        [Key]
        public int Id { get; set; }

        public string ClientKey { get; set; }
    }
}
