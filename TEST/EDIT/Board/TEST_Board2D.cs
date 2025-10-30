using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;

using inonego;

// ============================================================
/// <summary>
/// Board2D 시스템의 핵심 기능 테스트 클래스입니다.
/// </summary>
// ============================================================
public class TEST_Board2D
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
    public void Board2D_01_크기_경계_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var board = new Board2D<TestSpace, TestPiece>(3, 2);

        // ------------------------------------------------------------
        // 크기 확인
        // ------------------------------------------------------------
        Assert.That(board.Width, Is.EqualTo(3));
        Assert.That(board.Height, Is.EqualTo(2));
        Assert.That(board.Size, Is.EqualTo(new Vector2Int(3, 2)));

        // ------------------------------------------------------------
        // 경계 확인
        // ------------------------------------------------------------
        Assert.That(board[new Vector2Int(+0, +0)], Is.Not.Null);
        Assert.That(board[new Vector2Int(+2, +1)], Is.Not.Null);
        Assert.That(board[new Vector2Int(-1, +0)], Is.Null);
        Assert.That(board[new Vector2Int(+3, +1)], Is.Null);
        Assert.That(board[new Vector2Int(+2, +2)], Is.Null);
    }

    [Test]
    public void Board2D_02_배치_및_인덱서_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var board = new Board2D<TestSpace, TestPiece>(2, 2);
        var a = new TestPiece("A");
        var b = new TestPiece("B");

        var p0 = new Vector2Int(0, 0);
        var p1 = new Vector2Int(1, 1);

        // ------------------------------------------------------------
        // 배치 및 인덱서 확인
        // ------------------------------------------------------------
        board.Place(p0, a);
        Assert.That(board[p0].Placed, Is.EqualTo(a));
        Assert.That(board[a].HasValue, Is.True);
        Assert.That(board[a].Value, Is.EqualTo(p0));

        // ------------------------------------------------------------
        // 같은 위치에 다른 기물 배치 시 예외
        // ------------------------------------------------------------
        Assert.Throws<BoardBase.InvalidPlacementException>(() => board.Place(p0, b));

        // ------------------------------------------------------------
        // 다른 위치 배치
        // ------------------------------------------------------------
        board.Place(p1, b);
        Assert.That(board[p1].Placed, Is.EqualTo(b));
        Assert.That(board[b].HasValue, Is.True);
    }

    [Test]
    public void Board2D_03_제거_공간_보존_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var board = new Board2D<TestSpace, TestPiece>(2, 2);
        var c = new TestPiece("C");
        var p = new Vector2Int(1, 0);
        board.Place(p, c);

        // ------------------------------------------------------------
        // 위치로 제거
        // ------------------------------------------------------------
        board.Remove(p);
        Assert.That(board[p], Is.Not.Null);
        Assert.That(board[p].IsFull, Is.False);
        Assert.That(board[c].HasValue, Is.False);

        // ------------------------------------------------------------
        // 다시 배치 후 기물로 제거
        // ------------------------------------------------------------
        board.Place(p, c);
        board.Remove(c);
        Assert.That(board[p].IsFull, Is.False);
    }

    [Test]
    public void Board2D_04_가득참_상태_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var board = new Board2D<TestSpace, TestPiece>(2, 1);
        Assert.That(board.IsAllSpaceFull, Is.False);

        // ------------------------------------------------------------
        // 모든 공간 채우기
        // ------------------------------------------------------------
        board.Place(new Vector2Int(0, 0), new TestPiece("X"));
        board.Place(new Vector2Int(1, 0), new TestPiece("Y"));
        Assert.That(board.IsAllSpaceFull, Is.True);

        // ------------------------------------------------------------
        // 한 공간 제거
        // ------------------------------------------------------------
        board.Remove(new Vector2Int(0, 0));
        Assert.That(board.IsAllSpaceFull, Is.False);
    }

    [Test]
    public void Board2D_05_공간_추가_제거_및_인덱스_동기화_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var board = new Board2D<TestSpace, TestPiece>(1, 1);
        var pt = new Vector2Int(0, 0);

        // ------------------------------------------------------------
        // 기본 공간 확인
        // ------------------------------------------------------------
        Assert.That(board[pt], Is.Not.Null);

        // ------------------------------------------------------------
        // 공간 제거
        // ------------------------------------------------------------
        board.RemoveSpace(pt);
        Assert.That(board[pt], Is.Null);

        // ------------------------------------------------------------
        // 공간 다시 추가
        // ------------------------------------------------------------
        board.AddSpace(pt);
        Assert.That(board[pt], Is.Not.Null);

        // ------------------------------------------------------------
        // 인덱스 동기화 확인
        // ------------------------------------------------------------
        var z = new TestPiece("Z");
        board.Place(pt, z);
        Assert.That(board[z].HasValue, Is.True);
        board.RemoveSpace(pt);
        Assert.That(board[z].HasValue, Is.False);
    }

    [Test]
    public void Board2D_06_범위밖_배치시_SpaceNotFound_예외()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var board = new Board2D<TestSpace, TestPiece>(2, 2);
        var piece = new TestPiece("O");
        var point = new Vector2Int(5, 5);

        // ------------------------------------------------------------
        // 범위 밖 배치 시 예외
        // ------------------------------------------------------------
        Assert.Throws<BoardBase.SpaceNotFoundException>(() => board.Place(point, piece));
    }

    [Test]
    public void Board2D_07_중복_AddSpace시_SpaceAlreadyExists_예외()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var board = new Board2D<TestSpace, TestPiece>(1, 1);
        var point = new Vector2Int(0, 0);

        // ------------------------------------------------------------
        // 중복 공간 추가 시 예외
        // ------------------------------------------------------------
        Assert.Throws<BoardBase.SpaceAlreadyExistsException>(() => board.AddSpace(point));
    }
}


