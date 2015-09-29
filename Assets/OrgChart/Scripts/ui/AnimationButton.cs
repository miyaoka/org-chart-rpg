using UnityEngine;
using System.Collections;
using UniRx;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class AnimationButton : Button {
  #region IPointerClickHandler implementation

  public override void OnPointerClick (PointerEventData eventData)
  {
    base.OnPointerClick (eventData);
    if (!interactable)
      return;
    GameSounds.submit.Play ();
    onSubmit ();
  }

  #endregion

  #region IPointerDownHandler implementation

  public override void OnPointerDown (PointerEventData eventData)
  {
    base.OnPointerDown (eventData);
    if (!interactable)
      return;
    onPress ();
  }

  #endregion

  #region IPointerEnterHandler implementation

  public override void OnPointerEnter (PointerEventData eventData)
  {
    base.OnPointerEnter (eventData);
    if (!interactable)
      return;
    if (eventData.eligibleForClick) {
      onPress ();
    }
  }

  #endregion

  #region IPointerExitHandler implementation

  public override void OnPointerExit (PointerEventData eventData)
  {
    base.OnPointerExit (eventData);

    if (!interactable)
      return;
    onExit ();
  }

  #endregion


  private float enterAnimTime = .1f;
  private float exitAnimTime = .02f;
  private float submitAnimTime = .2f;
  private float pushScale = .85f;
  private Vector3 origScale;


  void onPress(){
    LeanTween.cancel (targetGraphic.gameObject);
    LeanTween.scale (targetGraphic.gameObject, origScale * pushScale, enterAnimTime).setEase (LeanTweenType.easeOutQuint);
  }
  void onExit(){
    LeanTween.cancel (targetGraphic.gameObject);
    LeanTween.scale (targetGraphic.gameObject, origScale, exitAnimTime).setEase (LeanTweenType.easeOutQuint);
  }
  void onSubmit(){
    LeanTween.cancel (targetGraphic.gameObject);
    LeanTween.scale (targetGraphic.gameObject, origScale, submitAnimTime).setEase (LeanTweenType.easeOutBack);
  }
  // Use this for initialization
  void Start () {
    origScale = targetGraphic.transform.localScale;
  }

  // Update is called once per frame
  void Update () {

  }
}
