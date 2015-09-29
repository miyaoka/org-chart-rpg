using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;


public class EnemyPresenter : MonoBehaviour {

  [SerializeField] LayoutElement avatarLayout;
  [SerializeField] RectTransform avatarTrans;
  [SerializeField] Animator anim;

  public ReactiveProperty<EnemyModel> enemyModel = new ReactiveProperty<EnemyModel> ();
  CompositeDisposable enemyResources = new CompositeDisposable();
  public ReactiveProperty<bool> isAlive = new ReactiveProperty<bool>(true);
  public ReactiveProperty<float> currentHealth = new ReactiveProperty<float>();


	// Use this for initialization
	void Start () {

    var sd = new Vector2 (40, 40);
    avatarLayout.preferredWidth = sd.x;
    avatarLayout.preferredHeight = sd.y;
    avatarTrans.sizeDelta = sd;

    enemyModel
      .Subscribe (m => {
        enemyResources.Clear();
        if(m == null){
          return;
        }

        currentHealth =
          m.health
            .CombineLatest(m.damage, (l, r) => l - r)
            .ToReactiveProperty();

        isAlive =
          currentHealth
            .Select(h => 0 < h)
            .ToReactiveProperty();

        m.avatarSizeDelta
          .Subscribe(s =>{
            avatarLayout.preferredWidth = s.x;
            avatarLayout.preferredHeight = s.y;
            avatarTrans.sizeDelta = s;
          })
          .AddTo(enemyResources);


      })
      .AddTo (this);
	
	}
  /*
   * 攻撃アニメーション開始
   */ 
  void startAttack(){
    anim.SetBool ("isAttacking", true);
  }
  /*
   * 攻撃アニメ移動完了時のコールバック
   */ 
  public void onAttack(){
    anim.SetBool ("isAttacking", false);
  }
	
  void OnDestroy()
  {
    enemyResources.Dispose ();
  }
}
