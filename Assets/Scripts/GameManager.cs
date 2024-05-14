using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int fall;
    // Start is called before the first frame update
    void Start()
    {
        fall = Return.fallcount;
    }

    // Update is called once per frame
    void Update()
    {
        fall = Return.fallcount;
    }
}
