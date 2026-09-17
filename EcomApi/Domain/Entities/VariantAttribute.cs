namespace Domain.Entities;

public class VariantAttribute
{
    public int Id { get; private set; }
    public int VariantId { get; private set; }
    public ProductVariant? Variant { get; private set; }
    public string Key { get; private set; }
    public string Value { get; private set; }

    protected VariantAttribute() { }

    public VariantAttribute(int variantId, string key, string value)
    {
        VariantId = variantId;
        Key = key;
        Value = value;
    }

    public void UpdateValue(string value)
    {
        Value = value;
    }
}