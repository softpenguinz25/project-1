#if !(UNITY_IOS || UNITY_ANDROID)
using UnityEngine;

public class DisableObjectIfNotOnMobile : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }
}
#endif
