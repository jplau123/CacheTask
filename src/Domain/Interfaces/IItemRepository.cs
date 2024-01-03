namespace Domain.Interfaces
{
    public interface IItemRepository
    {
        public Task<int> DeleteOlderThan(DateTimeOffset timestamp);
    }
}