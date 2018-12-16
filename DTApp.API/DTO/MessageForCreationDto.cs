using System;

namespace DTApp.API.DTO
{
    public class MessageForCreationDto
    {
        public MessageForCreationDto()
        {
            MessageSent = DateTime.Now;
        }
        public int SenderId { get; set; }        

        public DateTime MessageSent { get; set; }

        
        public int RecipientId { get; set; }
        
        public string Content { get; set; }

        public string SenderKnownAs {get; set;}

        public string RecipientKnownAs {get; set;}

        public string SenderPhotoUrl {get; set;}

        public string RecipientPhotoUrl {get; set;}

    }
}