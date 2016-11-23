using UnityEngine;
public class Bonus: MonoBehaviour{

    protected Field field;
    protected Crystal crystal;
    public TypeLineBonus type;
    public bool bounceStart { get; protected set; }
    public bool bounceComplite { get; protected set; }
    
    public virtual void Acivate()
    { }


}
