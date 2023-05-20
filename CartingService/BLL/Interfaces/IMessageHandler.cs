public interface IMessageHandler<in T> { Task Handle(T message); }
