using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;
using UnityEngine.EventSystems;

public class EndButtonPresenter : MonoBehaviour, IPointerClickHandler {
  #region IPointerClickHandler implementation
  public void OnPointerClick (PointerEventData eventData)
  {
    GameController.Instance.nextPhase ();
  }
  #endregion

  [SerializeField] Text btnText;
  void Start(){
    GameController.Instance.onQuest
      .Subscribe (q => {
        btnText.text = q ? "帰還する" : "出発";
    });
  }
}
