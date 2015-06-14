using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StaffNodeDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

  private GameObject dragPointer;
  private StaffNodePresenter node;

  void Start(){
    node = GetComponentInParent<StaffNodePresenter> ();
  }

  #region IBeginDragHandler implementation

  public void OnBeginDrag (PointerEventData eventData)
  {
    GameSounds.select.Play ();

    //clone item
    dragPointer = GameController.Instance.createStaffCursor(node.staff.Value);

    GameController.Instance.draggingNode.Value = node;

    //hide original
//    node.isAssigned.Value = false;

    node.isDragging.Value = true;

    //notify to all
//    EventManager.Instance.TriggerEvent (new StaffBeginDragEvent(gameObject));

  }

  #endregion

  #region IDragHandler implementation

  public void OnDrag (PointerEventData eventData)
  {
    RectTransform rect = dragPointer.transform as RectTransform;
    rect.position = Input.mousePosition +  new Vector3(0, 0, 0);
  }

  #endregion

  #region IEndDragHandler implementation

  public void OnEndDrag (PointerEventData eventData)
  {

    Destroy (dragPointer);

    node.isDragging.Value = false;

    /*
    if (node.isMoved) {
      node.isMoved = false;
      enabled = false;
    } else {
      node.isAssigned.Value = true;
    }
    */

    GameController.Instance.draggingNode.Value = null;


  }

  #endregion


}
