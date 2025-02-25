using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace maena_se.Models
{
    public class Paciente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido paterno es obligatorio.")]
        public string ApellidoPaterno { get; set; }

        [Required(ErrorMessage = "El apellido materno es obligatorio.")]
        public string ApellidoMaterno { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El motivo de consulta es obligatorio.")]
        [StringLength(50)]
        public string MotivoConsulta { get; set; }

        public string CondicionMedica { get; set; }

        [Required(ErrorMessage = "La escuela de procedencia es obligatoria.")]
        [StringLength(100)]
        public string EscuelaProcedencia { get; set; }

        [Required(ErrorMessage = "El contacto telefónico es obligatorio.")]
        [StringLength(50)]
        public string ContactoTel { get; set; }

    }
}