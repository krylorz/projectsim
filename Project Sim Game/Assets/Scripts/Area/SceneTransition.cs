using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour {

    public string sceneName;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Transition()
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Transition();
    }
}
