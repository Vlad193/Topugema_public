
function escapeHtml(text) {
  return text.replace(/[&<>"']/g, function(match) {
    switch (match) {
      case "&": return "&amp;";
      case "<": return "&lt;";
      case ">": return "&gt;";
      case '"': return "&quot;";
      case "'": return "&#039;";
    }
  });
}

function AddServer(id){
    let name = servers[id];
    $("#server-container").append(`
        <div class="server-card" onclick="LoadChats(${id})">
            <img src="/attachments/avatars/${id}.jpg" alt="Avatar" onerror="this.onerror=null; this.src='noimg.png';">
        </div>`);
}
function ResetServers(){
    $("#server-container").empty();
}


function AddChat(id){
    let name = nchats[id];
    $("#chat-container").append(`
        <div class="chat-card" onclick="LoadChat(${id})">
            <div class="chat-card-img-wrap">
                <img src="/attachments/avatars/${id}.jpg" alt="Avatar" onerror="this.onerror=null; this.display='none';">
            </div>
            <div class="chat-card-content">
                <h2>${name}</h2>
            </div>
        </div>`);
    $("#s-chat-container").append(`
                            <sl-tree-item>
                            ${name}
                            <sl-tree-item>
                                Change Name
                                <sl-tree-item>
                                <form class="setting-form" action="/api/settings/channel_name">
                                    <input type="hidden" name="chat_id" value="${id}">
                                    <input type="text" name="name" autocomplete="off" placeholder="Name">
                                    <input type="submit" value="Change">
                                </form>
                                </sl-tree-item>
                            </sl-tree-item>
                            <sl-tree-item>
                                Change Avatar
                                <sl-tree-item>
                                <form class="setting-form" action="/api/images/channel_avatar">
                                    <input type="hidden" name="chat_id" value="${id}">
                                    <input type="file" name="file">
                                    <input type="submit" value="Change">
                                </form>
                                </sl-tree-item>
                            </sl-tree-item>
                            <sl-tree-item>
                                Delete
                                <sl-tree-item>
                                <form class="setting-form" action="/api/settings/channel_delete">
                                    <input type="hidden" name="chat_id" value="${id}">
                                    <input type="submit" value="Confirm">
                                </form>
                                </sl-tree-item>
                            </sl-tree-item>
                        </sl-tree-item>
                        `);
}

function ResetChats(){
    $("#chat-container").empty();
    $("#s-chat-container").empty();
}

function AddTXTMessage(id, user, text, img_id) {
    const name = nusers[user];
    const $chat = $("#glob-chat");

    const isScrolledToBottom = Math.abs($chat[0].scrollHeight - $chat.scrollTop() - $chat.outerHeight()) < 5;

    const msgCard = $(`
        <div class="msg-card" id="msg-${id}">
            <div class="msg-user-img">
                <img src="/attachments/avatars/${user}.jpg" alt="Avatar">
            </div>
            <div class="msg-content-wrap">
                <h3>${name}</h3>
                <div class="msg-content">
                    <p class="txt-break"></p>
                </div>
            </div>
        </div>
    `);
    msgCard.find('p.txt-break').text(text);

    if (img_id != 0) {
        msgCard.find('.msg-content').append(`
            <div class="msg-img-wrapper">
                <img class="msg-content-img" src="/attachments/${img_id}.jpg" onerror="this.onerror=null; this.src='noimg.png';">
            </div>
            `);
    }

    $chat.append(msgCard);

    if (isScrolledToBottom) {
        requestAnimationFrame(() => {
            $chat.scrollTop($chat[0].scrollHeight);
        });
    }
}


function ResetChat(){
    $("#glob-chat").empty();
}

function AddSVUser(id){
    let name = nusers[id];
    $("#users-container").append(`
        <div class="usr-card">
            <div class="usr-user-img">
                <img src="/attachments/avatars/${id}.jpg" alt="Avatar" onerror="this.onerror=null; this.src='noimg.png';">
            </div>
            <h3>${name}</h3>
        </div>`);
}

function ResetSVUsers(){
    $("#users-container").empty();
}

function Testa(){
    ResetServers();
    ResetChats();
    for (var i = 0; i<100; i++){
        AddServer("test "+i);
        AddChat("test "+i);
    }
}



function LoadServers(){
    ResetServers();
    Object.entries(servers).forEach(([key,value]) => {
        AddServer(key);
    });
}

function LoadChats(serverID){
    $("#server-name").html(servers[serverID]);
    currentServer = serverID;
    sessionStorage.setItem("currServ", serverID);
    ResetChats();
    const chatList = chats[serverID];
    Object.entries(chatList).forEach(([key,value])=>{
        AddChat(value);
    })
    ResetSVUsers();
    const userList = sv_users[serverID];
    Object.entries(userList).forEach(([key,value])=>{
        AddSVUser(value);
    })
}

function LoadChat(chatID) {
    ResetChat();
    /*$("#glob-header").html("<h4>"+chatID+"</h4>");
    currentChat = chatID;
    const msgList = messages[chatID];
    if (msgList){
        Object.entries(msgList).forEach(([key,value]) => {
            AddTXTMessageNoImg(key, value[0], value[1]);
        });
    }*/
    $("#channel-name").html("<h4>"+nchats[chatID]+"</h4>");
    currentChat = chatID;
    sessionStorage.setItem("currChat", chatID);
    //GetMessages(string token, UInt64 chat_id)
    //alert(token + " " + currentChat);
    conn.invoke("GetMessages", token, currentChat)
        .catch(err => console.error(err));
    
}

ResetServers();
ResetChats();
ResetSVUsers();
ResetChat();

