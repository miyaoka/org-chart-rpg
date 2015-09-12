using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public class StaffPresenter : MonoBehaviour {
  //view
  [SerializeField] Text currentLevelText;
  [SerializeField] Text baseLevelText;
  //    [SerializeField] GameObject diffLevelUI;
  [SerializeField] Image ageContainer;
  [SerializeField] Text ageText;

  [SerializeField] RectTransform healthUI;
  [SerializeField] Text nameText;
  [SerializeField] RectTransform avatarUI;
  [SerializeField] GameObject costUI;
  [SerializeField] Text costText;



  //model
  CompositeDisposable staffResources = new CompositeDisposable();

  void Start(){
    var node = GetComponentInParent<NodePresenter> ();
    //        var relation = GetComponent<Image> ();

    //        diffBg = diffLevelUI.GetComponent<Image> ();
    //        diffText = diffLevelUI.GetComponentInChildren<Text> ();


    node.currentLevel
      .SubscribeToText(currentLevelText)
      .AddTo(this);

    node.parentDiff
      .Subscribe(diff => {
        if(diff.HasValue && diff.Value < 0)
        {
          currentLevelText.color = new Color (1, .2f, .2f);
        }else{
          currentLevelText.color = new Color (.2f, .2f, .2f);
        }
      })
      .AddTo(this);

    node.staffModel
      .Subscribe (s => {

        staffResources.Clear();
        if(s == null){
          return;
        }

        //部下が居ればbaseも表示する
        s.baseLevel
          .CombineLatest(node.hasChild, (l, r) => r ? "/" + l : "" )
          .SubscribeToText(baseLevelText)
          .AddTo(staffResources);

        /*
                s.baseLevel
                    .CombineLatest(s.age, (skill,age) => age == 0 ? .5f : Mathf.Min(1, (float)skill/age/.8f))
                    .Subscribe (rate => {
                        currentLevelText.color = Util.HSVToRGB(rate * 100f/360f, .9f, .7f);
                    })
                    .AddTo(staffResources);
*/

        /*
        s.baseLevel
          .Select(l => Mathf.Max(0, 1 - l / 15f))
          .Subscribe (rate => {
            //                        currentLevelText.color = Util.HSVToRGB(0, 0, rate * .5f);
          })
          .AddTo(staffResources);

        s.stdScore
          .Select(ss => "FFFFEEDDDCCBBAASSSS".Substring((int)Mathf.Floor(ss * 20),1) + " " + ss.ToString("P1"))
          .SubscribeToText(nameText)
          .AddTo(staffResources);
        */

        //年齢表示
        s.age
          .Select(age => age + StaffModel.startAge)
          .SubscribeToText(ageText, age => "(" + age.ToString() + ")" )
          .AddTo(staffResources);

        //
        s.age
          .Select(age => age >= StaffModel.ageSpan ? 1 : (float)age / StaffModel.ageSpan)
//          .Select(age => Mathf.Floor(age * 4f) / 4f) //normを4分割 15-19, 20-24, 25-29, 30-34, 35-
          .Subscribe (age => {
            ageContainer.color =  age == 1 ? new Color(.2f, .2f, .2f)
              : Util.correctHSVToRGB( (age * .6f + .55f) % 1f,  .5f - age * .2f, .9f - age * .3f);

//            ageContainer.color =  Util.HSVToRGB(age * 3.5f/7f + 1f/7f, .7f, age == 1 ? .4f : .9f);
            ageText.color = age == 1 ? new Color(.8f,.8f,.8f) : new Color(0,0,0);
          })
          .AddTo (staffResources);

        /*
                //世代別色分け
        s.age
          .Subscribe (age => {
            avatarImage.color =  Util.HSVToRGB((float)(age / 10) * .2f + hueInit, .7f, .7f);
          })
          .AddTo (staffResources);
        */

        /*
        s.stdScore
        //0-4
          .Select(ss => Mathf.Min(8f, Mathf.Max(0, Mathf.Round(ss * 10f) - 4f)))
          .Subscribe (ss => {
            avatarImage.color =  Util.HSVToRGB( 1f - ss  * .2f, .7f, .7f);

          })
          .AddTo (staffResources);
        */


        /*
                s.name
                    .SubscribeToText (nameText)
                    .AddTo (staffResources);
*/


        //今回と直前の値のペア
        s.baseLevel
          .Buffer(2, 1)
          .Select(l => l[1] - l[0])
          .Subscribe(l => Debug.Log(l))
          .AddTo(staffResources);

        s.health = 
          s.damage
            .CombineLatest (node.currentLevel, (l, r) => r- l)
            .ToReactiveProperty();

        s.health
          .CombineLatest (node.currentLevel, (l, r) => Mathf.Max(0, r == 0 ? 0 : l / r ))
          .Subscribe (w => healthUI.anchorMax = new Vector2(w, 1))
          .AddTo (staffResources);


      })
      .AddTo (this);



  }
  void OnDestroy()
  {
    staffResources.Dispose ();
  }
}
