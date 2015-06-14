using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ProjectPresenter : MonoBehaviour, IPointerDownHandler {
  #region IPointerDownHandler implementation
  public void OnPointerDown (PointerEventData eventData)
  {
    isSelected.Value = !isSelected.Value;
  }
  #endregion

  [SerializeField] Text titleText;
  [SerializeField] Text healthText;
  [SerializeField] Text attackText;
  [SerializeField] Text rewardText;
  [SerializeField] RectTransform healthUI;

  public ReactiveProperty<bool> isSelected = new ReactiveProperty<bool>();
  public ReactiveProperty<string> title = new ReactiveProperty<string>();
  public ReactiveProperty<int> maxHealth = new ReactiveProperty<int>();
  public ReactiveProperty<int> health = new ReactiveProperty<int>();
  public ReactiveProperty<int> attack = new ReactiveProperty<int>();
  public ReactiveProperty<int> reward = new ReactiveProperty<int>();

  CompositeDisposable eventResources = new CompositeDisposable();

	// Use this for initialization
	void Start () {

    title
      .SubscribeToText (titleText)
      .AddTo (eventResources);

    health
      .Select (v => v.ToString ("N0"))
      .SubscribeToText (healthText)
      .AddTo (eventResources);

    attack
      .Select (v => v.ToString ("N0"))
      .SubscribeToText (attackText)
      .AddTo (eventResources);

    reward
      .Select (v => v.ToString ("N0"))
      .SubscribeToText (rewardText)
      .AddTo (eventResources);

    health
      .Subscribe (v => healthUI.sizeDelta = new Vector2( Mathf.Ceil( (float)health.Value / (float)maxHealth.Value  * 126), 10) )
      .AddTo (eventResources);

	
	}
	
  void OnDestroy()
  {
    eventResources.Dispose();
  }

}
