using System;

public static class SkillFactory
{
    public static ISkill CreateSkill(SkillSO so)
    {
        var typeName = "Skill" + so.skillType.ToString();
        var type = Type.GetType(typeName);
        if (type == null)
            throw new ArgumentException("Unknown skill type: " + so.skillType);

        return type.GetConstructor(new[] { typeof(SkillSO) })?.Invoke(new object[] { so }) as ISkill
            ?? throw new ArgumentException("Could not create skill of type: " + typeName);
    }
}
