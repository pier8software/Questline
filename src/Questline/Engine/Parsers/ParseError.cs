using Questline.Framework.Mediator;

namespace Questline.Engine.Parsers;

public record ParseError(string Message) : IResponse;
