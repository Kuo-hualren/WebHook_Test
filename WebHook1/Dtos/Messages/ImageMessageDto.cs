using WebHook1.Enum;

namespace WebHook1.Dtos.Messages
{
	public class ImageMessageDto : BaseMessageDto
	{
		public ImageMessageDto() 
		{
			Type = MessageTypeEnum.Image;
		}
		public string OriginalContentUrl { get; set; }
		public string PreviewImageUrl { get; set; }
	}
}
