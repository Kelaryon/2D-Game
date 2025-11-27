using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationEventScript : MonoBehaviour
{

    private CharacterControler characterControler;

    // Start is called before the first frame update
    void Start()
    {
        this.characterControler = this.GetComponentInParent<CharacterControler>();
        enabled = false;
    }

    public void SetAttackEnd()
    {
        characterControler.EndAttack();
    }

}
