using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.ReposicionDTOs
{
    public class ReposicionForCreateDTO
    {
        public ReposicionForCreateDTO(string usuarioUserName, string nombre, string apellidos, string telefono, int metodoPagoId, string? comentario, IList<ReposicionItemDTO> reposicionItems)
        {
            UsuarioUserName = usuarioUserName ?? throw new ArgumentNullException(nameof(usuarioUserName));
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Apellidos = apellidos ?? throw new ArgumentNullException(nameof(apellidos));
            Telefono = telefono ?? throw new ArgumentNullException(nameof(telefono));
            MetodoPagoId = metodoPagoId;
            Comentario = comentario; 
            ReposicionItems = reposicionItems ?? throw new ArgumentNullException(nameof(reposicionItems));
        }

        public ReposicionForCreateDTO()
        {
            ReposicionItems = new List<ReposicionItemDTO>();
        }

        [EmailAddress(ErrorMessage = "El formato del usuario debe ser un correo electrónico válido")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, establece el usuario (email)")]
        public string UsuarioUserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "El método de pago es obligatorio")]
        public int MetodoPagoId { get; set; }

        [StringLength(100, MinimumLength = 20, ErrorMessage = "El comentario debe tener entre 20 y 100 caracteres")]
        public string? Comentario { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Debes incluir al menos un libro en la reposición")]
        public IList<ReposicionItemDTO> ReposicionItems { get; set; }
    

    public double PrecioTotal
        {
            get
            {
                // LINQ .Sum(): Recorre cada ítem (ci), multiplica su precio por la cantidad y suma todos los subtotales
                return ReposicionItems.Sum(ci => ci.PrecioReposicion * ci.Cantidad); //ci==reposicion item
            }
    }
}
}