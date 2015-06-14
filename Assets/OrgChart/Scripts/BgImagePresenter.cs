using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;

public class BgImagePresenter : MonoBehaviour {

  public Sprite planBg;
  public Sprite battleBg;

  // Use this for initialization
  void Start () {
    Image bg = GetComponent<Image> ();
    GameController.Instance.onQuest
      .Subscribe (q => {
        bg.sprite = q ? battleBg : planBg;
      });

  }

  // Update is called once per frame
  void Update () {

  }
}
