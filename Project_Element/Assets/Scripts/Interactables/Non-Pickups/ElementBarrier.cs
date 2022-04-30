using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Elements;

public class ElementBarrier : MonoBehaviour
{
    public Element BarrierElement = Element.Undefined;

    public void CheckElementCollision(MainAbility ability)
    {
        if (BarrierElement == Element.Fire && ability.AbilityElementType == Element.Water)
        {
            Destroy(this.gameObject);
        }

        if (BarrierElement == Element.Plant && ability.AbilityElementType == Element.Fire)
        {
            Destroy(this.gameObject);
        }

        if (BarrierElement == Element.Water && ability.AbilityElementType == Element.Plant)
        {
            Destroy(this.gameObject);
        }
    }
}
