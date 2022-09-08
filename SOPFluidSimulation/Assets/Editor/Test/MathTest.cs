using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using EulerFluidEngine;


//为了测试极限情况，采用了尽可能大的维度，也就是允许的Group数65535 * 允许的Thread数1024
//问题1赋值的时候简单赋值当前索引会导致浮点数尾数位不够，注意不是超过了浮点数所能表示的最大值而是尾数位不够，因此采用m_Coefficient去限制赋值
//问题2维度过大导致判断循环过长，机器扛不住，因此%m_LimitingFactor是限制一下循环次数
public class MathTest
{
    float m_Coefficient = 1e-5f;
    int m_LoopLimitingFactor = 64;
    int m_LoopJudgeFactor = 3;
    int m_AssignmentLimitingFactor = 55;

    Vector3Int m_Resolution;
    int m_TotalDim;

    int TestDimA = 512;
    int TestDimB = 512;

    float[] m_SrcVectorValueA;
    float[] m_SrcVectorValueB;
    float[] m_DstVectorValueA;
    float[] m_DstVectorValueB;

    ComputeBuffer m_SrcVectorA;
    ComputeBuffer m_SrcVectorB;
    ComputeBuffer m_DstVectorA;
    ComputeBuffer m_DstVectorB;

    private void __init(Vector3Int vResolution)
    {
        CGlobalMacroAndFunc.init();

        m_Resolution = vResolution;
        m_TotalDim = m_Resolution.x * m_Resolution.y * m_Resolution.z;

        m_SrcVectorValueA = new float[m_TotalDim];
        m_SrcVectorValueB = new float[m_TotalDim];
        m_DstVectorValueA = new float[m_TotalDim];
        m_DstVectorValueB = new float[m_TotalDim];

        m_SrcVectorA = new ComputeBuffer(m_TotalDim, sizeof(float));
        m_SrcVectorB = new ComputeBuffer(m_TotalDim, sizeof(float));
        m_DstVectorA = new ComputeBuffer(m_TotalDim, sizeof(float));
        m_DstVectorB = new ComputeBuffer(m_TotalDim, sizeof(float));
    }
    private void __free()
    {
        CGlobalMacroAndFunc.free();

        m_SrcVectorValueA = null;
        m_SrcVectorValueB = null;
        m_DstVectorValueA = null;
        m_DstVectorValueB = null;

        if (m_SrcVectorA != null)
        {
            m_SrcVectorA.Release();
        }

        if (m_SrcVectorB != null)
        {
            m_SrcVectorB.Release();
        }

        if (m_DstVectorA != null)
        {
            m_DstVectorA.Release();
        }

        if (m_DstVectorB != null)
        {
            m_DstVectorB.Release();
        }
    }

    [Test]
    public void MathTest_Copy()
    {
        __init(new Vector3Int(TestDimA, TestDimB, 1));

        for (int i = 0; i < m_TotalDim; i++)
        {
            float TempValue = i % m_AssignmentLimitingFactor + 1;
            m_SrcVectorValueA[i] = TempValue;
            m_DstVectorValueA[i] = 2 * TempValue;
        }

        m_SrcVectorA.SetData(m_SrcVectorValueA);
        m_DstVectorA.SetData(m_DstVectorValueA);

        CMathTool.copy(m_SrcVectorA, m_DstVectorA);

        m_DstVectorA.GetData(m_DstVectorValueA);

        for (int i = 0; i < m_TotalDim; i++)
        {
            if (i % m_LoopLimitingFactor == m_LoopJudgeFactor)
            {
                float TempValue = i % m_AssignmentLimitingFactor + 1;
                Assert.IsTrue(Mathf.Abs(m_DstVectorValueA[i] - TempValue) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
            }
        }

        __free();
    }

    [Test]
    public void MathTest_Scale()
    {
        __init(new Vector3Int(TestDimA, TestDimB, 1));

        for (int i = 0; i < m_TotalDim; i++)
        {
            float TempValue = i % m_AssignmentLimitingFactor + 1;
            m_SrcVectorValueA[i] = TempValue;
        }

        m_SrcVectorA.SetData(m_SrcVectorValueA);

        CMathTool.scale(m_SrcVectorA, 4.5f);

        m_SrcVectorA.GetData(m_DstVectorValueA);

        for (int i = 0; i < m_TotalDim; i++)
        {
            if (i % m_LoopLimitingFactor == m_LoopJudgeFactor)
            {
                float TempValue = i % m_AssignmentLimitingFactor + 1;
                Assert.IsTrue(Mathf.Abs(m_DstVectorValueA[i] - 4.5f * TempValue) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
            }
        }

        __free();
    }

    [Test]
    public void MathTest_Plus()
    {
        __init(new Vector3Int(TestDimA, TestDimB, 1));

        for (int i = 0; i < m_TotalDim; i++)
        {
            float TempValue = i % m_AssignmentLimitingFactor + 1;
            m_SrcVectorValueA[i] = TempValue;
        }

        m_SrcVectorA.SetData(m_SrcVectorValueA);

        CMathTool.plus(m_SrcVectorA, 4.5f);

        m_SrcVectorA.GetData(m_DstVectorValueA);

        for (int i = 0; i < m_TotalDim; i++)
        {
            if (i % m_LoopLimitingFactor == m_LoopJudgeFactor)
            {
                float TempValue = i % m_AssignmentLimitingFactor + 1 + 4.5f;
                Assert.IsTrue(Mathf.Abs(m_DstVectorValueA[i] - TempValue) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
            }
        }

        __free();
    }

    [Test]
    public void MathTest_PlusAlphaX()
    {
        __init(new Vector3Int(TestDimA, TestDimB, 1));

        for (int i = 0; i < m_TotalDim; i++)
        {
            float TempValue = i % m_AssignmentLimitingFactor + 1;
            m_SrcVectorValueA[i] = TempValue;
            m_SrcVectorValueB[i] = 2 * TempValue;
        }

        m_SrcVectorA.SetData(m_SrcVectorValueA);
        m_SrcVectorB.SetData(m_SrcVectorValueB);

        CMathTool.plusAlphaX(m_SrcVectorA, m_SrcVectorB, -4.5f);

        m_SrcVectorA.GetData(m_DstVectorValueA);

        for (int i = 0; i < m_TotalDim; i++)
        {
            if (i % m_LoopLimitingFactor == m_LoopJudgeFactor)
            {
                float TempValue = -8 * (i % m_AssignmentLimitingFactor + 1);
                Assert.IsTrue(Mathf.Abs(m_DstVectorValueA[i] - TempValue) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
            }
        }

        __free();
    }

    [Test]
    public void MathTest_Sum()
    {
        __init(new Vector3Int(TestDimA, TestDimB, 1));

        for (int i = 0; i < m_TotalDim; i++)
        {
            m_SrcVectorValueA[i] = 1;
        }

        m_SrcVectorA.SetData(m_SrcVectorValueA);

        float Sum = CMathTool.sum(m_SrcVectorA, m_TotalDim);

        Assert.IsTrue(Mathf.Abs(Sum - m_TotalDim) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        __free();
    }

    [Test]
    public void MathTest_Dot()
    {
        __init(new Vector3Int(TestDimA, TestDimB, 1));

        for (int i = 0; i < m_TotalDim; i++)
        {
            m_SrcVectorValueA[i] = 10;
            m_SrcVectorValueB[i] = 0.1f;
        }

        m_SrcVectorA.SetData(m_SrcVectorValueA);
        m_SrcVectorB.SetData(m_SrcVectorValueB);

        float Dot = CMathTool.dot(m_SrcVectorA, m_SrcVectorB);

        Assert.IsTrue(Mathf.Abs(Dot - m_TotalDim) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        __free();
    }

    [Test]
    public void MathTest_Norm2()
    {
        __init(new Vector3Int(TestDimA, TestDimB, 1));

        for (int i = 0; i < m_TotalDim; i++)
        {
            m_SrcVectorValueA[i] = 10;
        }

        m_SrcVectorA.SetData(m_SrcVectorValueA);

        float Norm2 = CMathTool.norm2(m_SrcVectorA);

        Assert.IsTrue(Mathf.Abs(Norm2 / 100 - m_TotalDim) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        __free();
    }

    [Test]
    public void MathTest_GetMaxValue()
    {
        __init(new Vector3Int(TestDimA, TestDimB, 1));

        float MaxValueCPU = float.MinValue;
        for (int i = 0; i < m_TotalDim; i++)
        {
            m_SrcVectorValueA[i] = Random.Range(-10000.0f, 10000.0f);
            MaxValueCPU = Mathf.Max(MaxValueCPU, m_SrcVectorValueA[i]);
        }

        m_SrcVectorA.SetData(m_SrcVectorValueA);

        float MaxValue = CMathTool.getMaxValue(m_SrcVectorA);

        Assert.IsTrue(Mathf.Abs(MaxValue - MaxValueCPU) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        __free();
    }

    [Test]
    public void MathTest_GetMinValue()
    {
        __init(new Vector3Int(TestDimA, TestDimB, 1));

        float MinValueCPU = float.MaxValue;
        for (int i = 0; i < m_TotalDim; i++)
        {
            m_SrcVectorValueA[i] = Random.Range(-10000.0f, 10000.0f);
            MinValueCPU = Mathf.Min(MinValueCPU, m_SrcVectorValueA[i]);
        }

        m_SrcVectorA.SetData(m_SrcVectorValueA);

        float MaxValue = CMathTool.getMinValue(m_SrcVectorA);

        Assert.IsTrue(Mathf.Abs(MaxValue - MinValueCPU) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        __free();
    }

    [Test]
    public void MathTest_GetAbsMaxValue()
    {
        __init(new Vector3Int(TestDimA, TestDimB, 1));

        float AbsMaxValueCPU = float.MinValue;
        for (int i = 0; i < m_TotalDim; i++)
        {
            m_SrcVectorValueA[i] = Random.Range(-20000.0f, 10000.0f);
            AbsMaxValueCPU = Mathf.Max(AbsMaxValueCPU, Mathf.Abs(m_SrcVectorValueA[i]));
        }

        m_SrcVectorA.SetData(m_SrcVectorValueA);

        float MaxValue = CMathTool.getAbsMaxValue(m_SrcVectorA);

        Assert.IsTrue(Mathf.Abs(MaxValue - AbsMaxValueCPU) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        __free();
    }
}