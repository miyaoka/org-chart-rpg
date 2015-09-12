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
    var gm = GameManager.Instance;
    gm.manPower.SubscribeToText (manPowerText).AddTo (this);
    gm.manCount.SubscribeToText (manCountText).AddTo (this);

    gm.manCount
      .Subscribe (c => warnNoMenberUI.SetActive (1 > c))
      .AddTo (this);

    gm.money.SubscribeToText (moneyText).AddTo (this);
    gm.year
//      .Select(y => Util.AddOrdinal(y) + " year")
      .Select(y => y.ToString() + "年目")
      .SubscribeToText (yearText);


    gm.draggingNode
      .Subscribe (n => {
        bool d = n != null;
        bool hired = d && n.isHired.Value;
        dismissArea.SetActive(hired);

        recruitsUI.SetActive(!hired);
        recruitsCG.alpha = d ? 0 : 1;
        recruitsCG.blocksRaycasts = !d;
      });

    gm.onQuest
      .Subscribe (q => recruitsUI.SetActive (!q));

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
