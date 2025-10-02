using inonego;

using NUnit.Framework;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.TestTools;

using UnityEngine;

public class Test_GroundChecker2D
{
    [UnityTest]
    public IEnumerator Test_GroundChecker2D_01()
    {
        var go = new GameObject("GroundChecker2D 테스트용");

        var rigid = go.AddComponent<Rigidbody2D>();

        var groundChecker = new GroundChecker2D(rigid);

        for (int i = 0; i < 1000; i++)
        {
            groundChecker.Check();

            yield return null;

            Debug.Log($"[{i}] {rigid.linearVelocity}");
        }
    }
}
