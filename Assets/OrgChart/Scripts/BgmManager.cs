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
    var gm = GameManager.Instance;
    gm.gameState
      .Subscribe (s => {
        switch (s) {
        case GameState.Organizing:
          Play(townBGM);
          break;
        case GameState.QuestEnter:
          Play(battleBGM);
          break;
        default:
          break;
        }
      });

    /*
		acs = Resources.LoadAll<AudioClip>("bgm");
		play();
  */  
	}
  void Play(AudioClip ac){
    au.clip = ac;
    au.Play ();
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
