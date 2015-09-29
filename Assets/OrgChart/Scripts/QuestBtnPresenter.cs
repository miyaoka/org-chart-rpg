using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;

public class QuestBtnPresenter : ModalBtnPresenter {

  private GameManager gm;
  [SerializeField] Text warningText;
  protected override void Start(){
    base.Start ();
    gm = GameManager.Instance;
    var cg = GetComponent<CanvasGroup> ();
    var btn = GetComponentInChildren<Button> ();

    gm.gameState
      .Select (s => s == GameState.Organizing)
      .Subscribe (b => {
        cg.alpha = b ? 1 : 0;
        cg.blocksRaycasts = b;
      })
      .AddTo (this);

    gm.manCount
      .Select (c => 0 < c)
      .Subscribe (b => {
        btn.interactable = b;
        warningText.enabled = !b;
      })
      .AddTo (this);

  }
  protected override void onSubmit ()
  {
    gm.questEnter ();
    
  }
}
