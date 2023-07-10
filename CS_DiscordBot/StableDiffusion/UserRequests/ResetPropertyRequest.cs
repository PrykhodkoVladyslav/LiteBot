using Discord;
using Discord.WebSocket;
using LiteBot.StableDiffusion.DTOAccessors;

namespace LiteBot.StableDiffusion.UserRequests;

public class ResetPropertyRequest : UserRequest {
	protected Txt2imgAccessor propertyAccessor;

	public ResetPropertyRequest(SocketMessage socketMessage, Txt2imgAccessor propertyAccessor) : base(socketMessage) {
		this.propertyAccessor = propertyAccessor;
	}

	public override void Exucute() {
		propertyAccessor.SetDefaultValues(socketMessage.Author.Id);

		MessageReference messageReference = new MessageReference(socketMessage.Id, socketMessage.Channel.Id);
		SendMessage("Default values are set", messageReference);
	}
}