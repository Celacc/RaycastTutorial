using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using UnityEngine;

public class RayCastShoot : MonoBehaviour
{

    public int gunDamage = 1;       //Guns damage
    public float fireRate = .25f;   //How often it fires
    public float weaponRange = 50f; //Weapons range (units)
    public float hitForce = 100f;   //force applied from raycast
    public Transform gunEnd;        //Empty game object, marks position the end of the gun where the laser will begin

    private Camera fpsCam;
    private WaitForSeconds shotDuration = new WaitForSeconds(.07f); //How long the laser remains visible
    private AudioSource gunAudio; //gun sound
    private LineRenderer laserLine; //Draws the laser line
    private float nextFire; //holds the value for firing next time

    void Start()
    {

        laserLine = GetComponent<LineRenderer>();

        gunAudio = GetComponent<AudioSource>();

        
        fpsCam = GetComponentInParent<Camera>();
    }

    void Update()
    {
        
        if (Input.GetButtonDown("Fire1") && Time.time > nextFire) //Checks for fire button and time from shot
        {
            
            nextFire = Time.time + fireRate; // Update time when  player can fire next

            
            StartCoroutine(ShotEffect()); // Starts ShotEffect coroutine to turn our laser line on/off

            // Create a vector at the center of the cameras viewport
            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

            
            RaycastHit hit; // Raycast stores information what it has hit

            
            laserLine.SetPosition(0, gunEnd.position); // Set start position for visual effect for laser to the position of gunEnd

            
            if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange)) // Check if our raycast has hit anything
            {
                
                laserLine.SetPosition(1, hit.point); // Set the end position for our laser line 

                ShootableBox health = hit.collider.GetComponent<ShootableBox>(); // Get a reference to a health script attached to the collider we hit

                if (health != null)
                {
                    health.Damage(gunDamage);
                }

                if (hit.rigidbody != null) // Check if the object player hit has a rigidbody attached
                {
                    hit.rigidbody.AddForce(-hit.normal * hitForce);  // Add force to the rigidbody we hit, in the direction from which it was hit
                }

            }
            else
            {
                laserLine.SetPosition(1, rayOrigin + (fpsCam.transform.forward * weaponRange)); //set the end of the line to a position directly in front of the camera at the distance of weaponRange
            }

        }
    }


    private IEnumerator ShotEffect ()
    {
        gunAudio.Play();    //shooting sound
        laserLine.enabled = true;   //line renderer true
        yield return shotDuration; //Wait for .07 seconds
        laserLine.enabled = false;
    }
}
