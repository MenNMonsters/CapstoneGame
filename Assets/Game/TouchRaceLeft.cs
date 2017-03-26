using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class TouchRaceLeft : MonoBehaviour
{

    public static bool isRacePressed = false;

    public void onPointerDownRaceButton()
    {
        isRacePressed = true;
    }
    public void onPointerUpRaceButton()
    {
        isRacePressed = false;
    }
}
