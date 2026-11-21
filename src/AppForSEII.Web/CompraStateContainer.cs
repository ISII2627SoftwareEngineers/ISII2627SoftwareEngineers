using AppForSEII.Web.API;

namespace AppForSEII.Web
{
    /// <summary>
    /// Servicio que mantiene el carrito de compra entre páginas (select -> post -> details).
    /// Se registra con AddScoped para que todas las páginas de la sesión compartan el mismo carrito.
    /// </summary>
    public class CompraStateContainer
    {
        // El cliente generado por NSwag deja CompraItems a null, así que lo inicializamos aquí
        public CompraForCreateDTO Compra { get; private set; } = new CompraForCreateDTO()
        {
            CompraItems = new List<CompraItemDTO>()
        };

        // Añade un libro al carrito, saltando los que ya están (sin duplicados)
        public void AddLibroToCompra(LibroParaComprarDTO libro)
        {
            if (!Compra.CompraItems.Any(ci => ci.LibroID == libro.Id))
            {
                Compra.CompraItems.Add(new CompraItemDTO()
                {
                    LibroID = libro.Id,
                    Titulo = libro.Titulo,
                    Genero = libro.Genero,
                    Editorial = libro.Editorial,
                    PrecioCompra = libro.PrecioCompra
                    // Cantidad se pedirá en el paso 5 (formulario de compra)
                });
            }
            ComputeTotalPrice();
        }

        // Elimina un libro del carrito
        public void RemoveCompraItem(CompraItemDTO item)
        {
            Compra.CompraItems.Remove(item);
            ComputeTotalPrice();
        }

        // Vacía el carrito
        public void ClearCompraCart()
        {
            Compra.CompraItems.Clear();
            ComputeTotalPrice();
        }

        // Recalcula el precio total. Mientras no se haya fijado la cantidad (paso 5) se cuenta 1 unidad por libro.
        public void ComputeTotalPrice()
        {
            Compra.PrecioTotal = Compra.CompraItems
                .Sum(ci => ci.PrecioCompra * (ci.Cantidad > 0 ? ci.Cantidad : 1));
        }
        // Se llama cuando la compra ya se ha creado en la API: empieza un carrito nuevo y vacío
        public void CompraProcessed()
        {
                Compra = new CompraForCreateDTO()
            {
                CompraItems = new List<CompraItemDTO>()
            };
        }
    }
}