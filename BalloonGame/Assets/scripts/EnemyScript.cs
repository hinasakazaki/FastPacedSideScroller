﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour {

    public int index;
    public GameObject healer;

    public GameObject catBullet;

    public bool radar;
    private LineRenderer line;
    private Vector3 eyePosition;
    private RaycastHit2D hit;

    public int segments;
    public float radius;
    private float yValue;
    private bool notInvoked;

    private int bossHealth = 100;

    // Use this for initialization
    void Start () {
        notInvoked = true;
     
       
    }

    // Update is called once per frame
    void Update() {

       
        if (notInvoked && this.gameObject.transform.position.x < 440 && this.gameObject.transform.position.x > 426)
        {
            if (index == 400)
            {
                InvokeRepeating("LaunchProjectile", 0f, 0.1f);
            }
            else
            {
                InvokeRepeating("LaunchProjectile", 0f, Random.Range(0.2f, 0.6f)); //we could also make it random within .2f and .6f
            }
        }


    }

    void LaunchProjectile()
    {
        notInvoked = false;
        if (this.radar == false || GameScript.ended || !this.gameObject.activeInHierarchy || GameScript.nekolordExorcised)
        {
            return;
        }

        
        if (index == 400)
        {
            eyePosition = new Vector3(transform.position.x - 1f, transform.position.y + .7f, transform.position.z);
            GameObject instantiated = Instantiate(catBullet, eyePosition, Quaternion.identity);
        }
        
        else
        {
            eyePosition = new Vector3(transform.position.x - 0.12f, transform.position.y + 0.01f, transform.position.z);
            GameObject instantiated = Instantiate(catBullet, eyePosition, Quaternion.identity);
        }
    }


    public void Exorcised()
    {
//        this.line.enabled = false;
        if (!(this.gameObject.transform.position.x < 450 && this.gameObject.transform.position.x > 410))
        {
            return;
        }

        if (index == 400)
        {
            if (bossHealth > 0)
            {
                bossHealth -= 5;
                if (this.GetComponentInParent<GameScript>() != null)
                {
                    this.GetComponentInParent<GameScript>().OnBossDamaged();
                }
                return;
            }
        }

        this.transform.parent = healer.transform;
        this.transform.localScale = new Vector3(1f, 1f, 0f);
        float i = this.GetComponentInParent<GameScript>().CatSlider.value;
        this.transform.localPosition = new Vector3(0.9f + i, -0.2f, 0f);
        this.GetComponent<BoxCollider2D>().enabled = false;

        if (this.GetComponentInParent<GameScript>() != null)
        {
            this.GetComponentInParent<GameScript>().OnCatExorcised(index);
        }
        this.radar = false;
    }



    private void setupLineRadar()
    {
        yValue = transform.position.y;
        this.line = GetComponent<LineRenderer>();

        if (index == 400)
        {
            line.SetVertexCount(segments + 1);
            line.useWorldSpace = false;
        }
    }
    private void doCollisionWithRadar()
    {
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.green;
        line.endColor = Color.yellow;
        line.widthMultiplier = 0.1f;
        line.sortingOrder = 1;


        if (radar && this.gameObject.transform.position.x < 441 && this.gameObject.transform.position.x > 425)
        {
            if (index == 400) //or whatever distinguishes nekolord
            {

                radius = Mathf.Repeat(Time.time * 6, 10);

                float x;
                float y;
                float z = 0f;

                float angle = 20f;

                for (int i = 0; i < (segments + 1); i++)
                {
                    x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                    y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

                    line.SetPosition(i, new Vector3(x, y, z));

                    angle += (360f / segments);
                }
            }
            else
            {
                eyePosition = new Vector3(transform.position.x - 0.12f, transform.position.y + 0.01f, transform.position.z);

                Vector3[] positions = new Vector3[2];

                positions[0] = eyePosition;
                positions[1] = new Vector3(transform.position.x - Mathf.Repeat(Time.time * 2, 10), transform.position.y + Mathf.Sin(Time.time * (index + 1) % 5));
                line.numPositions = positions.Length;
                line.SetPositions(positions);

                // line.SetPositions(positions);
                //   AnimationCurve curve = new AnimationCurve();

                /** For 3d, proven to be accurate 
                var heading = positions[1] - positions[0];
                var distance = heading.magnitude;
                var direction = heading / distance; // This is now the normalized direction.

                Debug.DrawRay(positions[0], direction, Color.red);

                 */
                //vector2 origin vector2 direction vfloat distance
                //hit = Physics2D.Raycast(positions[0], heading, distance);
                Vector2 eyePosition2D = new Vector2(eyePosition.x, eyePosition.y);
                Vector2 endOfLinePosition2D = new Vector2(positions[1].x, positions[1].y);

                /// /* this is for 2D
                var heading = endOfLinePosition2D - eyePosition2D;
                var distance = heading.magnitude;
                var direction = heading / distance; // This is now the normalized direction.

                Debug.DrawRay(endOfLinePosition2D, direction, Color.red);
                //*/
                hit = Physics2D.Raycast(endOfLinePosition2D, direction, distance, LayerMask.NameToLayer("Enemy")); //added anti mask for self. which seems to be working
                                                                                                                   // , 1 << LayerMask.NameToLayer("Enemy"))

                //if (index == 0) Debug.Log("eyePos2d" + eyePosition2D + "endOfLine2d" + endOfLinePosition2D); raycast location should be accurate! 

            }


            /**
            if (hit.collider != null)
            {

                hitObject = hit.collider.gameObject;

                if (!colliding && hitObject.GetComponent<HeroScript>() != null && hitObject.tag == "Hero")
                {
                    hitObject.GetComponent<HeroScript>().TakeDamage(10);
                    colliding = true;
                }
            }
            else
            {
                colliding = false;
            }
    **/
        }

    }


}
