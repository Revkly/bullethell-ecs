using Unity.Entities;
using Unity.Collections;
using UnityEngine;

public partial struct GenerateUpgradeOptionsSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var pool = SystemAPI.GetSingletonBuffer<WeaponPoolElement>();

        foreach (var (exp, level, slot, buffer, entity) in
            SystemAPI.Query<
                RefRO<PlayerExp>,
                RefRO<PlayerLevel>,
                RefRO<WeaponSlot>,
                DynamicBuffer<OwnedWeapon>>()
            .WithAll<PlayerTag>()
            .WithNone<PendingUpgrade>()
            .WithEntityAccess())
        {
            if (!state.EntityManager.HasComponent<PlayerLevel>(entity))
                continue;

            if (!state.EntityManager.HasComponent<PendingUpgrade>(entity))
            {
                // Hanya generate jika level up sudah terjadi
                if (!state.EntityManager.HasComponent<PendingUpgrade>(entity) &&
                    !state.EntityManager.HasComponent<SelectedUpgrade>(entity))
                {
                    var validWeapons = new NativeList<WeaponType>(Allocator.Temp);

                    foreach (var w in pool)
                    {
                        bool owned = false;
                        int currentLevel = 0;

                        foreach (var ownedWeapon in buffer)
                        {
                            var type = state.EntityManager
                                .GetComponentData<WeaponTypeComponent>(ownedWeapon.WeaponEntity);

                            if (type.Value == w.Value)
                            {
                                owned = true;
                                currentLevel = state.EntityManager
                                    .GetComponentData<WeaponLevel>(ownedWeapon.WeaponEntity).Value;
                                break;
                            }
                        }

                        if (owned && currentLevel < 3)
                            validWeapons.Add(w.Value);
                        else if (!owned && buffer.Length < slot.ValueRO.MaxSlot)
                            validWeapons.Add(w.Value);
                    }

                    if (validWeapons.Length < 3)
                        continue;

                    WeaponType a = validWeapons[Random.Range(0, validWeapons.Length)];
                    WeaponType b;
                    WeaponType c;

                    do { b = validWeapons[Random.Range(0, validWeapons.Length)]; }
                    while (b == a);

                    do { c = validWeapons[Random.Range(0, validWeapons.Length)]; }
                    while (c == a || c == b);

                    ecb.AddComponent(entity, new PendingUpgrade
                    {
                        OptionA = a,
                        OptionB = b,
                        OptionC = c
                    });

                    validWeapons.Dispose();
                }
            }
        }

        ecb.Playback(state.EntityManager);
    }
}
