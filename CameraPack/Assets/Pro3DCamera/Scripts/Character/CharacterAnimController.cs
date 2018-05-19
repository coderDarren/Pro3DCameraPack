using UnityEngine;
using System.Collections;

public class CharacterAnimController : MonoBehaviour {

    PlayerController controller;
    Animator anim;

    void Start()
    {
        controller = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        anim.SetBool("OnGround", controller.Grounded());
        anim.SetFloat("Forward", controller.forwardInput);
        anim.SetFloat("Turn", controller.turnInput);
        anim.SetFloat("Jump", controller.timeInAir);
        anim.SetFloat("JumpLeg", controller.timeInAir);
    }
}
