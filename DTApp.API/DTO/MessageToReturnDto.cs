using System;

namespace DTApp.API.DTO
{
    public class MessageToReturnDto
    {
         public int Id {get; set;}
        public int SenderId { get; set; }        
        public string SenderKnownAs { get; set; }

        public string SenderPhotoUrl { get; set; }

        public DateTime MessageSent { get; set; }

        
        public int RecipientId { get; set; }
        public string RecipientKnownAs { get; set; }
        public string RecipientPhotoUrl { get; set; }

        public bool IsRead {get; set;}
        public DateTime? DateRead { get; set; }

        public string Content { get; set; }
    }
}