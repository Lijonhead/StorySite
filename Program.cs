namespace StorySite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddAuthentication("CookieAuthentication")
                            .AddCookie("CookieAuthentication", options =>
                            {
                                options.LoginPath = "/Account/Login"; // Redirect here if not logged in
                                options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect here for unauthorized access
                                options.Cookie.Name = "StorySiteAuth"; // Cookie name
                                options.ExpireTimeSpan = TimeSpan.FromHours(1); // Expiry time for the cookie
                                options.SlidingExpiration = true; // Renew cookie on activity

                            });
            builder.Services.AddHttpClient("StoryPromptAPI", client =>
            {
                client.BaseAddress = new Uri("https://storypromptaprememberthis.azurewebsites.net/"); // Replace with your API base URL
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            });

            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); 
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
