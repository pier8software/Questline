using Questline.Domain.Shared.Data;
using Questline.Engine.Messages;
using Questline.Framework.Mediator;
using Questline.Framework.Persistence;
using Questline.Tests.TestHelpers.Builders;

namespace Questline.Tests.Framework.Mediator;

public class AutoSaveDecoratorTests
{
    [Fact]
    public void Calls_inner_handler_and_returns_its_response()
    {
        var state = new GameBuilder()
            .WithRoom("start", "Start", "A starting room.")
            .BuildState("player1", "start");

        var expectedResponse = new Responses.VersionResponse("1.0.0");
        var innerHandler = new StubHandler(expectedResponse);
        var repository = new SpyRepository();
        var decorator = new AutoSaveDecorator<Requests.VersionQuery>(innerHandler, repository);

        var result = decorator.Handle(state, new Requests.VersionQuery());

        result.ShouldBe(expectedResponse);
    }

    [Fact]
    public void Calls_save_on_repository_after_handler_completes()
    {
        var state = new GameBuilder()
            .WithRoom("start", "Start", "A starting room.")
            .BuildState("player1", "start");

        var innerHandler = new StubHandler(new Responses.VersionResponse("1.0.0"));
        var repository = new SpyRepository();
        var decorator = new AutoSaveDecorator<Requests.VersionQuery>(innerHandler, repository);

        decorator.Handle(state, new Requests.VersionQuery());

        repository.SaveCalledWith.ShouldBe(state);
    }

    [Fact]
    public void Calls_handler_before_saving()
    {
        var state = new GameBuilder()
            .WithRoom("start", "Start", "A starting room.")
            .BuildState("player1", "start");

        var callOrder = new List<string>();
        var innerHandler = new TrackingHandler(callOrder);
        var repository = new TrackingRepository(callOrder);
        var decorator = new AutoSaveDecorator<Requests.VersionQuery>(innerHandler, repository);

        decorator.Handle(state, new Requests.VersionQuery());

        callOrder.ShouldBe(["Handle", "Save"]);
    }

    private class StubHandler(IResponse response) : IRequestHandler<Requests.VersionQuery>
    {
        public IResponse Handle(GameState state, Requests.VersionQuery request) => response;
    }

    private class SpyRepository : IGameStateRepository
    {
        public GameState? SaveCalledWith { get; private set; }

        public void Save(GameState state) => SaveCalledWith = state;
    }

    private class TrackingHandler(List<string> callOrder) : IRequestHandler<Requests.VersionQuery>
    {
        public IResponse Handle(GameState state, Requests.VersionQuery request)
        {
            callOrder.Add("Handle");
            return new Responses.VersionResponse("1.0.0");
        }
    }

    private class TrackingRepository(List<string> callOrder) : IGameStateRepository
    {
        public void Save(GameState state) => callOrder.Add("Save");
    }
}
