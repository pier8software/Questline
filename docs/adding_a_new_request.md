# Adding a New Request

1. **Request** - Add a record inside `Engine/Messages/Requests.cs`, annotate with `[Verbs("...")]`, implement `IRequest` (including the `static abstract CreateRequest` factory):
   ```csharp
   [Verbs("eat")]
   public record EatItemCommand(string ItemName) : IRequest
   {
       public static IRequest CreateRequest(string[] args) => new EatItemCommand(string.Join(" ", args));
   }
   ```
2. **Response** - Add a record inside `Engine/Messages/Responses.cs` implementing `IResponse`. The response formats its own `Message` property:
   ```csharp
   public record ItemEatenResponse(string Item) : IResponse
   {
       public string Message => $"You eat the {Item}.";
   }
   ```
3. **Handler** - Create a handler class in `Engine/Handlers/` implementing `IRequestHandler<T>`:
   ```csharp
   public class EatItemHandler : IRequestHandler<Requests.EatItemCommand>
   {
       public IResponse Handle(GameState state, Requests.EatItemCommand command) { ... }
   }
   ```
4. **Register** - Add the handler in `Engine/ServiceCollectionExtensions.cs`:
   ```csharp
   services.AddSingleton<IRequestHandler<EatItemCommand>, EatItemHandler>();
   ```

The `Parser` discovers the new `[Verbs]` automatically via reflection - no parser changes needed.
