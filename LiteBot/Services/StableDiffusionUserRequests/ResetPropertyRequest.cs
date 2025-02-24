using LiteBot.Interfaces;

namespace LiteBot.Services.StableDiffusionUserRequests;

public class ResetPropertyRequest(
	ICurrentChannelMessageService messageService,
	IStableDiffusionUserSettingsAccessor propertyAccessor
) : IStableDiffusionUserRequest {

	public async Task ExucuteAsync() {
		propertyAccessor.SetDefaultValues();

		await messageService.SendReplyMessageAsync("Default values are set");
	}
}
