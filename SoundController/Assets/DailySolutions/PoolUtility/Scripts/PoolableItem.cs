using UnityEngine;

namespace DailySolutions.PoolUtility.Scripts
{
    public class PoolableItem : MonoBehaviour
    {
        private PoolUtility _pool;

        public bool IsOnPool { get; set; }

        public void SetupPool(PoolUtility pool)
        {
            gameObject.SetActive(false);
            _pool = pool;
        }

        public virtual void Init()
        {
            gameObject.SetActive(true);
            enabled = true;
            IsOnPool = false;
        }
        protected virtual void OnDisable()
        {
            _pool?.ReturnToPool(this);
            gameObject.SetActive(false);
        }
    }
}