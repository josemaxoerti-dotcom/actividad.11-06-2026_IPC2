# ApiEstructurasDemo — Instrucciones de Configuración y Uso

## Requisitos Previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado
- VS Code o Visual Studio (opcional)
- Postman, Bruno, o la extensión **REST Client** de VS Code para las pruebas

---

## Cómo ejecutar el proyecto

### Opción A — Terminal (cualquier SO)

```bash
# 1. Navegar a la carpeta del proyecto
cd ApiEstructurasDemo

# 2. Restaurar dependencias (solo la primera vez)
dotnet restore

# 3. Ejecutar la API
dotnet run
```

La API quedará disponible en: `http://localhost:5000`

---

### Opción B — Crear el proyecto desde cero con el template oficial

Si prefieres crearlo tú mismo desde la plantilla de .NET:

```bash
dotnet new webapi -o ApiEstructurasDemo --no-openapi
cd ApiEstructurasDemo
# Reemplaza Program.cs con el código de este repositorio
dotnet run
```

---

## Pruebas con Postman

### Prueba 1 — GET (Leer todos los nodos)

| Campo | Valor |
|-------|-------|
| Método | `GET` |
| URL | `http://localhost:5000/api/nodos` |
| Body | *(ninguno)* |

**Resultado esperado:** `200 OK` con este JSON:
```json
[
  { "id": 10, "valor": "Raíz Inicial (ABB)" },
  { "id": 5, "valor": "Hijo Izquierdo" }
]
```

---

### Prueba 2 — POST (Crear un nuevo nodo)

| Campo | Valor |
|-------|-------|
| Método | `POST` |
| URL | `http://localhost:5000/api/nodos` |
| Body | `raw` → `JSON` |

**Body a enviar:**
```json
{
  "id": 15,
  "valor": "Nuevo Nodo Derecho"
}
```

**Resultado esperado:** `201 Created` con el nodo creado.

---

### Prueba 3 — GET después del POST

Repite la Prueba 1. Ahora el resultado debe incluir los 3 nodos:
```json
[
  { "id": 10, "valor": "Raíz Inicial (ABB)" },
  { "id": 5, "valor": "Hijo Izquierdo" },
  { "id": 15, "valor": "Nuevo Nodo Derecho" }
]
```

---

## Pruebas con REST Client (VS Code)

1. Instala la extensión **REST Client** en VS Code.
2. Abre el archivo `pruebas.http` incluido en este proyecto.
3. Haz clic en **"Send Request"** sobre cada bloque.

---

## Estructura del Proyecto

```
ApiEstructurasDemo/
├── ApiEstructurasDemo.csproj   # Configuración del proyecto .NET 8
├── Program.cs                   # Código principal de la API (endpoints GET y POST)
├── appsettings.json             # Configuración de logging
├── pruebas.http                 # Pruebas listas para REST Client de VS Code
└── README.md                    # Este archivo
```
