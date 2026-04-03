using System.Collections;
using UnityEngine;

namespace _Scripts.LYC.Skill
{
	[RequireComponent(typeof(ParticleSystem))]
	internal class PooledParticle : MonoBehaviour
	{
		internal ParticleSystem Prefab { get; private set; }
		private ParticleSystem _ps;

		private void Awake() => _ps = GetComponent<ParticleSystem>();

		internal void SetPrefab(ParticleSystem prefab) => Prefab = prefab;

		internal void PlayAndReturn()
		{
			_ps.Play();
			StartCoroutine(ReturnAfterPlay());
		}

		private IEnumerator ReturnAfterPlay()
		{
			yield return new WaitForSeconds(_ps.main.duration);
			ParticlePool.Return(this);
		}
	}
}