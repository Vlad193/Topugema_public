public class ReceiveServer {
    public ulong? id { get; set; }
    public string? name { get; set; }
}

public class ReceiveChat {
    public ulong server_id { get; set; }
    public ulong id { get; set; }
    public string? name { get; set; }
}

public class ReceiveUser {
    public ulong server_id { get; set; }
    public ulong id { get; set; }
    public string? name { get; set; }
}

public class ReceiveMessage {
    public ulong user_id { get; set; }
    public ulong chat_id { get; set; }
    public ulong message_id { get; set; }
    public string? message { get; set; }
    public ulong attachment { get; set; }
}