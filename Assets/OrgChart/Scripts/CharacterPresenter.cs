using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;

public class CharacterPresenter : MonoBehaviour {

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
    var node = GetComponentInParent<StaffNodePresenter> ();
    var gc = GameController.Instance;
    var origX = avatar_UI.transform.localPosition.x;
    node.staff
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

        s.baseLevel
          .Subscribe(l => {
            weaponImg.transform.localScale = new Vector3(l * .1f + 1f, l * .1f + .5f, 1);
            weaponImg.enabled = 1 <= l;
            shieldImg.enabled = 5 <= l;
            armorImg.enabled = 10 <= l;
            helmImg.enabled = 15 <= l;
          })
          .AddTo(staffResources);

        s.health
          .Select(h => 0 < h)
          .Subscribe(l => {
            transform.localRotation = Quaternion.Euler(0, 0, l ? 0 : -90);
          })
          .AddTo(staffResources);

        var armR_UI = armR_Img.gameObject;

        gc.onQuest
          .Select(q => !q)
          .Subscribe(_ => {
            LeanTween.cancel(avatar_UI);
            LeanTween.cancel(armR_UI);

            armR_UI.transform.localRotation = Quaternion.Euler(0, 0, 10);
          })
          .AddTo(staffResources);
        gc.battleTimer
          .CombineLatest(node.isHired, (l, r) => r)
          .CombineLatest(s.health, (l, r) => l && 0 < r) 
          .Where(r => r)
          .Subscribe (_ => {

            s.attackTimer.Value += Time.deltaTime;
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
                  gc.attackToQuest(node);
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
