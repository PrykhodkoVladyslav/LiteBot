using LiteBot.StableDiffusion.DTOAccessors;

namespace LiteBot.StableDiffusion;

public record ApiWithProperties(StableDiffusionApi StableDiffusionApi, Txt2imgAccessor PropertyAccessor);
