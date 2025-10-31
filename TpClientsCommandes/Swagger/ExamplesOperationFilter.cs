using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TpClientsCommandes.Swagger
{
    // Définit des exemples par défaut pour les corps de requête afin d'éviter les valeurs invalides
    public class ExamplesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var method = context.ApiDescription.HttpMethod?.ToUpperInvariant();
            var relPath = context.ApiDescription.RelativePath?.ToLowerInvariant() ?? string.Empty;

            if (operation.RequestBody != null && operation.RequestBody.Content.TryGetValue("application/json", out var json))
            {
                // POST /api/clients
                if (method == "POST" && relPath.StartsWith("api/clients"))
                {
                    var ex = BuildClientCreateExample();
                    json.Example = ex;
                    json.Schema.Example = ex;
                }

                // PUT /api/clients/{id}
                if (method == "PUT" && relPath.StartsWith("api/clients/"))
                {
                    var ex = BuildClientUpdateExample();
                    json.Example = ex;
                    json.Schema.Example = ex;
                }

                // POST /api/commandes
                if (method == "POST" && relPath.StartsWith("api/commandes"))
                {
                    var ex = BuildCommandeCreateExample();
                    json.Example = ex;
                    json.Schema.Example = ex;
                }

                // PUT /api/commandes/{id}
                if (method == "PUT" && relPath.StartsWith("api/commandes/"))
                {
                    var ex = BuildCommandeUpdateExample();
                    json.Example = ex;
                    json.Schema.Example = ex;
                }
            }

            // Exemples de réponses (listes et détails) pour GET et 201 Created
            foreach (var kv in operation.Responses)
            {
                var status = kv.Key;
                var response = kv.Value;
                if (!response.Content.TryGetValue("application/json", out var jsonResp))
                {
                    // Créer un conteneur JSON pour 400/404 afin d'afficher un exemple dans Swagger
                    if (status == "400" || status == "404")
                    {
                        jsonResp = new OpenApiMediaType();
                        response.Content["application/json"] = jsonResp;
                    }
                    else
                    {
                        continue;
                    }
                }

                // GET clients (liste et détail)
                if (method == "GET" && relPath.StartsWith("api/clients"))
                {
                    if (!relPath.Contains("{")) // liste
                    {
                        var ex = BuildClientsListExample();
                        jsonResp.Example = ex;
                        jsonResp.Schema.Example = ex;
                    }
                    else // détail
                    {
                        var ex = BuildClientDetailExample();
                        jsonResp.Example = ex;
                        jsonResp.Schema.Example = ex;
                    }
                }

                // GET commandes (liste et détail)
                if (method == "GET" && relPath.StartsWith("api/commandes"))
                {
                    if (!relPath.Contains("{")) // liste
                    {
                        var ex = BuildCommandesListExample();
                        jsonResp.Example = ex;
                        jsonResp.Schema.Example = ex;
                    }
                    else // détail
                    {
                        var ex = BuildCommandeDetailExample();
                        jsonResp.Example = ex;
                        jsonResp.Schema.Example = ex;
                    }
                }

                // Réponse de création 201
                if (status == "201")
                {
                    if (method == "POST" && relPath.StartsWith("api/clients"))
                    {
                        var ex = BuildClientCreatedResponseExample();
                        jsonResp.Example = ex;
                        jsonResp.Schema.Example = ex;
                    }
                    if (method == "POST" && relPath.StartsWith("api/commandes"))
                    {
                        var ex = BuildCommandeCreatedResponseExample();
                        jsonResp.Example = ex;
                        jsonResp.Schema.Example = ex;
                    }
                }

                // Réponses d'erreurs 400
                if (status == "400")
                {
                    if (method == "POST" && relPath.StartsWith("api/clients"))
                    {
                        jsonResp.Example = BuildValidationProblemExample(new Dictionary<string, string[]>
                        {
                            ["email"] = new[] { "Email invalide." },
                            ["nom"] = new[] { "Nom est requis." }
                        });
                    }
                    else if (method == "POST" && relPath.StartsWith("api/commandes"))
                    {
                        jsonResp.Example = new OpenApiObject
                        {
                            ["message"] = new OpenApiString("ClientId introuvable")
                        };
                    }
                    else if (method == "PUT" && relPath.StartsWith("api/commandes/"))
                    {
                        jsonResp.Example = BuildValidationProblemExample(new Dictionary<string, string[]>
                        {
                            ["montantTotal"] = new[] { "Le montant doit être supérieur à 0." }
                        });
                    }
                }

                // Réponses 404
                if (status == "404")
                {
                    if (relPath.StartsWith("api/clients/"))
                    {
                        jsonResp.Example = new OpenApiObject
                        {
                            ["status"] = new OpenApiInteger(404),
                            ["title"] = new OpenApiString("Not Found"),
                            ["detail"] = new OpenApiString("Client introuvable")
                        };
                    }
                    else if (relPath.StartsWith("api/commandes/"))
                    {
                        jsonResp.Example = new OpenApiObject
                        {
                            ["status"] = new OpenApiInteger(404),
                            ["title"] = new OpenApiString("Not Found"),
                            ["detail"] = new OpenApiString("Commande introuvable")
                        };
                    }
                }
            }
        }

        private static OpenApiObject BuildClientCreateExample()
        {
            return new OpenApiObject
            {
                ["nom"] = new OpenApiString("Durand"),
                ["prenom"] = new OpenApiString("Pierre"),
                ["email"] = new OpenApiString("pierre.durand@example.com"),
                ["telephone"] = new OpenApiString("0612345678"),
                ["adresse"] = new OpenApiString("10 Rue de la Paix, Paris"),
                ["commandes"] = new OpenApiArray() // vide par défaut (test simple recommandé)
            };
        }

        private static OpenApiObject BuildClientUpdateExample()
        {
            return new OpenApiObject
            {
                ["nom"] = new OpenApiString("Doe"),
                ["prenom"] = new OpenApiString("Jane"),
                ["email"] = new OpenApiString("jane.doe.modifie@example.com"),
                ["telephone"] = new OpenApiString("0102030406"),
                ["adresse"] = new OpenApiString("12 Rue des Fleurs, 75001 Paris")
            };
        }

        private static OpenApiObject BuildCommandeCreateExample()
        {
            return new OpenApiObject
            {
                ["numeroCommande"] = new OpenApiString("CMD-2025-011"),
                ["dateCommande"] = new OpenApiString("2025-10-30T15:00:00Z"),
                ["montantTotal"] = new OpenApiDouble(89.90),
                ["statut"] = new OpenApiString("Nouvelle"),
                ["clientId"] = new OpenApiInteger(1)
            };
        }

        private static OpenApiObject BuildCommandeUpdateExample()
        {
            return new OpenApiObject
            {
                ["numeroCommande"] = new OpenApiString("CMD-2025-011-MODIFIE"),
                ["dateCommande"] = new OpenApiString("2025-10-30T15:00:00Z"),
                ["montantTotal"] = new OpenApiDouble(92.50),
                ["statut"] = new OpenApiString("EnPreparation"),
                ["clientId"] = new OpenApiInteger(1)
            };
        }

        private static OpenApiArray BuildClientsListExample()
        {
            var arr = new OpenApiArray();
            arr.Add(new OpenApiObject
            {
                ["id"] = new OpenApiInteger(1),
                ["nom"] = new OpenApiString("Durand"),
                ["prenom"] = new OpenApiString("Pierre"),
                ["email"] = new OpenApiString("pierre.durand@example.com"),
                ["telephone"] = new OpenApiString("0612345678"),
                ["adresse"] = new OpenApiString("10 Rue de la Paix, Paris"),
                ["dateCreation"] = new OpenApiString("2025-10-23T09:02:55Z"),
                ["commandes"] = new OpenApiArray()
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
            });
            arr.Add(new OpenApiObject
            {
                ["id"] = new OpenApiInteger(2),
                ["nom"] = new OpenApiString("Martin"),
                ["prenom"] = new OpenApiString("Sophie"),
                ["email"] = new OpenApiString("sophie.martin@example.com"),
                ["telephone"] = new OpenApiString("0787654321"),
                ["adresse"] = new OpenApiString("5 Avenue du Test, Lyon"),
                ["dateCreation"] = new OpenApiString("2025-10-22T14:30:00Z"),
                ["commandes"] = new OpenApiArray()
            });
            return arr;
        }

        private static OpenApiObject BuildClientDetailExample()
        {
            return new OpenApiObject
            {
                ["id"] = new OpenApiInteger(1),
                ["nom"] = new OpenApiString("Doe"),
                ["prenom"] = new OpenApiString("Jane"),
                ["email"] = new OpenApiString("jane.doe@example.com"),
                ["telephone"] = new OpenApiString("0102030406"),
                ["adresse"] = new OpenApiString("12 Rue des Fleurs, 75001 Paris"),
                ["dateCreation"] = new OpenApiString("2025-10-23T09:02:55Z"),
                ["commandes"] = new OpenApiArray()
                {
                    new OpenApiObject
                    {
                        ["id"] = new OpenApiInteger(11),
                        ["numeroCommande"] = new OpenApiString("CMD-2025-0001"),
                        ["dateCommande"] = new OpenApiString("2025-10-16T09:02:55Z"),
                        ["montantTotal"] = new OpenApiDouble(75.20),
                        ["statut"] = new OpenApiString("EnCours"),
                        ["clientId"] = new OpenApiInteger(1)
                    },
                    new OpenApiObject
                    {
                        ["id"] = new OpenApiInteger(12),
                        ["numeroCommande"] = new OpenApiString("CMD-2025-0002"),
                        ["dateCommande"] = new OpenApiString("2025-10-20T09:02:55Z"),
                        ["montantTotal"] = new OpenApiDouble(110.00),
                        ["statut"] = new OpenApiString("Livree"),
                        ["clientId"] = new OpenApiInteger(1)
                    }
                }
            };
        }

        private static OpenApiArray BuildCommandesListExample()
        {
            var arr = new OpenApiArray();
            arr.Add(new OpenApiObject
            {
                ["id"] = new OpenApiInteger(21),
                ["numeroCommande"] = new OpenApiString("CMD-2025-010"),
                ["dateCommande"] = new OpenApiString("2025-10-30T14:30:00Z"),
                ["montantTotal"] = new OpenApiDouble(149.99),
                ["statut"] = new OpenApiString("En attente"),
                ["clientId"] = new OpenApiInteger(2),
                ["client"] = new OpenApiObject
                {
                    ["id"] = new OpenApiInteger(2),
                    ["nom"] = new OpenApiString("Martin"),
                    ["prenom"] = new OpenApiString("Sophie")
                }
            });
            arr.Add(new OpenApiObject
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
            });
            return arr;
        }

        private static OpenApiObject BuildCommandeDetailExample()
        {
            return new OpenApiObject
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

        private static OpenApiObject BuildClientCreatedResponseExample()
        {
            return new OpenApiObject
            {
                ["id"] = new OpenApiInteger(6),
                ["nom"] = new OpenApiString("Durand"),
                ["prenom"] = new OpenApiString("Pierre"),
                ["email"] = new OpenApiString("pierre.durand@example.com"),
                ["telephone"] = new OpenApiString("0612345678"),
                ["adresse"] = new OpenApiString("10 Rue de la Paix, Paris"),
                ["dateCreation"] = new OpenApiString("2025-10-31T12:00:00Z"),
                ["commandes"] = new OpenApiArray()
            };
        }

        private static OpenApiObject BuildCommandeCreatedResponseExample()
        {
            return new OpenApiObject
            {
                ["id"] = new OpenApiInteger(23),
                ["numeroCommande"] = new OpenApiString("CMD-2025-012"),
                ["dateCommande"] = new OpenApiString("2025-10-31T12:30:00Z"),
                ["montantTotal"] = new OpenApiDouble(129.99),
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

        private static OpenApiObject BuildValidationProblemExample(Dictionary<string, string[]> errors)
        {
            var errorsObj = new OpenApiObject();
            foreach (var kvp in errors)
            {
                var arr = new OpenApiArray();
                foreach (var msg in kvp.Value)
                    arr.Add(new OpenApiString(msg));
                errorsObj[kvp.Key] = arr;
            }
            return new OpenApiObject
            {
                ["type"] = new OpenApiString("https://tools.ietf.org/html/rfc7231#section-6.5.1"),
                ["title"] = new OpenApiString("One or more validation errors occurred."),
                ["status"] = new OpenApiInteger(400),
                ["errors"] = errorsObj
            };
        }
    }
}