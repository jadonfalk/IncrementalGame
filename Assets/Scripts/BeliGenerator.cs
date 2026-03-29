public class BeliGenerator : Generator
{
    public BeliGenerator() : base("Beli Generator") { }

    public override void Produce(ref float resource, float amount)
    {
        resource += amount;
    }
}