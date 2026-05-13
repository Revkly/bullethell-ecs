using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;

/// <summary>
/// Berikan senjata awal kepada player secara random saat game mulai.
///
/// BUG FIX: Seed sebelumnya pakai SystemAPI.Time.ElapsedTime.GetHashCode()
/// yang selalu 0 di awal game → random selalu pilih index 0 → selalu weapon pertama.
///
/// Fix: Seed dari kombinasi beberapa sumber agar benar-benar acak tiap play.
/// </summary>
[BurstCompile]
public partial struct GameStartWeaponSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<WeaponPoolElement>();
        state.RequireForUpdate<WeaponPrefabElement>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var pool     = SystemAPI.GetSingletonBuffer<WeaponPoolElement>();
        var registry = SystemAPI.GetSingletonBuffer<WeaponPrefabElement>();

        if (pool.Length == 0) return;

        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        // FIX: Seed dari frame count + entity count + waktu
        // Hasilnya benar-benar berbeda setiap kali Play ditekan
        uint seed = (uint)(
            state.WorldUnmanaged.Time.ElapsedTime * 1000 +
            state.EntityManager.UniversalQuery.CalculateEntityCount() +
            System.DateTime.Now.Millisecond +
            1); // +1 pastikan seed tidak pernah 0

        var rng = Unity.Mathematics.Random.CreateFromIndex(seed);

        foreach (var (slot, buffer, player) in
            SystemAPI.Query<
                RefRO<WeaponSlot>,
                DynamicBuffer<OwnedWeapon>>()
            .WithAll<PlayerTag>()
            .WithEntityAccess())
        {
            if (buffer.Length > 0) continue;

            // Pilih senjata random dari pool
            int        r           = rng.NextInt(0, pool.Length);
            WeaponType startWeapon = pool[r].Value;

            Entity weapon = ecb.CreateEntity();

            ecb.AddComponent<Weapon>(weapon);
            ecb.AddComponent(weapon, new WeaponOwner         { Player = player      });
            ecb.AddComponent(weapon, new WeaponLevel         { Value  = 1           });
            ecb.AddComponent(weapon, new WeaponCooldown      { Value  = 1f, Timer = 0f });
            ecb.AddComponent(weapon, new WeaponTypeComponent { Value  = startWeapon });

            foreach (var entry in registry)
            {
                if (entry.Type == startWeapon)
                {
                    ecb.AddComponent(weapon, new WeaponProjectilePrefab
                    {
                        Value = entry.Prefab
                    });
                    break;
                }
            }

            ecb.AppendToBuffer(player, new OwnedWeapon { WeaponEntity = weapon });
        }

        state.Enabled = false; // hanya sekali
    }
}