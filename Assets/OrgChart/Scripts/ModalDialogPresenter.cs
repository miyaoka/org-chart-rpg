using UnityEngine;
using System.Collections;
using UniRx;
using UnityEngine.UI;

public class ModalDialogPresenter : MonoBehaviour {
  [SerializeField] Text titleText;
  [SerializeField] Text bodyText;
  [SerializeField] Text submitText;
  [SerializeField] Button closeBtn;
  [SerializeField] Button fadeBtn;
  [SerializeField] Button submitBtn;
  public ReactiveProperty<string> titleString = new ReactiveProperty<string>();
  public ReactiveProperty<string> bodyString = new ReactiveProperty<string>();
  public ReactiveProperty<string> submitString = new ReactiveProperty<string>();
  private Animator animator;

  public IObservable<Unit> onSubmit;
	void Awake () {
    animator = GetComponent<Animator> ();
	
    closeBtn
      .OnClickAsObservable ()
      .Subscribe (b => hideDialog ())
      .AddTo (this);
    fadeBtn
      .OnClickAsObservable ()
      .Subscribe (b => hideDialog ())
      .AddTo (this);
    onSubmit = 
      submitBtn
        .OnClickAsObservable ();
    onSubmit
      .Subscribe (b => hideDialog ())
      .AddTo (this);

    titleString
      .SubscribeToText (titleText)
      .AddTo (this);
    bodyString
      .SubscribeToText (bodyText)
      .AddTo (this);
    submitString
      .SubscribeToText (submitText)
      .AddTo (this);

	}
  private void hideDialog(){
    animator.SetBool("isDisplayed", false);
  }
  private void destroyDialog(){
//    animator.Stop ();
  //  gameObject.GetComponent<Animator> ().enabled = false;

    //var cg = GetComponent<CanvasGroup> ();
    //cg.alpha = 0;
    Destroy (gameObject);
  }
	
}
