using UnityEngine;
using System.Collections;
using UniRx;
using UnityEngine.UI;

public class LogPresenter : MonoBehaviour {
  [SerializeField] Text logText;


	// Use this for initialization
	void Start () {
    var gm = GameManager.Instance;
    var cg = gameObject.GetComponent<CanvasGroup> ();
    var sr = GetComponentInChildren<ScrollRect> ();

    gm.onQuest.Subscribe (q => {
      cg.alpha = q ? 1 : 0;
      cg.blocksRaycasts = q;
        
    });
    gm.logText.Subscribe(t =>{
      logText.text = t;
      sr.velocity = new Vector2(0, 1000);

    }).AddTo (this);
	
	}
}
