using UnityEngine;
using System.Collections;
using UniRx;
using UnityEngine.UI;
using UniRx.Triggers;


public class SelectedQuestPresenter : MonoBehaviour {
  public Text healthText;
  public Text attackText;
  public Text rewardText;
  public Text titleText;
  public RectTransform healthUI;

  public GameObject hasQuest;
  public GameObject noQuest;

  CompositeDisposable questResources = new CompositeDisposable();

	// Use this for initialization
	void Start () {
    var gc = GameController.Instance;

    gc.selectedQuest
      .Select(q => q != null)
      .Subscribe (q => {
        hasQuest.SetActive(q);
        noQuest.SetActive(!q);
      });

    gc.selectedQuest
      .Where(q => q)
      .Subscribe (q => {

        questResources.Clear();


        
        q.health
          .CombineLatest (q.maxHealth, (l, r) => Mathf.Max(0, r == 0 ? 0 : l / r ))
          .Subscribe (w => healthUI.anchorMax = new Vector2(w, 1))
          .AddTo (questResources);

        q.title
          .SubscribeToText (titleText)
          .AddTo (questResources);


        q.health
          .CombineLatest (q.maxHealth, (l, r) => l.ToString("N0") + "/" + r.ToString("N0") )
//          .Select (v => v.ToString ("N0"))
          .SubscribeToText (healthText)
          .AddTo (questResources);

        q.attackDamage
          .CombineLatest(q.attackCount, (l, r) => l.ToString ("N0") + " x " + r.ToString ("N0"))
//          .Select (v => v.ToString ("N0"))
          .SubscribeToText (attackText)
          .AddTo (questResources);

        q.reward
          .Select (v => v.ToString ("N0"))
          .SubscribeToText (rewardText)
          .AddTo (questResources);        

        gc.onQuest
          .Where(_ => _)
          .Subscribe(_ => {
            q.attackInterval.Value = 5f;
            q.attackTimer.Value = 0;
          }).AddTo(questResources);

        gc.battleTimer
          .Subscribe (_ => {

            q.attackTimer.Value += Time.deltaTime;
            if(q.attackInterval.Value <= q.attackTimer.Value){
              q.attackTimer.Value = 0;
              q.attackInterval.Value = (Random.value * .2f + .9f) * 5f;
              var count = q.attackCount.Value;
              while(count-- > 0){
                gc.attackToStaff( (int)Mathf.Ceil( q.attackDamage.Value * Random.value) );
              }
            }
          })
          .AddTo (questResources);

    });



	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
