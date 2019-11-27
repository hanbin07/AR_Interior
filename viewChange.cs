using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class viewChange : MonoBehaviour
{
    public Camera Arcamera;
    public Camera Minicamera;
    

    public int IsAR = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Change2AR()
    {
        if (IsAR == 0)
        {
            Arcamera.rect = new Rect(0, 0, 1, 1);
            Minicamera.rect = new Rect(0.7f, 0, 0.3f, 0.3f);
            IsAR = 1;
        }

    }

    public void Change2Plane()
    {
        if (IsAR == 1)
        {
            Arcamera.rect = new Rect(0.7f, 0, 0.3f, 0.3f);
            Minicamera.rect = new Rect(0, 0, 1, 1);
            IsAR = 0;
        }

    }
    public void No2D()
    {
        Arcamera.rect = new Rect(0, 0, 1, 1);
        Minicamera.rect = new Rect(0.7f, 0, 0.0f, 0.0f);
        IsAR = 1;

    }
}
