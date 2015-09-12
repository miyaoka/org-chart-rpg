using UnityEngine;
using System.Collections;
using UniRx;

public class EnemyModel : CombatantModel {
  public ReactiveProperty<float> baseHealth =  new ReactiveProperty<float> (); 
}
