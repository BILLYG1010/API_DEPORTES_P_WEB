using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deportes.DTO.Api.Models
{
    public class biitacora_certificacion
    {
       
        public int id_bitacora { get; set; }

        public int id_factura { get; set; }

        public byte tipo_evento { get; set; }

     
        public DateTime fecha_evento { get; set; }

        public int id_usuario { get; set; }

   
        public string comentario { get; set; }

     
       
    }
}
