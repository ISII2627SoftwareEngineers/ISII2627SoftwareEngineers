using AppForSEII.Web.API; 

namespace AppForSEII.Web 
{
    public class ResenaStateContainer 
    {
        // Creamos una instancia de ResenaForCreateDTO cuando se crea el contenedor
        public ResenaForCreateDTO Resena { get; private set; } = new ResenaForCreateDTO() 
        {
            ResenaItems = new List<ResenaItemDTO>()
        };

        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();
        
        public void AddLibroToResena(LibroParaResenarDTO libro) 
        {
            // Comprobamos si el libro ya está añadido antes de insertarlo
            if (!Resena.ResenaItems.Any(ri => ri.LibroId == libro.Id)) 
            {
                // Lo añadimos si no está en la lista guardando los datos visuales
                Resena.ResenaItems.Add(new ResenaItemDTO() 
                {
                    LibroId = libro.Id,
                    Titulo = libro.Titulo,
                    Editorial = libro.Editorial,
                    TipoLibro = libro.TipoLibro,
                    Calificacion = 0,
                    Descripcion = string.Empty
                });
                NotifyStateChanged();
            }
        }

        // Para eliminar libros de la lista de reseña
        public void RemoveResenaItem(ResenaItemDTO item) 
        {
            Resena.ResenaItems.Remove(item);
            NotifyStateChanged();
        }

        // Eliminamos todos los libros de la lista
        public void ClearResenaCart() 
        {
            Resena.ResenaItems.Clear();
            NotifyStateChanged();
        }

        // Ya hemos terminado el proceso de reseñar, creamos una nueva reseña vacía
        public void ResenaProcessed() 
        {
            Resena = new ResenaForCreateDTO() 
            {
                ResenaItems = new List<ResenaItemDTO>()
            };
            NotifyStateChanged();
        }
    }
}