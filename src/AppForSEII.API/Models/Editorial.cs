using System.ComponentModel.DataAnnotations; //necesario para poder usar las etiquetas de los atributos de la base de datos(en este caso para Id y Nombre)

namespace AppForSEII.API.Models
{
    public class Editorial
    {
        /*
        lo que hay entre corchetes es solo para 
        indicarle a la base de datos que restricciones 
        tiene cada atributo cuando los cree    
        */

        //get (coger): permite leer el valor que tiene el atributo
        //set (poner): permite modificarlo o asignarle un valor nuevo

        [Key] //es para decir que Id es la Clave Primaria
        public int Id { get; set; } //Identificador de la editorial

        [Required] //Para decir que Nombre es un campo obligatorio de rellenar(no puede ser NULL)
        [StringLength(50)] //Para decir que Nombre tiene un máximo de 50 caracteres
        public string Nombre { get; set; } //Nombre de la editorial
    }
}