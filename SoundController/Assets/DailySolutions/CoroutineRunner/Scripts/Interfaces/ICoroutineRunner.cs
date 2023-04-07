
using System.Collections;
using UnityEngine;

namespace DailySolutions.CoroutineRunner.Scripts.Interfaces
{
    public interface ICoroutineRunner
    {
        public Coroutine StartCoroutine(IEnumerator enumerator);
    }
}
