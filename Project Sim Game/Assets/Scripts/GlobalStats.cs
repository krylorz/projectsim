using UnityEngine;
using System.Collections;

public class GlobalStats : MonoBehaviour
{

    static GlobalStats mInstance;

    public static GlobalStats Instance
    {
        get
        {
            if (mInstance == null)
            {
                GameObject go = new GameObject();
                mInstance = go.AddComponent<GlobalStats>();
            }
            return mInstance;
        }
    }

    private float synchronizer = 0f;
    private static int globalCounter = 0;
    public static int GlobalCounter
    {
        get
        {
            return globalCounter;
        }
        set
        {

        }
    }

    public static bool tick = false;
    void Update()
    {
        synchronizer += Time.deltaTime;
        if(synchronizer > 1f/30f)
        {
            globalCounter += 1;
            synchronizer = 0f;
            tick = true;
        }
        else
        {
            tick = false;
        }
    }
}
