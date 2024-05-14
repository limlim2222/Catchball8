using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Return : MonoBehaviour
{
    public GameObject cube;
    public Rigidbody rb;
    public static int fallcount;
    // Start is called before the first frame update
    void Start()
    {
        fallcount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "Main")
        {
            cube.transform.position = new Vector3(0f, 0.5f, 0f);
            cube.transform.rotation = Quaternion.Euler(0f,0f,0f);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            fallcount++;
        }
    }
}
