﻿using System.Net.Http.Headers;
using System.Text;
using WebHook1.Dtos.Messages;
using WebHook1.Dtos.Messages.Request;
using WebHook1.Dtos.Webhook;
using WebHook1.Enum;
using WebHook1.Providers;

namespace WebHook1.Domain
{
    public class LineBotService : ILineBotService
    {

        private readonly string replyMessageUri = "https://api.line.me/v2/bot/message/reply";
        private readonly string broadcastMessageUri = "https://api.line.me/v2/bot/message/broadcast";
        private static HttpClient client = new HttpClient(); // 負責處理HttpRequest
        private readonly JsonProvider _jsonProvider = new JsonProvider();

        private readonly string channelAccessToken = "eVglgQGv1vP0vYrl90IWQNIDpHpDjLJcJ0KxQrUFLNm5GyCyNx3mtJ3KNfvMzthZZOuPgz4O8jeo8s2rr2tww1yzEpqvnlR1CtCp92p1aQxqW18vGwMwjlKxL1+LSJ2UYQw2+Tj6LOrqbQSzti93TwdB04t89/1O/w1cDnyilFU=";
        private readonly string channelSecret = "a6a3a2b7c5d8f5fba0bde70239b6f97a";

        public LineBotService() 
        { 
        
        }

		/// 接收到廣播請求時，在將請求傳至 Line 前多一層處理，依據收到的 messageType 將 messages 轉換成正確的型別，這樣 Json 轉換時才能正確轉換。
		/// </summary>
		/// <param name="messageType"></param>
		/// <param name="requestBody"></param>
		public void BroadcastMessageHandler(string messageType, object requestBody)
		{
			string strBody = requestBody.ToString();
			dynamic messageRequest = new BroadcastMessageRequestDto<BaseMessageDto>();
			switch (messageType)
			{
				case MessageTypeEnum.Text:
					messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<TextMessageDto>>(strBody);
					break;
				case MessageTypeEnum.Sticker:
					messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<StickerMessageDto>>(strBody);
					break;
				case MessageTypeEnum.Image:
					messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<ImageMessageDto>>(strBody);
					break;
				case MessageTypeEnum.Video:
					messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<VideoMessageDto>>(strBody);
					break;

			}
            BroadcastMessage(messageRequest);

		}

		// <summary>
		// 將廣播訊息請求送到 Line
		// <typeparam name="T"></typeparam>
		// <param name="request"></param>
		public async void BroadcastMessage<T>(BroadcastMessageRequestDto<T> request)
		{
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken); //帶入 channel access token
			var json = _jsonProvider.Serialize(request);
			var requestMessage = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri(broadcastMessageUri),
				Content = new StringContent(json, Encoding.UTF8, "application/json")
			};

			var response = await client.SendAsync(requestMessage);
			Console.WriteLine(await response.Content.ReadAsStringAsync());
		}

		// <summary>
		// 接收到回覆請求時，在將請求傳至 Line 前多一層處理(目前為預留)
		// </summary>
		public void ReplyMessageHandler<T>(string messageType, ReplyMessageRequestDto<T> requestBody)
        {
            ReplyMessage(requestBody);
        }

        // <summary>
        // 將回覆訊息請求送到 Line
        // <typeparam name="T"></typeparam>
        // <param name="request"></param>    
        public async void ReplyMessage<T>(ReplyMessageRequestDto<T> request)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken); //帶入 channel access token
            var json = _jsonProvider.Serialize(request);
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(replyMessageUri),
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(requestMessage);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }


        public void ReceiveWebhook(WebhookRequestBodyDto requestBody)
        {
            foreach (var eventObject in requestBody.Events)
            {
                switch (eventObject.Type)
                {
                    case WebhookEventTypeEnum.Message:
                        Console.WriteLine($"收到使用者傳送訊息！ {eventObject.Message.Text}");
						Console.WriteLine($"收到使用者傳送訊息！ {eventObject.Source.UserId}");
						//var v = Int32.Parse(eventObject.Message.Text) * 10;
						var replyMessage = new ReplyMessageRequestDto<TextMessageDto>()
                        {
                            ReplyToken = eventObject.ReplyToken,
                            Messages = new List<TextMessageDto>
                            {
                                new TextMessageDto(){Text = $"{eventObject.Message.Text}"},
								new TextMessageDto(){Text = $"{eventObject.Source.UserId}"}
							}
                        };
                        ReplyMessageHandler("text", replyMessage);
                        break;
                    case WebhookEventTypeEnum.Unsend:
                        Console.WriteLine($"使用者{eventObject.Source.UserId}在聊天室收回訊息！");
                        break;
                    case WebhookEventTypeEnum.Follow:
                        Console.WriteLine($"使用者{eventObject.Source.UserId}將我們新增為好友！");
                        break;
                    case WebhookEventTypeEnum.Unfollow:
                        Console.WriteLine($"使用者{eventObject.Source.UserId}封鎖了我們！");
                        break;
                    case WebhookEventTypeEnum.Join:
                        Console.WriteLine("我們被邀請進入聊天室了！");
                        break;
                    case WebhookEventTypeEnum.Leave:
                        Console.WriteLine("我們被聊天室踢出了");
                        break;
                    case WebhookEventTypeEnum.VideoPlayComplete:
						Console.WriteLine("收到使用者傳送訊息");
						//replyMessage = new ReplyMessageRequestDto<TextMessageDto>()
						//{
						//	ReplyToken = eventObject.ReplyToken,
						//	Messages = new List<TextMessageDto>
						//	{
						//		new TextMessageDto(){Text = $"sssssss"}
						//	}
						//};
						//ReplyMessageHandler("text", replyMessage);
						break;
				}
            }
        }
    }
}
