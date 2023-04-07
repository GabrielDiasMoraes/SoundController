namespace DailySolutions.PoolUtility.Scripts.Interfaces
{
    public interface IPoolableUtility
    {
        public T GetFromPool<T>() where T : PoolableItem;

        public void ReturnToPool(PoolableItem item);

    }
}
