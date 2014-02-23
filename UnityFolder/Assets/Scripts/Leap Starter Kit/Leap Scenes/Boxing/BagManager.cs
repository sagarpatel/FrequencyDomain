using UnityEngine;
using System.Collections;

public static class BagManager {

    public static BagSpawner bagSpawner;

    public static void RespawnBag()
    {
        if (bagSpawner)
        {
            bagSpawner.RespawnNewBag();
        }
    }

}
