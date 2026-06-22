using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// Authoring untuk semua projectile (Knife, FireWand, MagicWand, dll).
///
/// FIX: Sebelumnya hanya tag, tidak bake scale dari Transform prefab.
/// Akibatnya semua weapon system hardcode Scale=0.1f sendiri-sendiri,
/// jadi projectile terlihat sangat kecil dibanding sprite aslinya.
///
/// Sekarang scale prefab di Inspector (Transform → Scale) ikut di-bake
/// ke ProjectileScale, dan setiap weapon system tinggal baca nilai ini
/// tanpa hardcode.
/// </summary>
public class ProjectileAuthoring : MonoBehaviour
{
    class Baker : Baker<ProjectileAuthoring>
    {
        public override void Bake(ProjectileAuthoring authoring)
        {
            Entity e = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<ProjectileTag>(e);

            // Simpan scale asli prefab — dibaca oleh weapon system saat Instantiate
            AddComponent(e, new ProjectileScale
            {
                Value = authoring.transform.localScale.x
            });
        }
    }
}