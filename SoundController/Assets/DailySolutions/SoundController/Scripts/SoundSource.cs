using DailySolutions.PoolUtility.Scripts;
using UnityEngine;

namespace DailySolutions.SoundController.Scripts
{
    public class SoundSource : PoolableItem
    {
        [SerializeField] AudioSource _audioSource;


        public AudioSource AudioSource => _audioSource;

        protected override void OnDisable()
        {
            base.OnDisable();
            _audioSource.Stop();
        }
    }
}