using Unity.Entities;
using UnityEngine;

public class GameTimeAuthoring : MonoBehaviour
{
    class Baker : Baker<GameTimeAuthoring>
    {
        public override void Bake(GameTimeAuthoring authoring)
        {
            Entity e = GetEntity(TransformUsageFlags.None);

            AddComponent(e, new GameTime
            {
                Elapsed = 0f
            });
        }
    }
}   