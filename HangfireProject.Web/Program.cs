using Hangfire;
using HangfireProject.Core.Common;
using HangfireProject.Core.Hangfire;
using HangfireProject.Core.Hangfire.Extensions;
using HangfireProject.Core.Repository;
using HangfireProject.Data.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IUserRepository, UserRepository>();

#region ConfigDbContext

var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
builder.Services.AddDbContext<HangfireDbContext>(options =>
{
    options.UseSqlServer(defaultConnectionString);
});

#endregion

#region Hangfire

var hangfireConnectionString = builder.Configuration.GetConnectionString("HangfireContextString");
builder.Services.AddHangfire(p =>
{
    p.UseSqlServerStorage(hangfireConnectionString);
});

builder.Services.AddHangfireServer(option =>
{
    option.ServerName = "Hangfire";
    option.Queues = [
        nameof(EnHangfireQueue.defaults),
        nameof(EnHangfireQueue.edited),
        nameof(EnHangfireQueue.created),
        nameof(EnHangfireQueue.deleted)
    ];
});

builder.Services.AddJob();

#endregion

var app = builder.Build();

var provider = builder.Services.BuildServiceProvider();
ServiceLocator.Initialize(provider);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

#region Hangfire Dashboard

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [new HangfireAuthorizationFilter()],
});

await HangfireExtensions.RegisterJob();

#endregion

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();