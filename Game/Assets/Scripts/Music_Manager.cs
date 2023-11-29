using UnityEngine;

public class Music_Manager : MonoBehaviour
{
    private static Music_Manager mManager;
    void Awake()
    {
        DontDestroyOnLoad(this);
        if(mManager == null)
        {
            mManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
