using crraut.Quartz;
using crraut.Services;
using crraut.SignalR;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// services
builder.Services.AddScoped<SynchronizeService>();

// logs
builder.Services.AddSingleton(new Bugsnag.Client("05e1795885afb0c8fbeb36dc1e409408"));
builder.Services.AddTransient<ErrorNotifier>();

// quartz
builder.Services.AddTransient<JobFactory>();
builder.Services.AddScoped<SyncJob>();

builder.Services.AddSignalR();

var app = builder.Build();

using(var scope = app.Services.CreateScope()) {
    var serviceProvider = scope.ServiceProvider;
    SyncScheduler.Start(serviceProvider);
}

// Configure the HTTP request pipeline.
if(!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseEndpoints(endpoints => {
    endpoints.MapHub<QuartzHub>("/quartzHub");
});
app.Run();
