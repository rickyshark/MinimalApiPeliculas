# MinimalApiPeliculas

## Descripción

MinimalApiPeliculas es un proyecto de API minimalista desarrollado en .NET Core. Este proyecto es parte de un curso que enseña a desarrollar Minimal APIs con ASP.NET Core desde cero. A lo largo del curso, aprenderás a crear, desarrollar y desplegar un Web API, y podrás utilizar este proyecto como parte de tu portafolio.

## Características

- **Creación de Web APIs REST**: Desarrollo de endpoints para la manipulación de recursos.
- **Base de Datos**: Uso de Entity Framework Core para leer, insertar, actualizar y editar datos.
- **Sistema de Usuarios**: Registro y autenticación de usuarios utilizando Json Web Tokens (JWT).
- **Autorización Basada en Claims**: Control de acceso a endpoints específicos.
- **Caché**: Implementación de caché para mejorar el rendimiento de la aplicación.
- **Redis**: Uso de caché distribuido con Redis.
- **GraphQL**: Permite a los clientes especificar exactamente lo que desean consultar.

## Requisitos Previos

- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (o cualquier otro servidor de base de datos compatible)
- [Azure Account](https://azure.microsoft.com/en-us/free/) (para despliegue en Azure)
- [Azure DevOps Account](https://azure.microsoft.com/en-us/services/devops/) (para CI/CD)

## Configuración del Proyecto

1. **Clonar el Repositorio**
   
2. **Configurar la Base de Datos**

   Actualiza la cadena de conexión en `appsettings.json` con los detalles de tu servidor de base de datos.

   
3. **Aplicar Migraciones**

   Ejecuta los siguientes comandos para aplicar las migraciones y crear la base de datos.

   
