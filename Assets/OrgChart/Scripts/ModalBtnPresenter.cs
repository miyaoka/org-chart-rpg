using UnityEngine;
using System.Collections;
using UniRx;
using UnityEngine.UI;

public class ModalBtnPresenter : MonoBehaviour {

  [SerializeField] GameObject modalDialogPrefab;
  [SerializeField] Transform container;

  public string titleString;
  [Multiline] public string bodyString;
  public string submitString;

  private GameObject dialog;

  protected virtual void Start () {
    var btn = GetComponentInChildren<Button> ();
    btn
      .OnClickAsObservable ()
      .Subscribe (b => showDialog ())
      .AddTo (this);
  }

  void showDialog(){
    if (dialog != null)
      return;

    dialog = Instantiate (modalDialogPrefab);
    dialog.transform.SetParent (container, false);

    var dp = dialog.GetComponent<ModalDialogPresenter>();
    dp.titleString.Value = titleString;
    dp.bodyString.Value = bodyString;
    dp.submitString.Value = submitString;

    dp
      .onSubmit
      .Subscribe (_ => onSubmit())
      .AddTo (dp);
  }

  protected virtual void onSubmit(){
  }
}
