using UnityEngine;
using System.Collections;
using UniRx;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class QuestPresenter : MonoBehaviour, IPointerDownHandler {
  #region IPointerDownHandler implementation

  public void OnPointerDown (PointerEventData eventData)
  {
    GameManager.Instance.selectedQuest.Value = this;
  }

  #endregion

  public Text healthText;
  public Text attackText;
  public Text rewardText;


  public ReactiveProperty<string> title = new ReactiveProperty<string>();
  public ReactiveProperty<float> maxHealth = new ReactiveProperty<float>();
  public ReactiveProperty<float> health = new ReactiveProperty<float>();
  public ReactiveProperty<float> attack = new ReactiveProperty<float>();
  public ReactiveProperty<float> attackerCount = new ReactiveProperty<float>();
  public ReactiveProperty<float> reward = new ReactiveProperty<float>();
  public ReactiveProperty<float> attackInterval = new ReactiveProperty<float>(5);
  public ReactiveProperty<float> attackTimer = new ReactiveProperty<float>();
  public ReactiveProperty<float> attackDamage = new ReactiveProperty<float>();
  public ReactiveProperty<float> attackCount = new ReactiveProperty<float>();
	// Use this for initialization
	protected void Start () {

    GameManager.Instance.selectedQuest
      .Select (q => q == this)
      .Subscribe (q => {
        var olist = gameObject.GetComponents<Outline>();
        foreach(Outline o in olist){
          o.enabled = q;
        };
    })
      .AddTo (this);


    health
      .Select (v => v.ToString ("N0"))
      .SubscribeToText (healthText)
      .AddTo (this);

    attackDamage
      .CombineLatest(attackCount, (l, r) => l.ToString ("N0") + " x " + r.ToString ("N0"))
//      .Select (v => v.ToString ("N0"))
      .SubscribeToText (attackText)
      .AddTo (this);

    reward
      .Select (v => v.ToString ("N0"))
      .SubscribeToText (rewardText)
      .AddTo (this);
    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
