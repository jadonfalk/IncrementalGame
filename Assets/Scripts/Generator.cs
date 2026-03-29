public abstract class Generator
{
    public string Name;

    public Generator(string name)
    {
        Name = name;
    }

    public abstract void Produce(ref float resource, float amount);
}