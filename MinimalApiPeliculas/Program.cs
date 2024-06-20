using Microsoft.EntityFrameworkCore;
using MinimalApiPelicula.Context;
using MinimalApiPelicula.EndPoints;
using MinimalApiPelicula.Repositorios;
using MinimalApiPelicula.Servicios;
using FluentValidation;
using MinimalApiPeliculas.Validaciones;
using MinimalApiPeliculas.Repositorios;
using Microsoft.AspNetCore.Diagnostics;
using MinimalApiPeliculas.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MinimalApiPeliculas.Utilidades;
using MinimalApiPeliculas.EndPoints;

var builder = WebApplication.CreateBuilder(args);
var origenesPermitidos = builder.Configuration.GetValue<string>("origenesPermitidos")!;
//Inicio de area de los servicios

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer("name=DefaultConnection");
});

builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<IdentityUser>>();
builder.Services.AddScoped<SignInManager<IdentityUser>>();

builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(configuracion =>
    {
        configuracion.WithOrigins(origenesPermitidos).AllowAnyHeader().AllowAnyMethod();
    });

    opciones.AddPolicy("libre", configuracion =>
    {
        configuracion.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddOutputCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRepositorioGeneros, RepositorioGenero>();
builder.Services.AddScoped<IRepositorioActores, RepositorioActores>();
builder.Services.AddScoped<IRepositorioPeliculas, RepositorioPeliculas>();
builder.Services.AddScoped<IRepositorioComentarios, RepositorioComentarios>();
builder.Services.AddScoped<IRepositorioErrores, RepositorioErrores>();


builder.Services.AddScoped<IAlmacenadorArchivo, AlmacenadorArchicosAzure>();

builder.Services.AddHttpContextAccessor();


builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();


builder.Services.AddProblemDetails();


//Configurando JWT en mi aplicacion
builder.Services.AddAuthentication().AddJwtBearer(opciones =>
{
    opciones.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true, //Validar tiempo de vida de mi token.
        ValidateIssuerSigningKey = true, //Validar que el token este firmado con su llave.
        IssuerSigningKey = Llaves.ObtenerLlave(builder.Configuration).First(), //Si quiero utilizar una unica llave. La mia.
        //IssuerSigningKeys = Llaves.ObtenerTodasLlaves(builder.Configuration)
        ClockSkew = TimeSpan.Zero //Investigar esto

    };
});

builder.Services.AddAuthorization();


//Fin de area de los servicios


var app = builder.Build();
//Inicio del area de los middeware
//if (builder.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI();

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
{
    var esceptionHanleFeatures = context.Features.Get<IExceptionHandlerFeature>();
    var excepcion = esceptionHanleFeatures?.Error!;

    var error = new Error();
    error.Fecha = DateTime.UtcNow;
    error.MensajeDeError = excepcion.Message;
    error.StackTrace = excepcion.StackTrace;

    var repositorio = context.RequestServices.GetRequiredService<IRepositorioErrores>();
    await repositorio.Crear(error);

    await TypedResults.BadRequest( new {tipo ="error", mensaje = "Ha ocurrido un error inesperado", estatus = 500 })
                    .ExecuteAsync(context);

}));
app.UseStatusCodePages();

app.UseStaticFiles();

app.UseCors();
app.UseOutputCache();

app.UseAuthorization();

//Seccion para endpoints Generos
app.MapGroup("/generos").MapGeneros();
app.MapGroup("/actores").MapActores();
app.MapGroup("/peliculas").MapPeliculas();
app.MapGroup("/peliculas/{peliculaId:int}/comentarios").MapComentarios();
app.MapGroup("/usuarios").MapUsuarios();


app.Map("/error", () =>
{
    throw new InvalidProgramException("Error de prueba");
});

//Fin del area de los middeware

app.Run();

