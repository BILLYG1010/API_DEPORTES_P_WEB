using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deportes.DTO.Api.Models
{
    public class Factura
    {
        public int id_factura { get; set; }

        public Guid uuid { get; set; }

        public int id_cliente_emisor { get; set; }

        public int id_cliente_receptor { get; set; }

        public int id_usuario { get; set; }

     
        public DateTime fecha_emision { get; set; }

        public byte estado { get; set; }

        public ulong eliminado { get; set; }

      
        public decimal subtotal { get; set; }

  
        public decimal total { get; set; }

        public string observaciones { get; set; }

 
        public string numero_autorizacion { get; set; }

        public string serie { get; set; }

        public int? correlativo { get; set; }
    }
}
