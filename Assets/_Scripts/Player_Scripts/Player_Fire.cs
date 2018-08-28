using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace PlayerComponent {
    public class Player_Fire : MonoBehaviour {
        //References to components
        private Player p;
        private Rigidbody2D rb;
        private Animator anim;

        public GameObject weaponPrefab; //The prefab of the weapon

        private Weapon weapon; //The current weapon being fired

        //This is needed b/c the gun locations on the grounded sprite and the arial sprites are different heights
        private Vector2 baseArialOffset = new Vector2(0.2f, 0.2f); 

        private bool last_IsGrounded = true; //Used to track when the player changes to grounded or arial state
        private Vector3 last_UpDirection = Vector3.up;

        //Various timers
        private float fireTimeStamp = 0;
        [SerializeField]
        private float fireCoolDown = 0.5f;

        private float fireAnimTimeStamp = 0;
        private float fireAnimCoolDown = 0.1f;

        void Awake () {
            p = GetComponent<Player>();
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
        }
	
	    void Start () {
            UpdateWeapon(weaponPrefab); //Initialize the weapon prefab
	    }

        public void UpdateWeapon (GameObject prefab) { //Applies a new prefab
            weaponPrefab = prefab;
            
            if (weaponPrefab.GetComponent<Weapon>() == null) return; //If there is no weapon script attached

            SimplePool.Preload(weaponPrefab, weaponPrefab.GetComponent<Weapon>().SpawnAmount); //Preload the pool
            //Preemptively assign a weapon to the weapon variable
            weapon = SimplePool.Spawn(weaponPrefab, new Vector3(1000, 1000, 0), Quaternion.identity).GetComponent<Weapon>();

            //Update the values
            fireCoolDown = weapon.Cooldown;
            fireAnimCoolDown = weapon.AnimationCooldown;
        }
	
	    void Update () {
            if (PauseManager.Instance.Paused) return;
            if (!p.IsAlive) return;

    //TODO: change this to compile to standalone only
    //#if UNITY_EDITOR 
            KeyboardFire();
    //#endif

            if (p.IsGrounded != last_IsGrounded || transform.up != last_UpDirection) { //If the player changed grounded state from the last time it was checked or changed it's local up direction
                last_IsGrounded = p.IsGrounded; //Update the state
                last_UpDirection = transform.up; //Update the last local up direction

                if (weapon == null) return; //If there is no weapon

                if (p.IsGrounded)
                    weapon.SetOffset(); //Set offset to none
                else {
                    Vector2 up = transform.up; //Get the local up direction

                    Vector2 offset = new Vector2(baseArialOffset.x * up.x, baseArialOffset.y * up.y); //Calculate the weapon offset based off the direction
                    weapon.SetOffset(offset); //Apply the offset to the weapon
                }
            }


            if ((Accessories.time - fireAnimTimeStamp) > fireAnimCoolDown) { //Fire animation timer
                anim.SetBool("fire", false);
            }

            if ((Accessories.time - fireTimeStamp) > fireCoolDown) { //Fire cooldown timer
                p.CanFire = true;
            }
	    }

        private void KeyboardFire() {
            //if (Input.GetMouseButtonDown(0))
            if (Input.GetKeyDown(KeyCode.Return)) 
                Fire();
        }

        public void Fire() {
            if (PauseManager.Instance.Paused) return;
            if (p.CanFire && p.IsAlive) {
                anim.SetBool("fire", true);
                fireTimeStamp = Accessories.time;
                fireAnimTimeStamp = Accessories.time;
                p.CanFire = false;

                weapon = SimplePool.Spawn(weaponPrefab, transform.position, transform.rotation).GetComponent<Weapon>(); //Get the weapon to spawn
                weapon.Spawn(transform, transform.position, transform.rotation); //Spawn the weapon in


            }
        }
    }
}
