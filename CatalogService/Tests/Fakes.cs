public class FakePermissionVerifier : IPermissionVerifier { public Task Verify(Permission permission) => Task.CompletedTask; }
public class FakeMessagingService : IMessagingService { public Task Save(BaseMessage message) => Task.CompletedTask; }
public class FakeUnitOfWork : IUnitOfWork { public Task Start() => Task.CompletedTask; public void Commit() { } }

