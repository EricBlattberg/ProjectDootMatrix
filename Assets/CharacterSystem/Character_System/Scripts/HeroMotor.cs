using UnityEngine;
using System.Collections;

public delegate void JumpDelegate ();
public delegate void SwanDelegate ();
public delegate void BalanceDelegate (bool b);
public delegate void CombatDelegate ();
public delegate void EvadeDelegate (string s);
public delegate void SneakDelegate (bool b);

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(HeroKeymap))]

public class HeroMotor : MonoBehaviour
{
	public bool canMove = true;
	
	public bool canStopFast = true;
	
	public LayerMask groundLayers = -1;
	public float sideAirSpeed = .2f;
	public float speed = 0.7f;
	public float sideSpeedOrig = 0.6f;
	public float sideSpeed = 0.6f;
	public float speedOrig = 1.3f;
	public float walkSpeed = 0.31f;
	public float sprintSpeed = 1.2f;
	public float mouseRotSpeed = 1.0f;
	public float jumpPower = 1.0f;
	public float groundDrag = 6.5f;
	public float airDrag = 0.0f;
	
	public float speedTimer = 0.0f;//timer start
	public float massTimer = 0.0f;
	public float pillWorth = 10.0f;//timer add
	public float pillAirDragMultiplier = 0.0f;
	public float pillMoveMultiplier = 1.5f;//run multiplier
	public float pillJumpMultiplier = 1.5f;//jump multiplier
	public float pillAirMultiplier = 1.5f;
	public float origJump = 1.4f;
	
	public float groundedOffsetRay = -0.1f;
	const float groundedDistance = 0.5f;
	const float inputThreshold = 0.01f;
	
	
	public JumpDelegate doJumpDel = null;
	public SwanDelegate doSwanDel = null;
	public BalanceDelegate doBalanceDel = null;
	public CombatDelegate doCombatDel = null;
	public CombatDelegate doSwitchWeapDel = null;
	public EvadeDelegate doEvadeDel = null;
	public SneakDelegate doSneakDel = null;
	
	Rigidbody rb;
	Transform hero;
	HeroClimb hClimb;
	HeroAnim hAnim;
	HeroPhysic hPhys;
	HeroKeymap hKey;
	
	public enum MoveState
	{
		Normal,
		Evade,
		Walking,
		Sprinting,
		Sneaking,
		Balancing,
		CombatStance
	}
	public MoveState moveState = MoveState.Normal;
	
	bool grounded;
	public bool Grounded { get { return grounded; } }
	
	
	// Double tap
	Vector3 evadeDir;
	public float tapSpeed = 0.3f; // Time between the taps
	float lastTapTime = 0.0f;
	
	bool isDoubleTap = false;
	string lastKey = " ";
	
	float curSpeed;
	bool getsSpeed = false;
	
	public float airSpeed = .23f;
	
	internal float nextJump;
	
	public bool isAfterTap = true;
	bool isAfterStop = true;
	bool canJump = true;
	
	public float fieldAdd = 0.0f;
	
	public float origField;

	
	//=================================================================================================================o
	void Start ()
	{
		origField = Camera.main.fieldOfView;
		
		rb = rigidbody;
		hero = transform;
		rb.freezeRotation = true;
		
		hClimb = GetComponent <HeroClimb> () as HeroClimb;
		hAnim = GetComponent <HeroAnim> () as HeroAnim;
		hPhys = GetComponent <HeroPhysic> () as HeroPhysic;
		hKey = GetComponent <HeroKeymap> () as HeroKeymap;
		
		hAnim.doSpeedDel += DoSetSpeed;
		hAnim.doSwitchDel += DoSwitch;
		hPhys.doPhysDel += DoReset;
		hClimb.doClimbDel += DoReset;
	}
	//=================================================================================================================o
	void DoSetSpeed (float f)
	{
		curSpeed = f;
		getsSpeed = true;
	}
	//=================================================================================================================o
	void DoSwitch (bool b)
	{
		isDoubleTap = b;
		
		// start a fresh cooldown
		isAfterTap = false;
		StartCoroutine (DTapCoolDown(0.2f)); // Normal modus
	}
	//=================================================================================================================o
	void DoReset (string s)
	{
		// Ragdoll modus ends here
		if (s == "End" || s == "None" || s == "Edge" || s == "Ledge")
		{
			//Reset
			moveState = MoveState.Normal;
			if (moveState == MoveState.Sneaking)
			{
				if (doSneakDel != null) { doSneakDel (true); }
			}
		}
	}
	//=================================================================================================================o
	void DoubleTap ()
	{
		// Double Tap WASD
		if (Input.GetKeyDown (hKey.forwardKey) || Input.GetKeyDown (hKey.backwardKey)
			|| Input.GetKeyDown (hKey.leftKey) || Input.GetKeyDown (hKey.rightKey))
		{
			if ((Time.time - lastTapTime) < tapSpeed)
			{
				if (Input.GetAxis ("Vertical") < 0.0f)
				{
					evadeDir = -hero.forward;
					lastKey = "S";
				}
				else if (Input.GetAxis ("Vertical") > 0.0f)
				{
					evadeDir = hero.forward;
					lastKey = "W";
				}
				else if (Input.GetAxis ("Horizontal") < 0.0f)
				{
					evadeDir = -hero.right;
					lastKey = "A";
				}
				else if (Input.GetAxis ("Horizontal") > 0.0f)
				{
					evadeDir = hero.right;
					lastKey = "D";
				}
				
				if (doEvadeDel != null) { doEvadeDel (lastKey); }
				
				isDoubleTap = true;
				moveState = MoveState.Evade;
				isAfterTap = false;
			}
			lastTapTime = Time.time;
		}
	}
	//=================================================================================================================o
	void Update ()
	{
		
		// MAIN INPUT
		if (canMove && !isDoubleTap) 
		{
			// Stop Rotation if Left Shift is pressed
			if (!Input.GetKey (hKey.sprintToggleKey)) 
			{
				float rotato = Input.GetAxisRaw ("Rotato") * mouseRotSpeed * Time.deltaTime;
				hero.RotateAround (hero.up, rotato);
				
				DoubleTap ();
			}
			
			// Switch states if not balancing
			if (moveState != MoveState.Balancing)
			{
				// Left Shift to toggle sprint
				if (Input.GetKeyDown (hKey.sprintToggleKey) && moveState != MoveState.Sneaking && Grounded)
				{
					moveState = moveState != MoveState.Sprinting ? MoveState.Sprinting : MoveState.Normal;
				}
				
				// Switch Walk/Run with X
				else if (Input.GetKeyDown (hKey.walkToggleKey)) 
				{
					// Walking if not already else Normal
					moveState = moveState != MoveState.Walking ? MoveState.Walking : MoveState.Normal;
				}
				
				// Sneak mode
				else if (Input.GetKeyDown (hKey.sneakToggleKey)) 
				{
					// Sneaking if not already else Normal
					moveState = moveState != MoveState.Sneaking ? MoveState.Sneaking : MoveState.Normal;
					
					bool b = moveState == MoveState.Sneaking ? true : false;
					// Sneak state delegate
					if (doSneakDel != null) { doSneakDel (b); }
				}
				
				// Switch Combat mode if not in "Cooldown"
				else if (Input.GetKeyDown (hKey.readyWeaponKey))
				{
					// Combat stance if not already else Normal
					if (hAnim.mainState != HeroAnim.MainState.Combat)
						moveState = MoveState.CombatStance;
					else
						moveState = MoveState.Normal;
					
					// Combat state delegate
					if (doCombatDel != null) { doCombatDel(); }
				}
				else if (Input.GetKeyDown(hKey.nextWeaponKey))
				{
					if (hAnim.mainState != HeroAnim.MainState.Combat && !hAnim.isWeaponDraw)
					{
						// cycle through weapon states
					    hAnim.weaponState++; // Next
						if (hAnim.weaponState > HeroAnim.WeaponState.None) // Last in the enum
							hAnim.weaponState = HeroAnim.WeaponState.Unarmed; // Start at the first
						
						// WeaponSwitch state delegate
						if (doSwitchWeapDel != null) { doSwitchWeapDel(); }
					}
				}
			}
		}
	}
	//=================================================================================================================o
	
	// Forward movement vector for moving down stronger slopes
	Vector3 ForwardVec (RaycastHit hit)
	{
		float angl = Vector3.Angle(hero.forward, hit.normal);
		if (angl < 68f) // 90 is flat ground
		{
			Vector3 slopeV = hero.forward - hero.up;
			return slopeV;
		}
		else // Move straight 
			return hero.forward;
	}
	//=================================================================================================================o
		//Start ability Timer
	void hitSpeed ()
	{
		speedTimer += pillWorth;
		fieldAdd += 9;
	}
	
	void hitMass ()
	{
		massTimer += pillWorth;
	}
	
	//=================================================================================================================o
	void FixedUpdate ()
	{
		
		if(speedTimer > 0){
			speedTimer -= Time.deltaTime;
		}
		if(massTimer > 0){
			massTimer -= Time.deltaTime;
		}
		
		RaycastHit hit;
		grounded = Physics.Raycast (hero.position + hero.up * -groundedOffsetRay,
			hero.up * -1, out hit, groundedDistance, groundLayers);
			
		//reverts physics to ground physics - sam
		if(speedTimer <= 0){
			if(canMove && grounded){
			rb.drag = groundDrag;
			walkSpeed = speedOrig;
			sideSpeed = speedOrig;
			speed = speedOrig;
			StartCoroutine (JumpCoolDown(0f));
			}
			else{
			rb.drag = airDrag;
			walkSpeed = airSpeed;
			sideSpeed = sideAirSpeed;
			speed = airSpeed;

			}
			}
		else{
			if(canMove && grounded){
			rb.drag = groundDrag;
			walkSpeed = speedOrig * pillMoveMultiplier;
			sideSpeed = speedOrig * pillMoveMultiplier;
			speed = speedOrig * pillMoveMultiplier;
			StartCoroutine (JumpCoolDown(0f));
			}
			else{
			rb.drag = airDrag * pillAirDragMultiplier;
			walkSpeed = airSpeed * pillMoveMultiplier;
			sideSpeed = sideAirSpeed * pillMoveMultiplier;
			speed = airSpeed * pillMoveMultiplier;

		}
		}
		if(massTimer <= 0){
			if(canMove && grounded){
			rb.drag = groundDrag;
			StartCoroutine (JumpCoolDown(0f));
			jumpPower = origJump;
			}
			else{
			rb.drag = airDrag;
			jumpPower = origJump;
			}
		}
		else{
			if(canMove && grounded){
			rb.drag = groundDrag;
			StartCoroutine (JumpCoolDown(0f));
			jumpPower = origJump * pillJumpMultiplier;
			}
			else{
			rb.drag = airDrag * 0;
			jumpPower = origJump * pillJumpMultiplier;
			}
		}
		
		
		if (canMove) //&& grounded)//grounded && 
		{
			
			// If any Double tap
			if (!isDoubleTap)
			{
				// Horizontal / Vertical velocity
				
				Vector3 curVelocity = Input.GetAxis ("Vertical") * ForwardVec(hit)
					+ Input.GetAxis ("Horizontal") * hero.right;
				
				
				
			if(speedTimer >= 0 ){
				speedTimer -= Time.deltaTime;
				fieldAdd -= Time.deltaTime * 2;
				Camera.main.fieldOfView = origField + fieldAdd;
					if(Camera.main.fieldOfView <= origField){
						Camera.main.fieldOfView = origField;
						}
					}
				else{
				Camera.main.fieldOfView = origField;
					}
				
				//Debug.Log(Input.GetAxis("Vertical"));
				
				//Debug.Log("curVelocity is " +curVelocity );

					
				// If not receiving speed via delegate
				
				if (!getsSpeed)
				{
					if(Input.GetKeyDown("joystick button 3")){
							Debug.Log("try to swandive");
							//animation.Play("swim");
							
						}
					// Jump
//					if (Input.GetKey (hKey.jumpKey) && canJump && grounded && pillTimer > 0)
//					{
//						
//						//grounded = false;
//						rb.drag = airDrag;
//						walkSpeed = airSpeed;
//						sideSpeed = sideAirSpeed;
//						speed = airSpeed;
//						
//						rb.AddForce (jumpPower * hero.up + 
//							rb.velocity.normalized /10f, ForceMode.VelocityChange);
//						
//						// Jump delegate
//						if (doJumpDel != null) { doJumpDel(); }
//	
//
//						
//						// Start cooldown until we can jump again
//							StartCoroutine (JumpCoolDown(5f));
//					}
					
					if (Input.GetKey (hKey.jumpKey) && canJump && grounded)
					{
						
						//grounded = false;
						rb.drag = airDrag;
						walkSpeed = airSpeed;
						sideSpeed = sideAirSpeed;
						speed = airSpeed;
						
						rb.AddForce (jumpPower * hero.up + 
							rb.velocity.normalized /10f, ForceMode.VelocityChange);
						
						// Jump delegate
						if (doJumpDel != null) { doJumpDel(); }
	

						
						// Start cooldown until we can jump again
							StartCoroutine (JumpCoolDown(5f));
					}
					
					// Walk, sprint and sneak speed
					else if (moveState == MoveState.Walking || moveState == MoveState.Sneaking){
						curSpeed = walkSpeed;
					}
				
					else if (moveState == MoveState.Sprinting) {
						curSpeed = sprintSpeed;
					}
						
					else{
						curSpeed = speed;
					}
					
					// Back-Side speed
					if (Input.GetAxis ("Vertical") < 0.0f)
					{
						curSpeed = walkSpeed;
					}
					else if (Input.GetAxis ("Horizontal") != 0.0f && moveState == MoveState.Normal)
							curSpeed = sideSpeed;
				}
				
				// Apply movement if descent input and not double tab
				if (curVelocity.magnitude > inputThreshold || curVelocity.magnitude < inputThreshold)
				{
					rb.AddForce (curVelocity.normalized * curSpeed, ForceMode.VelocityChange);
					
					// Stop "anti slide" timed trigger
					if (Input.GetKeyUp (KeyCode.W) && canStopFast)
					{
						isAfterStop = false;
						StartCoroutine (StopCoolDown(0.5f));
					}
				}
				
				else // Don't slide if not jumping and not in evade modus
				{
					if (!Input.GetKey(hKey.jumpKey) && isAfterTap && isAfterStop)
					{
						rb.velocity = new Vector3 (0.0f, rb.velocity.y, 0.0f);
					}
				}
				
				// Balancing on climb collider, layer 9
				if (moveState != MoveState.Balancing)
				{
					if (hit.transform && hit.transform.gameObject.layer == 9) // Hit climb-layer collider
					{
						if (doBalanceDel != null) { doBalanceDel(true); }
						moveState = MoveState.Balancing;
					}
				}
				else // Normal mode
				{
					if (hit.transform && hit.transform.gameObject.layer != 9) // Hit other-layer collider
					{
						if (doBalanceDel != null) { doBalanceDel(false); }
						// Back to normal
						moveState = MoveState.Normal;
					}
				}
			}
			else // Situaltional velocity ( Double tap )
			{
				if (isDoubleTap)
				{
					rb.AddForce (evadeDir.normalized * 0.7f/*curSpeed*/, ForceMode.VelocityChange);
				}
				else // Don't move horizontal
				{
					rb.velocity = new Vector3 (0.0f, rb.velocity.y, 0.0f);
				}
			}
			//rb.drag = groundDrag;
			}
		//else //in air
			//rb.drag = airDrag;

		if (!Input.GetButton("Fire1")) // If not shooting, left mouse
		{
			getsSpeed = false; // Is not receiving speed float from HeroAnim --> Back to normal
		}
	}
	
	// Double tap cool-down
	IEnumerator DTapCoolDown (float sec)
	{
		yield return new WaitForSeconds (sec);
		if (hAnim.mainState != HeroAnim.MainState.Combat)
			moveState = MoveState.Normal;
		else
			moveState = MoveState.CombatStance;
		isAfterTap = true;
	}
	// Stop cool-down
	IEnumerator StopCoolDown (float sec)
	{
		yield return new WaitForSeconds (sec);
		isAfterStop = true;
	}
	// Double tap cool-down
	IEnumerator JumpCoolDown (float sec)
	{
		canJump = true;
		yield return new WaitForSeconds (sec);
		canJump = false;
		yield return new WaitForSeconds (sec);
		canJump = true;
	}
}