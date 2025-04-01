using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class AccessibilityModifier : MonoBehaviour
{
    public ActionBasedSnapTurnProvider snapProvider;
    public ActionBasedContinuousTurnProvider continuousProvider;

    public InputActionProperty rightInput;
    public InputActionProperty leftInput;
    InputActionProperty emptyInput;

    public Transform wand;
    public DropReturn wandDrop;
    public XROneObjectSocket rightSocket;
    public XROneObjectSocket leftSocket;

    public Transform table;
    public Transform leftTable;
    public Transform rightTable;

    bool initialSet = true;

    // Start is called before the first frame update
    void Start()
    {
        Set();
        initialSet = false;
    }

    public void Set()
    {
        snapProvider.turnAmount = SaveLoad.snapAmount;
        if (SaveLoad.lefty)
        {
            if (SaveLoad.snapTurn)
            {
                snapProvider.enabled = true;

                snapProvider.rightHandSnapTurnAction = rightInput;
                snapProvider.leftHandSnapTurnAction = emptyInput;

                continuousProvider.enabled = false;
            }
            else
            {
                continuousProvider.enabled = true;

                continuousProvider.rightHandTurnAction = rightInput;
                continuousProvider.leftHandTurnAction = emptyInput;

                snapProvider.enabled = false;
            }

            leftSocket.gameObject.SetActive(true);

            wandDrop.resetPoint = leftSocket.transform;
            leftSocket.singleObject = wand.gameObject;

            rightSocket.gameObject.SetActive(false);

            if (initialSet)
            {
                wand.position = leftSocket.transform.position;
            }

            if (table)
            {
                table.position = leftTable.position;
                table.rotation = leftTable.rotation;
            }
        }
        else
        {
            if (SaveLoad.snapTurn)
            {
                snapProvider.enabled = true;

                snapProvider.leftHandSnapTurnAction = leftInput;
                snapProvider.rightHandSnapTurnAction = emptyInput;

                continuousProvider.enabled = false;
            }
            else
            {
                continuousProvider.enabled = true;

                continuousProvider.leftHandTurnAction = leftInput;
                continuousProvider.rightHandTurnAction = emptyInput;

                snapProvider.enabled = false;
            }

            rightSocket.gameObject.SetActive(true);

            wandDrop.resetPoint = rightSocket.transform;
            rightSocket.singleObject = wand.gameObject;

            leftSocket.gameObject.SetActive(false);

            if (initialSet)
            {
                wand.position = rightSocket.transform.position;
            }

            if (table)
            {
                table.position = rightTable.position;
                table.rotation = rightTable.rotation;
            }
        }
    }
}
