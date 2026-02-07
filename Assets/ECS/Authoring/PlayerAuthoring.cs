using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float maxHealth = 100f;
    public float damageCooldown = 1f;

    class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);

            AddComponent<PlayerTag>(entity);
            AddComponent(entity, new MoveSpeed { Value = authoring.moveSpeed });
            AddComponent<PlayerInput>(entity);
            AddComponent(entity, new PlayerHealth
            {
                Current = authoring.maxHealth,
                Max = authoring.maxHealth
            });

            AddComponent(entity, new PlayerDamageCooldown
            {
                Cooldown = authoring.damageCooldown,
                Timer = 0f
            });

            AddComponent(entity, new Weapon
            {
                Damage = 10f,
                Cooldown = 0.5f,
                ProjectileCount = 1
            });

            AddComponent(entity, new WeaponTimer
            {
                Timer = 0f
            });

            AddComponent(entity, new PlayerExp
            {
                Current = 0
            });
        }
    }
}
