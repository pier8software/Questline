using MongoDB.Driver;
using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Entity;
using Questline.Framework.Persistence;
using Questline.Tests.TestHelpers.Builders;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Tests.Framework.Persistence;

[Trait("Category", "Integration")]
public class MongoGameStateRepositoryTests : IDisposable
{
    private const string ConnectionString = "mongodb://localhost:27017";
    private const string DatabaseName = "questline_test";
    private const string CollectionName = "game_states";

    private readonly IMongoCollection<GameStateDocument> _collection;
    private readonly MongoGameStateRepository _repository;

    public MongoGameStateRepositoryTests()
    {
        var client = new MongoClient(ConnectionString);
        var database = client.GetDatabase(DatabaseName);
        _collection = database.GetCollection<GameStateDocument>(CollectionName);
        _collection.DeleteMany(Builders<GameStateDocument>.Filter.Empty);
        _repository = new MongoGameStateRepository(_collection, "0.6.0");
    }

    public void Dispose()
    {
        _collection.DeleteMany(Builders<GameStateDocument>.Filter.Empty);
    }

    [Fact]
    public void First_save_inserts_a_new_document()
    {
        var state = new GameBuilder()
            .WithRoom("entrance", "Entrance", "A dark entrance.")
            .BuildState("player1", "entrance");

        _repository.Save(state);

        var count = _collection.CountDocuments(Builders<GameStateDocument>.Filter.Empty);
        count.ShouldBe(1);
    }

    [Fact]
    public void Subsequent_save_updates_the_existing_document()
    {
        var state = new GameBuilder()
            .WithRoom("entrance", "Entrance", "A dark entrance.")
            .WithRoom("hallway", "Hallway", "A long hallway.")
            .BuildState("player1", "entrance");

        _repository.Save(state);
        state.Player.Character.MoveTo("hallway");
        _repository.Save(state);

        var count = _collection.CountDocuments(Builders<GameStateDocument>.Filter.Empty);
        count.ShouldBe(1);

        var document = _collection.Find(Builders<GameStateDocument>.Filter.Empty).First();
        document.Player.Character.Location.ShouldBe("hallway");
    }

    [Fact]
    public void Saved_document_contains_player_and_character_data()
    {
        var state = new GameBuilder()
            .WithRoom("entrance", "Entrance", "A dark entrance.")
            .BuildState("player1", "entrance");

        _repository.Save(state);

        var document = _collection.Find(Builders<GameStateDocument>.Filter.Empty).First();
        document.Id.ShouldBe("player1");
        document.Player.Character.Name.ShouldBe("TestHero");
        document.Player.Character.Race.ShouldBe("Human");
        document.Player.Character.Class.ShouldBe("Fighter");
        document.Player.Character.Location.ShouldBe("entrance");
    }

    [Fact]
    public void Saved_document_contains_room_data_with_items()
    {
        var state = new GameBuilder()
            .WithRoom("entrance", "Entrance", "A dark entrance.", r => r
                .WithItem(new Item { Id = "key", Name = "rusty key", Description = "An old key." }))
            .BuildState("player1", "entrance");

        _repository.Save(state);

        var document = _collection.Find(Builders<GameStateDocument>.Filter.Empty).First();
        document.Rooms.Count.ShouldBe(1);
        document.Rooms[0].Id.ShouldBe("entrance");
        document.Rooms[0].Items.Count.ShouldBe(1);
        document.Rooms[0].Items[0].Name.ShouldBe("rusty key");
    }

    [Fact]
    public void Saved_document_contains_barrier_unlock_status()
    {
        var barrier = new Barrier
        {
            Id = "iron-door",
            Name = "Iron Door",
            Description = "A heavy iron door.",
            BlockedMessage = "The door is locked.",
            UnlockItemId = "key",
            UnlockMessage = "The door swings open."
        };
        barrier.Unlock();

        var state = new GameBuilder()
            .WithRoom("entrance", "Entrance", "A dark entrance.")
            .WithBarrier(barrier)
            .BuildState("player1", "entrance");

        _repository.Save(state);

        var document = _collection.Find(Builders<GameStateDocument>.Filter.Empty).First();
        document.Barriers.Count.ShouldBe(1);
        document.Barriers[0].Id.ShouldBe("iron-door");
        document.Barriers[0].IsUnlocked.ShouldBeTrue();
    }

    [Fact]
    public void Saved_document_contains_version()
    {
        var state = new GameBuilder()
            .WithRoom("entrance", "Entrance", "A dark entrance.")
            .BuildState("player1", "entrance");

        _repository.Save(state);

        var document = _collection.Find(Builders<GameStateDocument>.Filter.Empty).First();
        document.Version.ShouldBe("0.6.0");
    }
}
