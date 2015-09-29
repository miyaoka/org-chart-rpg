using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;

public class FloorBtnPresenter : MonoBehaviour {

	// Use this for initialization
	void Start () {
    var gm = GameManager.Instance;
    var cg = GetComponent<CanvasGroup> ();
    var btn = GetComponentInChildren<Button> ();

    gm.gameState
      .Select (s => s == GameState.FloorEnter)
      .Subscribe (b => {
        cg.alpha = b ? 1 : 0;
        cg.blocksRaycasts = b;
      })
      .AddTo (this);

    btn
      .OnClickAsObservable ()
      .Subscribe (b => gm.floorEnter())
      .AddTo (this);


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
