using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; } // Id of the user who sent the message
        public string SenderUsername { get; set; } // Username of the user who sent the message
        public AppUser Sender { get; set; } // Navigation property to the user who sent the message
        public int RecipientId { get; set; } // Id of the user who received the message
        public string RecipientUsername { get; set; } // Username of the user who received the message
        public AppUser Recipient { get; set; } // Navigation property to the user who received the message
        public string Content { get; set; } // Content of the message
        public DateTime? DateRead { get; set; } // Date the message was read
        public DateTime MessageSent { get; set; } = DateTime.Now; // Date the message was sent
        public bool SenderDeleted { get; set; } // Whether the sender has deleted the message
        public bool RecipientDeleted { get; set; } // Whether the recipient has deleted the message

    }
}