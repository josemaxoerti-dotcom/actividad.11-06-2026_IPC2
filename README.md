# ControlAcademicoMvc

Proyecto ASP.NET Core 8 (MVC) construido para el laboratorio de Arquitectura Multi-Nivel (N-Tier) y Patrón MVC.

## Estructura

```
ControlAcademicoMvc/
├── Controllers/
│   ├── EstudianteController.cs   (Skinny Controller: Listar, Registrar)
│   └── HomeController.cs         (atiende la ruta por defecto /Home/Index)
├── Models/
│   └── Estudiante.cs             (POCO de dominio)
├── Views/
│   ├── Home/Index.cshtml
│   ├── Home/Error.cshtml
│   ├── Estudiante/Listar.cshtml
│   ├── Shared/_Layout.cshtml
│   ├── _ViewImports.cshtml
│   └── _ViewStart.cshtml
├── wwwroot/css/site.css
├── Program.cs
└── ControlAcademicoMvc.csproj
```

## Cómo ejecutar (requiere .NET 8 SDK instalado)

```bash
cd ControlAcademicoMvc
dotnet restore
dotnet run
```

La aplicación abrirá en `http://localhost:5080/`. Rutas disponibles para la auditoría de la Parte 4:

- `GET /` o `/Home/Index` → página de bienvenida.
- `GET /Estudiante/Listar` → tabla con los estudiantes simulados en memoria.
- `POST /Estudiante/Registrar` (cuerpo JSON, por ejemplo desde Postman):

```json
{
  "carne": 2026099,
  "nombre": "Ana López",
  "promedio": 88.2
}
```

## Verificación de antipatrones (Parte 4)

Ambos métodos de `EstudianteController` se mantienen muy por debajo del límite de 20 líneas
por método: `Listar()` solo despacha la lista en memoria hacia la vista, y `Registrar()`
solo valida los datos de entrada y delega el almacenamiento, sin mezclar SQL en texto plano
ni cálculos de presentación dentro del controlador.
