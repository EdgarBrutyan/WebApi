using Microsoft.AspNetCore.Mvc.RazorPages;
using Packt.Shared;


namespace Northwind.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddNorthwindContext(); 
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseRouting();


            app.Use(async (HttpContext context, Func<Task> next) =>
            {
                RouteEndpoint? rep = context.GetEndpoint() as RouteEndpoint;
                if (rep is not null)
                {
                    Console.WriteLine($"Endpoint name: {rep.DisplayName}");
                    Console.WriteLine($"Endpoint route pattern: {rep.RoutePattern.RawText}");
                }
                if (context.Request.Path == "/Suppliers")
                {
                    // в случае совпадения URL-пути становится возвращаемым 
                    // завершающим делегатом, поэтому следующий делегат
                    // не вызывается
                    await context.Response.WriteAsync("Bonjour Monde!");
                    return;
                }
                // можно изменить запрос перед вызовом следующего делегата
                await next();
                // можно изменить ответ после вызова следующего делегата
            });


            app.UseHttpsRedirection();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();

                endpoints.MapGet("/", context =>
                {
                    context.Response.Redirect("index"); // Перенаправить на Index.cshtml
                    return Task.CompletedTask;
                });  

                endpoints.MapGet("/hello", () => "Hello World!");
            });
        }
    }
}

