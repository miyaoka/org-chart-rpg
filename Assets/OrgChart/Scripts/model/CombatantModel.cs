using UniRx;

public class CombatantModel {
  public ReactiveProperty<float> health =  new ReactiveProperty<float> (); 
  public ReactiveProperty<float> damage =  new ReactiveProperty<float> (); 
  public ReactiveProperty<float> attackInterval = new ReactiveProperty<float>();
  public ReactiveProperty<float> attackTimer = new ReactiveProperty<float>();
  public ReactiveProperty<float> attackStrength = new ReactiveProperty<float>();
}
