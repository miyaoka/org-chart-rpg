using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;

public class BgImagePresenter : MonoBehaviour {

  public Sprite planBg;
  public Sprite battleBg;

  // Use this for initialization
  void Start () {
    var bg = GetComponent<Image> ();
    var gm = GameManager.Instance;
    gm.gameState
      .Subscribe (s => {
        switch (s) {
        case GameState.PeriodClosing:
        case GameState.Organizing:
          bg.sprite = planBg;
          break;
        case GameState.QuestEnter:
          bg.sprite = battleBg;
          break;
        default:
          break;
        }
      });

  }

  // Update is called once per frame
  void Update () {

  }
}
