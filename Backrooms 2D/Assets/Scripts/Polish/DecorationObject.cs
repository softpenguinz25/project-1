using UnityEngine;

[CreateAssetMenu(fileName = "New Decoration Object", menuName = "Decoration/Decoration Object")]
public class DecorationObject : ScriptableObject
{
    public GameObject forwardObj, rightObj, backwardObj, leftObj;
}
