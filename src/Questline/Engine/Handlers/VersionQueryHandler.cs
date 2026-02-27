using System.Reflection;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;

namespace Questline.Engine.Handlers;

public class VersionQueryHandler : IRequestHandler<Requests.VersionQuery>
{
    public Task<IResponse> Handle(Requests.VersionQuery request)
    {
        var version = Assembly.GetEntryAssembly()?
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion ?? "unknown";

        return Task.FromResult<IResponse>(new Responses.VersionResponse(version));
    }
}
