using UnityEngine;

public class DisableObjectIfNotOnMobile : MonoBehaviour
{
#if !(UNITY_IOS || UNITY_ANDROID)
    void Start()
    {
        gameObject.SetActive(false);
    }
#endif
}
