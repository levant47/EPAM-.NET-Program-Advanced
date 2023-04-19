public interface ITransaction { Task<IDbTransaction> Start(); }
