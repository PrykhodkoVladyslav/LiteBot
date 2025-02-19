using LiteBot.DTOs.StableDiffusion.Requests;
using LiteBot.DTOs.StableDiffusion.Responses;

namespace LiteBot.Interfaces;

public interface IStableDiffusionApi {
	Task<Txt2ImgResponseDto> GenerateImagesAsync(Txt2ImgRequestDto postDto, CancellationToken cancellationToken = default);

	Task<ProgressResponseDto> GetProgressAsync(CancellationToken cancellationToken = default);
}
