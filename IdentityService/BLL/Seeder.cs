public class Seeder : ISeeder
{
    private readonly IRepository _repository;

    public Seeder(IRepository repository) => _repository = repository;

    public async Task Seed()
    {
        if (await _repository.GetRoleCount() == 0)
        {
            await _repository.SeedRole("Buyer", new[] { Permission.Read });
            var managerId = await _repository.SeedRole("Manager", new[] { Permission.Read, Permission.Create, Permission.Update, Permission.Delete });
            await _repository.SeedUser("admin", PasswordHasher.HashPassword("admin"), managerId);
        }
    }
}
