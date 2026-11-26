using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AppForSEII.Web.API
{
    /*
    Esto es necesario en la API, porque CompraItemDTO.Cantidad tiene [Required] y a la vez [JsonIgnore(WhenWritingDefault)]:
      - [Required] hace que Swagger la marque como obligatoria, y NSwag la copia al cliente
        como Required.Always.
      - [JsonIgnore] hace que la API NO envíe "cantidad" cuando vale 0, que es lo que ocurre en el
        detalle de una compra (el paso 7 no muestra la cantidad).
    Resultado: el cliente exige un campo que la API no manda y lanza una excepción al leer la
    respuesta de GetDetailsForComprasAsync, aunque la API responda 200.

    SOLUCIÓN: al LEER respuestas, se hace opcional solo ese campo ("cantidad" de CompraItemDTO).
    El resto de reglas del cliente quedan igual y no hace falta tocar la API.
    */
    public partial class AppForSEIIAPIClient
    {
        static partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
        {
            settings.ContractResolver = new RelaxedContractResolver();
        }

        // La API no envía "cantidad" en el detalle de una compra, pero el cliente generado la marca como obligatoria
        private class RelaxedContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);

                /*
                "cantidad" pasa de obligatoria (Required.Always) a opcional (Required.Default)
                solo en CompraItemDTO. El resto de propiedades no se modifican.
                */
                if (property.DeclaringType == typeof(CompraItemDTO) && property.PropertyName == "cantidad")
                {
                    property.Required = Required.Default;
                }

                return property;
            }
        }
    }
}