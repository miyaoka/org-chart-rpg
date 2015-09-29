using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;

public class MenuPresenter : MonoBehaviour {

  [SerializeField] Text manPowerText;
  [SerializeField] Text manCountText;
  [SerializeField] Text moneyText;
  [SerializeField] Text salaryText;
  [SerializeField] Text yearText;
  [SerializeField] Text stateText;
  [SerializeField] Text floorText;

  [SerializeField] GameObject warnNoMenberUI;


  [SerializeField] GameObject endTurnBtn;
  [SerializeField] GameObject resetBtn;
  [SerializeField] GameObject info;
  [SerializeField] CanvasGroup recruitsContainerCG;
  [SerializeField] CanvasGroup recruitsCG;
  [SerializeField] CanvasGroup dismissCG;
  [SerializeField] CanvasGroup enemiesCG;


	// Use this for initialization
	void Start () {
    var gm = GameManager.Instance;

    gm.gameState
      .Subscribe(gs => {
        Debug.Log(gs.ToString());
        stateText.text = gs.ToString();
      })
//      .SubscribeToText (stateText)
      .AddTo (this);
    gm.manPower
      .SubscribeToText (manPowerText)
      .AddTo (this);
    gm.manCount
      .SubscribeToText (manCountText)
      .AddTo (this);

    gm.manCount
      .Subscribe (c => warnNoMenberUI.SetActive (1 > c))
      .AddTo (this);

    gm.money
      .SubscribeToText (moneyText)
      .AddTo (this);
    gm.salary
      .SubscribeToText (salaryText)
      .AddTo (this);
    gm.year
//      .Select(y => Util.AddOrdinal(y) + " year")
      .Select(y => y.ToString() + "年目")
      .SubscribeToText (yearText);

    gm.floor
      .Select(f => "B" + f.ToString() + "F")
      .SubscribeToText (floorText);

    var isOrganizing =
      gm.gameState
        .Select (s => s == GameState.Organizing)
        .ToReactiveProperty ();
      
    //解雇欄の表示
    //雇用者のドラッグ中
    gm.draggingNode
      .Subscribe (n => showCG (dismissCG, (n != null) && n.isHired.Value));

    //雇用欄の表示
    //編成中 && 非ドラッグ中
    isOrganizing
      .Subscribe (b => showCG (recruitsContainerCG, b))
      .AddTo (this);

    isOrganizing
      .CombineLatest(gm.draggingNode, (l,r) => l && r == null)
      .Subscribe (b => showCG (recruitsCG, b))
      .AddTo (this);

    gm.gameState
      .Select (s => s == GameState.FloorEnter)
      .Subscribe (b => showCG (enemiesCG, b))
      .AddTo (this);




    gm.gameState
      .Where (s => s == GameState.Organizing)
      .Subscribe ()
      .AddTo (this);
    gm.gameState
      .Where (s => s == GameState.QuestEnter)
      .Subscribe ()
      .AddTo (this);
    gm.gameState
      .Where (s => s == GameState.FloorEnter)
      .Subscribe ()
      .AddTo (this);
    gm.gameState
      .Where (s => s == GameState.Battle)
      .Subscribe ()
      .AddTo (this);
    gm.gameState
      .Where (s => s == GameState.Looting)
      .Subscribe ()
      .AddTo (this);
    gm.gameState
      .Where (s => s == GameState.FloorExit)
      .Subscribe ()
      .AddTo (this);
    gm.gameState
      .Where (s => s == GameState.QuestExit)
      .Subscribe ()
      .AddTo (this);
    gm.gameState
      .Where (s => s == GameState.PeriodClosing)
      .Subscribe ()
      .AddTo (this);

	}
  private void showCG(CanvasGroup cg, bool show){
    cg.alpha = show ? 1 : 0;
    cg.blocksRaycasts = show;
  }
}
