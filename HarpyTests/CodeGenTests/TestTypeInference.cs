using Harpy.CodeGen;

namespace HarpyTests.CodeGenTests;

[TestClass]
public class TestTypeInference
{
    [TestMethod]
    public void TestInferType_NullOrEmpty_ReturnsDynamic()
    {
        Assert.AreEqual("dynamic", TypeInference.InferType(null!));
        Assert.AreEqual("dynamic", TypeInference.InferType(""));
    }

    [TestMethod]
    public void TestInferType_BooleanPrefix_ReturnsBool()
    {
        Assert.AreEqual("bool", TypeInference.InferType("lFlag"));
        Assert.AreEqual("bool", TypeInference.InferType("lIsValid"));
        Assert.AreEqual("bool", TypeInference.InferType("lActive"));
    }

    [TestMethod]
    public void TestInferType_NumericPrefix_ReturnsDouble()
    {
        Assert.AreEqual("double", TypeInference.InferType("nValue"));
        Assert.AreEqual("double", TypeInference.InferType("nAmount"));
        Assert.AreEqual("double", TypeInference.InferType("nTotal"));
    }

    [TestMethod]
    public void TestInferType_StringPrefix_ReturnsString()
    {
        Assert.AreEqual("string", TypeInference.InferType("cName"));
        Assert.AreEqual("string", TypeInference.InferType("cDescription"));
        Assert.AreEqual("string", TypeInference.InferType("cText"));
    }

    [TestMethod]
    public void TestInferType_DatePrefix_ReturnsDateTime()
    {
        Assert.AreEqual("DateTime", TypeInference.InferType("dBirthDate"));
        Assert.AreEqual("DateTime", TypeInference.InferType("dCreated"));
        Assert.AreEqual("DateTime", TypeInference.InferType("dToday"));
    }

    [TestMethod]
    public void TestInferType_HashPrefix_ReturnsDictionary()
    {
        Assert.AreEqual("Dictionary<string, dynamic>", TypeInference.InferType("hMap"));
        Assert.AreEqual("Dictionary<string, dynamic>", TypeInference.InferType("hData"));
        Assert.AreEqual("Dictionary<string, dynamic>", TypeInference.InferType("hConfig"));
    }

    [TestMethod]
    public void TestInferType_ArrayPrefix_ReturnsList()
    {
        Assert.AreEqual("List<dynamic>", TypeInference.InferType("aItems"));
        Assert.AreEqual("List<dynamic>", TypeInference.InferType("aValues"));
        Assert.AreEqual("List<dynamic>", TypeInference.InferType("aData"));
    }

    [TestMethod]
    public void TestInferType_ObjectPrefix_ReturnsDynamic()
    {
        Assert.AreEqual("dynamic", TypeInference.InferType("oObject"));
        Assert.AreEqual("dynamic", TypeInference.InferType("oInstance"));
        Assert.AreEqual("dynamic", TypeInference.InferType("oItem"));
    }

    [TestMethod]
    public void TestInferType_PointerPrefix_ReturnsIntPtr()
    {
        // Note: "pHandle" contains "handle" keyword, so it returns int
        // Testing with "pAddr" which doesn't contain int keywords
        Assert.AreEqual("IntPtr", TypeInference.InferType("pAddr"));
        Assert.AreEqual("IntPtr", TypeInference.InferType("pPtr"));
        Assert.AreEqual("IntPtr", TypeInference.InferType("pMem"));
    }

    [TestMethod]
    public void TestInferType_StaticBooleanPrefix_ReturnsBool()
    {
        Assert.AreEqual("bool", TypeInference.InferType("slFlag"));
        Assert.AreEqual("bool", TypeInference.InferType("slIsValid"));
        Assert.AreEqual("bool", TypeInference.InferType("slActive"));
    }

    [TestMethod]
    public void TestInferType_StaticNumericPrefix_ReturnsDouble()
    {
        Assert.AreEqual("double", TypeInference.InferType("snValue"));
        Assert.AreEqual("double", TypeInference.InferType("snCount"));
        Assert.AreEqual("double", TypeInference.InferType("snTotal"));
    }

    [TestMethod]
    public void TestInferType_StaticStringPrefix_ReturnsString()
    {
        Assert.AreEqual("string", TypeInference.InferType("scName"));
        Assert.AreEqual("string", TypeInference.InferType("scDescription"));
        Assert.AreEqual("string", TypeInference.InferType("scText"));
    }

    [TestMethod]
    public void TestInferType_StaticDatePrefix_ReturnsDateTime()
    {
        Assert.AreEqual("DateTime", TypeInference.InferType("sdBirthDate"));
        Assert.AreEqual("DateTime", TypeInference.InferType("sdCreated"));
        Assert.AreEqual("DateTime", TypeInference.InferType("sdToday"));
    }

    [TestMethod]
    public void TestInferType_StaticHashPrefix_ReturnsDictionary()
    {
        Assert.AreEqual("Dictionary<string, dynamic>", TypeInference.InferType("shMap"));
        Assert.AreEqual("Dictionary<string, dynamic>", TypeInference.InferType("shData"));
        Assert.AreEqual("Dictionary<string, dynamic>", TypeInference.InferType("shConfig"));
    }

    [TestMethod]
    public void TestInferType_StaticArrayPrefix_ReturnsList()
    {
        Assert.AreEqual("List<dynamic>", TypeInference.InferType("saItems"));
        Assert.AreEqual("List<dynamic>", TypeInference.InferType("saValues"));
        Assert.AreEqual("List<dynamic>", TypeInference.InferType("saData"));
    }

    [TestMethod]
    public void TestInferType_StaticObjectPrefix_ReturnsDynamic()
    {
        Assert.AreEqual("dynamic", TypeInference.InferType("soObject"));
        Assert.AreEqual("dynamic", TypeInference.InferType("soInstance"));
        Assert.AreEqual("dynamic", TypeInference.InferType("soItem"));
    }

    [TestMethod]
    public void TestInferType_StaticPointerPrefix_ReturnsIntPtr()
    {
        // Note: "spHandle" contains "handle" keyword, so it returns int
        // Testing with "spAddr" which doesn't contain int keywords
        Assert.AreEqual("IntPtr", TypeInference.InferType("spAddr"));
        Assert.AreEqual("IntPtr", TypeInference.InferType("spPtr"));
        Assert.AreEqual("IntPtr", TypeInference.InferType("spMem"));
    }

    [TestMethod]
    public void TestInferType_SingleCharacter_ReturnsInt()
    {
        Assert.AreEqual("int", TypeInference.InferType("i"));
        Assert.AreEqual("int", TypeInference.InferType("j"));
        Assert.AreEqual("int", TypeInference.InferType("k"));
        Assert.AreEqual("int", TypeInference.InferType("x"));
        Assert.AreEqual("int", TypeInference.InferType("y"));
    }

    [TestMethod]
    public void TestInferType_KeywordColumn_ReturnsInt()
    {
        Assert.AreEqual("int", TypeInference.InferType("column"));
        Assert.AreEqual("int", TypeInference.InferType("nColumn"));
        Assert.AreEqual("int", TypeInference.InferType("myColumn"));
        Assert.AreEqual("int", TypeInference.InferType("Column123"));
    }

    [TestMethod]
    public void TestInferType_KeywordCol_ReturnsInt()
    {
        Assert.AreEqual("int", TypeInference.InferType("col"));
        Assert.AreEqual("int", TypeInference.InferType("nCol"));
        Assert.AreEqual("int", TypeInference.InferType("myCol"));
        Assert.AreEqual("int", TypeInference.InferType("currentCol"));
    }

    [TestMethod]
    public void TestInferType_KeywordError_ReturnsInt()
    {
        Assert.AreEqual("int", TypeInference.InferType("error"));
        Assert.AreEqual("int", TypeInference.InferType("nError"));
        Assert.AreEqual("int", TypeInference.InferType("errorCode"));
        Assert.AreEqual("int", TypeInference.InferType("lastError"));
    }

    [TestMethod]
    public void TestInferType_KeywordHandle_ReturnsInt()
    {
        Assert.AreEqual("int", TypeInference.InferType("handle"));
        Assert.AreEqual("int", TypeInference.InferType("nHandle"));
        Assert.AreEqual("int", TypeInference.InferType("fileHandle"));
        Assert.AreEqual("int", TypeInference.InferType("windowHandle"));
    }

    [TestMethod]
    public void TestInferType_KeywordIndex_ReturnsInt()
    {
        Assert.AreEqual("int", TypeInference.InferType("index"));
        Assert.AreEqual("int", TypeInference.InferType("nIndex"));
        Assert.AreEqual("int", TypeInference.InferType("currentIndex"));
        Assert.AreEqual("int", TypeInference.InferType("arrayIndex"));
    }

    [TestMethod]
    public void TestInferType_KeywordLength_ReturnsInt()
    {
        Assert.AreEqual("int", TypeInference.InferType("length"));
        Assert.AreEqual("int", TypeInference.InferType("nLength"));
        Assert.AreEqual("int", TypeInference.InferType("stringLength"));
        Assert.AreEqual("int", TypeInference.InferType("maxLength"));
    }

    [TestMethod]
    public void TestInferType_KeywordLines_ReturnsInt()
    {
        Assert.AreEqual("int", TypeInference.InferType("lines"));
        Assert.AreEqual("int", TypeInference.InferType("nLines"));
        Assert.AreEqual("int", TypeInference.InferType("totalLines"));
        Assert.AreEqual("int", TypeInference.InferType("numLines"));
    }

    [TestMethod]
    public void TestInferType_KeywordRow_ReturnsInt()
    {
        Assert.AreEqual("int", TypeInference.InferType("row"));
        Assert.AreEqual("int", TypeInference.InferType("nRow"));
        Assert.AreEqual("int", TypeInference.InferType("currentRow"));
        Assert.AreEqual("int", TypeInference.InferType("rowIndex"));
    }

    [TestMethod]
    public void TestInferType_CaseInsensitiveKeywords_ReturnsInt()
    {
        Assert.AreEqual("int", TypeInference.InferType("INDEX"));
        Assert.AreEqual("int", TypeInference.InferType("Index"));
        Assert.AreEqual("int", TypeInference.InferType("ERROR"));
        Assert.AreEqual("int", TypeInference.InferType("Error"));
        Assert.AreEqual("int", TypeInference.InferType("COLUMN"));
        Assert.AreEqual("int", TypeInference.InferType("Column"));
    }

    [TestMethod]
    public void TestInferType_UnknownPrefix_ReturnsDynamic()
    {
        Assert.AreEqual("dynamic", TypeInference.InferType("xValue"));
        Assert.AreEqual("dynamic", TypeInference.InferType("zData"));
        Assert.AreEqual("dynamic", TypeInference.InferType("qItem"));
    }

    [TestMethod]
    public void TestIsHungarianNotation_ValidNotation_ReturnsTrue()
    {
        Assert.IsTrue(TypeInference.IsHungarianNotation("lFlag"));
        Assert.IsTrue(TypeInference.IsHungarianNotation("nValue"));
        Assert.IsTrue(TypeInference.IsHungarianNotation("cName"));
        Assert.IsTrue(TypeInference.IsHungarianNotation("dDate"));
        Assert.IsTrue(TypeInference.IsHungarianNotation("hMap"));
        Assert.IsTrue(TypeInference.IsHungarianNotation("aArray"));
        Assert.IsTrue(TypeInference.IsHungarianNotation("oObject"));
        Assert.IsTrue(TypeInference.IsHungarianNotation("pPointer"));
    }

    [TestMethod]
    public void TestIsHungarianNotation_StaticValidNotation_ReturnsTrue()
    {
        Assert.IsTrue(TypeInference.IsHungarianNotation("slFlag"));
        Assert.IsTrue(TypeInference.IsHungarianNotation("snValue"));
        Assert.IsTrue(TypeInference.IsHungarianNotation("scName"));
        Assert.IsTrue(TypeInference.IsHungarianNotation("sdDate"));
        Assert.IsTrue(TypeInference.IsHungarianNotation("shMap"));
        Assert.IsTrue(TypeInference.IsHungarianNotation("saArray"));
        Assert.IsTrue(TypeInference.IsHungarianNotation("soObject"));
        Assert.IsTrue(TypeInference.IsHungarianNotation("spPointer"));
    }

    [TestMethod]
    public void TestIsHungarianNotation_InvalidNotation_ReturnsFalse()
    {
        Assert.IsFalse(TypeInference.IsHungarianNotation("flag"));
        Assert.IsFalse(TypeInference.IsHungarianNotation("l"));
        Assert.IsFalse(TypeInference.IsHungarianNotation("ln"));
        Assert.IsFalse(TypeInference.IsHungarianNotation("xValue"));
        Assert.IsFalse(TypeInference.IsHungarianNotation(""));
        Assert.IsFalse(TypeInference.IsHungarianNotation(null!));
    }

    [TestMethod]
    public void TestGetDefaultValue_Bool_ReturnsFalse()
    {
        Assert.AreEqual("false", TypeInference.GetDefaultValue("bool"));
    }

    [TestMethod]
    public void TestGetDefaultValue_Double_ReturnsZero()
    {
        Assert.AreEqual("0.0", TypeInference.GetDefaultValue("double"));
    }

    [TestMethod]
    public void TestGetDefaultValue_Int_ReturnsZero()
    {
        Assert.AreEqual("0", TypeInference.GetDefaultValue("int"));
    }

    [TestMethod]
    public void TestGetDefaultValue_String_ReturnsEmptyString()
    {
        Assert.AreEqual("\"\"", TypeInference.GetDefaultValue("string"));
    }

    [TestMethod]
    public void TestGetDefaultValue_DateTime_ReturnsMinValue()
    {
        Assert.AreEqual("DateTime.MinValue", TypeInference.GetDefaultValue("DateTime"));
    }

    [TestMethod]
    public void TestGetDefaultValue_List_ReturnsNewList()
    {
        Assert.AreEqual("new List<dynamic>()", TypeInference.GetDefaultValue("List<dynamic>"));
    }

    [TestMethod]
    public void TestGetDefaultValue_Dictionary_ReturnsNewDictionary()
    {
        Assert.AreEqual("new Dictionary<string, dynamic>()", TypeInference.GetDefaultValue("Dictionary<string, dynamic>"));
    }

    [TestMethod]
    public void TestGetDefaultValue_IntPtr_ReturnsZero()
    {
        Assert.AreEqual("IntPtr.Zero", TypeInference.GetDefaultValue("IntPtr"));
    }

    [TestMethod]
    public void TestGetDefaultValue_Unknown_ReturnsNull()
    {
        Assert.AreEqual("null", TypeInference.GetDefaultValue("dynamic"));
        Assert.AreEqual("null", TypeInference.GetDefaultValue("CustomType"));
        Assert.AreEqual("null", TypeInference.GetDefaultValue("object"));
    }
}
