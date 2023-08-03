using WebHook1.Enum;

namespace WebHook1.Dtos.Messages
{
	public class StickerMessageDto : BaseMessageDto
	{
		public StickerMessageDto() 
		{
			Type = MessageTypeEnum.Sticker;
		}
		public string PackageId { get; set; }
		public string StickerId { get; set; }
	}
}
