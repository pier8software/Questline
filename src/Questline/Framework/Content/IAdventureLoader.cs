using Questline.Domain;

namespace Questline.Framework.Content;

public interface IAdventureLoader
{
    Adventure Load(string adventurePath);
}
