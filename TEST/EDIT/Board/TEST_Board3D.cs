using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;

using inonego;

// ============================================================
/// <summary>
///
/// </summary>
// ============================================================
public class TEST_Board3D
{
    // ------------------------------------------------------------
    /// <summary>
    /// 테스트용 기물입니다.
    /// </summary>
    // ------------------------------------------------------------
    public class TestPiece
    {
        public string Name;
        public TestPiece() {}
        public TestPiece(string name) { Name = name; }
        public override string ToString() => Name;
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 테스트용 공간입니다.
    /// </summary>
    // ------------------------------------------------------------
    [Serializable]
    public class TestSpace : BoardSpace<TestPiece>
    {
        public TestSpace() {}
    }

    [Test]
    public void Board3D_01_크기_경계_테스트()
    {
        var board = new Board3D<TestSpace, TestPiece>(3, 2, 4);

        Assert.That(board.Width, Is.EqualTo(3));
        Assert.That(board.Height, Is.EqualTo(2));
        Assert.That(board.Depth, Is.EqualTo(4));
        Assert.That(board.Size, Is.EqualTo(new Vector3Int(3, 2, 4)));

        Assert.That(board[new Vector3Int(+0, +0, +0)], Is.Not.Null);
        Assert.That(board[new Vector3Int(+2, +1, +3)], Is.Not.Null);
        Assert.That(board[new Vector3Int(-1, +0, +0)], Is.Null);
        Assert.That(board[new Vector3Int(+3, +1, +0)], Is.Null);
        Assert.That(board[new Vector3Int(+2, +2, +0)], Is.Null);
        Assert.That(board[new Vector3Int(+0, +0, +4)], Is.Null);
    }

    [Test]
    public void Board3D_02_배치_및_인덱서_테스트()
    {
        var board = new Board3D<TestSpace, TestPiece>(2, 2, 2);
        var a = new TestPiece("A");
        var b = new TestPiece("B");

        var p0 = new Vector3Int(0, 0, 0);
        var p1 = new Vector3Int(1, 1, 1);

        board.Place(p0, a);
        Assert.That(board[p0].Placed, Is.EqualTo(a));
        Assert.That(board[a].HasValue, Is.True);

        // 기물로 접근 가능
        Assert.That(board[a].Value, Is.EqualTo(p0));

        // 같은 위치에 다른 기물은 불가 → InvalidPlacementException
        Assert.Throws<BoardBase.InvalidPlacementException>(() => board.Place(p0, b));

        // 다른 위치는 가능
        board.Place(p1, b);
        Assert.That(board[p1].Placed, Is.EqualTo(b));
        Assert.That(board[b].HasValue, Is.True);
    }

    [Test]
    public void Board3D_03_제거_공간_보존_테스트()
    {
        var board = new Board3D<TestSpace, TestPiece>(2, 2, 2);
        var c = new TestPiece("C");
        var p = new Vector3Int(1, 0, 1);

        board.Place(p, c);
        Assert.That(board[p].IsFull, Is.True);

        board.Remove(p);
        Assert.That(board[p], Is.Not.Null);
        Assert.That(board[p].IsFull, Is.False);
        Assert.That(board[c].HasValue, Is.False);

        // 다시 배치 후, 기물로 제거
        board.Place(p, c);
        board.Remove(c);
        Assert.That(board[p].IsFull, Is.False);
    }

    [Test]
    public void Board3D_04_가득참_상태_테스트()
    {
        var board = new Board3D<TestSpace, TestPiece>(2, 1, 1);
        Assert.That(board.IsAllSpaceFull, Is.False);

        board.Place(new Vector3Int(0, 0, 0), new TestPiece("X"));
        board.Place(new Vector3Int(1, 0, 0), new TestPiece("Y"));
        Assert.That(board.IsAllSpaceFull, Is.True);

        board.Remove(new Vector3Int(0, 0, 0));
        Assert.That(board.IsAllSpaceFull, Is.False);
    }

    [Test]
    public void Board3D_05_공간_추가_제거_및_인덱스_동기화_테스트()
    {
        var board = new Board3D<TestSpace, TestPiece>(1, 1, 1);
        var pt = new Vector3Int(0, 0, 0);

        // 기본 공간 존재
        Assert.That(board[pt], Is.Not.Null);

        // 제거 후 접근 불가
        board.RemoveSpace(pt);
        Assert.That(board[pt], Is.Null);

        // 다시 추가
        board.AddSpace(pt);
        Assert.That(board[pt], Is.Not.Null);

        // 공간에 기물을 넣고 제거하면 역인덱스도 비워짐
        var z = new TestPiece("Z");
        board.Place(pt, z);
        Assert.That(board[z].HasValue, Is.True);
        board.RemoveSpace(pt);
        Assert.That(board[z].HasValue, Is.False);
    }

    [Test]
    public void Board3D_06_범위밖_배치시_SpaceNotFound_예외()
    {
        var piece = new TestPiece("O");

        var board = new Board3D<TestSpace, TestPiece>(2, 2, 2);
        var point = new Vector3Int(5, 5, 5);

        Assert.Throws<BoardBase.SpaceNotFoundException>(() => board.Place(point, piece));
    }

    [Test]
    public void Board3D_07_중복_AddSpace시_SpaceAlreadyExists_예외()
    {
        var board = new Board3D<TestSpace, TestPiece>(1, 1, 1);
        var point = new Vector3Int(0, 0, 0);

        // 생성자에서 이미 공간 생성됨 → 중복 추가 시 예외
        Assert.Throws<BoardBase.SpaceAlreadyExistsException>(() => board.AddSpace(point));
    }
}



