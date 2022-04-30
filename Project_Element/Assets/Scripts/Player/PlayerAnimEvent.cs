using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvent : MonoBehaviour
{
    void PlayFootstepSFX()
    {
        PlayerManager.Instance.Player.PlayFootstepSFX();
    }
}
