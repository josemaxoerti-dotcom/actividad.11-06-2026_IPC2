# Reporte de Laboratorio: Arquitectura Multi-Nivel (N-Tier) y Patrón MVC en .NET

**Curso:** Introducción a la Programación y Computación 2
**Actividad:** Arquitectura Multi-Nivel (N-Tier) y Patrón Lógico de Software (MVC) en .NET
**Modalidad:** Individual

---

## Parte 1: Fundamentación Teórica y Análisis Crítico

### 1. El Tránsito hacia los Sistemas Distribuidos y Multi-Capa

#### La Limitación del Monolito Local

Cuando la interfaz, la lógica de negocio y el almacenamiento de datos residen exclusivamente en una sola máquina física, el sistema queda atado a los límites de ese único equipo. La sincronización se vuelve un problema porque no existe una fuente central de verdad accesible para otros usuarios o procesos: cada copia local del programa maneja su propia versión de los datos, lo que genera inconsistencias si dos personas intentan trabajar sobre la misma información al mismo tiempo. La escalabilidad también se ve comprometida, ya que la única forma de atender más carga de trabajo es aumentar los recursos de esa máquina (escalado vertical), lo cual tiene un techo físico y económico. Además, si esa máquina falla, se pierde simultáneamente el acceso a la interfaz, a la lógica y a los datos, porque los tres componentes comparten el mismo punto de fallo.

#### Distinción Crítica (Layers vs. Tiers)

Las **Capas Lógicas (Layers)** son una forma de organizar el código fuente según su responsabilidad (presentación, lógica de negocio, acceso a datos), pero esa organización es puramente conceptual y puede convivir perfectamente dentro de un mismo proceso o ejecutable. Los **Niveles Físicos (Tiers)** se refieren a la distribución real del software sobre máquinas, procesos o servidores distintos, conectados por una red. En otras palabras, las capas son una decisión de diseño de software, mientras que los niveles son una decisión de despliegue de infraestructura: un sistema puede tener tres capas lógicas perfectamente separadas en el código y, sin embargo, ejecutarse todas en un solo nivel físico (un único servidor), o bien distribuir esas mismas tres capas en tres servidores físicos distintos.

#### Responsabilidades en la Arquitectura de 3 Niveles

- **Nivel 1: Capa de Presentación (Presentation Tier).** Su misión exclusiva es la interacción directa con el usuario: capturar las acciones que realiza (clics, formularios) y mostrarle resultados en un formato comprensible. No debe contener lógica de negocio ni acceso directo a la base de datos. Tecnologías comunes: navegadores web ejecutando HTML/CSS/JavaScript, aplicaciones Razor o React, o clientes móviles.
- **Nivel 2: Capa de Aplicación o Negocio (Application Tier).** Es el nivel encargado de procesar las reglas del negocio: validaciones, cálculos, flujos de autorización y orquestación de las operaciones solicitadas por la presentación. Tecnologías comunes: servidores de aplicación como ASP.NET Core, Node.js o Spring Boot, ejecutando la lógica en un servidor intermedio.
- **Nivel 3: Capa de Datos (Data Tier).** Su responsabilidad exclusiva es el almacenamiento persistente y la integridad de la información. Tecnologías comunes: motores de bases de datos relacionales (SQL Server, PostgreSQL, MySQL) o no relacionales (MongoDB), ejecutándose en un servidor dedicado.

#### Seguridad Perimetral

Exponer públicamente el puerto de una base de datos a internet es un error crítico porque convierte al motor de datos en un objetivo directo de ataques de fuerza bruta, escaneo de vulnerabilidades y explotación de fallos conocidos del propio gestor de base de datos, sin que exista ninguna capa intermedia que filtre, valide o registre esas peticiones. Una base de datos no fue diseñada para defenderse de tráfico hostil de internet; fue diseñada para responder eficientemente a consultas, confiando en que solo clientes autorizados pueden alcanzarla. La buena práctica recomendada es ubicar el servidor de datos en una red privada o subred aislada (sin IP pública), permitiendo el acceso únicamente desde la capa de aplicación a través de la red interna, complementado con reglas de firewall, credenciales robustas y, cuando sea necesario, una VPN o un *bastion host* para administración remota.

### 2. Desacoplamiento Lógico con el Patrón MVC

#### La Crisis del Código Espagueti

Cuando sentencias SQL, lógica matemática y etiquetas visuales conviven en un mismo archivo físico, cualquier cambio mínimo —por ejemplo ajustar un estilo visual— obliga a tocar un archivo que también contiene reglas de negocio y consultas a la base de datos, multiplicando el riesgo de introducir errores no relacionados con el cambio original. El mantenimiento se vuelve costoso porque entender una sola responsabilidad exige leer y comprender simultáneamente las otras tres. El diseño de pruebas unitarias se vuelve prácticamente imposible, ya que no se puede aislar la lógica de negocio para probarla sin antes simular un navegador, una conexión a base de datos y un motor de renderizado al mismo tiempo: la prueba termina siendo una prueba de todo el sistema, no de una unidad específica.

#### Separación de Preocupaciones (SoC)

El patrón formulado por Trygve Reenskaug divide la aplicación en tres componentes con responsabilidades estrictamente aisladas:

- **Modelo.** Representa los datos y las reglas de negocio del dominio (en este laboratorio, la clase `Estudiante`). No debe conocer absolutamente nada sobre cómo esos datos se presentarán al usuario; no contiene HTML, ni lógica de formato, ni referencias a la vista. Esto permite que el mismo modelo pueda reutilizarse para mostrarse en una página web, una API o un reporte impreso sin modificar una sola línea.
- **Vista.** Se define como una entidad pasiva e inteligente porque su única función es recibir datos ya procesados y transformarlos en una representación visual; no toma decisiones de negocio ni modifica datos por cuenta propia. Tiene estrictamente prohibido contener lógica de negocio, sentencias SQL o cálculos complejos: solo debe iterar, formatear y mostrar lo que el controlador le entregó.
- **Controlador.** Actúa como intermediario táctico entre la petición HTTP entrante y el resto del sistema: interpreta la solicitud del usuario, decide qué necesita el Modelo, le pide los datos, y selecciona la Vista adecuada para presentarlos. Es el director de orquesta de la petición, pero no toca directamente la base de datos ni construye HTML; únicamente coordina.

#### Métricas de Ingeniería de Software

El patrón MVC ayuda a alcanzar **Alta Cohesión** porque cada componente agrupa exclusivamente responsabilidades relacionadas entre sí (el Modelo solo se ocupa de datos y reglas de negocio, la Vista solo de presentación, el Controlador solo de orquestación), lo que facilita entender y modificar cada pieza de forma independiente. Simultáneamente logra un **Bajo Acoplamiento**, ya que los tres componentes se comunican mediante contratos simples (objetos de datos, llamadas a métodos) en lugar de depender de los detalles internos de implementación de los demás; esto permite reemplazar, por ejemplo, el motor de renderizado de vistas sin afectar al Modelo, o cambiar el origen de datos sin afectar a la Vista. En un entorno de desarrollo profesional, esta combinación reduce el costo de mantenimiento, facilita las pruebas automatizadas y permite que distintos integrantes de un equipo trabajen en paralelo sobre componentes distintos sin generar conflictos constantes.

---

## Parte 2: Modelado del Ciclo de Vida y Enrutamiento Semántico

### 1. Mapeo Analítico de URLs

Plantilla de enrutamiento por defecto: `{controller=Home}/{action=Index}/{id?}`

| URL Entrante del Cliente | Clase Controladora Buscada por el Framework | Método (Acción) Ejecutado | Parámetro `id` Inyectado |
|---|---|---|---|
| `https://ingenieria.usac.edu.gt/ControlAcademico/Login` | `ControlAcademicoController` | `Login` | (Ninguno) |
| `https://ingenieria.usac.edu.gt/Estudiante/Historial/20260123` | `EstudianteController` | `Historial` | `20260123` |
| `https://ingenieria.usac.edu.gt/Asignacion/Detalle/10` | `AsignacionController` | `Detalle` | `10` |
| `https://ingenieria.usac.edu.gt/Home` | `HomeController` | `Index` (valor por defecto de la plantilla) | (Ninguno / Opcional) |

### 2. Diagramación del Flujo Interactivo

1. **Clic del usuario en el navegador.** El usuario interactúa con un elemento de la interfaz (un botón o un enlace) en la Capa de Presentación. El navegador construye una petición HTTP (GET o POST) dirigida a una URL específica del servidor.
2. **Llegada al middleware de enrutamiento.** La petición llega al servidor ASP.NET Core y atraviesa el pipeline de middlewares hasta `UseRouting()`, donde el motor de enrutamiento compara la URL recibida contra la plantilla registrada (`{controller}/{action}/{id?}`) para determinar qué Controlador y qué Acción deben atender la solicitud.
3. **Intervención del Controlador.** Se instancia la clase controladora correspondiente (por ejemplo, `EstudianteController`) y se ejecuta el método de acción identificado. El Controlador interpreta los parámetros recibidos, realiza validaciones perimetrales rápidas y solicita al Modelo la información necesaria, sin construir HTML ni ejecutar SQL directamente.
4. **Consulta y respuesta del Modelo.** El Modelo (o la capa de acceso a datos que este representa) procesa la solicitud del Controlador y devuelve los objetos de dominio resultantes (por ejemplo, una lista de objetos `Estudiante`). El Controlador recibe estos datos limpios, listos para ser mostrados, sin haber realizado ningún cálculo de presentación.
5. **Renderizado por la Vista y respuesta al cliente.** El Controlador invoca `return View(modelo)`, entregando los datos a la Vista seleccionada. El motor de vistas Razor combina la plantilla `.cshtml` con los datos del modelo y genera el HTML final. Este HTML viaja de regreso a través del pipeline de middlewares hasta el navegador del usuario, que lo renderiza dinámicamente en pantalla.

---

## Parte 5: Referencias Bibliográficas

- Facultad de Ingeniería, USAC. (2026). *Sesión 11: Modelado Base y Arquitecturas de Despliegue. Evolución de Sistemas Distribuidos, Fundamentos del Modelo Cliente-Servidor y Diseño Físico Multi-Capas (N-Tier)*. Laboratorio del curso Introducción a la Programación y Computación 2. Guatemala.
- Facultad de Ingeniería, USAC. (2026). *Sesión 12: Arquitectura y Componentes del Patrón MVC. Desacoplamiento Lógico de Software, Ciclo de Vida de las Peticiones y Enrutamiento en Aplicaciones Interactivas Modernas*. Laboratorio del curso Introducción a la Programación y Computación 2. Guatemala.
