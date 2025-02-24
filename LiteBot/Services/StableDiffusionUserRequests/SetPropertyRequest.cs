using LiteBot.Interfaces;

namespace LiteBot.Services.StableDiffusionUserRequests;

public class SetPropertyRequest(
	ICurrentChannelMessageService messageService,
	IStableDiffusionUserSettingsAccessor propertyAccessor
) : IStableDiffusionUserRequest {

	public string? Property { private get; set; }
	public object? Value { private get; set; }

	public async Task ExucuteAsync() {
		ArgumentNullException.ThrowIfNull(Property);
		ArgumentNullException.ThrowIfNull(Value);

		propertyAccessor.SetProperty(Property, Value);

		await messageService.SendReplyMessageAsync($"Встановлено нове значення для властивості: {Property}");
	}
}
