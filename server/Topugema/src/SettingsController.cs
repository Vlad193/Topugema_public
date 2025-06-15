using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

public static class SettingsController
{
    public static void RegisterMaps(WebApplication app) {
        //SERVER
        app.MapPost("api/settings/new_channel", async (HttpContext context, AppDbContext db) => {
            var form = await context.Request.ReadFormAsync();
            string? token = form["token"];
            string? rawid = form["id"];
            string? name = form["name"];
            Console.WriteLine(token + " " + rawid + " " + name);
            if (token == null || rawid == null || name == null) return Results.BadRequest("Blank Form");
            string err = "";
            if (!Validator.Name(name, out err)) return Results.BadRequest(err);
            UInt64 id = UInt64.Parse(rawid);
            User? usr = await db.Users.FirstOrDefaultAsync(u => u.token == token);
            if (usr == null) return Results.BadRequest("Invalid User");
            Server? srv = await db.Servers.FirstOrDefaultAsync(u => u.ID == id);
            if (srv == null) return Results.BadRequest("Invalid Server");
            if (srv.Owner_ID != usr.ID) return Results.BadRequest("You dont have permission to do that");
            TextChannel txt = new TextChannel { Server_ID = srv.ID, ID = await Validator.GetNewID(db), name = name }; await db.TextChannels.AddAsync(txt);
            await db.SaveChangesAsync();
            return Results.Ok();
        });
        app.MapPost("/api/settings/server_name", async (HttpContext context, AppDbContext db) => {
            var form = await context.Request.ReadFormAsync();
            string? token = form["token"];
            string? rawid = form["id"];
            string? name = form["name"];
            if (token == null || rawid == null || name == null) return Results.BadRequest("Blank Form");
            string err = "";
            if (!Validator.Name(name, out err)) return Results.BadRequest(err);
            UInt64 id = UInt64.Parse(rawid);
            User? usr = await db.Users.FirstOrDefaultAsync(u => u.token == token);
            if (usr == null) return Results.BadRequest("Invalid User");
            Server? srv = await db.Servers.FirstOrDefaultAsync(u => u.ID == id);
            if (srv == null) return Results.BadRequest("Invalid Server");
            if (srv.Owner_ID != usr.ID) return Results.BadRequest("You dont have permission to do that");
            srv.name = name;
            await db.SaveChangesAsync();
            return Results.Ok();
        });
        app.MapPost("/api/settings/leave_server", async (HttpContext context, AppDbContext db) => {
            var form = await context.Request.ReadFormAsync();
            string? token = form["token"];
            string? rawid = form["id"];
            //string? name = form["name"];
            if (token == null || rawid == null /*|| name == null*/) return Results.BadRequest("Blank Form");
            //string err = "";
            //if (!Validator.Name(name, out err)) return Results.BadRequest(err);
            UInt64 id = UInt64.Parse(rawid);
            User? usr = await db.Users.FirstOrDefaultAsync(u => u.token == token);
            if (usr == null) return Results.BadRequest("Invalid User");
            Server? srv = await db.Servers.FirstOrDefaultAsync(u => u.ID == id);
            if (srv == null) return Results.BadRequest("Invalid Server");
            if (srv.Owner_ID == usr.ID) return Results.BadRequest("You Owner and can't leave this server!");
            ServerSubscribe? sub = await db.ServerSubscribes.FirstOrDefaultAsync(u => u.User_ID == usr.ID && u.Server_ID == srv.ID);
            if (sub != null) {
                db.ServerSubscribes.Remove(sub);
                await db.SaveChangesAsync();
            }
            return Results.Ok();
        });
        app.MapPost("/api/settings/delete_server", async (HttpContext context, AppDbContext db) => {
            var form = await context.Request.ReadFormAsync();
            string? token = form["token"];
            string? rawid = form["id"];
            string? name = form["name"];
            if (token == null || rawid == null || name == null) return Results.BadRequest("Blank Form");
            UInt64 id = UInt64.Parse(rawid);
            User? usr = await db.Users.FirstOrDefaultAsync(u => u.token == token);
            if (usr == null) return Results.BadRequest("Invalid User");
            Server? srv = await db.Servers.FirstOrDefaultAsync(u => u.ID == id);
            if (srv == null) return Results.BadRequest("Invalid Server");
            if (srv.Owner_ID != usr.ID) return Results.BadRequest("Only Owner can delete this server!");
            Console.WriteLine(name + "==?" + srv.name);
            if (srv.name != name) return Results.BadRequest("Names are not equal, try again");
            List<ServerSubscribe>? subs = await db.ServerSubscribes.Where(u => u.Server_ID == srv.ID).ToListAsync();
            db.ServerSubscribes.RemoveRange(subs);
            db.Servers.Remove(srv);
            await db.SaveChangesAsync();
            return Results.Ok();
        });
        //CHANNEL
        app.MapPost("/api/settings/channel_name", async (HttpContext context, AppDbContext db) => {
            var form = await context.Request.ReadFormAsync();
            string? token = form["token"];
            string? rawid = form["chat_id"];
            string? name = form["name"];
            if (token == null || rawid == null || name == null) return Results.BadRequest("Blank Form");
            string err = "";
            if (!Validator.Name(name, out err)) return Results.BadRequest(err);
            UInt64 id = UInt64.Parse(rawid);
            User? usr = await db.Users.FirstOrDefaultAsync(u => u.token == token);
            if (usr == null) return Results.BadRequest("Invalid User");
            TextChannel? chnl = await db.TextChannels.FirstOrDefaultAsync(u => u.ID == id);
            if (chnl == null) return Results.BadRequest("Invalid Channel");
            Server? srv = await db.Servers.FindAsync(chnl.Server_ID);
            if (srv == null) return Results.BadRequest("Invalid Server");
            if (srv.Owner_ID != usr.ID) return Results.BadRequest("You dont have permission to do that");
            chnl.name = name;
            await db.SaveChangesAsync();
            return Results.Ok();
        });
        app.MapPost("/api/settings/channel_delete", async (HttpContext context, AppDbContext db) => {
            var form = await context.Request.ReadFormAsync();
            string? token = form["token"];
            string? rawid = form["chat_id"];
            if (token == null || rawid == null) return Results.BadRequest("Blank Form");
            UInt64 id = UInt64.Parse(rawid);
            User? usr = await db.Users.FirstOrDefaultAsync(u => u.token == token);
            if (usr == null) return Results.BadRequest("Invalid User");
            TextChannel? chnl = await db.TextChannels.FirstOrDefaultAsync(u => u.ID == id);
            if (chnl == null) return Results.BadRequest("Invalid Channel");
            Server? srv = await db.Servers.FindAsync(chnl.Server_ID);
            if (srv == null) return Results.BadRequest("Invalid Server");
            if (srv.Owner_ID != usr.ID) return Results.BadRequest("You dont have permission to do that");
            db.TextChannels.Remove(chnl);
            await db.SaveChangesAsync();
            return Results.Ok();
        });
        //USER
        app.MapPost("/api/settings/change_username", async (HttpContext context, AppDbContext db) => {
            var form = await context.Request.ReadFormAsync();
            string? token = form["token"];
            string? name = form["name"];
            if (token == null || name == null) return Results.BadRequest("Blank Form");
            string err = "";
            if (!Validator.Name(name, out err)) return Results.BadRequest(err);
            User? usr = await db.Users.FirstOrDefaultAsync(u => u.token == token);
            if (usr == null) return Results.BadRequest("Invalid User");
            if (await db.Users.AnyAsync(u => u.name == name)) return Results.BadRequest("Name is already taken");
            usr.name = name;
            await db.SaveChangesAsync();
            return Results.Ok();
        });
        app.MapPost("/api/settings/change_password", async (HttpContext context, AppDbContext db) => {
            var form = await context.Request.ReadFormAsync();
            string? token = form["token"];
            string? current_password = form["current_password"];
            string? password = form["password"];
            string? confirm_password = form["confirm_password"];
            if (current_password  == null || password == null || confirm_password == null) return Results.BadRequest("Blank form");
            User? usr = await db.Users.FirstOrDefaultAsync(u => u.token == token);
            if (usr == null) return Results.BadRequest("Invalid User");
            string curr_token = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(usr.ID + Authentificator.SCRAMBLE_KEY + current_password)));
            if ( usr.token != curr_token) return Results.BadRequest("Wrong current password");
            string err = "";
            if (!Validator.Password(password, out err)) return Results.BadRequest(err);
            if (password != confirm_password) return Results.BadRequest("New and Confirm passwords are not equal");
            string new_token = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(usr.ID + Authentificator.SCRAMBLE_KEY + password)));
            usr.token = new_token;
            await db.SaveChangesAsync();
            return Results.Ok();
        });
    }
    
}