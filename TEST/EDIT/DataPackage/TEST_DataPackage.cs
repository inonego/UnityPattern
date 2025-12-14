using System;

using NUnit.Framework;

using UnityEngine;

using inonego;

// ============================================================================
/// <summary>
/// DataPackage 시스템의 핵심 기능 테스트 클래스입니다.
/// </summary>
// ============================================================================
public class TEST_DataPackage
{

#region 테스트 설정 및 정리

    // ------------------------------------------------------------
    /// <summary>
    /// 각 테스트 전에 정적 상태를 정리합니다.
    /// </summary>
    // ------------------------------------------------------------
    [SetUp]
    public void SetUp()
    {
        DataPackage.Release();
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 각 테스트 후에 정적 상태를 정리합니다.
    /// </summary>
    // ------------------------------------------------------------
    [TearDown]
    public void TearDown()
    {
        DataPackage.Release();
    }

#endregion

#region DataPackage 기본 기능 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// DataPackage의 기본 생성 및 초기 상태를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void DataPackage_01_기본_생성_테스트()
    {
        // Arrange & Act
        var package = new DataPackage();

        // Assert
        Assert.IsNotNull(package);
        Assert.IsNull(DataPackage.Loaded, "Load하기 전까지는 Loaded가 null이어야 합니다");
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 테이블 추가 기능을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void DataPackage_02_테이블_추가_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var package = new DataPackage();
        var testTable = new TestTable();
        
        var testData1 = new TestData { Key = "key1", Value = 100 };
        var testData2 = new TestData { Key = "key2", Value = 200 };
        
        testTable.Dictionary.Add("key1", testData1);
        testTable.Dictionary.Add("key2", testData2);

        // ------------------------------------------------------------
        // 테이블 추가
        // ------------------------------------------------------------
        package.AddTable<TestTable, TestData>(testTable);

        var readData1 = package.Read<TestData>("key1");
        var readData2 = package.Read<TestData>("key2");

        Assert.IsNotNull(readData1);
        Assert.IsNotNull(readData2);
        Assert.AreEqual("key1", readData1.Key);
        Assert.AreEqual("key2", readData2.Key);
        Assert.AreEqual(100, readData1.Value);
        Assert.AreEqual(200, readData2.Value);
        
        // ------------------------------------------------------------
        // 중복 테이블 추가 시 예외
        // ------------------------------------------------------------
        var duplicateTable = new TestTable();
        
        Assert.Throws<InvalidOperationException>(() => package.AddTable<TestTable, TestData>(duplicateTable));
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 테이블 읽기 기능을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void DataPackage_03_테이블_읽기_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var package = new DataPackage();
        var testTable = new TestTable();
        
        var testData = new TestData { Key = "key1", Value = 100 };
        testTable.Dictionary.Add("key1", testData);
        package.AddTable<TestTable, TestData>(testTable);

        // ------------------------------------------------------------
        // 정상 읽기
        // ------------------------------------------------------------
        var readData = package.Read<TestData>("key1");
        Assert.IsNotNull(readData);
        Assert.AreEqual("key1", readData.Key);
        Assert.AreEqual(100, readData.Value);
        
        // ------------------------------------------------------------
        // 존재하지 않는 키로 읽기 시 null 반환
        // ------------------------------------------------------------
        var result = package.Read<TestData>("nonexistent_key");
        Assert.IsNull(result, "존재하지 않는 키로 읽기 시 null이 반환되어야 합니다");
        
        // ------------------------------------------------------------
        // 존재하지 않는 테이블 읽기 시 예외
        // ------------------------------------------------------------
        package.RemoveTable<TestData>();
        Assert.Throws<InvalidOperationException>(() => package.Read<TestData>("key1"));
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 테이블 제거 기능을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void DataPackage_04_테이블_제거_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var package = new DataPackage();
        var testTable = new TestTable();
        
        var testData = new TestData { Key = "key1", Value = 100 };
        testTable.Dictionary.Add("key1", testData);
        package.AddTable<TestTable, TestData>(testTable);

        var readData = package.Read<TestData>("key1");
        Assert.IsNotNull(readData);

        // ------------------------------------------------------------
        // 테이블 제거
        // ------------------------------------------------------------
        package.RemoveTable<TestData>();

        // ------------------------------------------------------------
        // 제거 후 읽기 실패
        // ------------------------------------------------------------
        Assert.Throws<InvalidOperationException>(() => package.Read<TestData>("key1"));
        
        // ------------------------------------------------------------
        // 존재하지 않는 테이블 제거 시 예외
        // ------------------------------------------------------------
        Assert.Throws<InvalidOperationException>(() => package.RemoveTable<TestData>());
    }

#endregion

#region 정적 메서드 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// Load/Release 기능을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void DataPackage_05_Load_Release_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var package = new DataPackage();
        
        var testTable = new TestTable();
        var testData = new TestData { Key = "key1", Value = 100 };
        testTable.Dictionary.Add("key1", testData);
        package.AddTable<TestTable, TestData>(testTable);

        // ------------------------------------------------------------
        // Load 기능
        // ------------------------------------------------------------
        DataPackage.Load(package);
        Assert.AreEqual(package, DataPackage.Loaded);
        
        var readData = DataPackage.Loaded.Read<TestData>("key1");
        Assert.IsNotNull(readData);
        Assert.AreEqual(100, readData.Value);
        
        // ------------------------------------------------------------
        // 중복 로드 시 예외 발생
        // ------------------------------------------------------------
        var package2 = new DataPackage();
        Assert.Throws<InvalidOperationException>(() => DataPackage.Load(package2));
        
        // ------------------------------------------------------------
        // Release 기능
        // ------------------------------------------------------------
        DataPackage.Release();
        Assert.IsNull(DataPackage.Loaded);
        
        // ------------------------------------------------------------
        // Release 후 접근 시 예외
        // ------------------------------------------------------------
        Assert.Throws<NullReferenceException>(() => DataPackage.Loaded.Read<TestData>("key1"));
    }

#endregion

#region 다중 타입 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// 다중 타입 테이블 관리 기능을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void DataPackage_06_다중_타입_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var package = new DataPackage();
        
        var testTable1 = new TestTable();
        var testTable2 = new TestTable2();
        
        var testData1 = new TestData { Key = "key1", Value = 100 };
        var testData2 = new TestData2 { Key = "key2", Name = "TestName" };
        
        testTable1.Dictionary.Add("key1", testData1);
        testTable2.Dictionary.Add("key2", testData2);

        // ------------------------------------------------------------
        // 다중 타입 테이블 추가
        // ------------------------------------------------------------
        package.AddTable<TestTable, TestData>(testTable1);
        package.AddTable<TestTable2, TestData2>(testTable2);

        var readData1 = package.Read<TestData>("key1");
        var readData2 = package.Read<TestData2>("key2");

        Assert.IsNotNull(readData1);
        Assert.IsNotNull(readData2);
        Assert.AreEqual(100, readData1.Value);
        Assert.AreEqual("TestName", readData2.Name);
    }

#endregion

#region 직렬화 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// DataPackage 클래스의 JSON 직렬화/역직렬화를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void DataPackage_07_JSON_직렬화_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var originalPackage = new DataPackage();
        var testTable = new TestTable();
        
        var testData1 = new TestData { Key = "key1", Value = 100 };
        var testData2 = new TestData { Key = "key2", Value = 200 };
        
        testTable.Dictionary.Add("key1", testData1);
        testTable.Dictionary.Add("key2", testData2);
        originalPackage.AddTable<TestTable, TestData>(testTable);

        // ------------------------------------------------------------
        // JSON 직렬화/역직렬화
        // ------------------------------------------------------------
        string json = JsonUtility.ToJson(originalPackage, prettyPrint: true);
        Debug.Log($"=== JSON 직렬화 결과 ===");
        Debug.Log($"JSON 길이: {json.Length} 문자");
        Debug.Log($"JSON 내용:\n{json}");
        
        var deserializedPackage = JsonUtility.FromJson<DataPackage>(json);
        Debug.Log($"역직렬화 성공: {deserializedPackage != null}");
        
        Assert.IsNotNull(deserializedPackage);
        
        var readData1 = deserializedPackage.Read<TestData>("key1");
        var readData2 = deserializedPackage.Read<TestData>("key2");
        
        Debug.Log($"복원된 데이터1: Key={readData1?.Key}, Value={readData1?.Value}");
        Debug.Log($"복원된 데이터2: Key={readData2?.Key}, Value={readData2?.Value}");
        
        Assert.IsNotNull(readData1);
        Assert.IsNotNull(readData2);
        Assert.AreEqual(100, readData1.Value);
        Assert.AreEqual(200, readData2.Value);
    }

#endregion

#region 통합 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// DataPackage의 전체적인 사용 시나리오를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void DataPackage_08_통합_사용_시나리오_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var package = new DataPackage();
        
        var playerTable = new TestTable();
        var itemTable = new TestTable2();
        
        var player1 = new TestData { Key = "player1", Value = 1000 };
        var player2 = new TestData { Key = "player2", Value = 1500 };
        playerTable.Dictionary.Add("player1", player1);
        playerTable.Dictionary.Add("player2", player2);
        
        var item1 = new TestData2 { Key = "item1", Name = "Sword" };
        var item2 = new TestData2 { Key = "item2", Name = "Shield" };
        itemTable.Dictionary.Add("item1", item1);
        itemTable.Dictionary.Add("item2", item2);
        
        // ------------------------------------------------------------
        // 테이블 추가
        // ------------------------------------------------------------
        package.AddTable<TestTable, TestData>(playerTable);
        package.AddTable<TestTable2, TestData2>(itemTable);
        
        // ------------------------------------------------------------
        // 패키지 로드
        // ------------------------------------------------------------
        DataPackage.Load(package);
        
        var loadedPlayer1 = DataPackage.Loaded.Read<TestData>("player1");
        var loadedPlayer2 = DataPackage.Loaded.Read<TestData>("player2");
        var loadedItem1 = DataPackage.Loaded.Read<TestData2>("item1");
        var loadedItem2 = DataPackage.Loaded.Read<TestData2>("item2");
        
        Assert.AreEqual(1000, loadedPlayer1.Value);
        Assert.AreEqual(1500, loadedPlayer2.Value);
        Assert.AreEqual("Sword", loadedItem1.Name);
        Assert.AreEqual("Shield", loadedItem2.Name);
        
        // ------------------------------------------------------------
        // 테이블 제거
        // ------------------------------------------------------------
        package.RemoveTable<TestData>();
        
        Assert.Throws<InvalidOperationException>(() => DataPackage.Loaded.Read<TestData>("player1"));
        
        var remainingItem = DataPackage.Loaded.Read<TestData2>("item1");
        Assert.AreEqual("Sword", remainingItem.Name);
        
        // ------------------------------------------------------------
        // 패키지 해제
        // ------------------------------------------------------------
        DataPackage.Release();
        
        Assert.Throws<NullReferenceException>(() => DataPackage.Loaded.Read<TestData2>("item1"));
    }

#endregion

#region 테스트용 클래스들

    // ------------------------------------------------------------
    /// <summary>
    /// 테스트용 데이터 클래스
    /// </summary>
    // ------------------------------------------------------------
    [Serializable]
    private class TestData : ITableValue
    {
        [SerializeField]
        private string key;
        public string Key 
        { 
            get => key;
            set => key = value;
        }
        
        public int Value; 
        public bool HasKey => !string.IsNullOrEmpty(Key);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 테스트용 데이터 클래스 2
    /// </summary>
    // ------------------------------------------------------------
    [Serializable]
    private class TestData2 : ITableValue
    {
        [SerializeField]
        private string key;
        public string Key 
        { 
            get => key; 
            set => key = value; 
        }

        public string Name;
        public bool HasKey => !string.IsNullOrEmpty(Key);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 테스트용 테이블 클래스
    /// </summary>
    // ------------------------------------------------------------
    [Serializable]
    private class TestTable : Table_V<TestData> {}

    // ------------------------------------------------------------
    /// <summary>
    /// 테스트용 테이블 클래스 2
    /// </summary>
    // ------------------------------------------------------------
    [Serializable]
    private class TestTable2 : Table_V<TestData2> {}

#endregion

}