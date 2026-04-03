using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.LYC.Skill
{
	public static class ParticlePool
	{
		private static readonly Dictionary<ParticleSystem, Stack<PooledParticle>> _pools = new();

		public static void Play(ParticleSystem prefab, Vector3 position, Quaternion rotation)
		{
			if (prefab == null) return;

			var instance = Pop(prefab);
			instance.gameObject.SetActive(true);
			instance.transform.SetPositionAndRotation(position, rotation);
			instance.PlayAndReturn();
		}

		internal static void Return(PooledParticle instance)
		{
			if (instance == null) return;
			instance.gameObject.SetActive(false);

			if (!_pools.TryGetValue(instance.Prefab, out var stack))
			{
				stack = new Stack<PooledParticle>();
				_pools[instance.Prefab] = stack;
			}

			stack.Push(instance);
		}

		private static PooledParticle Pop(ParticleSystem prefab)
		{
			if (_pools.TryGetValue(prefab, out var stack))
			{
				while (stack.Count > 0)
				{
					var candidate = stack.Pop();
					if (candidate != null) return candidate; // 씬 전환으로 파괴된 인스턴스 스킵
				}
			}

			var ps = Object.Instantiate(prefab);
			var pooled = ps.gameObject.AddComponent<PooledParticle>();
			pooled.SetPrefab(prefab);
			return pooled;
		}
	}
}