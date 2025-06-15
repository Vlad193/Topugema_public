using System.Data.Common;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using BidirectionalMap;
public class ChatHub : Hub {
    private readonly AppDbContext db;

    public ChatHub(AppDbContext _db) {
        db = _db;
    }

    public static BiMap<string, UInt64> UsrOnline = new BiMap<string, UInt64>();

    public async Task SendMessage(string token, UInt64 chat_id, string message) {
        //await Clients.All.SendAsync("ReceiveMessage", user, chat_id, Validator.GetNewID(db), message);
        User? usr = await db.Users.FirstOrDefaultAsync(u => u.token == token);
        if (usr == null) { await Clients.Caller.SendAsync("Error", "Invalid Token, please login again"); return; }
        TextChannel? chnl = await db.TextChannels.FirstOrDefaultAsync(u => u.ID == chat_id);
        if (chnl == null) { await Clients.Caller.SendAsync("Error", "Acces dennied/Invalid Channel"); return; }
        if (await db.Servers.FindAsync(chnl.Server_ID) == null) { await Clients.Caller.SendAsync("Error", "Server was deleted? Reload page pls."); return; }
        UInt64 valID = await Validator.GetNewID(db);
        Message msg = new Message { ID = valID, Channel_ID = chat_id, User_ID = usr.ID, message = message , attachment = 0};
        await db.Messages.AddAsync(msg);
        await db.SaveChangesAsync();
        List<ServerSubscribe> subs = await db.ServerSubscribes.Where(u => u.Server_ID == chnl.Server_ID).ToListAsync();
        foreach (ServerSubscribe sub in subs) {
            Console.WriteLine("Get Conn " + sub.User_ID + " " + sub.User_ID);
            if (UsrOnline.Reverse.ContainsKey(sub.User_ID)) {
                await Clients.Client(UsrOnline.Reverse[sub.User_ID]).SendAsync("ReceiveMessage", usr.ID, chnl.ID, valID, msg.message, 0);
                //await Clients.Client(UsrOnline.Reverse[sub.User_ID]).SendAsync("Error", "RECEIVED");
                Console.WriteLine("Conn " + sub.User_ID + " " + UsrOnline.Reverse[sub.User_ID]);
            }
        }
    }

    public async Task GetMessages(string token, UInt64 chat_id) {
        User? usr = await db.Users.FirstOrDefaultAsync(u => u.token == token);
        if (usr == null) { await Clients.Caller.SendAsync("Error", "Invalid Token, please login again"); return; }
        TextChannel? chnl = await db.TextChannels.FirstOrDefaultAsync(u => u.ID == chat_id);
        if (chnl == null) { await Clients.Caller.SendAsync("Error", "Acces dennied/Invalid Channel"); return; }
        List<Message> msgs = await db.Messages.Where(u => u.Channel_ID == chnl.ID).OrderByDescending(u => (long)u.ID).Take(100).ToListAsync();
        msgs.Reverse();
        List<ReceiveMessage> receiveMessages = new List<ReceiveMessage>();
        foreach (Message msg in msgs) {
            receiveMessages.Add(new ReceiveMessage { user_id= msg.User_ID, chat_id = msg.Channel_ID, message_id = msg.ID, message = msg.message, attachment = msg.attachment});
            //await Clients.Caller.SendAsync("ReceiveMessage", msg.User_ID, msg.Channel_ID, msg.ID, msg.message);
        }
        await Clients.Caller.SendAsync("ReceiveMessages", receiveMessages);
    }

    public async Task GetMyData(string token) {
        //Console.WriteLine(token);
        User? usr = await db.Users.FirstOrDefaultAsync(u => u.token == token);
        if (usr != null) {
            if (UsrOnline.Reverse.ContainsKey(usr.ID)) { await Clients.Caller.SendAsync("Error", "User already online, please delog from other devices."); return; }
            UsrOnline.Add(Context.ConnectionId, usr.ID);
            List<ReceiveServer> receiveServers = new List<ReceiveServer>();
            List<ReceiveChat> receiveChats = new List<ReceiveChat>();
            List<ReceiveUser> receiveUsers = new List<ReceiveUser>();
            //await Clients.Caller.SendAsync("ReceiveMyData", usr.name, usr.ID);
            List<ServerSubscribe>? subs = await db.ServerSubscribes.Where(u => u.User_ID == usr.ID).ToListAsync();
            foreach (ServerSubscribe sub in subs) {
                Server? srv = await db.Servers.FirstOrDefaultAsync(u => u.ID == sub.Server_ID);
                if (srv != null) {
                    receiveServers.Add(new ReceiveServer { id = srv.ID, name = srv.name });
                    //await Clients.Caller.SendAsync("ReceiveServer", srv.ID, srv.name);
                    List<TextChannel>? txtchanls = await db.TextChannels.Where(u => u.Server_ID == srv.ID).ToListAsync();
                    foreach (TextChannel txtchnl in txtchanls) {
                        receiveChats.Add(new ReceiveChat { server_id= srv.ID, id = txtchnl.ID, name = txtchnl.name });
                        //await Clients.Caller.SendAsync("ReceiveChat", srv.ID, txtchnl.ID, txtchnl.name);
                    }
                    List<ServerSubscribe>? othersubs = await db.ServerSubscribes.Where(u => u.Server_ID == srv.ID).ToListAsync();
                    foreach (ServerSubscribe osub in othersubs) {
                        User? ousr = await db.Users.FirstOrDefaultAsync(u => u.ID == osub.User_ID);
                        if (ousr != null) {
                            receiveUsers.Add(new ReceiveUser { server_id= srv.ID, id = ousr.ID, name = ousr.name });
                            //await Clients.Caller.SendAsync("ReceiveUser", srv.ID, ousr.ID, ousr.name);
                        }
                    }
                }
            }
            await Clients.Caller.SendAsync("ReceiveMyData", usr.name, usr.ID, receiveServers, receiveChats, receiveUsers);

        }
        else {
            //await Clients.Caller.SendAsync("ReceiveMyData", "Invalid", -1);
            await Clients.Caller.SendAsync("Error", "Invalid_Token");
        }

        //await Clients.All.SendAsync("ReceiveMessage", user, chat_id, Validator.GetNewID(), message);
    }

    public async Task CreateServer(string token, string ServerName) {
        User? usr = await db.Users.FirstOrDefaultAsync(u => u.token == token);
        if (usr == null) { await Clients.Caller.SendAsync("Error", "Invalid Token, please login again"); return; }
        string err = "";
        if (!Validator.Name(ServerName, out err)) { await Clients.Caller.SendAsync("Error", err); return; }
        Server srv = new Server { ID = await Validator.GetNewID(db), name = ServerName, Owner_ID = usr.ID }; await db.Servers.AddAsync(srv);
        TextChannel txtc = new TextChannel { Server_ID = srv.ID, ID = await Validator.GetNewID(db), name = "Main"}; await db.TextChannels.AddAsync(txtc);
        ServerSubscribe nsub = new ServerSubscribe { ID = await Validator.GetNewID(db), Server_ID = srv.ID, User_ID = usr.ID };
        await db.ServerSubscribes.AddAsync(nsub);
        await db.SaveChangesAsync();
        await Clients.Caller.SendAsync("FlashMessage", "You created server", ServerName);
    }

    public override Task OnDisconnectedAsync(Exception? exception) {
        UsrOnline.Remove(Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }
}
