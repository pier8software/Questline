using System.Reflection;
using Questline.Domain.Shared.Data;
using Questline.Engine.Core;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class VersionQueryHandler : IRequestHandler<Requests.VersionQuery>
{
    public IResponse Handle(GameState state, Requests.VersionQuery request)
    {
        var version = Assembly.GetEntryAssembly()?
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion ?? "unknown";

        return new Responses.VersionResponse(version);
    }
}
