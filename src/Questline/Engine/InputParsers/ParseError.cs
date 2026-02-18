using Questline.Framework.Mediator;

namespace Questline.Engine.InputParsers;

public record ParseError(string Message) : IResponse;
