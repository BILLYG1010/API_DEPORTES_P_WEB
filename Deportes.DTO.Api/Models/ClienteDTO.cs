using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deportes.DTO.Api.Models
{
    public class ClienteDTO
    {

        public int id_cliente { get; set; }

      
        public string nit { get; set; }

       
        public string nombre { get; set; }

        
        public string direccion { get; set; }

       
        public ulong activo { get; set; }

       
        public DateTime creado_en { get; set; }

      
        public DateTime actualizado_en { get; set; }



    }
}
