using UnityEngine;
using System.Collections;
using UniRx;

public class BgmManager : MonoBehaviour {

  public AudioClip townBGM;
  public AudioClip battleBGM;
  [SerializeField] AudioSource au;

//	AudioClip[] acs;
	// Use this for initialization
	void Start () {

    GameController.Instance.onQuest
      .Subscribe (q => {
        au.clip = q ? battleBGM : townBGM;
        au.Play();
    });

    /*
		acs = Resources.LoadAll<AudioClip>("bgm");
		play();
  */  
	}
  /*
	void play(){
		if(acs.Length == 0){
			return;
		}
		au.Stop();
		AudioClip ac = acs[Random.Range(0,acs.Length)];
		au.clip = ac;
		
		au.Play();
		Debug.Log ("-- BGM -- " +  ac.name);
		Invoke("play", ac.length);
	}
	// Update is called once per frame
	void Update () {
	
	}
 */
}
