using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deportes.DTO.Api.Models
{
    public  class UsuarioDTO
    {
 
        public int id_usuario { get; set; }

        public string nombre_usuario { get; set; }

        public string password_hash { get; set; }

        public int id_rol { get; set; }

        
        public ulong activo { get; set; }

        
        public DateTime creado_en { get; set; }

        public DateTime actualizado_en { get; set; }
    }
}
