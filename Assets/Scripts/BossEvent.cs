using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEvent : MonoBehaviour
{
    void Shake(int strength)
    {
        CinemachineShake.Instance.ShakeCamera(strength, .15f);
    }
    void AttackForTargets()
    {
        FindObjectOfType<Boss>().AttackForTargets();
    }
}
