using UnityEngine;
using System.Collections;
using UniRx;
using UnityEngine.UI;

public class DissmissAreaPresenter : MonoBehaviour {

	// Use this for initialization
	void Start () {
    var gm = GameManager.Instance;
    var drop = GetComponent<DropTrigger> ();
    var outline = GetComponent<Outline> ();

    //drop trigger
    drop.OnDropAsObservable ()
      .Subscribe (e => {
      Debug.Log ("drop");
      var dragNode = e.pointerDrag.GetComponentInParent<NodePresenter> ();
      //clear pointer value
      dragNode.model.Value = null;
      gm.draggingNode.Value = null;

    })
      .AddTo (this);

    drop.OnPointerEnterAsObservable ()
      .Subscribe (_ => {
        outline.enabled = true;
      })
      .AddTo (this);

    drop.OnPointerExitAsObservable ()
      .Subscribe (_ => {
        outline.enabled = false;
      })
      .AddTo (this); 

	}
	

}
