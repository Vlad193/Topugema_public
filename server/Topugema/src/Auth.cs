
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

public static class Authentificator
{
    public const string SCRAMBLE_KEY = "1234_5678_90-="; // USED FOR TOKEN GENERATION
    public static void RegisterMaps(WebApplication app) {
        app.MapPost("/api/auth/log", async (HttpContext context, AppDbContext db) => {
            var form = await context.Request.ReadFormAsync();
            string? name = form["name"];
            string? password = form["password"];
            if (name == null || password == null) return Results.BadRequest("Got Blank Password Or Login");

            User? usr = await db.Users.FirstOrDefaultAsync(u => u.name == name);
            if (usr == null) return Results.BadRequest("Invalid User");
            string token = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(usr.ID + SCRAMBLE_KEY + password)));
            if (usr == null || usr.token != token) return Results.BadRequest("Wrong name or password");

            return Results.Ok(token);
        });

        app.MapPost("/api/auth/reg", async (HttpContext context, AppDbContext db) => {
            var form = await context.Request.ReadFormAsync();
            string? name = form["name"];
            string? password = form["password"];
            if (name == null || password == null) return Results.BadRequest("Got Blank Password Or Login");
            string err = "";
            if (!Validator.Name(name, out err)) return Results.BadRequest(err);
            if (!Validator.Password(password, out err)) return Results.BadRequest(err);
            if (await db.Users.AnyAsync(u => u.name == name)) return Results.BadRequest("Name is already taken");
            UInt64 UsrID = await Validator.GetNewID(db);
            string token = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(UsrID + SCRAMBLE_KEY + password)));
            User usr = new User() { ID = UsrID, name = name, token = token, verifed = false };
            await db.Users.AddAsync(usr);
            await db.SaveChangesAsync();

            return Results.Ok(token);
        });
        app.MapPost("api/invite/data", async (UInt64 id, AppDbContext db) => {
            //Console.WriteLine(id);
            Server? srv = await db.Servers.FirstOrDefaultAsync(u => u.ID == id);
            if (srv == null) return Results.BadRequest("Invalid Server");
            return Results.Ok(srv.name);

        });
        app.MapPost("api/invite", async (HttpContext context, AppDbContext db) => {
            var form = await context.Request.ReadFormAsync();
            string? token = form["token"];
            string? rawid = form["id"];
            if (token == null || rawid == null) return Results.BadRequest("Blank Form");
            UInt64 id = UInt64.Parse(rawid);
            User? usr = await db.Users.FirstOrDefaultAsync(u => u.token == token);
            if (usr == null) return Results.BadRequest("Invalid User");
            Server? srv = await db.Servers.FirstOrDefaultAsync(u => u.ID == id);
            if (srv == null) return Results.BadRequest("Invalid Server");
            ServerSubscribe? sub = await db.ServerSubscribes.FirstOrDefaultAsync(u => u.User_ID == usr.ID && u.Server_ID == srv.ID);
            if (sub != null) return Results.BadRequest("You already on this server");
            ServerSubscribe nsub = new ServerSubscribe { ID = await Validator.GetNewID(db), Server_ID = srv.ID, User_ID = usr.ID };
            await db.ServerSubscribes.AddAsync(nsub);
            await db.SaveChangesAsync();
            return Results.Ok();
        });
        app.MapGet("init", async (AppDbContext db) => {
            Server srv1 = new Server { ID = await Validator.GetNewID(db), name = "Server 1", Owner_ID = 0 }; await db.Servers.AddAsync(srv1);
            Server srv2 = new Server { ID = await Validator.GetNewID(db), name = "Games", Owner_ID = 0 }; await db.Servers.AddAsync(srv2);
            TextChannel txt1 = new TextChannel { Server_ID = srv1.ID, ID = await Validator.GetNewID(db), name = "Chat1" }; await db.TextChannels.AddAsync(txt1);
            TextChannel txt2 = new TextChannel { Server_ID = srv1.ID, ID = await Validator.GetNewID(db), name = "Chat2" }; await db.TextChannels.AddAsync(txt2);
            TextChannel txt3 = new TextChannel { Server_ID = srv2.ID, ID = await Validator.GetNewID(db), name = "Minecraft" }; await db.TextChannels.AddAsync(txt3);
            TextChannel txt4 = new TextChannel { Server_ID = srv2.ID, ID = await Validator.GetNewID(db), name = "Terraria" }; await db.TextChannels.AddAsync(txt4);
            await db.SaveChangesAsync();
            return Results.Ok("Default Servers Created IDs: " + srv1.ID + ", " + srv2.ID);
        });
    }
    
}