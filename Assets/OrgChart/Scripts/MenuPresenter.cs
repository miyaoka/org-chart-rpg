using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;

public class MenuPresenter : MonoBehaviour {

  [SerializeField] Text manPowerText;
  [SerializeField] Text manCountText;
  [SerializeField] Text moneyText;
  [SerializeField] Text yearText;

  [SerializeField] GameObject warnNoMenberUI;


  [SerializeField] GameObject dismissArea;
  [SerializeField] GameObject endTurnBtn;
  [SerializeField] GameObject resetBtn;
  [SerializeField] GameObject info;
  [SerializeField] GameObject recruitsUI;
  [SerializeField] CanvasGroup recruitsCG;

	// Use this for initialization
	void Start () {
    var gc = GameController.Instance;
    gc.manPower.SubscribeToText (manPowerText).AddTo (this);
    gc.manCount.SubscribeToText (manCountText).AddTo (this);

    gc.manCount
      .Subscribe (c => warnNoMenberUI.SetActive (1 > c))
      .AddTo (this);

    gc.money.SubscribeToText (moneyText).AddTo (this);
    gc.year
//      .Select(y => Util.AddOrdinal(y) + " year")
      .Select(y => y.ToString() + "年目")
      .SubscribeToText (yearText);


    gc.draggingNode
      .Subscribe (n => {
        bool d = n != null;
        bool hired = d && n.isHired.Value;
        dismissArea.SetActive(hired);

        recruitsUI.SetActive(!hired);
        recruitsCG.alpha = d ? 0 : 1;
        recruitsCG.blocksRaycasts = !d;
      });

    gc.onQuest
      .Subscribe (q => recruitsUI.SetActive (!q));

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
