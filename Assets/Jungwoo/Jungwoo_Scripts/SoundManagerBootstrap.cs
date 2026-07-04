using UnityEngine;

public class SoundManagerBootstrap : MonoBehaviour
{
    public GameObject soundManagerPrefab;
    private static bool isInitialized = false;

    void Awake()
    {
        if (!isInitialized)
        {
            Instantiate(soundManagerPrefab);
            isInitialized = true;
        }

        Destroy(gameObject);
    }
}

