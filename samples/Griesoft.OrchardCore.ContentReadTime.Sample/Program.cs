var builder = WebApplication.CreateBuilder(args);

// Local development sample only — fixed localhost port, never exposed.
builder.WebHost.UseUrls("http://localhost:5103");

builder.Services
    .AddOrchardCms()
    .AddSetupFeatures("OrchardCore.AutoSetup");

var app = builder.Build();

app.UseStaticFiles();
app.UseOrchardCore();

app.Run();
