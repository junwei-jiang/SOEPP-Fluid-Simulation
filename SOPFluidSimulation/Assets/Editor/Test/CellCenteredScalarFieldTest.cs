using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using EulerFluidEngine;

public class CellCenteredScalarFieldTest
{
    [Test]
    public void CellCenteredScalarFieldTest_Constructor()
    {
        #region InitialVariable
        Vector3Int Resolution = new Vector3Int(16, 16, 16);
        Vector3 Origin = new Vector3(2, 2, 2);
        Vector3 Spacing = new Vector3(8, 8, 8);

        float[] InitialValue = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstGridValue = new float[Resolution.x * Resolution.y * Resolution.z];
        for (int i = 0; i < Resolution.x * Resolution.y * Resolution.z; i++)
        {
            InitialValue[i] = i;
        }

        ComputeBuffer GridData = new ComputeBuffer(Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        GridData.SetData(InitialValue);
        #endregion

        #region Constructor1
        CCellCenteredScalarField CCSField1 = new CCellCenteredScalarField(Resolution, Origin, Spacing);

        Assert.AreEqual(CCSField1.getResolution(), Resolution);
        Assert.AreEqual(CCSField1.getOrigin(), Origin);
        Assert.AreEqual(CCSField1.getSpacing(), Spacing);

        CCSField1.getGridData().GetData(DstGridValue);
        for (int i = 0; i < Resolution.x * Resolution.y * Resolution.z; i++)
        {
            Assert.AreEqual(0, DstGridValue[i]);
        }
        #endregion

        #region Constructor2
        CCellCenteredScalarField CCSField2 = new CCellCenteredScalarField(Resolution, Origin, Spacing, InitialValue);

        Assert.AreEqual(CCSField2.getResolution(), Resolution);
        Assert.AreEqual(CCSField2.getOrigin(), Origin);
        Assert.AreEqual(CCSField2.getSpacing(), Spacing);

        CCSField2.getGridData().GetData(DstGridValue);
        for (int i = 0; i < Resolution.x * Resolution.y * Resolution.z; i++)
        {
            Assert.AreEqual(InitialValue[i], DstGridValue[i]);
        }
        #endregion

        #region Constructor3
        CCellCenteredScalarField CCSField3 = new CCellCenteredScalarField(Resolution, Origin, Spacing, GridData);

        Assert.AreEqual(CCSField3.getResolution(), Resolution);
        Assert.AreEqual(CCSField3.getOrigin(), Origin);
        Assert.AreEqual(CCSField3.getSpacing(), Spacing);

        //这里是为了测试是否把传入的Buffer的引用给了新创建的Field
        float[] OtherValue = new float[Resolution.x * Resolution.y * Resolution.z];
        for (int i = 0; i < Resolution.x * Resolution.y * Resolution.z; i++)
        {
            OtherValue[i] = InitialValue[i] + 1;
        }
        GridData.SetData(OtherValue);

        CCSField3.getGridData().GetData(DstGridValue);
        for (int i = 0; i < Resolution.x * Resolution.y * Resolution.z; i++)
        {
            Assert.AreEqual(InitialValue[i], DstGridValue[i]);
        }

        GridData.SetData(InitialValue);
        #endregion

        #region Constructor4
        CCellCenteredScalarField CCSField4 = new CCellCenteredScalarField(CCSField3);

        Assert.AreEqual(CCSField4.getResolution(), Resolution);
        Assert.AreEqual(CCSField4.getOrigin(), Origin);
        Assert.AreEqual(CCSField4.getSpacing(), Spacing);

        CCSField4.getGridData().GetData(DstGridValue);
        for (int i = 0; i < Resolution.x * Resolution.y * Resolution.z; i++)
        {
            Assert.AreEqual(InitialValue[i], DstGridValue[i]);
        }
        #endregion

        #region Resize
        Vector3Int TempResolution = new Vector3Int(64, 12, 23);
        Vector3 TempOrigin = new Vector3(0, 5, 4);
        Vector3 TempSpacing = new Vector3(4, 2.2f, 1);

        float[] TempInitialValue = new float[TempResolution.x * TempResolution.y * TempResolution.z];
        float[] TempDstGridValue = new float[TempResolution.x * TempResolution.y * TempResolution.z];
        for (int i = 0; i < TempResolution.x * TempResolution.y * TempResolution.z; i++)
        {
            TempInitialValue[i] = i * 0.7f;
        }

        CCSField4.resize(TempResolution, TempOrigin, TempSpacing, TempInitialValue);

        Assert.AreEqual(CCSField4.getResolution(), TempResolution);
        Assert.AreEqual(CCSField4.getOrigin(), TempOrigin);
        Assert.AreEqual(CCSField4.getSpacing(), TempSpacing);

        CCSField4.getGridData().GetData(TempDstGridValue);
        for (int i = 0; i < TempResolution.x * TempResolution.y * TempResolution.z; i++)
        {
            Assert.AreEqual(TempInitialValue[i], TempDstGridValue[i]);
        }
        #endregion

        #region Release
        GridData.Release();
        CCSField1.free();
        CCSField2.free();
        CCSField3.free();
        CCSField4.free();
        #endregion
    }

    [Test]
    public void CellCenteredScalarFieldTest_SampleFieldLinear()
    {
        //简单测试采样
        #region SampleTest1
        Vector3Int Resolution = new Vector3Int(2, 2, 1);
        Vector3 Origin = new Vector3(0, 0, 0);
        Vector3 Spacing = new Vector3(1, 1, 1);

        float[] SrcScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (int)(x % 2);
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x + 0.4f;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y + 0.4f;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z + 0.4f;
                }
            }
        }

        CCellCenteredVectorField CCVPosField = new CCellCenteredVectorField(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCellCenteredScalarField CCSSrcField = new CCellCenteredScalarField(Resolution, Origin, Spacing, SrcScalarFieldValue);
        CCellCenteredScalarField CCSDstField = new CCellCenteredScalarField(Resolution, Origin, Spacing);
        
        CCSSrcField.sampleField(CCVPosField, CCSDstField);

        CCSDstField.getGridData().GetData(DstScalarFieldValue);

        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[0] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[1] - 0.9f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[2] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[3] - 0.9f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x + 0.6f;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y + 0.6f;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z + 0.6f;
                }
            }
        }

        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCSDstField.resize(Resolution, Origin, Spacing);

        CCSSrcField.sampleField(CCVPosField, CCSDstField);

        CCSDstField.getGridData().GetData(DstScalarFieldValue);

        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[0] - 0.1f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[1] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[2] - 0.1f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[3] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x + 0.5f;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y + 0.5f;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z + 0.5f;
                }
            }
        }

        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCSDstField.resize(Resolution, Origin, Spacing);

        CCSSrcField.sampleField(CCVPosField, CCSDstField);

        CCSDstField.getGridData().GetData(DstScalarFieldValue);

        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[0] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[1] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[2] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[3] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //测试网格Origin和Spacing不是默认情况下的采样
        #region SampleTest2
        Resolution = new Vector3Int(16, 16, 16);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        DstScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (int)(x % 2) * 10;
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 10 - 10 + 5;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y * 20 - 40 + 10;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * 30 - 50 + 15;
                }
            }
        }

        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCSSrcField.resize(Resolution, Origin, Spacing, SrcScalarFieldValue);
        CCSDstField.resize(Resolution, Origin, Spacing);

        CCSSrcField.sampleField(CCVPosField, CCSDstField);

        CCSDstField.getGridData().GetData(DstScalarFieldValue);

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] - 10 * (int)(x % 2)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }
        #endregion

        //测试源网格和目的网格分辨率不同的情况下的采样
        #region SampleTest3
        Resolution = new Vector3Int(16, 16, 16);
        Vector3Int Resolution2 = new Vector3Int(32, 2, 16);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        DstScalarFieldValue = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        PosVectorFieldValueX = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        PosVectorFieldValueY = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        PosVectorFieldValueZ = new float[Resolution2.x * Resolution2.y * Resolution2.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (int)(x % 2) * 10;
                }
            }
        }

        for (int z = 0; z < Resolution2.z; z++)
        {
            for (int y = 0; y < Resolution2.y; y++)
            {
                for (int x = 0; x < Resolution2.x; x++)
                {
                    PosVectorFieldValueX[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x] = ((int)(x / 2)) * 10 - 10 + 5;
                    PosVectorFieldValueY[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x] = y * 20 - 40 + 10;
                    PosVectorFieldValueZ[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x] = z * 30 - 50 + 15;
                }
            }
        }

        CCVPosField.resize(Resolution2, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCSSrcField.resize(Resolution, Origin, Spacing, SrcScalarFieldValue);
        CCSDstField.resize(Resolution2, Origin, Spacing);

        CCSSrcField.sampleField(CCVPosField, CCSDstField);

        CCSDstField.getGridData().GetData(DstScalarFieldValue);

        for (int z = 0; z < Resolution2.z; z++)
        {
            for (int y = 0; y < Resolution2.y; y++)
            {
                for (int x = 0; x < Resolution2.x; x++)
                {
                    Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x] - 10 * (int)(((int)(x / 2)) % 2)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }
        #endregion

        //测试采样的边界情况
        #region SampleTest4
        Resolution = new Vector3Int(2, 2, 1);
        Origin = new Vector3(0, 0, 0);
        Spacing = new Vector3(1, 1, 1);

        SrcScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        DstScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (int)(y % 2);
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x + 0.5f;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y + 0.4f;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z + 8.5f;
                }
            }
        }

        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCSSrcField.resize(Resolution, Origin, Spacing, SrcScalarFieldValue);
        CCSDstField.resize(Resolution, Origin, Spacing);

        CCSSrcField.sampleField(CCVPosField, CCSDstField);

        CCSDstField.getGridData().GetData(DstScalarFieldValue);

        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[0] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[1] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[2] - 0.9f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[3] - 0.9f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //测试只输入位置的情况下的采样
        #region SampleTest5
        Resolution = new Vector3Int(16, 16, 16);
        Resolution2 = new Vector3Int(32, 2, 16);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        DstScalarFieldValue = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        float[] DstScalarFieldValue2 = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        PosVectorFieldValueX = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        PosVectorFieldValueY = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        PosVectorFieldValueZ = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        float[] PosVectorBufferValue = new float[3 * Resolution2.x * Resolution2.y * Resolution2.z];


        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (int)(x % 2) * 10;
                }
            }
        }

        for (int z = 0; z < Resolution2.z; z++)
        {
            for (int y = 0; y < Resolution2.y; y++)
            {
                for (int x = 0; x < Resolution2.x; x++)
                {
                    PosVectorFieldValueX[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x] = ((int)(x / 2)) * 10 - 10 + 5;
                    PosVectorFieldValueY[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x] = y * 20 - 40 + 10;
                    PosVectorFieldValueZ[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x] = z * 30 - 50 + 15;
                    PosVectorBufferValue[3 * (z * Resolution2.x * Resolution2.y + y * Resolution2.x + x)] = PosVectorFieldValueX[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x];
                    PosVectorBufferValue[3 * (z * Resolution2.x * Resolution2.y + y * Resolution2.x + x) + 1] = PosVectorFieldValueY[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x];
                    PosVectorBufferValue[3 * (z * Resolution2.x * Resolution2.y + y * Resolution2.x + x) + 2] = PosVectorFieldValueZ[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x];
                }
            }
        }

        ComputeBuffer SampleDstData = new ComputeBuffer(Resolution2.x * Resolution2.y * Resolution2.z, sizeof(float));
        ComputeBuffer SamplePosData = new ComputeBuffer(3 * Resolution2.x * Resolution2.y * Resolution2.z, sizeof(float));
        SamplePosData.SetData(PosVectorBufferValue);

        CCSSrcField.resize(Resolution, Origin, Spacing, SrcScalarFieldValue);
        CCSDstField.resize(Resolution2, Origin, Spacing);
        CCVPosField.resize(Resolution2, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);

        CCSSrcField.sampleField(CCVPosField, CCSDstField);
        CCSSrcField.sampleField(SamplePosData, SampleDstData);

        CCSDstField.getGridData().GetData(DstScalarFieldValue);
        SampleDstData.GetData(DstScalarFieldValue2);

        for (int z = 0; z < Resolution2.z; z++)
        {
            for (int y = 0; y < Resolution2.y; y++)
            {
                for (int x = 0; x < Resolution2.x; x++)
                {
                    Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x] - 10 * (int)(((int)(x / 2)) % 2)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstScalarFieldValue2[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x] - DstScalarFieldValue[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x]) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }
        #endregion
    }

    [Test]
    public void CellCenteredScalarFieldTest_SampleFieldCatmullRom()
    {
        //CatmullRom采样点位于网格中心测试
        #region SampleTest1
        Vector3Int Resolution = new Vector3Int(16, 16, 16);
        Vector3Int Resolution2 = new Vector3Int(32, 2, 16);
        Vector3 Origin = new Vector3(-10, -40, -50);
        Vector3 Spacing = new Vector3(10, 20, 30);

        float[] SrcScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstScalarFieldValue = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        float[] PosVectorFieldValueX = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        float[] PosVectorFieldValueY = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        float[] PosVectorFieldValueZ = new float[Resolution2.x * Resolution2.y * Resolution2.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 10 + y * 20 + z * 30;
                }
            }
        }

        for (int z = 0; z < Resolution2.z; z++)
        {
            for (int y = 0; y < Resolution2.y; y++)
            {
                for (int x = 0; x < Resolution2.x; x++)
                {
                    PosVectorFieldValueX[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x] = ((int)(x / 2)) * 10 - 10 + 5;
                    PosVectorFieldValueY[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x] = y * 20 - 40 + 10;
                    PosVectorFieldValueZ[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x] = z * 30 - 50 + 15;
                }
            }
        }

        CCellCenteredVectorField CCVPosField = new CCellCenteredVectorField(Resolution2, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCellCenteredScalarField CCSSrcField = new CCellCenteredScalarField(Resolution, Origin, Spacing, SrcScalarFieldValue);
        CCellCenteredScalarField CCSDstField = new CCellCenteredScalarField(Resolution2, Origin, Spacing);

        CCSSrcField.sampleField(CCVPosField, CCSDstField, ESamplingAlgorithm.CATMULLROM);

        CCSDstField.getGridData().GetData(DstScalarFieldValue);

        for (int z = 0; z < Resolution2.z; z++)
        {
            for (int y = 0; y < Resolution2.y; y++)
            {
                for (int x = 0; x < Resolution2.x; x++)
                {
                    Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x] - (((int)(x / 2)) * 10 + y * 20 + z * 30)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }
        #endregion

        //CatmullRom简单手算测试，只考虑了一维其实
        #region SampleTest2
        Resolution = new Vector3Int(4, 4, 1);
        Origin = new Vector3(0, 0, 0);
        Spacing = new Vector3(1, 1, 1);

        SrcScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        DstScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * Resolution.x * Resolution.y + y * Resolution.x + x;
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x + 0.4f;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y + 0.5f;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z + 0.5f;
                }
            }
        }

        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCSSrcField.resize(Resolution, Origin, Spacing, SrcScalarFieldValue);
        CCSDstField.resize(Resolution, Origin, Spacing);

        CCSSrcField.sampleField(CCVPosField, CCSDstField, ESamplingAlgorithm.CATMULLROM);

        CCSDstField.getGridData().GetData(DstScalarFieldValue);

        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[0] + 0.0405f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[1] - 0.8955f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[2] - 1.9000f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[3] - 2.9405f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[4] - 3.9595f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[5] - 4.8955f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[6] - 5.9000f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[7] - 6.9405f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[8] - 7.9595f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[9] - 8.8955f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[10] - 9.9000f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[11] - 10.9405f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[12] - 11.9595f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[13] - 12.8955f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[14] - 13.9000f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[15] - 14.9405f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //CatmullRom手算测试
        #region SampleTest3
        Resolution = new Vector3Int(4, 4, 4);
        Origin = new Vector3(0, 0, 0);
        Spacing = new Vector3(1, 1, 1);

        SrcScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        DstScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x + 0.4f;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y + 0.6f;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z + 0.8f;
                }
            }
        }

        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCSSrcField.resize(Resolution, Origin, Spacing, SrcScalarFieldValue);
        CCSDstField.resize(Resolution, Origin, Spacing);

        CCSSrcField.sampleField(CCVPosField, CCSDstField, ESamplingAlgorithm.CATMULLROM);

        CCSDstField.getGridData().GetData(DstScalarFieldValue);

        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[22] - 2.4415f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //测试CatmullRom只输入位置的情况下的采样
        #region SampleTest4
        Resolution = new Vector3Int(4, 4, 4);
        Origin = new Vector3(0, 0, 0);
        Spacing = new Vector3(1, 1, 1);

        SrcScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        DstScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstScalarFieldValue2 = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorBufferValue = new float[3 * Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;

                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x + 0.4f;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y + 0.6f;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z + 0.8f;
                    PosVectorBufferValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x)] = PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                    PosVectorBufferValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 1] = PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                    PosVectorBufferValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 2] = PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                }
            }
        }

        ComputeBuffer SampleDstData = new ComputeBuffer(Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        ComputeBuffer SamplePosData = new ComputeBuffer(3 * Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        SamplePosData.SetData(PosVectorBufferValue);

        CCSSrcField.resize(Resolution, Origin, Spacing, SrcScalarFieldValue);
        CCSDstField.resize(Resolution, Origin, Spacing);
        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);

        CCSSrcField.sampleField(CCVPosField, CCSDstField, ESamplingAlgorithm.CATMULLROM);
        CCSSrcField.sampleField(SamplePosData, SampleDstData, ESamplingAlgorithm.CATMULLROM);

        CCSDstField.getGridData().GetData(DstScalarFieldValue);
        SampleDstData.GetData(DstScalarFieldValue2);

        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[22] - 2.4415f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue2[22] - 2.4415f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        SampleDstData.Release();
        SamplePosData.Release();
        #endregion

        //MonoCatmullRom简单手算测试，只考虑了一维其实
        #region SampleTest5
        Resolution = new Vector3Int(4, 4, 1);
        Origin = new Vector3(0, 0, 0);
        Spacing = new Vector3(1, 1, 1);

        SrcScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        DstScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * Resolution.x * Resolution.y + y * Resolution.x + x;
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x + 0.4f;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y + 0.5f;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z + 0.5f;
                }
            }
        }

        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCSSrcField.resize(Resolution, Origin, Spacing, SrcScalarFieldValue);
        CCSDstField.resize(Resolution, Origin, Spacing);

        CCSSrcField.sampleField(CCVPosField, CCSDstField, ESamplingAlgorithm.MONOCATMULLROM);

        CCSDstField.getGridData().GetData(DstScalarFieldValue);

        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[0] - 0.0000f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[1] - 0.8955f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[2] - 1.9000f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[3] - 2.9405f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[4] - 4.0000f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[5] - 4.8955f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[6] - 5.9000f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[7] - 6.9405f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[8] - 8.0000f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[9] - 8.8955f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[10] - 9.9000f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[11] - 10.9405f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[12] - 12.0000f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[13] - 12.8955f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[14] - 13.9000f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[15] - 14.9405f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //MonoCatmullRom手算测试
        #region SampleTest6
        Resolution = new Vector3Int(4, 4, 4);
        Origin = new Vector3(0, 0, 0);
        Spacing = new Vector3(1, 1, 1);

        SrcScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        DstScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x + 0.4f;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y + 0.6f;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z + 0.8f;
                }
            }
        }

        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCSSrcField.resize(Resolution, Origin, Spacing, SrcScalarFieldValue);
        CCSDstField.resize(Resolution, Origin, Spacing);

        CCSSrcField.sampleField(CCVPosField, CCSDstField, ESamplingAlgorithm.MONOCATMULLROM);

        CCSDstField.getGridData().GetData(DstScalarFieldValue);

        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[22] - 2.437f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //测试MonoCatmullRom只输入位置的情况下的采样
        #region SampleTest7
        Resolution = new Vector3Int(4, 4, 4);
        Origin = new Vector3(0, 0, 0);
        Spacing = new Vector3(1, 1, 1);

        SrcScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        DstScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        DstScalarFieldValue2 = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorBufferValue = new float[3 * Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;

                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x + 0.4f;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y + 0.6f;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z + 0.8f;
                    PosVectorBufferValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x)] = PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                    PosVectorBufferValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 1] = PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                    PosVectorBufferValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 2] = PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                }
            }
        }

        SampleDstData = new ComputeBuffer(Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        SamplePosData = new ComputeBuffer(3 * Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        SamplePosData.SetData(PosVectorBufferValue);

        CCSSrcField.resize(Resolution, Origin, Spacing, SrcScalarFieldValue);
        CCSDstField.resize(Resolution, Origin, Spacing);
        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);

        CCSSrcField.sampleField(CCVPosField, CCSDstField, ESamplingAlgorithm.MONOCATMULLROM);
        CCSSrcField.sampleField(SamplePosData, SampleDstData, ESamplingAlgorithm.MONOCATMULLROM);

        CCSDstField.getGridData().GetData(DstScalarFieldValue);
        SampleDstData.GetData(DstScalarFieldValue2);

        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[22] - 2.437f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue2[22] - 2.437f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        SampleDstData.Release();
        SamplePosData.Release();
        #endregion
    }

    [Test]
    public void CellCenteredScalarFieldTest_SampleFieldCubicBridson()
    {
        //CubicBridson采样点位于网格中心测试
        #region SampleTest1
        Vector3Int Resolution = new Vector3Int(16, 16, 16);
        Vector3Int Resolution2 = new Vector3Int(32, 2, 16);
        Vector3 Origin = new Vector3(-10, -40, -50);
        Vector3 Spacing = new Vector3(10, 20, 30);

        float[] SrcScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstScalarFieldValue = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        float[] PosVectorFieldValueX = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        float[] PosVectorFieldValueY = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        float[] PosVectorFieldValueZ = new float[Resolution2.x * Resolution2.y * Resolution2.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 10 + y * 20 + z * 30;
                }
            }
        }

        for (int z = 0; z < Resolution2.z; z++)
        {
            for (int y = 0; y < Resolution2.y; y++)
            {
                for (int x = 0; x < Resolution2.x; x++)
                {
                    PosVectorFieldValueX[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x] = ((int)(x / 2)) * 10 - 10 + 5;
                    PosVectorFieldValueY[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x] = y * 20 - 40 + 10;
                    PosVectorFieldValueZ[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x] = z * 30 - 50 + 15;
                }
            }
        }

        CCellCenteredVectorField CCVPosField = new CCellCenteredVectorField(Resolution2, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCellCenteredScalarField CCSSrcField = new CCellCenteredScalarField(Resolution, Origin, Spacing, SrcScalarFieldValue);
        CCellCenteredScalarField CCSDstField = new CCellCenteredScalarField(Resolution2, Origin, Spacing);

        CCSSrcField.sampleField(CCVPosField, CCSDstField, ESamplingAlgorithm.CUBICBRIDSON);

        CCSDstField.getGridData().GetData(DstScalarFieldValue);

        for (int z = 0; z < Resolution2.z; z++)
        {
            for (int y = 0; y < Resolution2.y; y++)
            {
                for (int x = 0; x < Resolution2.x; x++)
                {
                    Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x] - (((int)(x / 2)) * 10 + y * 20 + z * 30)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }
        #endregion

        //CubicBridson简单手算测试，只考虑了一维其实
        #region SampleTest2
        Resolution = new Vector3Int(4, 4, 1);
        Origin = new Vector3(0, 0, 0);
        Spacing = new Vector3(1, 1, 1);

        SrcScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        DstScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * Resolution.x * Resolution.y + y * Resolution.x + x;
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x + 0.4f;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y + 0.5f;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z + 0.5f;
                }
            }
        }

        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCSSrcField.resize(Resolution, Origin, Spacing, SrcScalarFieldValue);
        CCSDstField.resize(Resolution, Origin, Spacing);

        CCSSrcField.sampleField(CCVPosField, CCSDstField, ESamplingAlgorithm.CUBICBRIDSON);

        CCSDstField.getGridData().GetData(DstScalarFieldValue);

        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[0] + 0.0285f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[1] - 0.8835f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[2] - 1.9000f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[3] - 2.9285f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[4] - 3.9715f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[5] - 4.8835f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[6] - 5.9000f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[7] - 6.9285f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[8] - 7.9715f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[9] - 8.8835f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[10] - 9.9000f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[11] - 10.9285f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[12] - 11.9715f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[13] - 12.8835f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[14] - 13.9000f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[15] - 14.9285f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //CubicBridson手算测试
        #region SampleTest3
        Resolution = new Vector3Int(4, 4, 4);
        Origin = new Vector3(0, 0, 0);
        Spacing = new Vector3(1, 1, 1);

        SrcScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        DstScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x + 0.4f;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y + 0.6f;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z + 0.8f;
                }
            }
        }

        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCSSrcField.resize(Resolution, Origin, Spacing, SrcScalarFieldValue);
        CCSDstField.resize(Resolution, Origin, Spacing);

        CCSSrcField.sampleField(CCVPosField, CCSDstField, ESamplingAlgorithm.CUBICBRIDSON);

        CCSDstField.getGridData().GetData(DstScalarFieldValue);

        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[22] - 2.5255f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //测试CubicBridson只输入位置的情况下的采样
        #region SampleTest4
        Resolution = new Vector3Int(4, 4, 4);
        Origin = new Vector3(0, 0, 0);
        Spacing = new Vector3(1, 1, 1);

        SrcScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        DstScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstScalarFieldValue2 = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorBufferValue = new float[3 * Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;

                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x + 0.4f;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y + 0.6f;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z + 0.8f;
                    PosVectorBufferValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x)] = PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                    PosVectorBufferValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 1] = PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                    PosVectorBufferValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 2] = PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                }
            }
        }

        ComputeBuffer SampleDstData = new ComputeBuffer(Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        ComputeBuffer SamplePosData = new ComputeBuffer(3 * Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        SamplePosData.SetData(PosVectorBufferValue);

        CCSSrcField.resize(Resolution, Origin, Spacing, SrcScalarFieldValue);
        CCSDstField.resize(Resolution, Origin, Spacing);
        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);

        CCSSrcField.sampleField(CCVPosField, CCSDstField, ESamplingAlgorithm.CUBICBRIDSON);
        CCSSrcField.sampleField(SamplePosData, SampleDstData, ESamplingAlgorithm.CUBICBRIDSON);

        CCSDstField.getGridData().GetData(DstScalarFieldValue);
        SampleDstData.GetData(DstScalarFieldValue2);

        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[22] - 2.5255f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue2[22] - 2.5255f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        SampleDstData.Release();
        SamplePosData.Release();
        #endregion

        //ClampCubicBridson简单手算测试，只考虑了一维其实
        #region SampleTest5
        Resolution = new Vector3Int(4, 4, 1);
        Origin = new Vector3(0, 0, 0);
        Spacing = new Vector3(1, 1, 1);

        SrcScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        DstScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * Resolution.x * Resolution.y + y * Resolution.x + x;
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x + 0.4f;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y + 0.5f;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z + 0.5f;
                }
            }
        }

        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCSSrcField.resize(Resolution, Origin, Spacing, SrcScalarFieldValue);
        CCSDstField.resize(Resolution, Origin, Spacing);

        CCSSrcField.sampleField(CCVPosField, CCSDstField, ESamplingAlgorithm.CLAMPCUBICBRIDSON);

        CCSDstField.getGridData().GetData(DstScalarFieldValue);

        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[0] - 0.0000f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[1] - 0.8835f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[2] - 1.9000f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[3] - 2.9285f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[4] - 4.0000f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[5] - 4.8835f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[6] - 5.9000f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[7] - 6.9285f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[8] - 8.0000f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[9] - 8.8835f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[10] - 9.9000f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[11] - 10.9285f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[12] - 12.0000f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[13] - 12.8835f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[14] - 13.9000f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[15] - 14.9285f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //ClampCubicBridson手算测试
        #region SampleTest6
        Resolution = new Vector3Int(4, 4, 4);
        Origin = new Vector3(0, 0, 0);
        Spacing = new Vector3(1, 1, 1);

        SrcScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        DstScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x + 0.4f;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y + 0.6f;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z + 0.8f;
                }
            }
        }

        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCSSrcField.resize(Resolution, Origin, Spacing, SrcScalarFieldValue);
        CCSDstField.resize(Resolution, Origin, Spacing);

        CCSSrcField.sampleField(CCVPosField, CCSDstField, ESamplingAlgorithm.CLAMPCUBICBRIDSON);

        CCSDstField.getGridData().GetData(DstScalarFieldValue);

        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[22] - 2.5255f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //测试ClampCubicBridson只输入位置的情况下的采样
        #region SampleTest7
        Resolution = new Vector3Int(4, 4, 4);
        Origin = new Vector3(0, 0, 0);
        Spacing = new Vector3(1, 1, 1);

        SrcScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        DstScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        DstScalarFieldValue2 = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorBufferValue = new float[3 * Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;

                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x + 0.4f;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y + 0.6f;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z + 0.8f;
                    PosVectorBufferValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x)] = PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                    PosVectorBufferValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 1] = PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                    PosVectorBufferValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 2] = PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                }
            }
        }

        SampleDstData = new ComputeBuffer(Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        SamplePosData = new ComputeBuffer(3 * Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        SamplePosData.SetData(PosVectorBufferValue);

        CCSSrcField.resize(Resolution, Origin, Spacing, SrcScalarFieldValue);
        CCSDstField.resize(Resolution, Origin, Spacing);
        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);

        CCSSrcField.sampleField(CCVPosField, CCSDstField, ESamplingAlgorithm.CLAMPCUBICBRIDSON);
        CCSSrcField.sampleField(SamplePosData, SampleDstData, ESamplingAlgorithm.CLAMPCUBICBRIDSON);

        CCSDstField.getGridData().GetData(DstScalarFieldValue);
        SampleDstData.GetData(DstScalarFieldValue2);

        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[22] - 2.5255f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstScalarFieldValue2[22] - 2.5255f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        SampleDstData.Release();
        SamplePosData.Release();
        #endregion
    }

    [Test]
    public void CellCenteredScalarFieldTest_Gradient()
    {
        Vector3Int Resolution = new Vector3Int(160, 160, 16);
        Vector3 Origin = new Vector3(-10, -40, -50);
        Vector3 Spacing = new Vector3(10, 20, 30);

        float[] SrcScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (int)(x % 2) * 10 + (int)(y % 2) * 20 + (int)(z % 2) * 30;
                }
            }
        }

        CCellCenteredScalarField CCSSrcField = new CCellCenteredScalarField(Resolution, Origin, Spacing, SrcScalarFieldValue);
        CCellCenteredVectorField CCVDstField = new CCellCenteredVectorField(Resolution, Origin, Spacing);

        CCSSrcField.gradient(CCVDstField);

        CCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        CCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        CCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] - ((int)((x + 1 < Resolution.x ? x + 1 : x) % 2) - (int)((x > 0 ? x - 1 : x) % 2)) * 0.5f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] - ((int)((y + 1 < Resolution.y ? y + 1 : y) % 2) - (int)((y > 0 ? y - 1 : y) % 2)) * 0.5f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] - ((int)((z + 1 < Resolution.z ? z + 1 : z) % 2) - (int)((z > 0 ? z - 1 : z) % 2)) * 0.5f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }
    }

    [Test]
    public void CellCenteredScalarFieldTest_Laplacian()
    {
        Vector3Int Resolution = new Vector3Int(160, 160, 16);
        Vector3 Origin = new Vector3(-10, -40, -50);
        Vector3 Spacing = new Vector3(10, 20, 30);

        float[] SrcScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (int)(x % 2) * 10 + (int)(y % 2) * 20 + (int)(z % 2) * 30;
                }
            }
        }

        CCellCenteredScalarField CCSSrcField = new CCellCenteredScalarField(Resolution, Origin, Spacing, SrcScalarFieldValue);
        CCellCenteredScalarField CCSDstField = new CCellCenteredScalarField(Resolution, Origin, Spacing);

        CCSSrcField.laplacian(CCSDstField);

        CCSDstField.getGridData().GetData(DstScalarFieldValue);

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    float TempX = (((int)(x + 1 < Resolution.x ? x + 1 : x) % 2) - 2 * (int)(x % 2) + (int)((x > 0 ? x - 1 : x) % 2)) / 10.0f;
                    float TempY = (((int)(y + 1 < Resolution.y ? y + 1 : y) % 2) - 2 * (int)(y % 2) + (int)((y > 0 ? y - 1 : y) % 2)) / 20.0f;
                    float TempZ = (((int)(z + 1 < Resolution.z ? z + 1 : z) % 2) - 2 * (int)(z % 2) + (int)((z > 0 ? z - 1 : z) % 2)) / 30.0f;
                    Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] - TempX - TempY - TempZ) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }
    }
}
