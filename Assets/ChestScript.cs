using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour, IInteractable
{

    private Animator animator;

    public void Interact()
    {
        animator.Play("OpenChest");
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }
}
