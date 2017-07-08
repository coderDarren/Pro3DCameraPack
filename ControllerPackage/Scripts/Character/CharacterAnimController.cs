using UnityEngine;
using System.Collections;

public class CharacterAnimController : MonoBehaviour {

    CharacterController controller;
    Animator anim;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        anim.SetBool("Grounded", controller.Grounded());
        anim.SetFloat("Running", controller.forwardInput);
    }
}
