using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy => policy.WithOrigins("192.168.137.1")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});
builder.Services.AddSignalR();
var app = builder.Build();
app.UseCors("AllowSpecificOrigin");

app.MapGet("/", (HttpContext context) =>
{
    context.Response.Redirect("/client/index.html");
    return Task.CompletedTask;
});

Authentificator.RegisterMaps(app);
ImageController.RegisterMaps(app);
SettingsController.RegisterMaps(app);

app.MapHub<ChatHub>("/api/chat");
var clientPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "client");
var attachmentsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "attachments");

app.UseStaticFiles(new StaticFileOptions {
    FileProvider = new PhysicalFileProvider(clientPath),
    RequestPath = "/client"
});
app.UseStaticFiles(new StaticFileOptions {
    FileProvider = new PhysicalFileProvider(attachmentsPath),
    RequestPath = "/attachments"
});
app.Run();

public class AppDbContext : DbContext {
    public DbSet<Counter> Counters { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Server> Servers { get; set; }
    public DbSet<TextChannel> TextChannels { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<ServerSubscribe> ServerSubscribes { get; set; }
    //public DbSet<Attachment> Attachments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=app.db");
    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
        var idProperty = entityType.FindProperty("ID");
        if (idProperty != null)
        {
            idProperty.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never;
        }
    }

    base.OnModelCreating(modelBuilder);
}
}

