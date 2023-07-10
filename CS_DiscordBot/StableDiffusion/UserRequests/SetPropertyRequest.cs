using Discord;
using Discord.WebSocket;
using LiteBot.StableDiffusion.DTOAccessors;

namespace LiteBot.StableDiffusion.UserRequests;

public class SetPropertyRequest : UserRequest {
	protected Txt2imgAccessor propertyAccessor;

	protected string property;
	protected object value;

	public SetPropertyRequest(SocketMessage socketMessage, Txt2imgAccessor propertyAccessor, string property, object value) : base(socketMessage) {
		this.propertyAccessor = propertyAccessor;
		this.property = property;
		this.value = value;
	}

	public override void Exucute() {
		propertyAccessor.SetProperty(socketMessage.Author.Id, property, value);

		MessageReference messageReference = new MessageReference(socketMessage.Id, socketMessage.Channel.Id);
		SendMessage($"A new property value set to: {property}", messageReference);
	}
}
