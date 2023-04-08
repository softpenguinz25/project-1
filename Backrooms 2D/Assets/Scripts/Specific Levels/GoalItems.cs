using System;
using UnityEngine;

public class GoalItems : MonoBehaviour
{
    public event Action<int> NextLevel;

    protected void NextLevelInvoke(int nextLevelIndex) => NextLevel?.Invoke(nextLevelIndex);
}
