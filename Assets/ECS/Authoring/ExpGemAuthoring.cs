using Unity.Entities;
using UnityEngine;

public class ExpGemAuthoring : MonoBehaviour
{
    public int expValue = 1;
    public float pickupRadius = 1f;

    class Baker : Baker<ExpGemAuthoring>
    {
        public override void Bake(ExpGemAuthoring a)
        {
            Entity e = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<ExpGem>(e);

            AddComponent(e, new ExpValue
            {
                Value = a.expValue
            });

            AddComponent(e, new PickupRadius
            {
                Value = a.pickupRadius
            });
        }
    }
}
 