using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Utility untuk perpindahan scene yang aman untuk Unity ECS.
///
/// PENTING: Jangan dispose/reset ECS World saat pindah scene!
/// Unity SubScene otomatis menangani load/unload entity.
/// Yang perlu kita lakukan hanyalah:
/// 1. Load scene biasa via SceneManager
/// 2. Re-enable system yang ter-disable (misal PlayerDeathSystem)
/// </summary>
public static class ECSWorldResetter
{
    public static void ResetAndLoadScene(string sceneName)
    {
        // Reset God Mode
        PlayerDamageSystem.IsGodMode = false;

        // Load scene biasa — SubScene akan otomatis di-load/unload oleh Unity ECS
        SceneManager.LoadScene(sceneName);
    }
}
