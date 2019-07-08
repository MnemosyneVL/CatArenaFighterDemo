using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCController : MonoBehaviour
{
    [SerializeField] private BasicMovement character;

    private string strHorizontal = "PCHorizontal";
    private string strVertical = "PCVertical";
    private string jump = "PCJump";
    private string basic = "PCBasic";
    private string cc = "PCCC";
    private string ultimate = "PCUltimate";

    //rotation things
    private string strRightHorizontal = "PCHorizontal2";
    private string strRightVertical = "PCVertical2";

    void Update()
    {
        float horizontal = Input.GetAxis(strHorizontal);
        float vertical = Input.GetAxis(strVertical);

        Vector3 moveDirection = new Vector3(horizontal, 0.0f, vertical);

        //rotation thing
        float rightHorizontal = Input.GetAxis(strRightHorizontal);
        float rightVertical = Input.GetAxis(strRightVertical);

        Vector3 rotationDirection = new Vector3(rightHorizontal, 0.0f, rightVertical);

        bool jumping = false;
        if (Input.GetButtonDown(jump))
            jumping = true;

        character.MovingCharacter(moveDirection, jumping);
        character.RotateCharacter(rotationDirection);

        if (Input.GetButtonDown(basic))
            character.HoldBasic(true);
        if (Input.GetButtonUp(basic))
            character.HoldBasic(false);

        if (Input.GetButtonDown(cc))
            character.UseCCAttack();

        if (Input.GetButtonDown(ultimate))
            character.UseUltimateAttack();

    }
}
