using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;

public class AvaterPresenter : MonoBehaviour {

  [SerializeField] Image bodyImg;
  [SerializeField] Image faceImg;
  [SerializeField] Image armR_Img;
  [SerializeField] Image armL_Img;
  [SerializeField] Image weaponImg;
  [SerializeField] Image armorImg;
  [SerializeField] Image shieldImg;
  [SerializeField] Image helmImg;
  [SerializeField] Image hairM_Img;
  [SerializeField] Image hairF_Img;
  [SerializeField] GameObject avatar_UI;

  CompositeDisposable staffResources = new CompositeDisposable();
  // Use this for initialization
  void Start () {
    var node = GetComponentInParent<NodePresenter> ();
    var gm = GameManager.Instance;
    var origX = avatar_UI.transform.localPosition.x;
    node.model
      .Where (s => s != null)
      .Subscribe (s => {

        staffResources.Clear ();


        s.gender
          .Select(g => g == 0)
          .Subscribe(f => {
            hairF_Img.enabled = f;
            hairM_Img.enabled = !f;
          })
          .AddTo(staffResources);

        s.hairColor
          .Subscribe(c => {
            hairF_Img.color = hairM_Img.color = c;
          }).AddTo(staffResources);

        s.skinColor
          .Subscribe(c => {
            faceImg.color = armL_Img.color = armR_Img.color = c;
          }).AddTo(staffResources);

        s.clothColor
          .Subscribe(c => bodyImg.color = c)
          .AddTo(staffResources);

        //レベルに応じて装備
        s.baseLevel
          .Subscribe(l => {
            weaponImg.transform.localScale = new Vector3(l * .02f + 1f, l * .1f + .5f, 1);
            weaponImg.enabled = 1 <= l;
            shieldImg.enabled = 5 <= l;
            armorImg.enabled = 10 <= l;
            helmImg.enabled = 15 <= l;
          })
          .AddTo(staffResources);

        /*
        //レベルに応じて服の色
        s.baseLevel
          .Select(b => b >= 9 ? 1 : b / 9f)
          .Subscribe(l => {
            bodyImg.color = Util.HSVToRGB (l, .4f, .7f);
          })
          .AddTo(staffResources);
        */

        s.health
          .Select(h => 0 < h)
          .Subscribe(l => {
            transform.localRotation = Quaternion.Euler(0, 0, l ? 0 : -90);
          })
          .AddTo(staffResources);


        //攻撃処理
        var armR_UI = armR_Img.gameObject;

        gm.onBattle
          .Subscribe(b => {
            if(b){
              armR_UI.transform.localRotation = Quaternion.Euler(0, 0, 10);
            }else{
              LeanTween.cancel(avatar_UI);
              LeanTween.cancel(armR_UI);
              armR_UI.transform.localRotation = Quaternion.identity;// Quaternion.Euler(0, 0, 10);
            }
            s.attackInterval.Value = (Random.value * .2f + .9f) * 5f;
            s.attackTimer.Value = 0;

          })
          .AddTo(staffResources);

        node.isHired
          .CombineLatest(s.health, (l, r) => l && 0 < r) 
          .Where(r => r)
          .CombineLatest(gm.battleUpdate, (l, r) => r)
          .Subscribe (timedelta => {
            s.attackTimer.Value += timedelta;
            if(s.attackInterval.Value <= s.attackTimer.Value){
              s.attackTimer.Value = 0;
              s.attackInterval.Value = (Random.value * .2f + .9f) * 5f;
              LeanTween.cancel(avatar_UI);
              LeanTween.cancel(armR_UI);

              LeanTween.rotateZ(armR_UI, -10, .1f).setEase(LeanTweenType.easeOutCubic).setOnComplete( () => {
                LeanTween.rotateZ(armR_UI, 90, .1f).setEase(LeanTweenType.easeOutBounce).setDelay(.2f).setOnComplete( () => {
                  LeanTween.rotateZ(armR_UI, 30, .5f).setEase(LeanTweenType.easeOutCubic);
                });
              });
              LeanTween.moveLocalX (avatar_UI, origX-20, .5f).setEase (LeanTweenType.easeOutBounce).setOnComplete( () => {
//                Debug.Log("attack2:" + node.currentLevel.Value.ToString());
//                  gm.attackToQuest(node);
                LeanTween.moveLocalX (avatar_UI, origX, .3f).setEase (LeanTweenType.easeOutCubic);
              });

            }
          })
          .AddTo (staffResources);

      })
      .AddTo (this);

  }

  void OnDestroy()
  {
    staffResources.Dispose ();
  }
}
