
servers = {
    /*1 : "Server 1",
    2 : "Gaming",*/
}

chats = {}
nusers = {}
nchats = {}

sv_users = {
    /*1 : {
        "User1",
        "User2"
    },
    2 : {
        "User2",
        "User3"
    }*/
}

messages = {
    /*1 : {
        1 : ["User1", "Aloha"],
        2 : ["User2", "Hi"]
    },
    2 : {
        3 : ["User3", "Test1"],
        4 : ["User4", "Test2"]
    },
    3 : {
        5 : ["ProGamer1", "Lest play minecraft"],
        6 : ["Noob", "Go"]
    },
    4 : {
        7 : ["ProGamer1", "What u thing about terraria"],
        8 : ["Gamer2", "Its a nice game"]
    }*/
}

/*servers = {};
chats = {};

for(var i = 0; i<100; i++){
    servers[i] = "Test "+i;
}
for(var i = 0; i<100; i++){
    var chatList = {};
    for (var j = 0; j<i; j++){
        chatList[j] = "Chat "+j;
    }
    chats[i] = chatList;
}*/

myname = "", myid = 0;

token = sessionStorage.getItem("token");
currentChat = 0;
currentServer = 0;
//alert(token);
const conn = new signalR.HubConnectionBuilder()
    .withUrl("/api/chat")
    //.withUrl("/api/chat")
    .build();

conn.on("ReceiveMessage", ReceiveMessage);/*(user, chat_id, message_id, message) => {
    //$("#test").append(`<p>usr:${user} msg: ${message}</p>`);
    if (chat_id != currentChat) {
        console.log("WTF " + chat_id + " " + currentChat);
        return;
    }
    console.log("NO WTF " + chat_id + " " + currentChat + " " + user + " " + message_id + " " + message);
    AddTXTMessageNoImg(message_id, nusers[user], message);

    /*console.log(message);
    if (!messages[chat_id]){
        messages[chat_id] = {};
    }
    messages[chat_id][message_id] = [user, message];
    if (chat_id == currentChat){
        LoadChat(currentChat);
    }
});*/
//user, chat_id, message_id, message
conn.on("ReceiveMessages", (recMessages) => {
    recMessages.forEach(u => {
        ReceiveMessage(u.user_id, u.chat_id, u.message_id, u.message, u.attachment);
    });

});

conn.on("ReceiveMyData", (_myname, _myid, recServers, recChats, recUsers) =>{
    myname = _myname;
    myid = _myid;
    nusers[myid] = myname;
    $(".my-name").text(myname);
    $(".my-id").text(myid);
    //console.log("recServers", recServers);
    //console.log("recChats", recChats);
    //console.log("recUsers", recUsers);
    recServers.forEach(u => {
        //console.log("Server", u.id, u.name);
        ReceiveServer(u.id, u.name);
    });
    recChats.forEach(u => {
        //console.log("Chat", u.server_id, u.id, u.name);
        ReceiveChat(u.server_id, u.id, u.name);
    });
    recUsers.forEach(u => {
        //console.log("User", u.server_id, u.id, u.name);
        ReceiveUser(u.server_id, u.id, u.name);
    });
    currentServer = sessionStorage.getItem("currServ");
    if (currentServer > 0){
        LoadChats(currentServer);
    }
});

conn.on("Error", (err)=>{
    alertPanel("Server Error", err);
    if (err == "Invalid_Token"){
        location.assign("auth.html");
    }
});
conn.on("FlashMessage", (name,content)=>{
    alertPanel(name, content);
});


function ReceiveMessage(user, chat_id, message_id, message, img_id) {
    if (chat_id != currentChat) {
        //console.log("WTF " + chat_id + " " + currentChat);
        return;
    }
    //console.log("NO WTF " + chat_id + " " + currentChat + " " + user + " " + message_id + " " + message);
    AddTXTMessage(message_id, user, message, img_id);
}


function ReceiveServer(server_id, server_name) {
    servers[server_id] = server_name;
    chats[server_id] = [];
    LoadServers();
}

function ReceiveChat(server_id, chat_id, chat_name) {
    //console.log("Received Chat " + server_id + " " + chat_id);
    if (!chats[server_id]) {
        chats[server_id] = [];
    }
    chats[server_id].push(chat_id);
    nchats[chat_id] = chat_name;
}

function ReceiveUser(server_id, user_id, user_name) {
    if (!sv_users[server_id]) {
        sv_users[server_id] = [];
    }
    sv_users[server_id].push(user_id);
    nusers[user_id] = user_name;
}


function sendMessage() {
    var message = $("#send-msg-txt").val();
    $("#send-msg-txt").val("");
    conn.invoke("SendMessage", token, currentChat, message)
        .catch(err => console.error(err));
}

function createServer(serverName){
    conn.invoke("CreateServer", token, serverName)
        .catch(err => console.error(err));
}


