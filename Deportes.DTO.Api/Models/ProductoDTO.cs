using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deportes.DTO.Api.Models
{
    public class ProductoDTO
    {
        public int id_producto { get; set; }

    
        public string sku { get; set; }

   
        public string nombre { get; set; }

        public string descripcion { get; set; }

        public decimal precio_unitario { get; set; }

        public int cantidad { get; set; }

      
        public ulong activo { get; set; }

        public DateTime creado_en { get; set; }

        public DateTime actualizado_en { get; set; }

    }
}
