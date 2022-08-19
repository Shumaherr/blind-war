public class Spearman : UnitInteractable
{
    protected override void Awake()
    {
        base.Awake();

        Perks.Add(gameObject.AddComponent<Fortificate>());
    }
}