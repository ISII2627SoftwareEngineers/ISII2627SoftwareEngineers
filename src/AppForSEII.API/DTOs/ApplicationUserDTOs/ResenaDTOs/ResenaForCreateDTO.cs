using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AppForSEII.API.DTOs
{
    public class ResenaForCreateDTO
    {
        public ResenaForCreateDTO()
        {
            ResenaItems = new List<ResenaItemDTO>();
        }

        [EmailAddress(ErrorMessage = "El formato del usuario debe ser un correo electrónico válido")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, establece el usuario (email)")]
        public string UsuarioUserName { get; set; } = "estudiante@uclm.es";

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria")]
        public string Direccion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "El título de la reseña es obligatorio")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "El título de la reseña debe tener entre 10 y 20 caracteres")]
        public string TituloResena { get; set; } = string.Empty;

        public IList<ResenaItemDTO> ResenaItems { get; set; } = new List<ResenaItemDTO>();
    }
}