using WebHook1.Enum;

namespace WebHook1.Dtos.Messages
{
    public class TextMessageDto : BaseMessageDto
    {
        public TextMessageDto()
        {
            Type = MessageTypeEnum.Text;
        }
        public string Text { get; set; }
    }
}
