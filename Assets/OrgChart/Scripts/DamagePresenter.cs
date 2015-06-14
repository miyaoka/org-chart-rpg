using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;

public class DamagePresenter : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

  public void pop(string text, Color? color = null, int size = 60){
    var t = GetComponentInChildren<Text> ();
    t.text = text;
    t.color = color ?? Color.red;
    t.fontSize = size;

    var xMove = (Random.value * .8f + .2f) * 100f;
    var time = Random.value * .2f + 1.0f;
    var yMove = 30f;
    var yGround = -20f;

    LeanTween.moveLocalX (t.gameObject, xMove, time).setEase (LeanTweenType.easeOutCubic);
    LeanTween.moveLocalY (t.gameObject, yMove, time * .2f).setEase (LeanTweenType.easeOutCubic).setOnComplete( () => {
      LeanTween.moveLocalY (t.gameObject, yGround, time * .8f).setEase (LeanTweenType.easeOutBounce).setOnComplete( () => {
        Destroy(gameObject);
      });
    });


  }
}
