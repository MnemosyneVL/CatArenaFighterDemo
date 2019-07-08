using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardController : MonoBehaviour
{
    public string name;
    public Color clr;

    private CharacterMenu menuCharacter;

    [SerializeField] private GameObject currentCharacter;
    [SerializeField] private BasicMovement character;

    [SerializeField] private string controller;

    private const string strHorizontal = "Horizontal";
    private const string strVertical = "Vertical";
    private const string jump = "Jump";
    private const string basic = "Basic";
    private const string cc = "CC";
    private const string ultimate = "Ultimate";

    private string controlHorizontal, controlVertical, controlJump;
    private string controlBasic, controlCC, controlUltimate;

    //rotation thing
    private const string strRightHorizontal = "Horizontal2";
    private const string strRightVertical = "Vertical2";
    private string controlRightHorizontal, controlRightVertical;
    private bool shootingUpdate;
    //CCattack modified controls
    private const string rightStick = "RightStick";
    private string controlRightStick;
    private float rotStickTapCount;
    private float rotStickTimeDelay;

    private bool game, menu;

    private bool stickDownLast;

    public bool canChoose
    {
        get; private set;
    }

    void Start()
    {
        canChoose = true;
        shootingUpdate = false;
    }

    void Update()
    {
        if (game)
        {
            float horizontal = Input.GetAxis(controlHorizontal);
            float vertical = Input.GetAxis(controlVertical);

            Vector3 moveDirection = new Vector3(horizontal, 0.0f, vertical);
            //rotation thing
            float rightHorizontal = Input.GetAxis(controlRightHorizontal);
            float rightVertical = Input.GetAxis(controlRightVertical);

            Vector3 rotationDirection = new Vector3(rightHorizontal, 0.0f, rightVertical);

            bool jumping = false;
            if (Input.GetButtonDown(controlJump))
                jumping = true;

            character.MovingCharacter(moveDirection, jumping);
            character.RotateCharacter(rotationDirection);

            if(Math.Abs(rightHorizontal) + Math.Abs(rightVertical)>0.9f)
            {
                character.HoldBasic(true);
                shootingUpdate = true;
            }
            else
            {
                if(shootingUpdate)
                {
                    character.HoldBasic(false);
                    shootingUpdate = false;
                }
            }
            if (Input.GetButtonDown(controlBasic))
                character.HoldBasic(true);
            if (Input.GetButtonUp(controlBasic))
                character.HoldBasic(false);
            if (Input.GetButtonDown(controlRightStick))
            {
                Debug.Log("Right Stick clicked");
            }
            if (Input.GetButtonUp(controlRightStick))
            {
                rotStickTapCount += 1f;
                Debug.Log("Right Stick released");
            }
    
            if (rotStickTapCount > 0 && rotStickTimeDelay < 0.75f)
                rotStickTimeDelay += Time.deltaTime;

            if(rotStickTapCount > 1 && rotStickTimeDelay < 0.75f)
            {
                if(Math.Abs(rightHorizontal) + Math.Abs(rightVertical) > 0.8f)
                {
                    Debug.Log("CCAttack Used");
                    character.UseCCAttack();
                    rotStickTimeDelay = 0f;
                    rotStickTapCount = 0f;
                }
            }
            if(rotStickTimeDelay>0.75f)
            {
                rotStickTimeDelay = 0f;
                rotStickTapCount = 0f;
            }


            if (Input.GetButtonDown(controlCC))
                character.UseCCAttack();

            if (Input.GetButtonDown(controlUltimate))
                character.UseUltimateAttack();
        }


        if (menu)
        {
            if (canChoose)
            {
                if (Input.GetAxis(controlHorizontal) < 0)
                {
                    if (!stickDownLast)
                        menuCharacter.LastItem();

                    stickDownLast = true;
                }
                else if (Input.GetAxis(controlHorizontal) > 0)
                {
                    if (!stickDownLast)
                        menuCharacter.NextItem();
                    stickDownLast = true;
                }
                else
                    stickDownLast = false;
            }

            if (Input.GetButtonDown(controlJump))
            {
                Confirm();
            }
            if (Input.GetButtonDown(controlCC))
            {
                Cancel();
            }
        }
    }

    public void Confirm()
    {
        canChoose = false;
        currentCharacter = menuCharacter.ChooseCharacter();
    }

    public void Cancel()
    {
        canChoose = true;
        menuCharacter.CancelCharacter();
    }

    public void SetupButtons(bool pc, int controllerIndex)
    {
        if (pc)
            controller = "PC";
        else
            controller = "J" + controllerIndex;

        //Set Rotation stick
        controlRightHorizontal = controller + strRightHorizontal;
        controlRightVertical = controller + strRightVertical;
        controlRightStick = controller + rightStick;

        controlHorizontal = controller + strHorizontal;
        controlVertical = controller + strVertical;
        controlJump = controller + jump;
        controlBasic = controller + basic;
        controlCC = controller + cc;
        controlUltimate = controller + ultimate;

    }

    public void StartMenu(CharacterMenu charMenu)
    {
        menuCharacter = charMenu;
        menu = true;
        game = false;
    }

    public GameObject StartGame(Transform parentTransform, Vector3 spawnPoint, Quaternion spawnRotation, SettingsLevel settings)
    {
        GameObject current = Instantiate(currentCharacter, spawnPoint, spawnRotation);
        current.transform.parent = parentTransform;
        character = current.GetComponent<BasicMovement>();

        settings.SetCharacter(name, currentCharacter.name);
        character.Settings(settings);

        character.spawnPoint = spawnPoint;
        character.rotationPoint = spawnRotation;

        if(menuCharacter != null)
        {
            name = menuCharacter.name;
            clr = menuCharacter.clr;
        }
            
        character.SetBasics(name, clr);

        game = true;
        menu = false;

        return current;
    }

    public void StopGame()
    {
        character = null;
        menu = true;
        game = false;
    }
}
