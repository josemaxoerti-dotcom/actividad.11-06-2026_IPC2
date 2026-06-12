# Actividad de Investigación y Práctica: Estructuras de Datos Avanzadas y APIs con ASP.NET Core

**Modalidad:** Individual  
**Fecha:** 11 de Junio de 2026

---

## Parte 1: Investigación Teórica

### 1. Estructuras de Datos Eficientes

#### Árbol Binario de Búsqueda (ABB)

Un **Árbol Binario de Búsqueda (ABB)** es una estructura de datos jerárquica que sigue la siguiente regla de ordenamiento:

- **Hijo izquierdo:** Contiene un valor **menor** que el nodo padre.
- **Hijo derecho:** Contiene un valor **mayor** que el nodo padre.

Esta propiedad se cumple de forma recursiva para cada nodo del árbol, lo que permite realizar búsquedas eficientes dividiendo el espacio de búsqueda a la mitad en cada nivel.

**Principal desventaja — Degeneración en lista vinculada:**

Cuando los datos se insertan en **orden secuencial** (por ejemplo: 1, 2, 3, 4, 5...), el árbol se degenera en una estructura lineal, similar a una lista vinculada. Esto ocurre porque cada nuevo nodo siempre se inserta como hijo derecho del anterior, generando un árbol completamente desbalanceado. En este caso degenerado, la complejidad de búsqueda pasa de O(log n) a **O(n)**, anulando la ventaja principal del ABB.

```
Inserción: 10 → 20 → 30 → 40
Resultado degenerado:
10
  \
  20
    \
    30
      \
      40   ← Se convierte en una lista
```

---

#### Árbol AVL

Un **Árbol AVL** (Adelson-Velsky y Landis) es un árbol binario de búsqueda **auto-balanceado**, lo que significa que se reorganiza automáticamente después de cada inserción o eliminación para mantener una altura equilibrada.

**Factor de Balanceo:**

El factor de balanceo de un nodo se calcula como:

```
Factor = Altura_Izquierda - Altura_Derecha
```

Un árbol AVL garantiza que el factor de balanceo de **cada nodo** sea siempre uno de estos valores: **{-1, 0, 1}**. Si después de una operación algún nodo presenta un factor fuera de ese rango (por ejemplo, 2 o -2), el árbol realiza **rotaciones** (simples o dobles) para restaurar el equilibrio.

**¿Por qué se mantiene O(log n)?**

Al garantizar que la diferencia de alturas entre los subárboles izquierdo y derecho nunca supera 1, el árbol AVL asegura que su altura total sea siempre proporcional a **log₂(n)**. Esto implica que las operaciones de **búsqueda, inserción y eliminación** nunca recorren más de O(log n) nodos, independientemente del orden en que se hayan insertado los datos.

---

### 2. Fundamentos de Web APIs

#### ¿Qué es una API y cómo funciona el modelo Cliente-Servidor?

Una **API (Application Programming Interface)** es un conjunto de reglas y definiciones que permite a diferentes aplicaciones comunicarse entre sí. En el contexto de Web APIs (REST), expone funcionalidades o datos de un servidor para que puedan ser consumidos por clientes externos.

**Modelo Cliente-Servidor — Flujo de una petición HTTP:**

```
[CLIENTE]                          [SERVIDOR]
   |                                    |
   |--- Request (HTTP) ---------------->|
   |    Método: GET                     |
   |    URL: /api/nodos                 |
   |    Headers: Content-Type, etc.     |
   |    Body: (en POST, datos JSON)     |
   |                                    |
   |<-- Response (HTTP) ----------------|
   |    Status Code: 200 OK             |
   |    Headers: Content-Type: JSON     |
   |    Body: [...datos en JSON...]     |
```

1. El **cliente** (navegador, Postman, app móvil) envía una **Request** HTTP al servidor, indicando el método (verbo), la URL del recurso y opcionalmente un cuerpo con datos.
2. El **servidor** procesa la petición, ejecuta la lógica correspondiente (consultar una base de datos, crear un recurso, etc.) y devuelve una **Response** HTTP con un código de estado y el resultado.

---

#### Verbos HTTP: GET y POST

| Verbo | Propósito | Idempotente | Envía Body |
|-------|-----------|-------------|------------|
| **GET** | Recuperar recursos existentes | ✅ Sí | ❌ No |
| **POST** | Crear un nuevo recurso | ❌ No | ✅ Sí |

**GET — Recuperación de recursos:**

El verbo `GET` se usa para **solicitar datos** del servidor sin modificar ningún estado. Es **idempotente**, lo que significa que realizar la misma petición GET múltiples veces siempre producirá el mismo resultado sin efectos secundarios. Ejemplo: `GET /api/nodos` devuelve la lista de todos los nodos.

**POST — Creación de nuevos recursos:**

El verbo `POST` se usa para **enviar datos al servidor** con la intención de crear un nuevo recurso. **No es idempotente**: ejecutar la misma petición POST varias veces generalmente crea múltiples recursos distintos. El cuerpo (body) de la petición contiene los datos del nuevo recurso en formato JSON. Ejemplo: `POST /api/nodos` con un body JSON crea un nuevo nodo en la colección.

---

## Parte 2: Implementación Práctica en C# con ASP.NET Core

El código fuente de la API se encuentra en el repositorio adjunto. A continuación se presenta el código completo del archivo `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Base de datos simulada en memoria
var coleccionNodos = new List<NodoElemento>
{
    new NodoElemento { Id = 10, Valor = "Raíz Inicial (ABB)" },
    new NodoElemento { Id = 5, Valor = "Hijo Izquierdo" }
};

// 1. GET: Retorna todos los nodos actuales
app.MapGet("/api/nodos", () => Results.Ok(coleccionNodos));

// 2. POST: Recibe un nuevo nodo y lo "inserta" en la colección
app.MapPost("/api/nodos", (NodoElemento nuevoNodo) =>
{
    if (nuevoNodo.Id <= 0 || string.IsNullOrEmpty(nuevoNodo.Valor))
    {
        return Results.BadRequest("Datos del nodo inválidos.");
    }
    coleccionNodos.Add(nuevoNodo);
    return Results.Created($"/api/nodos/{nuevoNodo.Id}", nuevoNodo);
});

app.Run();

public class NodoElemento
{
    public int Id { get; set; }
    public string Valor { get; set; } = string.Empty;
}
```

---

## Parte 3: Verificación y Pruebas

### Prueba 1 — GET `/api/nodos`

**Petición:**
```
GET http://localhost:5000/api/nodos
```

**Resultado esperado (200 OK):**
```json
[
  { "id": 10, "valor": "Raíz Inicial (ABB)" },
  { "id": 5, "valor": "Hijo Izquierdo" }
]
```

> ✅ Se recibe código `200 OK` y el arreglo JSON con los dos nodos iniciales.

---

### Prueba 2 — POST `/api/nodos`

**Petición:**
```
POST http://localhost:5000/api/nodos
Content-Type: application/json

{
  "id": 15,
  "valor": "Nuevo Nodo Derecho"
}
```

**Resultado esperado (201 Created):**
```json
{
  "id": 15,
  "valor": "Nuevo Nodo Derecho"
}
```

> ✅ Se recibe código `201 Created`. Al ejecutar nuevamente el GET, el nuevo nodo aparece en la lista.

---

### Prueba 3 — GET después del POST

**Resultado esperado (200 OK):**
```json
[
  { "id": 10, "valor": "Raíz Inicial (ABB)" },
  { "id": 5, "valor": "Hijo Izquierdo" },
  { "id": 15, "valor": "Nuevo Nodo Derecho" }
]
```

> ✅ El nodo insertado con POST ahora aparece en la colección.

---

*Actividad individual — IPC2P, Junio 2026*
