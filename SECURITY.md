# Política de seguridad

## Versiones soportadas

Solo la última línea de la migración a WinUI 3 recibe correcciones de seguridad.
Las versiones heredadas de WinForms ya no se mantienen.

| Versión           | UI        | ¿Soportada?            |
|-------------------|-----------|------------------------|
| `0.81.x` (WinUI 3) | WinUI 3   | ✅ Sí                  |
| `0.80.x` (WinForms)| WinForms  | ⚠️ Solo críticas       |
| `< 0.80`          | WinForms  | ❌ No                  |

## Cómo reportar una vulnerabilidad

Por favor **no abras un issue público** para vulnerabilidades de seguridad.

Usa cualquiera de estos canales privados:

1. **GitHub Security Advisories** (recomendado): en este repositorio,
   pestaña **Security → Advisories → Report a vulnerability**
   ([abrir un aviso privado](https://github.com/jrtejada2k/Free1x2/security/advisories/new)).
2. **Correo**: a través de [clubprogol.com](https://clubprogol.com) (sección de contacto).

Incluye, si es posible:

- Una descripción del problema y su impacto.
- Pasos para reproducirlo (versión, plataforma, archivos de ejemplo).
- Cualquier registro o captura relevante.

## Qué esperar

- **Acuse de recibo** en un plazo de unos **7 días**.
- Una **evaluación inicial** y, si procede, un plan de corrección.
- **Divulgación coordinada**: publicaremos los detalles solo después de tener una
  versión corregida disponible, dándote crédito si así lo deseas.

Free1X2 es una herramienta de escritorio sin servicios de red propios; el ámbito
principal de seguridad cubre el procesamiento de archivos de entrada
(`.comb`, `.xml`, `.txt`, configuración) y la integridad de los binarios distribuidos.
