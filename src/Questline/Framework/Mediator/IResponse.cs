namespace Questline.Framework.Mediator;

public interface IResponse;

public record ErrorResponse(string ErrorMessage) : IResponse;
