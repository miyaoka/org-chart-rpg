using UnityEngine;
using System.Collections;
using UniRx;

public class StaffModel : CombatantModel {
  public ReactiveProperty<string> name = new ReactiveProperty<string> ();
  public ReactiveProperty<int> gender = new ReactiveProperty<int> (1);
  public ReactiveProperty<int> baseLevel =  new ReactiveProperty<int> ();  
  public ReactiveProperty<int> lastLevel =  new ReactiveProperty<int> ();  
  public ReactiveProperty<int> age = new ReactiveProperty<int> ();

  public ReactiveProperty<Color> skinColor = new ReactiveProperty<Color>();
  public ReactiveProperty<Color> hairColor = new ReactiveProperty<Color>();
  public ReactiveProperty<Color> clothColor = new ReactiveProperty<Color>();

  public ReactiveProperty<float> recruitCost = new ReactiveProperty<float>();
  public ReactiveProperty<float> stdScore = new ReactiveProperty<float>();
  public ReactiveProperty<float> hue = new ReactiveProperty<float>();
  public ReactiveProperty<float> moral = new ReactiveProperty<float>();

  public const int startAge = 15;
  public const int ageSpan = 20;

  public void grow(){
    if (age.Value < ageSpan) {
      if (Random.value < .25f + (stdScore.Value - .5f) * .5f) {
        baseLevel.Value += 1;
      }
    } else {
      if (Random.value < .5) {
        baseLevel.Value = (int)Mathf.Max(1, baseLevel.Value - 1);
      }
    }
  }
}
