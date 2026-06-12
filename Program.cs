var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// -------------------------------------------------------
// Base de datos simulada en memoria
// Representa una colección de nodos como en un árbol ABB/AVL
// -------------------------------------------------------
var coleccionNodos = new List<NodoElemento>
{
    new NodoElemento { Id = 10, Valor = "Raíz Inicial (ABB)" },
    new NodoElemento { Id = 5,  Valor = "Hijo Izquierdo" }
};

// -------------------------------------------------------
// ENDPOINT 1: GET /api/nodos
// Retorna todos los nodos actuales de la colección en memoria
// Código de respuesta esperado: 200 OK
// -------------------------------------------------------
app.MapGet("/api/nodos", () =>
{
    return Results.Ok(coleccionNodos);
});

// -------------------------------------------------------
// ENDPOINT 2: POST /api/nodos
// Recibe un nuevo nodo en el body (JSON) y lo inserta en la colección
// Código de respuesta esperado: 201 Created
// -------------------------------------------------------
app.MapPost("/api/nodos", (NodoElemento nuevoNodo) =>
{
    // Validación simple de los datos recibidos
    if (nuevoNodo.Id <= 0 || string.IsNullOrEmpty(nuevoNodo.Valor))
    {
        return Results.BadRequest("Datos del nodo inválidos. El Id debe ser mayor que 0 y el Valor no puede estar vacío.");
    }

    // Verificar que no exista ya un nodo con el mismo Id
    if (coleccionNodos.Any(n => n.Id == nuevoNodo.Id))
    {
        return Results.Conflict($"Ya existe un nodo con Id = {nuevoNodo.Id}.");
    }

    // Insertar el nuevo nodo en la colección
    coleccionNodos.Add(nuevoNodo);

    // Retornar 201 Created con la URL del nuevo recurso y el objeto creado
    return Results.Created($"/api/nodos/{nuevoNodo.Id}", nuevoNodo);
});

app.Run();

// -------------------------------------------------------
// Modelo: Representa un elemento que pertenecería a un árbol
// -------------------------------------------------------
public class NodoElemento
{
    public int Id { get; set; }
    public string Valor { get; set; } = string.Empty;
}
