using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

[Index(nameof(name), IsUnique = true)]
public class User{
    [Key]
    public required UInt64 ID { get; set; }          //User ID
    public required string name { get; set; }        //Account Name
    public required string token { get; set; }       //Token
    public required bool verifed { get; set; }       //Probably will be used to allow user use heavy functions (Send Media, Send long messages, etc...)
}
[Index(nameof(User_ID))]
[Index(nameof(Server_ID))]
[Index(nameof(User_ID), nameof(Server_ID), IsUnique = true)]
public class ServerSubscribe
{
    [Key]
    public required UInt64 ID { get; set; }          //Subscribe ID

    public required UInt64 User_ID { get; set; }     //User ID subscribed on server

    public required UInt64 Server_ID { get; set; }   //Server ID
}
public class Server {
    [Key]
    public required UInt64 ID { get; set; }          //Server ID
    public required string name { get; set; }        //Server Name
    public required UInt64 Owner_ID { get; set; }    //Owner User ID
}
[Index(nameof(Server_ID))]
public class TextChannel{
    [Key]
    public required UInt64 ID { get; set; }          //Text Channel ID
    public required UInt64 Server_ID { get; set; }   //Server where this channel
    public required string name { get; set; }        //Channel name
}
[Index(nameof(Channel_ID))]
[Index(nameof(Channel_ID), nameof(ID))]
public class Message {
    [Key]
    public required UInt64 ID { get; set; }          //Message ID

    public required UInt64 Channel_ID { get; set; }  //Channel where this message
    public required UInt64 User_ID { get; set; }     //User ID who sent this message
    public required string message { get; set; }     //Message content
    public required UInt64 attachment { get; set; }

}

public class Counter{                                //Counter for server purposes
    [Key]
    public required UInt64 ID { get; set; }
    public required UInt64 Value { get; set; }
}
[Index(nameof(Target_ID))]
[Index(nameof(Key))]
[Index(nameof(Target_ID), nameof(Key), IsUnique = true)]
public class Setting {                               //Settings
    [Key]
    public required UInt64 ID { get; set; }
    public required UInt64 Target_ID { get; set; }   // User / Server / Channel ID
    public required string Key { get; set; }
    public required int Num { get; set; }
    public required string Str { get; set; }
}

// name
// description
// pf_ - Profile       Profile Settings - Img, Decorations etc.
// ac_ - Account       Account Settings - 2FA, restoring code etc.
// cl_ - Client        Client Settings  - Theme, Sizes
// sv_ - Server        Server Settings  - Public, 
// tx_ - Text Channel

/*[Index(nameof(Target_ID))]
public class Attachment {
    [Key]
    public required UInt64 ID { get; set; }
    public required UInt64 Target_ID { get; set; }
}*/