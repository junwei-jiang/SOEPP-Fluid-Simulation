using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using EulerFluidEngine;

public class CellCenteredVectorFieldTest
{
    [Test]
    public void CellCenteredVectorFieldTest_Constructor()
    {
        #region InitialVariable
        Vector3Int Resolution = new Vector3Int(16, 16, 16);
        Vector3 Origin = new Vector3(2, 2, 2);
        Vector3 Spacing = new Vector3(8, 8, 8);

        float[] InitialValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] InitialValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] InitialValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstGridValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstGridValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstGridValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        for (int i = 0; i < Resolution.x * Resolution.y * Resolution.z; i++)
        {
            InitialValueX[i] = i;
            InitialValueY[i] = 2 * i;
            InitialValueZ[i] = 4 * i;
        }

        ComputeBuffer GridDataX = new ComputeBuffer(Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        ComputeBuffer GridDataY = new ComputeBuffer(Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        ComputeBuffer GridDataZ = new ComputeBuffer(Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        GridDataX.SetData(InitialValueX);
        GridDataY.SetData(InitialValueY);
        GridDataZ.SetData(InitialValueZ);
        #endregion

        #region Constructor1
        CCellCenteredVectorField CCVField1 = new CCellCenteredVectorField(Resolution, Origin, Spacing);

        Assert.AreEqual(CCVField1.getResolution(), Resolution);
        Assert.AreEqual(CCVField1.getOrigin(), Origin);
        Assert.AreEqual(CCVField1.getSpacing(), Spacing);

        CCVField1.getGridDataX().GetData(DstGridValueX);
        CCVField1.getGridDataY().GetData(DstGridValueY);
        CCVField1.getGridDataZ().GetData(DstGridValueZ);
        for (int i = 0; i < Resolution.x * Resolution.y * Resolution.z; i++)
        {
            Assert.AreEqual(0, DstGridValueX[i]);
            Assert.AreEqual(0, DstGridValueY[i]);
            Assert.AreEqual(0, DstGridValueZ[i]);
        }
        #endregion

        #region Constructor2
        CCellCenteredVectorField CCVField2 = new CCellCenteredVectorField(Resolution, Origin, Spacing, InitialValueX, InitialValueY, InitialValueZ);

        Assert.AreEqual(CCVField2.getResolution(), Resolution);
        Assert.AreEqual(CCVField2.getOrigin(), Origin);
        Assert.AreEqual(CCVField2.getSpacing(), Spacing);

        CCVField2.getGridDataX().GetData(DstGridValueX);
        CCVField2.getGridDataY().GetData(DstGridValueY);
        CCVField2.getGridDataZ().GetData(DstGridValueZ);
        for (int i = 0; i < Resolution.x * Resolution.y * Resolution.z; i++)
        {
            Assert.AreEqual(InitialValueX[i], DstGridValueX[i]);
            Assert.AreEqual(InitialValueY[i], DstGridValueY[i]);
            Assert.AreEqual(InitialValueZ[i], DstGridValueZ[i]);
        }
        #endregion

        #region Constructor3
        CCellCenteredVectorField CCVField3 = new CCellCenteredVectorField(Resolution, Origin, Spacing, GridDataX, GridDataY, GridDataZ);

        Assert.AreEqual(CCVField3.getResolution(), Resolution);
        Assert.AreEqual(CCVField3.getOrigin(), Origin);
        Assert.AreEqual(CCVField3.getSpacing(), Spacing);

        float[] OtherValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] OtherValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] OtherValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        for (int i = 0; i < Resolution.x * Resolution.y * Resolution.z; i++)
        {
            OtherValueX[i] = InitialValueX[i] + 1;
            OtherValueY[i] = InitialValueY[i] + 1;
            OtherValueZ[i] = InitialValueZ[i] + 1;
        }
        GridDataX.SetData(OtherValueX);
        GridDataY.SetData(OtherValueY);
        GridDataZ.SetData(OtherValueZ);

        CCVField3.getGridDataX().GetData(DstGridValueX);
        CCVField3.getGridDataY().GetData(DstGridValueY);
        CCVField3.getGridDataZ().GetData(DstGridValueZ);
        for (int i = 0; i < Resolution.x * Resolution.y * Resolution.z; i++)
        {
            Assert.AreEqual(InitialValueX[i], DstGridValueX[i]);
            Assert.AreEqual(InitialValueY[i], DstGridValueY[i]);
            Assert.AreEqual(InitialValueZ[i], DstGridValueZ[i]);
        }

        GridDataX.SetData(InitialValueX);
        GridDataY.SetData(InitialValueY);
        GridDataZ.SetData(InitialValueZ);
        #endregion

        #region Constructor4
        CCellCenteredVectorField CCVField4 = new CCellCenteredVectorField(CCVField3);

        Assert.AreEqual(CCVField4.getResolution(), Resolution);
        Assert.AreEqual(CCVField4.getOrigin(), Origin);
        Assert.AreEqual(CCVField4.getSpacing(), Spacing);

        CCVField4.getGridDataX().GetData(DstGridValueX);
        CCVField4.getGridDataY().GetData(DstGridValueY);
        CCVField4.getGridDataZ().GetData(DstGridValueZ);
        for (int i = 0; i < Resolution.x * Resolution.y * Resolution.z; i++)
        {
            Assert.AreEqual(InitialValueX[i], DstGridValueX[i]);
            Assert.AreEqual(InitialValueY[i], DstGridValueY[i]);
            Assert.AreEqual(InitialValueZ[i], DstGridValueZ[i]);
        }
        #endregion

        #region Resize
        Vector3Int TempResolution = new Vector3Int(64, 12, 23);
        Vector3 TempOrigin = new Vector3(0, 5, 4);
        Vector3 TempSpacing = new Vector3(4, 2.2f, 1);

        float[] TempInitialValueX = new float[TempResolution.x * TempResolution.y * TempResolution.z];
        float[] TempInitialValueY = new float[TempResolution.x * TempResolution.y * TempResolution.z];
        float[] TempInitialValueZ = new float[TempResolution.x * TempResolution.y * TempResolution.z];
        float[] TempDstGridDataX = new float[TempResolution.x * TempResolution.y * TempResolution.z];
        float[] TempDstGridDataY = new float[TempResolution.x * TempResolution.y * TempResolution.z];
        float[] TempDstGridDataZ = new float[TempResolution.x * TempResolution.y * TempResolution.z];
        for (int i = 0; i < TempResolution.x * TempResolution.y * TempResolution.z; i++)
        {
            TempInitialValueX[i] = i * 0.7f;
            TempInitialValueY[i] = i * 2.7f;
            TempInitialValueZ[i] = i * 4.7f;
        }

        CCVField4.resize(TempResolution, TempOrigin, TempSpacing, TempInitialValueX, TempInitialValueY, TempInitialValueZ);

        Assert.AreEqual(CCVField4.getResolution(), TempResolution);
        Assert.AreEqual(CCVField4.getOrigin(), TempOrigin);
        Assert.AreEqual(CCVField4.getSpacing(), TempSpacing);

        CCVField4.getGridDataX().GetData(TempDstGridDataX);
        CCVField4.getGridDataY().GetData(TempDstGridDataY);
        CCVField4.getGridDataZ().GetData(TempDstGridDataZ);
        for (int i = 0; i < TempResolution.x * TempResolution.y * TempResolution.z; i++)
        {
            Assert.AreEqual(TempInitialValueX[i], TempDstGridDataX[i]);
            Assert.AreEqual(TempInitialValueY[i], TempDstGridDataY[i]);
            Assert.AreEqual(TempInitialValueZ[i], TempDstGridDataZ[i]);
        }
        #endregion

        #region Release
        GridDataX.Release();
        GridDataY.Release();
        GridDataZ.Release();
        #endregion
    }

    [Test]
    public void CellCenteredVectorFieldTest_SampleFieldLinear()
    {
        //简单测试采样
        #region SampleTest1
        Vector3Int Resolution = new Vector3Int(2, 2, 1);
        Vector3 Origin = new Vector3(0, 0, 0);
        Vector3 Spacing = new Vector3(1, 1, 1);

        float[] SrcVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] SrcVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] SrcVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (int)(x % 2);
                    SrcVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (int)(y % 2);
                    SrcVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (int)(z % 2);
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x + 0.5f;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y + 0.5f;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z + 0.5f;
                }
            }
        }

        CCellCenteredVectorField CCVPosField = new CCellCenteredVectorField(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCellCenteredVectorField CCVSrcField = new CCellCenteredVectorField(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CCellCenteredVectorField CCVDstField = new CCellCenteredVectorField(Resolution, Origin, Spacing);

        CCVSrcField.sampleField(CCVPosField, CCVDstField);

        CCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        CCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        CCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[0] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[1] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[2] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[3] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[0] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[1] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[2] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[3] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[0] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[1] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[2] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[3] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x + 0.4f;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y + 0.4f;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z + 0.4f;
                }
            }
        }

        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCVDstField.resize(Resolution, Origin, Spacing);

        CCVSrcField.sampleField(CCVPosField, CCVDstField);

        CCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        CCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        CCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[0] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[1] - 0.9f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[2] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[3] - 0.9f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[0] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[1] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[2] - 0.9f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[3] - 0.9f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[0] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[1] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[2] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[3] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

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
        CCVDstField.resize(Resolution, Origin, Spacing);

        CCVSrcField.sampleField(CCVPosField, CCVDstField);

        CCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        CCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        CCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[0] - 0.1f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[1] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[2] - 0.1f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[3] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[0] - 0.1f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[1] - 0.1f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[2] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[3] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[0] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[1] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[2] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[3] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //测试网格Origin和Spacing不是默认情况下的采样
        #region SampleTest2
        Resolution = new Vector3Int(16, 16, 16);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        SrcVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        SrcVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (int)(x % 2) * 10;
                    SrcVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (int)(y % 2) * 10;
                    SrcVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (int)(z % 2) * 10;
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 10 - 10 + 5;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y * 20 - 40 + 10;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * 30 - 50 + 15;
                }
            }
        }

        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCVSrcField.resize(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CCVDstField.resize(Resolution, Origin, Spacing);

        CCVSrcField.sampleField(CCVPosField, CCVDstField);

        CCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        CCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        CCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] - 10 * (int)(x % 2)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] - 10 * (int)(y % 2)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] - 10 * (int)(z % 2)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
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

        SrcVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        SrcVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        SrcVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueX = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        DstVectorFieldValueY = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        DstVectorFieldValueZ = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        PosVectorFieldValueX = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        PosVectorFieldValueY = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        PosVectorFieldValueZ = new float[Resolution2.x * Resolution2.y * Resolution2.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (int)(x % 2) * 10;
                    SrcVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (int)(y % 2) * 10;
                    SrcVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (int)(z % 2) * 10;
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
        CCVSrcField.resize(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CCVDstField.resize(Resolution2, Origin, Spacing);

        CCVSrcField.sampleField(CCVPosField, CCVDstField);

        CCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        CCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        CCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        for (int z = 0; z < Resolution2.z; z++)
        {
            for (int y = 0; y < Resolution2.y; y++)
            {
                for (int x = 0; x < Resolution2.x; x++)
                {
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x] - 10 * (int)(((int)(x / 2)) % 2)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x] - 10 * (int)(y % 2)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x] - 10 * (int)(z % 2)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }
        #endregion

        //测试只输入位置的情况下的采样
        #region SampleTest4
        Resolution = new Vector3Int(16, 16, 16);
        Resolution2 = new Vector3Int(32, 2, 16);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        SrcVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        SrcVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueX = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        DstVectorFieldValueY = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        DstVectorFieldValueZ = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        PosVectorFieldValueX = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        PosVectorFieldValueY = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        PosVectorFieldValueZ = new float[Resolution2.x * Resolution2.y * Resolution2.z];
        float[] DstVectorFieldValue = new float[3 * Resolution2.x * Resolution2.y * Resolution2.z];
        float[] PosVectorFieldValue = new float[3 * Resolution2.x * Resolution2.y * Resolution2.z];


        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (int)(x % 2) * 10;
                    SrcVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (int)(y % 2) * 10;
                    SrcVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (int)(z % 2) * 10;
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
                    PosVectorFieldValue[3 * (z * Resolution2.x * Resolution2.y + y * Resolution2.x + x)] = PosVectorFieldValueX[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x];
                    PosVectorFieldValue[3 * (z * Resolution2.x * Resolution2.y + y * Resolution2.x + x) + 1] = PosVectorFieldValueY[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x];
                    PosVectorFieldValue[3 * (z * Resolution2.x * Resolution2.y + y * Resolution2.x + x) + 2] = PosVectorFieldValueZ[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x];
                }
            }
        }

        ComputeBuffer SampleDstData = new ComputeBuffer(3 * Resolution2.x * Resolution2.y * Resolution2.z, sizeof(float));
        ComputeBuffer SamplePosData = new ComputeBuffer(3 * Resolution2.x * Resolution2.y * Resolution2.z, sizeof(float));
        SamplePosData.SetData(PosVectorFieldValue);

        CCVPosField.resize(Resolution2, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCVSrcField.resize(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CCVDstField.resize(Resolution2, Origin, Spacing);

        CCVSrcField.sampleField(CCVPosField, CCVDstField);
        CCVSrcField.sampleField(SamplePosData, SampleDstData);

        CCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        CCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        CCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);
        SampleDstData.GetData(DstVectorFieldValue);

        for (int z = 0; z < Resolution2.z; z++)
        {
            for (int y = 0; y < Resolution2.y; y++)
            {
                for (int x = 0; x < Resolution2.x; x++)
                {
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x] - 10 * (int)(((int)(x / 2)) % 2)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x] - 10 * (int)(y % 2)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x] - 10 * (int)(z % 2)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValue[3 * (z * Resolution2.x * Resolution2.y + y * Resolution2.x + x)] - DstVectorFieldValueX[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x]) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValue[3 * (z * Resolution2.x * Resolution2.y + y * Resolution2.x + x) + 1] - DstVectorFieldValueY[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x]) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValue[3 * (z * Resolution2.x * Resolution2.y + y * Resolution2.x + x) + 2] - DstVectorFieldValueZ[z * Resolution2.x * Resolution2.y + y * Resolution2.x + x]) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }
        #endregion
    }

    [Test]
    public void CellCenteredVectorFieldTest_SampleFieldCatmullRom()
    {
        //CatmullRom手算测试
        #region SampleTest1
        Vector3Int Resolution = new Vector3Int(4, 4, 4);
        Vector3 Origin = new Vector3(-10, -40, -50);
        Vector3 Spacing = new Vector3(10, 20, 30);

        float[] SrcVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] SrcVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] SrcVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;
                    SrcVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;
                    SrcVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 10 - 10 + 4;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        CCellCenteredVectorField CCVPosField = new CCellCenteredVectorField(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCellCenteredVectorField CCVSrcField = new CCellCenteredVectorField(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CCellCenteredVectorField CCVDstField = new CCellCenteredVectorField(Resolution, Origin, Spacing);

        CCVSrcField.sampleField(CCVPosField, CCVDstField, ESamplingAlgorithm.CATMULLROM);

        CCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        CCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        CCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[22] - 2.4415f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[22] - 2.4415f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[22] - 2.4415f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //MonoCatmullRom手算测试
        #region SampleTest2
        Resolution = new Vector3Int(4, 4, 4);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        SrcVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        SrcVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;
                    SrcVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;
                    SrcVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 10 - 10 + 4;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCVSrcField.resize(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CCVDstField.resize(Resolution, Origin, Spacing);

        CCVSrcField.sampleField(CCVPosField, CCVDstField, ESamplingAlgorithm.MONOCATMULLROM);

        CCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        CCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        CCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[22] - 2.437f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[22] - 2.437f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[22] - 2.437f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //测试只输入位置的情况下的采样
        #region SampleTest3
        Resolution = new Vector3Int(4, 4, 4);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        SrcVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        SrcVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstVectorFieldValue = new float[3 * Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValue = new float[3 * Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;
                    SrcVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;
                    SrcVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 10 - 10 + 4;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * 30 - 50 + 24;
                    PosVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x)] = PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                    PosVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 1] = PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                    PosVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 2] = PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                }
            }
        }

        ComputeBuffer SampleDstData = new ComputeBuffer(3 * Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        ComputeBuffer SamplePosData = new ComputeBuffer(3 * Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        SamplePosData.SetData(PosVectorFieldValue);

        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCVSrcField.resize(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CCVDstField.resize(Resolution, Origin, Spacing);

        CCVSrcField.sampleField(CCVPosField, CCVDstField, ESamplingAlgorithm.MONOCATMULLROM);
        CCVSrcField.sampleField(SamplePosData, SampleDstData, ESamplingAlgorithm.MONOCATMULLROM);

        CCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        CCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        CCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);
        SampleDstData.GetData(DstVectorFieldValue);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[22] - 2.437f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[22] - 2.437f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[22] - 2.437f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValue[3 * 22] - 2.437f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValue[3 * 22 + 1] - 2.437f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValue[3 * 22 + 2] - 2.437f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion
    }

    [Test]
    public void CellCenteredVectorFieldTest_SampleFieldCubicBridson()
    {
        //CubicBridson手算测试
        #region SampleTest1
        Vector3Int Resolution = new Vector3Int(4, 4, 4);
        Vector3 Origin = new Vector3(-10, -40, -50);
        Vector3 Spacing = new Vector3(10, 20, 30);

        float[] SrcVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] SrcVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] SrcVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;
                    SrcVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;
                    SrcVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 10 - 10 + 4;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        CCellCenteredVectorField CCVPosField = new CCellCenteredVectorField(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCellCenteredVectorField CCVSrcField = new CCellCenteredVectorField(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CCellCenteredVectorField CCVDstField = new CCellCenteredVectorField(Resolution, Origin, Spacing);

        CCVSrcField.sampleField(CCVPosField, CCVDstField, ESamplingAlgorithm.CUBICBRIDSON);

        CCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        CCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        CCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[22] - 2.5255f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[22] - 2.5255f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[22] - 2.5255f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //ClampCubicBridson手算测试
        #region SampleTest2
        Resolution = new Vector3Int(4, 4, 4);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        SrcVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        SrcVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;
                    SrcVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;
                    SrcVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 10 - 10 + 4;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCVSrcField.resize(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CCVDstField.resize(Resolution, Origin, Spacing);

        CCVSrcField.sampleField(CCVPosField, CCVDstField, ESamplingAlgorithm.CLAMPCUBICBRIDSON);

        CCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        CCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        CCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[22] - 2.5255f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[22] - 2.5255f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[22] - 2.5255f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //测试只输入位置的情况下的采样
        #region SampleTest3
        Resolution = new Vector3Int(4, 4, 4);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        SrcVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        SrcVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstVectorFieldValue = new float[3 * Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValue = new float[3 * Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;
                    SrcVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;
                    SrcVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 10 - 10 + 4;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * 30 - 50 + 24;
                    PosVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x)] = PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                    PosVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 1] = PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                    PosVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 2] = PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                }
            }
        }

        ComputeBuffer SampleDstData = new ComputeBuffer(3 * Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        ComputeBuffer SamplePosData = new ComputeBuffer(3 * Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        SamplePosData.SetData(PosVectorFieldValue);

        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCVSrcField.resize(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CCVDstField.resize(Resolution, Origin, Spacing);

        CCVSrcField.sampleField(CCVPosField, CCVDstField, ESamplingAlgorithm.CLAMPCUBICBRIDSON);
        CCVSrcField.sampleField(SamplePosData, SampleDstData, ESamplingAlgorithm.CLAMPCUBICBRIDSON);

        CCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        CCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        CCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);
        SampleDstData.GetData(DstVectorFieldValue);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[22] - 2.5255f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[22] - 2.5255f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[22] - 2.5255f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValue[3 * 22] - 2.5255f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValue[3 * 22 + 1] - 2.5255f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValue[3 * 22 + 2] - 2.5255f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion
    }

    [Test]
    public void CellCenteredVectorFieldTest_Divergence()
    {
        Vector3Int Resolution = new Vector3Int(30, 31, 32);
        Vector3 Origin = new Vector3(0, 0, 0);
        Vector3 Spacing = new Vector3(10, 20, 30);

        float[] SrcVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] SrcVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] SrcVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstScalarFieldData = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (int)(x % 2);
                    SrcVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (int)(y % 3);
                    SrcVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = (int)(z % 4);
                }
            }
        }

        CCellCenteredVectorField CCVSrcField = new CCellCenteredVectorField(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CCellCenteredScalarField CCSDstField = new CCellCenteredScalarField(Resolution, Origin, Spacing);

        CCVSrcField.divergence(CCSDstField);

        CCSDstField.getGridData().GetData(DstScalarFieldData);

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    float TempX = (((int)(x + 1 < Resolution.x ? x + 1 : x) % 2) - (int)((x > 0 ? x - 1 : x) % 2)) / 10.0f * 0.5f;
                    float TempY = (((int)(y + 1 < Resolution.y ? y + 1 : y) % 3) - (int)((y > 0 ? y - 1 : y) % 3)) / 20.0f * 0.5f;
                    float TempZ = (((int)(z + 1 < Resolution.z ? z + 1 : z) % 4) - (int)((z > 0 ? z - 1 : z) % 4)) / 30.0f * 0.5f;
                    Assert.IsTrue(Mathf.Abs(DstScalarFieldData[z * Resolution.x * Resolution.y + y * Resolution.x + x] - TempX - TempY - TempZ) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }
    }

    [Test]
    public void CellCenteredVectorFieldTest_Curl()
    {
        Vector3Int Resolution = new Vector3Int(30, 31, 32);
        Vector3 Origin = new Vector3(0, 0, 0);
        Vector3 Spacing = new Vector3(10, 20, 30);

        float[] SrcVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] SrcVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] SrcVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstScalarFieldDataX = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstScalarFieldDataY = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstScalarFieldDataZ = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x + y + z;
                    SrcVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x + y + z;
                    SrcVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x + y + z;
                }
            }
        }

        CCellCenteredVectorField CCVSrcField = new CCellCenteredVectorField(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CCellCenteredVectorField CCVDstField = new CCellCenteredVectorField(Resolution, Origin, Spacing);

        CCVSrcField.curl(CCVDstField);

        CCVDstField.getGridDataX().GetData(DstScalarFieldDataX);
        CCVDstField.getGridDataY().GetData(DstScalarFieldDataY);
        CCVDstField.getGridDataZ().GetData(DstScalarFieldDataZ);

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    float Fx_ym = z + (y > 0 ? y - 1 : y) + x;
                    float Fx_yp = z + (y + 1 < Resolution.y ? y + 1 : y) + x;
                    float Fx_zm = (z > 0 ? z - 1 : z) + y + x;
                    float Fx_zp = (z + 1 < Resolution.z ? z + 1 : z) + y + x;
                    float Fy_xm = z + y + (x > 0 ? x - 1 : x);
                    float Fy_xp = z + y + (x + 1 < Resolution.x ? x + 1 : x);
                    float Fy_zm = (z > 0 ? z - 1 : z) + y + x;
                    float Fy_zp = (z + 1 < Resolution.z ? z + 1 : z) + y + x;
                    float Fz_xm = z + y + (x > 0 ? x - 1 : x);
                    float Fz_xp = z + y + (x + 1 < Resolution.x ? x + 1 : x);
                    float Fz_ym = z + (y > 0 ? y - 1 : y) + x;
                    float Fz_yp = z + (y + 1 < Resolution.y ? y + 1 : y) + x;
                    float TempX = 0.5f * (Fz_yp - Fz_ym) / 20.0f - 0.5f * (Fy_zp - Fy_zm) / 30.0f;
                    float TempY = 0.5f * (Fx_zp - Fx_zm) / 30.0f - 0.5f * (Fz_xp - Fz_xm) / 10.0f;
                    float TempZ = 0.5f * (Fy_xp - Fy_xm) / 10.0f - 0.5f * (Fx_yp - Fx_ym) / 20.0f;
                    Assert.IsTrue(Mathf.Abs(DstScalarFieldDataX[z * Resolution.x * Resolution.y + y * Resolution.x + x] - TempX) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstScalarFieldDataY[z * Resolution.x * Resolution.y + y * Resolution.x + x] - TempY) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstScalarFieldDataZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] - TempZ) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }
    }

    [Test]
    public void CellCenteredVectorFieldTest_Length()
    {
        Vector3Int Resolution = new Vector3Int(30, 31, 32);
        Vector3 Origin = new Vector3(-10, -15, -20);
        Vector3 Spacing = new Vector3(10, 20, 30);

        float[] SrcVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] SrcVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] SrcVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstScalarFieldData = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SrcVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x;
                    SrcVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y;
                    SrcVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z;
                }
            }
        }

        CCellCenteredVectorField CCVSrcField = new CCellCenteredVectorField(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CCellCenteredScalarField CCSDstField = new CCellCenteredScalarField(Resolution, Origin, Spacing);

        CCVSrcField.length(CCSDstField);

        CCSDstField.getGridData().GetData(DstScalarFieldData);

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    Vector3 CurVector = new Vector3(x, y, z);
                    Assert.IsTrue(Mathf.Abs(DstScalarFieldData[z * Resolution.x * Resolution.y + y * Resolution.x + x] - CurVector.magnitude) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }
    }
}
