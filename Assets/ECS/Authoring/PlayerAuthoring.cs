using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    public float moveSpeed = 5f;

    class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);

            AddComponent<PlayerTag>(entity);
            AddComponent(entity, new MoveSpeed { Value = authoring.moveSpeed });
            AddComponent<PlayerInput>(entity);
        }
    }
}
