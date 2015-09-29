using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;

[RequireComponent (typeof (CanvasGroup))]
[RequireComponent (typeof (Button))]
public class SubmitBtnPresenter : MonoBehaviour {

  [SerializeField] GameState showState;
  [SerializeField] GameState stateTo;
  void Start () {
    var gm = GameManager.Instance;
    var cg = GetComponent<CanvasGroup> ();

    gm.gameState
      .Select (s => s == showState)
      .Subscribe (s => {
        cg.alpha = s ? 1 : 0;
        cg.blocksRaycasts = s;
      })
      .AddTo (this);


    var btn = GetComponentInChildren<Button> ();

    btn
      .OnClickAsObservable()
      .Subscribe (b => {
        gm.gameState.Value = stateTo;
      })
      .AddTo(this); 
  }
}
