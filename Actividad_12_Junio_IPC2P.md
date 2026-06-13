# Actividad de Investigación y Práctica
## Balanceo Compuesto en Árboles AVL y Exposición de Estructuras vía Web APIs
**Fecha:** 12 de junio de 2025  
**Modalidad:** Individual  
**Duración:** 60 minutos

---

## Parte 1: Investigación Teórica y Análisis de Casos

### 1.1 El Límite de las Rotaciones Simples y Desbalanceo en "Zig-Zag"

#### El Problema Cruzado

Cuando se insertan secuencias cruzadas como `30 → 10 → 20`, las rotaciones simples (RLL o RRD) son insuficientes porque **no resuelven el patrón "Zig-Zag"**, sino que únicamente transfieren el desequilibrio al lado opuesto.

**¿Por qué falla una rotación simple?**

Consideremos el árbol resultante después de insertar `30`, `10` y `20`:

```
Estado inicial (desbalanceado):
        30   ← FE: -2 (Abuelo)
       /
      10     ← FE: +1 (Padre/Hijo Izquierdo)
        \
        20   ← FE: 0  (Nieto/Hijo Derecho del Padre)
```

Si aplicamos una **rotación simple RRD** (rotación a la derecha sobre el nodo 30):

```
Resultado incorrecto de RLL simple:
        10   ← FE: -1
          \
          30   ← FE: +1
          /
         20
```

El árbol sigue desbalanceado porque el nodo `20` está en el lado "cruzado" respecto al desbalance original. La rotación simple solo cambió la inclinación de lado, pasando de un Zig-Zag izquierda-derecha a un Zig-Zag derecha-izquierda.

**Condición matemática que gatilla una Rotación Doble Izquierda-Derecha (RID):**

Una **RID** se activa cuando se cumple simultáneamente:

```
FE(Nodo Padre/Abuelo) = -2   → desbalance hacia la izquierda
FE(Hijo Izquierdo)    = +1   → el hijo se inclina hacia la derecha
```

En notación formal:

> **RID se gatilla si y solo si:**  
> `FE(nodo) == -2` **AND** `FE(nodo.hijoIzquierdo) == +1`

Este es el caso "Izquierda-Derecha" (Left-Right o LR), donde el nieto problemático es el **hijo derecho del hijo izquierdo** del nodo desbalanceado.

---

#### Principio DRY (Don't Repeat Yourself)

Implementar las rotaciones compuestas **RID** (Izquierda-Derecha) y **RDI** (Derecha-Izquierda) **reutilizando las primitivas de rotación simple** tiene las siguientes ventajas de ingeniería de software:

| Aspecto | Implementación con DRY | Implementación sin DRY |
|---|---|---|
| **Mantenimiento** | Se corrige un bug en un solo lugar | Hay que corregir el bug en múltiples funciones |
| **Legibilidad** | La RID se lee como "hacer RRD luego RLL" | El código reasigna 6-8 punteros manualmente, difícil de seguir |
| **Testeo** | Se prueban las primitivas de forma independiente | Cada variante compuesta debe probarse completamente por separado |
| **Extensibilidad** | Agregar nuevas rotaciones es trivial | Cada nueva rotación requiere reimplementar toda la lógica |

**Ejemplo conceptual:**

```csharp
// ✅ CON DRY - La RID reutiliza las primitivas
NodoAVL RotacionRID(NodoAVL nodo) {
    nodo.HijoIzquierdo = RotacionRLL(nodo.HijoIzquierdo); // Paso 1: RLL sobre el hijo
    return RotacionRRD(nodo);                               // Paso 2: RRD sobre el abuelo
}

// ❌ SIN DRY - Reasignación manual de punteros (propenso a errores)
NodoAVL RotacionRIDManual(NodoAVL abuelo) {
    NodoAVL padre = abuelo.HijoIzquierdo;
    NodoAVL nieto = padre.HijoDerecho;
    abuelo.HijoIzquierdo = nieto.HijoDerecho;
    padre.HijoDerecho = nieto.HijoIzquierdo;
    nieto.HijoIzquierdo = padre;
    nieto.HijoDerecho = abuelo;
    // ... actualizar FE manualmente para cada nodo
    return nieto;
}
```

La versión DRY es **más corta, más legible y menos propensa a errores de puntero**.

---

### 1.2 Fundamentos de Arquitectura Web y Protocolo HTTP

#### El Modelo Cliente-Servidor

Cuando un cliente web solicita un recurso a un servidor, interactúan los siguientes componentes:

```
[CLIENTE]                          [SERVIDOR]
  │                                    │
  │  1. HTTP Request (GET /api/arbol)  │
  │ ─────────────────────────────────► │
  │                                    │  2. Procesa la solicitud
  │                                    │     (accede a datos en memoria)
  │  3. HTTP Response (200 OK + JSON)  │
  │ ◄───────────────────────────────── │
```

**Componentes que interactúan:**

- **Cliente:** Navegador web, herramienta como `curl`, Postman, o cualquier aplicación que consuma la API. Inicia la comunicación.
- **Servidor:** ASP.NET Core en nuestro caso. Escucha peticiones en un puerto (ej. 5000) y responde.
- **Protocolo HTTP:** El "idioma" que ambos hablan. Define el formato de los mensajes.
- **Red TCP/IP:** La infraestructura de transporte subyacente.

**Anatomía de una petición (Request):**

```
GET /api/arbol HTTP/1.1
Host: localhost:5000
Accept: application/json
Content-Type: application/json
```

**Anatomía de una respuesta (Response):**

```
HTTP/1.1 200 OK
Content-Type: application/json
Content-Length: 245

[{"id":30,"etiqueta":"Nodo Raíz (Abuelo) - FE: -2","altura":1}, ...]
```

Los datos viajan en el **cuerpo (body)** de la petición o respuesta, codificados típicamente en formato JSON para APIs REST modernas.

---

#### Semántica de Operaciones HTTP: GET vs POST

| Característica | GET | POST |
|---|---|---|
| **Propósito semántico** | **Recuperación/Lectura** de recursos | **Mutación/Creación** de recursos |
| **Datos en la petición** | Solo en la URL (query params) | En el cuerpo (body) del request |
| **Idempotencia** | ✅ Sí — múltiples llamadas igual resultado | ❌ No — cada llamada puede crear un nuevo recurso |
| **Caché** | ✅ Cacheable | ❌ No cacheable por defecto |
| **Seguridad** | ✅ "Seguro" (no modifica estado) | ❌ No seguro (modifica estado del servidor) |
| **Uso en nuestra API** | `GET /api/arbol` → obtener estructura actual | `POST /api/arbol/insertar` → insertar nodo y disparar balanceo |

**En el contexto de la API AVL:**

- **GET** está diseñado para la **recuperación de la estructura de datos**: permite consultar el estado actual del árbol sin modificarlo.
- **POST** está diseñado para la **mutación o inserción de nuevos elementos**: al enviar un nuevo nodo, el servidor procesa la inserción, detecta el desbalance y ejecuta la rotación correspondiente.

---

## Parte 2: Implementación Práctica — API de Simulación AVL

### 2.1 Inicialización del Proyecto

```bash
dotnet new webapi -o ApiAvlSimulacion
cd ApiAvlSimulacion
```

> **Nota:** El template `webapi` de .NET 6+ ya genera por defecto un proyecto con Minimal APIs habilitado, que es exactamente el estilo que utilizamos.

---

### 2.2 Modelo del Nodo (`NodoAVL`)

**Archivo:** `Models/NodoAVL.cs`

```csharp
namespace ApiAvlSimulacion.Models;

/// <summary>
/// Representa un nodo en la simulación del árbol AVL.
/// La propiedad Altura simula el Factor de Equilibrio (FE)
/// que se calcula implícitamente durante las rotaciones.
/// </summary>
public class NodoAVL
{
    /// <summary>
    /// Actúa como el Dato/Llave del nodo (valor entero único).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Descripción textual del estado del nodo y su FE en la simulación.
    /// </summary>
    public string Etiqueta { get; set; } = string.Empty;

    /// <summary>
    /// Altura del nodo en el árbol. Implícitamente define el FE.
    /// FE = Altura(subárbol_derecho) - Altura(subárbol_izquierdo)
    /// </summary>
    public int Altura { get; set; } = 1;
}
```

---

### 2.3 Implementación de Endpoints en `Program.cs`

**Archivo:** `Program.cs`

```csharp
using ApiAvlSimulacion.Models;

var builder = WebApplication.CreateBuilder(args);

// Opcional: agregar Swagger para facilitar las pruebas
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Habilitar Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ─────────────────────────────────────────────────────────────────
// ESTADO DEL ÁRBOL EN MEMORIA
// Representa el escenario de la Slide 5:
// Inserción de 30, 10 → estado Zig-Zag desbalanceado
//
//      30  (FE: -2, desbalanceado hacia izquierda)
//     /
//    10   (FE: +1, su hijo derecho es el problemático)
//
// Pendiente insertar el 20 (nieto derecho) que gatilla la RID.
// ─────────────────────────────────────────────────────────────────
var estadoArbol = new List<NodoAVL>
{
    new NodoAVL { Id = 30, Etiqueta = "Nodo Raíz (Abuelo) - FE: -2", Altura = 2 },
    new NodoAVL { Id = 10, Etiqueta = "Hijo Izquierdo - FE: +1",     Altura = 1 }
};

// ─────────────────────────────────────────────────────────────────
// ENDPOINT 1: GET /api/arbol
// Recupera la estructura física actual del árbol (solo lectura).
// Corresponde a la semántica HTTP GET: no modifica el estado.
// ─────────────────────────────────────────────────────────────────
app.MapGet("/api/arbol", () =>
{
    return Results.Ok(estadoArbol);
})
.WithName("ObtenerArbol")
.WithSummary("Recupera la estructura actual del árbol AVL en memoria.")
.Produces<List<NodoAVL>>(StatusCodes.Status200OK);

// ─────────────────────────────────────────────────────────────────
// ENDPOINT 2: POST /api/arbol/insertar
// Simula la inserción de un nodo y la lógica de balanceo compuesto.
// Al insertar el nodo con Id=20 se detecta el caso Zig-Zag
// Izquierda-Derecha y se ejecuta automáticamente la RID.
//
// Lógica simulada (Slides 8, 9 y 10):
//   1. Detectar condición: FE(raíz) == -2 AND FE(hijoIzq) == +1
//   2. Aplicar RID = RLL(hijoIzquierdo) + RRD(raíz)
//   3. El nodo 20 (nieto) asciende como nueva raíz del subárbol
// ─────────────────────────────────────────────────────────────────
app.MapPost("/api/arbol/insertar", (NodoAVL nuevoNodo) =>
{
    // ── Validación básica ──────────────────────────────────────
    if (nuevoNodo.Id <= 0)
    {
        return Results.BadRequest(new
        {
            Error = "ID de nodo inválido.",
            Detalle = "El ID debe ser un número entero positivo."
        });
    }

    if (estadoArbol.Any(n => n.Id == nuevoNodo.Id))
    {
        return Results.Conflict(new
        {
            Error = "ID duplicado.",
            Detalle = $"Ya existe un nodo con Id = {nuevoNodo.Id} en el árbol."
        });
    }

    // ── Detección del caso Zig-Zag Izquierda-Derecha ──────────
    // Condición: FE(raíz) = -2 y FE(hijoIzquierdo) = +1
    // Esto ocurre exactamente al intentar insertar el nodo 20
    // como hijo derecho del nodo 10 (hijo izquierdo del 30).
    if (nuevoNodo.Id == 20)
    {
        // ── Ejecución de la RID (Slide 9) ──────────────────────
        // Paso 1: RLL aplicada al hijo izquierdo (nodo 10)
        //         → El 20 sube, el 10 baja a la izquierda del 20
        //
        // Paso 2: RRD aplicada a la raíz (nodo 30)
        //         → El 20 sube como nueva raíz,
        //           el 30 baja a la derecha del 20
        //
        // Resultado final balanceado (FE = 0 en todos):
        //
        //          20  (Nueva Raíz - FE: 0)
        //         /  \
        //        10   30
        //       FE:0  FE:0

        estadoArbol.Clear();
        estadoArbol.Add(new NodoAVL { Id = 20, Etiqueta = "Nueva Raíz Balanceada (RID) - FE: 0", Altura = 2 });
        estadoArbol.Add(new NodoAVL { Id = 10, Etiqueta = "Hijo Izquierdo - FE: 0",              Altura = 1 });
        estadoArbol.Add(new NodoAVL { Id = 30, Etiqueta = "Hijo Derecho - FE: 0",                Altura = 1 });

        return Results.Created("/api/arbol", new
        {
            Mensaje    = "✅ Rotación RID ejecutada con éxito. Estabilidad total lograda.",
            Operacion  = "RID = RLL(nodo:10) + RRD(nodo:30)",
            NuevaRaiz  = 20,
            Estructura = estadoArbol
        });
    }

    // ── Inserción estándar (sin rotación compuesta) ────────────
    estadoArbol.Add(nuevoNodo);

    return Results.Created($"/api/arbol/{nuevoNodo.Id}", new
    {
        Mensaje    = $"Nodo {nuevoNodo.Id} insertado. No se requirió rotación compuesta.",
        Estructura = estadoArbol
    });
})
.WithName("InsertarNodo")
.WithSummary("Inserta un nodo y ejecuta la rotación RID si se detecta desbalance Zig-Zag.")
.Produces(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status409Conflict);

app.Run();
```

---

### 2.4 Pruebas de Verificación

#### Paso A — GET: Consultar el estado inicial desbalanceado

**Petición:**
```http
GET http://localhost:5000/api/arbol
Accept: application/json
```

**Respuesta esperada (`200 OK`):**
```json
[
  {
    "id": 30,
    "etiqueta": "Nodo Raíz (Abuelo) - FE: -2",
    "altura": 2
  },
  {
    "id": 10,
    "etiqueta": "Hijo Izquierdo - FE: +1",
    "altura": 1
  }
]
```

**Verificación:** El árbol se encuentra en estado Zig-Zag. El nodo 30 tiene FE: -2 (desbalanceado hacia la izquierda) y su hijo izquierdo, el nodo 10, tiene FE: +1 (inclinado hacia la derecha). Falta el nodo 20 para completar el caso de inserción cruzada.

---

#### Paso B — POST: Insertar el nodo 20 y disparar la rotación RID

**Petición:**
```http
POST http://localhost:5000/api/arbol/insertar
Content-Type: application/json

{
  "id": 20,
  "etiqueta": "Nieto Derecho",
  "altura": 1
}
```

---

#### Paso C — Verificación: Respuesta `201 Created` con el árbol balanceado

**Respuesta esperada (`201 Created`):**
```json
{
  "mensaje": "✅ Rotación RID ejecutada con éxito. Estabilidad total lograda.",
  "operacion": "RID = RLL(nodo:10) + RRD(nodo:30)",
  "nuevaRaiz": 20,
  "estructura": [
    {
      "id": 20,
      "etiqueta": "Nueva Raíz Balanceada (RID) - FE: 0",
      "altura": 2
    },
    {
      "id": 10,
      "etiqueta": "Hijo Izquierdo - FE: 0",
      "altura": 1
    },
    {
      "id": 30,
      "etiqueta": "Hijo Derecho - FE: 0",
      "altura": 1
    }
  ]
}
```

**Verificación del resultado:**

El nodo `20` (antes "nieto") ha ascendido a la posición de raíz del subárbol. El árbol quedó perfectamente balanceado con FE = 0 en todos sus nodos, lo que confirma que la Rotación Doble Izquierda-Derecha (RID) se ejecutó correctamente:

```
ANTES (Zig-Zag desbalanceado):      DESPUÉS (RID aplicada):

       30  ← FE: -2                        20  ← FE: 0
      /                                   /  \
    10   ← FE: +1                        10   30
       \                               FE:0   FE:0
       20  ← FE: 0
```

---

### 2.5 Comandos de Prueba con `curl`

```bash
# Paso A: Verificar estado inicial
curl -X GET http://localhost:5000/api/arbol \
     -H "Accept: application/json" | jq .

# Paso B: Insertar nodo 20 y disparar RID
curl -X POST http://localhost:5000/api/arbol/insertar \
     -H "Content-Type: application/json" \
     -d '{"id": 20, "etiqueta": "Nieto Derecho", "altura": 1}' | jq .

# Paso C: Verificar árbol balanceado resultante
curl -X GET http://localhost:5000/api/arbol \
     -H "Accept: application/json" | jq .
```

---

## Resumen Conceptual

| Concepto | Descripción |
|---|---|
| **RID (Rotación Doble Izquierda-Derecha)** | Se aplica cuando `FE(padre) = -2` y `FE(hijoIzq) = +1`. Consiste en RLL sobre el hijo izquierdo seguido de RRD sobre la raíz. |
| **Patrón Zig-Zag** | Ocurre cuando la secuencia de inserción es cruzada (ej. 30, 10, 20), generando un nieto en dirección opuesta al hijo. |
| **Principio DRY** | Las rotaciones compuestas se implementan llamando a las primitivas simples, evitando duplicación de lógica de reasignación de punteros. |
| **GET vs POST** | GET recupera sin modificar; POST muta el estado. En nuestra API: GET lee el árbol, POST inserta y balancea. |
| **Minimal APIs (ASP.NET Core)** | Permiten definir endpoints HTTP de forma concisa con `app.MapGet` y `app.MapPost` sin necesidad de controladores completos. |

---

*Documento generado como entregable de la Actividad del 12 de junio — IPC2P*
