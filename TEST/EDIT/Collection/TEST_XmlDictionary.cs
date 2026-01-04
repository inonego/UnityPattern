using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

using NUnit.Framework;

using UnityEngine;

using inonego;
using inonego.Serializable;
using inonego.Serializable.Xml;

// ============================================================================
/// <summary>
/// XmlDictionary의 직렬화 및 역직렬화, 다형성 지원을 위한 테스트 클래스입니다.
/// </summary>
// ============================================================================
public class TEST_XmlDictionary
{

#region 테스트용 클래스 정의

    // ------------------------------------------------------------
    /// <summary>
    /// 테스트용 베이스 클래스 (IKeyable 구현)
    /// </summary>
    // ------------------------------------------------------------
    [Serializable]
    [XmlInclude(typeof(TestDerivedA))]
    [XmlInclude(typeof(TestDerivedB))]
    [XmlType("Base")]
    public class TestBase : IKeyable<string>
    {
        [XmlAttribute("_Key")]
        public string Key { get; set; }

        [XmlIgnore]
        string IKeyable<string>.Key => Key;
        [XmlIgnore]
        public bool HasKey => !string.IsNullOrEmpty(Key);

        [XmlElement("Data")]
        public string Data;

        public virtual void Log()
        {
            Debug.Log($"[XmlDictionary Test] Base: Key={Key}, Data={Data}");
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 테스트용 파생 클래스 A
    /// </summary>
    // ------------------------------------------------------------
    [Serializable]
    [XmlType("DerivedA")]
    public class TestDerivedA : TestBase
    {
        [XmlElement("ValueInt")]
        public int ValueInt;

        public override void Log()
        {
            Debug.Log($"[XmlDictionary Test] DerivedA: Key={Key}, Data={Data}, ValueInt={ValueInt}");
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 테스트용 파생 클래스 B
    /// </summary>
    // ------------------------------------------------------------
    [Serializable]
    [XmlType("DerivedB")]
    public class TestDerivedB : TestBase
    {
        [XmlElement("ValueString")]
        public string ValueString;

        public override void Log()
        {
            Debug.Log($"[XmlDictionary Test] DerivedB: Key={Key}, Data={Data}, ValueString={ValueString}");
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// XmlDictionary를 필드로 가지는 컨테이너 클래스
    /// </summary>
    // ------------------------------------------------------------
    [Serializable]
    [XmlRoot("Container")]
    public class TestContainer
    {
        [XmlElement("Items")]
        public XmlDictionary<string, TestBase> Items = new();
    }

#endregion

#region 통합 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// XmlDictionary의 Write(직렬화) 기능을 통합 테스트합니다.
    /// 다형성 객체들이 올바른 xsi:type과 함께 XML로 변환되는지 확인합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void XmlDictionary_01_Write_통합_테스트()
    {
        Debug.Log("---------- XmlDictionary Write Integration Test Start ----------");

        // ------------------------------------------------------------
        // 1. 데이터 준비
        // ------------------------------------------------------------
        var container = new TestContainer();
        
        container.Items.Add("K_BASE", new TestBase { Key = "K_BASE", Data = "BaseData" });
        container.Items.Add("K_A", new TestDerivedA { Key = "K_A", Data = "AData", ValueInt = 100 });
        container.Items.Add("K_B", new TestDerivedB { Key = "K_B", Data = "BData", ValueString = "Hello" });

        Debug.Log($"[1] Prepared items in container: {container.Items.Count}");

        // ------------------------------------------------------------
        // 2. XML 직렬화
        // ------------------------------------------------------------
        string xml = "";
        XmlSerializer serializer = new XmlSerializer(typeof(TestContainer));
        
        using (var sw = new StringWriter())
        {
            var settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };
            using (var writer = XmlWriter.Create(sw, settings))
            {
                serializer.Serialize(writer, container);
            }
            xml = sw.ToString();
        }

        Debug.Log($"[2] Serialized XML Output:\n{xml}");

        // ------------------------------------------------------------
        // 3. XML 검증 (문자열 포함 여부)
        // ------------------------------------------------------------
        Assert.IsTrue(xml.Contains("<Items>"), "컨테이너 내의 'Items' 요소가 존재해야 합니다.");
        Assert.IsTrue(xml.Contains("<Item"), "아이템 요소 이름이 'Item'이어야 합니다.");
        Assert.IsTrue(xml.Contains("_Key=\"K_BASE\""), "K_BASE 키 애트리뷰트가 포함되어야 합니다.");
        Assert.IsTrue(xml.Contains("type=\"DerivedA\""), "DerivedA의 type 정보가 포함되어야 합니다.");
        Assert.IsTrue(xml.Contains("type=\"DerivedB\""), "DerivedB의 type 정보가 포함되어야 합니다.");
        Assert.IsTrue(xml.Contains("<ValueInt>100</ValueInt>"), "DerivedA의 고유 데이터가 포함되어야 합니다.");
        Assert.IsTrue(xml.Contains("<ValueString>Hello</ValueString>"), "DerivedB의 고유 데이터가 포함되어야 합니다.");

        Debug.Log("---------- XmlDictionary Write Integration Test Success ----------");
    }

    // ------------------------------------------------------------
    /// <summary>
    /// XmlDictionary의 Read(역직렬화) 기능을 통합 테스트합니다.
    /// XML의 xsi:type을 기반으로 실제 파생 클래스 인스턴스가 올바르게 생성되는지 확인합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void XmlDictionary_02_Read_통합_테스트()
    {
        Debug.Log("---------- XmlDictionary Read Integration Test Start ----------");

        // ------------------------------------------------------------
        // 1. 테스트용 XML 문자열 준비 (컨테이너 포함)
        // ------------------------------------------------------------
        string xml = @"
<Container xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
  <Items>
    <Item _Key='K1' xsi:type='Base'>
      <Data>BaseOnly</Data>
    </Item>
    <Item _Key='K2' xsi:type='DerivedA'>
      <Data>AData</Data>
      <ValueInt>777</ValueInt>
    </Item>
    <Item _Key='K3' xsi:type='DerivedB'>
      <Data>BData</Data>
      <ValueString>World</ValueString>
    </Item>
  </Items>
</Container>";

        // ------------------------------------------------------------
        // 2. XML 역직렬화
        // ------------------------------------------------------------
        XmlSerializer serializer = new XmlSerializer(typeof(TestContainer));
        TestContainer deserialized;

        using (var sr = new StringReader(xml))
        {
            deserialized = (TestContainer)serializer.Deserialize(sr);
        }

        var items = deserialized.Items;
        Debug.Log($"[1] Deserialized items count: {items.Count}");

        // ------------------------------------------------------------
        // 3. 검증 및 로그 출력
        // ------------------------------------------------------------
        Assert.AreEqual(3, items.Count);

        // K1: Base
        Assert.IsInstanceOf<TestBase>(items["K1"]);
        Assert.IsNotInstanceOf<TestDerivedA>(items["K1"]);
        Assert.AreEqual("BaseOnly", items["K1"].Data);
        items["K1"].Log();

        // K2: DerivedA
        Assert.IsInstanceOf<TestDerivedA>(items["K2"]);
        Assert.AreEqual(777, ((TestDerivedA)items["K2"]).ValueInt);
        items["K2"].Log();

        // K3: DerivedB
        Assert.IsInstanceOf<TestDerivedB>(items["K3"]);
        Assert.AreEqual("World", ((TestDerivedB)items["K3"]).ValueString);
        items["K3"].Log();

        Debug.Log("---------- XmlDictionary Read Integration Test Success ----------");
    }

#endregion

}
