
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using ImageMagick;
using Microsoft.AspNetCore.SignalR;

public static class ImageController
{
    public static void RegisterMaps(WebApplication app) {
        app.MapPost("/api/images/upload", async (HttpContext context, AppDbContext db, IHubContext<ChatHub> hub) => {
            var form = await context.Request.ReadFormAsync();
            string? token = form["token"];
            string? chan_rawid = form["chat_id"];
            string? description = form["desc"];
            if (token == null || chan_rawid == null) return Results.BadRequest("Got Blank Request");
            User? usr = await db.Users.FirstOrDefaultAsync(u => u.token == token);
            if (usr == null) return Results.BadRequest("Invalid User");
            UInt64 chat_id = UInt64.Parse(chan_rawid);
            TextChannel? chnl = await db.TextChannels.FirstOrDefaultAsync(u => u.ID == chat_id);
            if (chnl == null) return Results.BadRequest("Invalid Chat");
            if (description == null) description = "";
            var file = form.Files["file"];
            if (file == null || file.Length == 0) return Results.BadRequest("No file");
            UInt64 ImgID = await Validator.GetNewID(db);

            MagickImage image;
            using var stream = file.OpenReadStream();
            try {
                image = new MagickImage(stream);
            }
            catch (MagickException) {
                return Results.BadRequest("File is not image");
            }

            image.Resize(new MagickGeometry(1920, 1920) {
                IgnoreAspectRatio = false
            });
            image.Format = MagickFormat.Jpeg;
            image.Quality = 75;
            var fileName = ImgID + ".jpg";
            var attachmentsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "attachments");
            var savePath = Path.Combine(attachmentsPath, fileName);
            Directory.CreateDirectory(attachmentsPath);
            await image.WriteAsync(savePath);

            UInt64 valID = await Validator.GetNewID(db);
            Message msg = new Message { ID = valID, Channel_ID = chat_id, User_ID = usr.ID, message = description, attachment = ImgID };
            await db.Messages.AddAsync(msg);
            await db.SaveChangesAsync();
            List<ServerSubscribe> subs = await db.ServerSubscribes.Where(u => u.Server_ID == chnl.Server_ID).ToListAsync();
            foreach (ServerSubscribe sub in subs) {
                Console.WriteLine("Get Conn " + sub.User_ID + " " + sub.User_ID);
                if (ChatHub.UsrOnline.Reverse.ContainsKey(sub.User_ID)) {
                    await hub.Clients.Client(ChatHub.UsrOnline.Reverse[sub.User_ID]).SendAsync("ReceiveMessage", usr.ID, chnl.ID, valID, msg.message, ImgID);
                    //await Clients.Client(UsrOnline.Reverse[sub.User_ID]).SendAsync("Error", "RECEIVED");
                    Console.WriteLine("Conn " + sub.User_ID + " " + ChatHub.UsrOnline.Reverse[sub.User_ID]);
                }
            }
            return Results.Ok();
        });
        app.MapPost("/api/images/change_avatar", async (HttpContext context, AppDbContext db, IHubContext<ChatHub> hub) => {
            var form = await context.Request.ReadFormAsync();
            string? token = form["token"];
            if (token == null) return Results.BadRequest("Got Blank Request");
            User? usr = await db.Users.FirstOrDefaultAsync(u => u.token == token);
            if (usr == null) return Results.BadRequest("Invalid User");
            var file = form.Files["file"];
            if (file == null || file.Length == 0) return Results.BadRequest("No file");
            UInt64 ImgID = usr.ID;

            MagickImage image;
            using var stream = file.OpenReadStream();
            try {
                image = new MagickImage(stream);
            }
            catch (MagickException) {
                return Results.BadRequest("File is not image");
            }

            image.Resize(new MagickGeometry(1920, 1920) {
                IgnoreAspectRatio = false
            });
            image.Format = MagickFormat.Jpeg;
            image.Quality = 75;
            var fileName = ImgID + ".jpg";
            var attachmentsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "attachments", "avatars");
            var savePath = Path.Combine(attachmentsPath, fileName);
            Directory.CreateDirectory(attachmentsPath);
            await image.WriteAsync(savePath);
            return Results.Ok();
        });
        app.MapPost("/api/images/server_avatar", async (HttpContext context, AppDbContext db, IHubContext<ChatHub> hub) => {
            var form = await context.Request.ReadFormAsync();
            string? token = form["token"];
            string? rawid = form["id"];
            if (token == null || rawid == null) return Results.BadRequest("Got Blank Request");
            User? usr = await db.Users.FirstOrDefaultAsync(u => u.token == token);
            if (usr == null) return Results.BadRequest("Invalid User");
            UInt64 id = UInt64.Parse(rawid);
            Server? srv = await db.Servers.FirstOrDefaultAsync(u => u.ID == id);
            if (srv == null) return Results.BadRequest("Invalid Server");
            if (srv.Owner_ID != usr.ID) return Results.BadRequest("You dont have permission to do that");
            var file = form.Files["file"];
            if (file == null || file.Length == 0) return Results.BadRequest("No file");
            UInt64 ImgID = srv.ID;

            MagickImage image;
            using var stream = file.OpenReadStream();
            try {
                image = new MagickImage(stream);
            }
            catch (MagickException) {
                return Results.BadRequest("File is not image");
            }

            image.Resize(new MagickGeometry(1920, 1920) {
                IgnoreAspectRatio = false
            });
            image.Format = MagickFormat.Jpeg;
            image.Quality = 75;
            var fileName = ImgID + ".jpg";
            var attachmentsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "attachments", "avatars");
            var savePath = Path.Combine(attachmentsPath, fileName);
            Directory.CreateDirectory(attachmentsPath);
            await image.WriteAsync(savePath);
            return Results.Ok();
        });
        app.MapPost("/api/images/channel_avatar", async (HttpContext context, AppDbContext db, IHubContext<ChatHub> hub) => {
            var form = await context.Request.ReadFormAsync();
            string? token = form["token"];
            string? rawid = form["chat_id"];
            if (token == null || rawid == null) return Results.BadRequest("Got Blank Request");
            User? usr = await db.Users.FirstOrDefaultAsync(u => u.token == token);
            if (usr == null) return Results.BadRequest("Invalid User");
            UInt64 id = UInt64.Parse(rawid);
            TextChannel? chnl = await db.TextChannels.FirstOrDefaultAsync(u => u.ID == id);
            if (chnl == null) return Results.BadRequest("Invalid Channel");
            Server? srv = await db.Servers.FindAsync(chnl.Server_ID);
            if (srv == null) return Results.BadRequest("Invalid Server");
            if (srv.Owner_ID != usr.ID) return Results.BadRequest("You dont have permission to do that");
            var file = form.Files["file"];
            if (file == null || file.Length == 0) return Results.BadRequest("No file");
            UInt64 ImgID = chnl.ID;

            MagickImage image;
            using var stream = file.OpenReadStream();
            try {
                image = new MagickImage(stream);
            }
            catch (MagickException) {
                return Results.BadRequest("File is not image");
            }

            image.Resize(new MagickGeometry(1920, 1920) {
                IgnoreAspectRatio = false
            });
            image.Format = MagickFormat.Jpeg;
            image.Quality = 75;
            var fileName = ImgID + ".jpg";
            var attachmentsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "attachments", "avatars");
            var savePath = Path.Combine(attachmentsPath, fileName);
            Directory.CreateDirectory(attachmentsPath);
            await image.WriteAsync(savePath);
            return Results.Ok();
        });
        app.MapGet("/attachments/avatars/{id}.jpg", async (HttpContext context, string id, IWebHostEnvironment env) =>
        {
            var fileName = id + ".jpg";
            var avatarPath = Path.Combine(env.ContentRootPath, "..", "..", "attachments", "avatars", fileName);
            var fallbackPath = Path.Combine(env.ContentRootPath, "..", "..", "empty", "noimg.png");

            string finalPath;
            string mimeType;

            if (System.IO.File.Exists(avatarPath)) {
                finalPath = avatarPath;
                mimeType = "image/jpeg";
            } else {
                finalPath = fallbackPath;
                mimeType = "image/png";
            }

            var fileInfo = new FileInfo(finalPath);
            var lastModified = fileInfo.LastWriteTimeUtc.ToString("R"); // RFC1123
            var etag = $"\"{fileInfo.LastWriteTimeUtc.Ticks}\"";

            context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.Response.Headers["Pragma"] = "no-cache";
            context.Response.Headers["Expires"] = "0";
            context.Response.Headers["Last-Modified"] = lastModified;
            context.Response.Headers["ETag"] = etag;

            return Results.File(finalPath, mimeType);
        });


    }
    
}