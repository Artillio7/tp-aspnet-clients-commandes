using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TpClientsCommandes.Models;

namespace TpClientsCommandes.Swagger
{
    // Fournit des exemples réalistes au niveau des schémas pour "Schema" et "Example Value"
    public class SchemaExamplesFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = context.Type;

            if (type == typeof(Client))
            {
                schema.Example = new OpenApiObject
                {
                    ["id"] = new OpenApiInteger(1),
                    ["nom"] = new OpenApiString("Durand"),
                    ["prenom"] = new OpenApiString("Pierre"),
                    ["email"] = new OpenApiString("pierre.durand@example.com"),
                    ["telephone"] = new OpenApiString("0612345678"),
                    ["adresse"] = new OpenApiString("10 Rue de la Paix, Paris"),
                    ["dateCreation"] = new OpenApiString("2025-10-23T09:02:55Z"),
                    ["commandes"] = new OpenApiArray
                    {
                        new OpenApiObject
                        {
                            ["id"] = new OpenApiInteger(10),
                            ["numeroCommande"] = new OpenApiString("CMD-2025-0001"),
                            ["dateCommande"] = new OpenApiString("2025-10-16T09:02:55Z"),
                            ["montantTotal"] = new OpenApiDouble(75.20),
                            ["statut"] = new OpenApiString("EnCours"),
                            ["clientId"] = new OpenApiInteger(1)
                        }
                    }
                };
            }
            else if (type == typeof(Commande))
            {
                schema.Example = new OpenApiObject
                {
                    ["id"] = new OpenApiInteger(22),
                    ["numeroCommande"] = new OpenApiString("CMD-2025-011"),
                    ["dateCommande"] = new OpenApiString("2025-10-30T15:00:00Z"),
                    ["montantTotal"] = new OpenApiDouble(89.90),
                    ["statut"] = new OpenApiString("Nouvelle"),
                    ["clientId"] = new OpenApiInteger(1),
                    ["client"] = new OpenApiObject
                    {
                        ["id"] = new OpenApiInteger(1),
                        ["nom"] = new OpenApiString("Durand"),
                        ["prenom"] = new OpenApiString("Pierre")
                    }
                };
            }
        }
    }
}