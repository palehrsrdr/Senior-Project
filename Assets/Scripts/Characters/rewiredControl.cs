using UnityEngine;
using System.Collections;
using Rewired;

[AddComponentMenu("")]
[RequireComponent(typeof(CharacterController))]
public class rewiredControl : MonoBehaviour {
	
	public int playerId = 0; // The Rewired player id of this character
	
	public float moveSpeed = 3.0f;
	public float bulletSpeed = 15.0f;
	public GameObject bulletPrefab;
	
	private Player player; // The Rewired Player
	private CharacterController cc;
	private Vector3 moveVector;
	private bool fire;
	private bool fireUp;
	private bool jump;
	private bool classAbility;
	private bool classAbilityUp;
	private bool utilityUp;
	private bool utilityDown;
	
	PlayerManager plyrMgr;
	
	private PlayerBase character;
	//for jumping and rotating torwards direction of travel
	public float jumpForce = 0.50f;
	public float verticalVelocity = 0.0f;
	public float rotationSpeed = 250.0f;
	
	[System.NonSerialized] // Don't serialize this so the value is lost on an editor script recompile.
	private bool initialized;
	
	void Awake() {
		cc = GetComponent<CharacterController>();
	}
	
	private void Initialize() {
		// Get the Rewired Player object for this player.
		player = ReInput.players.GetPlayer(playerId);
		
		initialized = true;
	}
	
	void Update () {
		if(!ReInput.isReady) return; // Exit if Rewired isn't ready. This would only happen during a script recompile in the editor.
		if(!initialized) Initialize(); // Reinitialize after a recompile in the editor
		
		GetInput();
		ProcessInput();
		
		
	}
	
	private void GetInput() {
		// Get the input from the Rewired Player. All controllers that the Player owns will contribute, so it doesn't matter
		// whether the input is coming from a joystick, the keyboard, mouse, or a custom controller.
		
		moveVector.x = player.GetAxis("Move Horizontal"); // get input by name or action id
		moveVector.z = player.GetAxis("Move Vertical");
		fire = player.GetButtonDown("X");
		fireUp = player.GetButtonUp ("X");
		classAbility = player.GetButtonDown ("B");
		classAbilityUp = player.GetButtonUp ("B");
		jump = player.GetButtonDown ("A");
		//utilityUp = player.GetButtonUp ("Y");
		utilityDown = player.GetButtonDown ("Y");
		//Utility = Y
		
	}
	
	private void ProcessInput() {
		
		plyrMgr = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
		
		
		//Debug.Log (plyrMgr.players[0]);
		//Debug.Log (playerId);
		//Determine which character we are controlling.
		switch (playerId) {
		case 0:
			character = plyrMgr.woods;
			break;
		case 1:
			character = plyrMgr.sorc;
			break;
		case 2:
			character = plyrMgr.rogue;
			break;
		case 3:
			character = plyrMgr.war;
			break;
		}
		//Debug.Log (character);
		//Handle jumping and add it to the movement vector
		if (jump)
		{
			if(character.canJump){
				character.canJump = false;
				verticalVelocity = character.jumpForce;
			}
		}		
		else if (cc.isGrounded)
		{
			character.canJump = true;
			verticalVelocity  = 0.0f;
		}		
		else 
		{
			verticalVelocity += Physics.gravity.y * 0.1f * Time.deltaTime;
		}
		
		// Rotate the character to face in the direction that they will move
		if (new Vector3(moveVector.x, 0.0f, moveVector.z).magnitude > 0.01f)
		{
			transform.rotation = Quaternion.RotateTowards (transform.rotation, Quaternion.LookRotation (new Vector3 (moveVector.x, 0.0f, moveVector.z)), rotationSpeed * Time.deltaTime);
		}
		
		// Process fire button down
		if (fire) {
			Debug.Log("FIRE");
			character.basicAttack ("down");	
		}
		
		//process fire button up
		if(fireUp) {
			character.basicAttack("up");
		}
		
		//process class ability button down
		if (classAbility) {
			character.classAbility("down");
		}
		
		//process class ability button up
		if (classAbilityUp) {
			character.classAbility("up");
		}
		
		
		//if (utilityUp) 
		//{
		//Debug.Log("Need to implement Utility (Y) Button Up");
		//}
		
		if (utilityDown) 
		{
			character.itemAbility();
		}
		
		
		
		// Process movement
		moveVector.y = verticalVelocity;
		
		if(moveVector.x != 0.0f || moveVector.z != 0.0f || moveVector.y != 0.0f) {
			cc.Move(moveVector * moveSpeed * Time.deltaTime);
		}
		
		
	}
}
































