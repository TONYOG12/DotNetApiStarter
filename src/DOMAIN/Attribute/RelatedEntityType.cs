namespace DOMAIN.Attribute;

[AttributeUsage(AttributeTargets.Property)]
public class RelatedEntityTypeAttribute(Type relatedType) : System.Attribute
{
    public Type RelatedType { get; } = relatedType;
}
