using Questline.Domain.Adventures.Entity;

namespace Questline.Engine.Content;

public interface IGameContentLoader
{
    Adventure Load(string adventureId);
}
