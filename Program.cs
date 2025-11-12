using System.Text.Json.Serialization;
using Veterinaria.Web.Services;

namespace Veterinaria.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Lee la URL base de la API (appsettings.json -> "Api": { "BaseUrl": "https://localhost:5001/" })
            var apiBase = builder.Configuration["Api:BaseUrl"];
            if (string.IsNullOrWhiteSpace(apiBase))
                throw new InvalidOperationException("Config faltante: Api:BaseUrl");
            if (!apiBase.EndsWith("/")) apiBase += "/"; // asegura slash final

            // Typed HttpClients (una sola vez, sin duplicados)
            builder.Services.AddHttpClient<ClientesApiClient>(c => c.BaseAddress = new Uri(apiBase));
            builder.Services.AddHttpClient<MascotasApiClient>(c => c.BaseAddress = new Uri(apiBase));
            builder.Services.AddHttpClient<EmpleadosApiClient>(c => c.BaseAddress = new Uri(apiBase));
            builder.Services.AddHttpClient<ProcedimientoMascotasApiClient>(c => c.BaseAddress = new Uri(apiBase));

            builder.Services.AddControllersWithViews()
                .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
