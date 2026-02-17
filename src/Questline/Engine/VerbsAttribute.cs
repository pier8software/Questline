namespace Questline.Engine;

public class VerbsAttribute(params string[] verbs) : Attribute
{
    public string[] Verbs => verbs;
}
