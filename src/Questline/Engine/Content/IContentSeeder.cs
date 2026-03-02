namespace Questline.Engine.Content;

public interface IContentSeeder
{
    Task SeedAdventure(string filePath, CancellationToken cancellationToken = default);
}
