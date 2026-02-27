using Questline.Domain.Adventures.Entity;

namespace Questline.Engine.Repositories;

public interface IAdventureRepository
{
    Task<Adventure> GetById(string id);
}
