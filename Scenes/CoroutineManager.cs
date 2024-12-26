using System.Collections;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    private static CoroutineManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static CoroutineManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("CoroutineManager");
                instance = obj.AddComponent<CoroutineManager>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }


    public Coroutine StartManagedCoroutine(IEnumerator coroutine)
    {
        return StartCoroutine(coroutine);
    }

}
