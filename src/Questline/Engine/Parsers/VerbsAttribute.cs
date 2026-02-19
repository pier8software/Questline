namespace Questline.Engine.Parsers;

public class VerbsAttribute(params string[] verbs) : Attribute
{
    public string[] Verbs => verbs;
}
