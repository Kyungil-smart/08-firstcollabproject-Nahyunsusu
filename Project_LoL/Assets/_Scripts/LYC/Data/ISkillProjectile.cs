using _Scripts.LYC.Skill;

public interface ISkillProjectile
{
    public void Init(UnityEngine.Vector2 direction,
        UnityEngine.Vector2 startPosition,
        SkillExecutor executor,
        UnityEngine.ParticleSystem projectileParticle,
        UnityEngine.ParticleSystem explosionParticle = null);
}