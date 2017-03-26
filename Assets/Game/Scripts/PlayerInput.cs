using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour {

	Player player;
    private bool IsRKeyPressed = false;
    private bool IsRocksButtonPressed = false;

	void Start () {
		player = GetComponent<Player> ();
	}

	void Update () {
		Vector2 directionalInput = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		player.SetDirectionalInput (directionalInput);

        if (TouchRaceLeft.isRacePressed && !TouchRaceRight.isRacePressed)
        {
            directionalInput = new Vector2(-1, Input.GetAxisRaw("Vertical"));
            player.SetDirectionalInput(directionalInput);
        }

        else if (TouchRaceRight.isRacePressed && !TouchRaceLeft.isRacePressed)
        {
            directionalInput = new Vector2(1, Input.GetAxisRaw("Vertical"));
            player.SetDirectionalInput(directionalInput);
        }
        else
        {
            directionalInput = new Vector2(0, Input.GetAxisRaw("Vertical"));
            player.SetDirectionalInput(directionalInput);
        }

        if (TouchRaceJump.isRacePressed)
        {
            player.OnJumpInputDown();
        }else
        {
            player.OnJumpInputUp();
        }

        if (Input.GetKeyDown (KeyCode.Space)) {
			player.OnJumpInputDown ();
		}
		if (Input.GetKeyUp (KeyCode.Space)) {
			player.OnJumpInputUp ();
		}
	}

}
