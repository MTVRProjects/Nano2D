//================================================
//描述 ： 
//作者 ：
//创建时间 ：2018/07/06 14:33:33  
//版本： 
//================================================

public class CodeGenerator
{
    const string NameSpaceMark = "#NAMESPACE#";
    const string ClassMark = "#CLASS#";
    const string FieldMark = "#FIELD#";

    const string ClassScriptTemplate1 = @"
namespace #NAMESPACE#
{
    public partial class #CLASS#
    {
#FIELD#
    }
}
";
    const string ClassScriptTemplate2 = @"
public partial class #CLASS#
{
#FIELD#
}
";
    /// <summary>
    /// 引用列表
    /// </summary>
    public string[] imports = new string[] { "System.Collections", "System.Collections.Generic", "UnityEngine" };
    public string nameSpace = "NameSpace";
    public string className = "ClassName";
    public string field = "";

    public string GenCode()
    {
        if (string.IsNullOrEmpty(className))
        {
            return string.Empty;
        }
        string code = null;
        if (string.IsNullOrEmpty(nameSpace) == false)
        {
            code = ClassScriptTemplate1.Replace(NameSpaceMark, nameSpace);
        }
        else
        {
            code = ClassScriptTemplate2;
        }

        code = code.Replace(ClassMark, className);
        code = code.Replace(FieldMark, field);

        return GetImports() + code;
    }

    public string GenDataBaseSingleton(bool initFuncs = true)
    {
        if (string.IsNullOrEmpty(className))
        {
            return string.Empty;
        }
        string code = null;
        if (string.IsNullOrEmpty(nameSpace) == false)
        {
            code = ClassScriptTemplate1.Replace(NameSpaceMark, nameSpace);
        }
        else
        {
            code = ClassScriptTemplate2;
        }

        code = code.Replace(ClassMark, string.Format("{0} : Singleton<{1}>", className, className));
        var fieldCode = initFuncs ? field +
@"

protected override void Init()
{
}
protected override void UnInit()
{
}
" : field;
        code = code.Replace(FieldMark, fieldCode);

        var retCode = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.UTF8.GetBytes(GetImports() + code));
        return retCode;
    }

    public string GetImports()
    {
        if (imports != null)
        {
            string importNames = string.Empty;
            for (int i = 0; i < imports.Length; i++)
            {
                importNames += "using " + imports[i] + ";\n";
            }
            return importNames;
        }
        return null;
    }

}
