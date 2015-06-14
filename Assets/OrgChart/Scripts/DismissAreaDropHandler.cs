using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DismissAreaDropHandler : StaffNodeDropHandler, IDropHandler{

  new void Awake(){
    base.Awake ();
  }
  public override void OnPointerEnter (PointerEventData eventData)
  {
    if (getPointerStaffNode(eventData) ) {
      LeanTween.cancel (animUI);
      LeanTween.scale (animUI, origScale * enlarge, enterAnimTime).setEase (LeanTweenType.easeOutBack);
      outline.enabled = true;
    }
  }
  #region IDropHandler implementation
  public void OnDrop (PointerEventData eventData)
  {
    StaffNodePresenter pointerNode = getPointerStaffNode (eventData);
    if (!pointerNode) {
      return;
    }
    GameController.Instance.moveStaffNode (pointerNode, null);
    GameSounds.retire.Play ();
  }
  #endregion
}
