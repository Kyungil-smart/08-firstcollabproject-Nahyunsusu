using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.LYC.Skill
{
    public class SkillProjectile : MonoBehaviour, ISkillProjectile
    {
        private Rigidbody2D _rb;
        private ParticleSystem _projectile;
        private ParticleSystem _explosion;

        private Vector2 _direction;
        private Vector2 _startPosition;
        private float _speed = 30f;
        private float _range = 7f;
        private float _explosionX = 1;
        private float _explosionY = 1;
        private float _damage;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void Init(Vector2 direction, Vector2 startPosition, SkillExecutor executor,
            ParticleSystem projectileParticle,
            ParticleSystem explosionParticle = null)
        {
            var skillData = executor.CurrentSkillData;
            var playerData = executor.Controller.Data;

            _direction = direction;
            _startPosition = startPosition;
            _range = skillData.Range;
            _speed = skillData.ProjectileSpeed;
            _explosionX = skillData.DamageRangeX;
            _explosionY = skillData.DamageRangeY;
            _damage = skillData.Damage + playerData.AtkDamage; // 데미지 계산

            // Root
            transform.right = _direction;
            transform.position = _startPosition;

            // Projectile Particle
            _projectile = Instantiate(projectileParticle, transform);
            _projectile.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            var main = _projectile.main;
            float remainTime = _range / _speed + 0.2f; // 자연스러운 스킬 표현을 위한 오프셋
            main.startLifetime = remainTime;
            main.duration = remainTime;
            _projectile.Play();

            // Explosion Particle
            if (explosionParticle != null)
            {
                _explosion = Instantiate(explosionParticle, transform);
                float scale = Mathf.Max(_explosionX, _explosionY);
                _explosion.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                _explosion.transform.localScale = Vector3.one * scale;
            }
        }

        private void FixedUpdate()
        {
            _rb.MovePosition(_rb.position + _direction * (_speed * Time.fixedDeltaTime));

            if (Vector2.Distance(_startPosition, _rb.position) >= _range)
            {
                DestroyProjectile();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Monster"))
                DestroyProjectile();
        }

        private void DestroyProjectile()
        {
            // Physics
            ContactFilter2D contactFilter2D = ContactFilter2D.noFilter; // Todo: Set monster layer
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            List<Collider2D> monsters = new();
            Physics2D.OverlapBox(transform.position, new Vector2(_explosionX, _explosionY), angle, contactFilter2D,
                monsters);

            foreach (Collider2D monster in monsters)
            {
                // monster.gameObject.GetComponent<?>().Hit();
                Destroy(monster.gameObject);
            }

            // Particle
            if (_explosion != null)
            {
                _explosion.transform.SetParent(null);
                _explosion.Play();
                Destroy(_explosion.gameObject, _explosion.main.duration + _explosion.main.startLifetime.constantMax);
            }

            Destroy(gameObject);
        }
    }
}