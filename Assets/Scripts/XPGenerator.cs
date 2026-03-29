public class XPGenerator : Generator
{
    public XPGenerator() : base("XP Generator") { }

    public override void Produce(ref float resource, float amount)
    {
        resource += amount;
    }
}