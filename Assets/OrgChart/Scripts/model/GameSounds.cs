using UnityEngine;
using System.Collections;

public class GameSounds : MonoBehaviour {

	public static AudioSource select;
  public static AudioSource drop;
  public static AudioSource promote;
  public static AudioSource retire;
  public static AudioSource accounting;
  public static AudioSource submit;
  public static AudioSource hit;
  public static AudioSource damage;
  public static AudioSource miss;
	    
	// Use this for initialization
	void Start () {
		AudioSource[] audios = GetComponents<AudioSource>();
		select = audios[0];
		drop = audios[1];
    promote = audios [2];
    retire = audios [3];
    accounting = audios [4];
    submit = audios [5];
    hit = audios [6];
    damage = audios [7];
    miss = audios [8];
	}
	
}
