using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class MultiTread : MonoBehaviour
{

    void Start()
    {
        
        string value = String.Empty; // Used to store the return value
        for (int i = 0; i < 14; i++)
        {
            int rd = Random.Range(0, 100);
            Thread thread = new Thread(() =>
            {
                
                value = "Hello World" + rd;
            });

            thread.Start();
            thread.Join();
            Debug.Log(value);
        }
    }
}
