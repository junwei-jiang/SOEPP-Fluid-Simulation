using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using EulerFluidEngine;

public class FaceCenteredVectorFieldTest
{
    [Test]
    public void FaceCenteredVectorFieldTest_Constructor()
    {
        #region InitialVariable
        Vector3Int Resolution = new Vector3Int(16, 16, 16);
        Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Vector3 Origin = new Vector3(2, 2, 2);
        Vector3 Spacing = new Vector3(8, 8, 8);

        float[] InitialValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] InitialValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] InitialValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        float[] DstGridValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] DstGridValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] DstGridValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        for (int i = 0; i < ResolutionX.x * ResolutionX.y * ResolutionX.z; i++)
        {
            InitialValueX[i] = i;
        }
        for (int i = 0; i < ResolutionY.x * ResolutionY.y * ResolutionY.z; i++)
        {
            InitialValueY[i] = 2 * i;
        }
        for (int i = 0; i < ResolutionZ.x * ResolutionZ.y * ResolutionZ.z; i++)
        {
            InitialValueZ[i] = 4 * i;
        }

        ComputeBuffer GridDataX = new ComputeBuffer(ResolutionX.x * ResolutionX.y * ResolutionX.z, sizeof(float));
        ComputeBuffer GridDataY = new ComputeBuffer(ResolutionY.x * ResolutionY.y * ResolutionY.z, sizeof(float));
        ComputeBuffer GridDataZ = new ComputeBuffer(ResolutionZ.x * ResolutionZ.y * ResolutionZ.z, sizeof(float));
        GridDataX.SetData(InitialValueX);
        GridDataY.SetData(InitialValueY);
        GridDataZ.SetData(InitialValueZ);
        #endregion

        #region Constructor1
        CFaceCenteredVectorField FCVField1 = new CFaceCenteredVectorField(Resolution, Origin, Spacing);

        Assert.AreEqual(FCVField1.getResolution(), Resolution);
        Assert.AreEqual(FCVField1.getOrigin(), Origin);
        Assert.AreEqual(FCVField1.getSpacing(), Spacing);

        FCVField1.getGridDataX().GetData(DstGridValueX);
        FCVField1.getGridDataY().GetData(DstGridValueY);
        FCVField1.getGridDataZ().GetData(DstGridValueZ);
        for (int i = 0; i < ResolutionX.x * ResolutionX.y * ResolutionX.z; i++)
        {
            Assert.AreEqual(0, DstGridValueX[i]);
        }
        for (int i = 0; i < ResolutionY.x * ResolutionY.y * ResolutionY.z; i++)
        {
            Assert.AreEqual(0, DstGridValueY[i]);
        }
        for (int i = 0; i < ResolutionZ.x * ResolutionZ.y * ResolutionZ.z; i++)
        {
            Assert.AreEqual(0, DstGridValueZ[i]);
        }
        #endregion

        #region Constructor2
        CFaceCenteredVectorField FCVField2 = new CFaceCenteredVectorField(Resolution, Origin, Spacing, InitialValueX, InitialValueY, InitialValueZ);

        Assert.AreEqual(FCVField2.getResolution(), Resolution);
        Assert.AreEqual(FCVField2.getOrigin(), Origin);
        Assert.AreEqual(FCVField2.getSpacing(), Spacing);

        FCVField2.getGridDataX().GetData(DstGridValueX);
        FCVField2.getGridDataY().GetData(DstGridValueY);
        FCVField2.getGridDataZ().GetData(DstGridValueZ);
        for (int i = 0; i < ResolutionX.x * ResolutionX.y * ResolutionX.z; i++)
        {
            Assert.AreEqual(InitialValueX[i], DstGridValueX[i]);
        }
        for (int i = 0; i < ResolutionY.x * ResolutionY.y * ResolutionY.z; i++)
        {
            Assert.AreEqual(InitialValueY[i], DstGridValueY[i]);
        }
        for (int i = 0; i < ResolutionZ.x * ResolutionZ.y * ResolutionZ.z; i++)
        {
            Assert.AreEqual(InitialValueZ[i], DstGridValueZ[i]);
        }
        #endregion

        #region Constructor3
        CFaceCenteredVectorField FCVField3 = new CFaceCenteredVectorField(Resolution, Origin, Spacing, GridDataX, GridDataY, GridDataZ);

        Assert.AreEqual(FCVField3.getResolution(), Resolution);
        Assert.AreEqual(FCVField3.getOrigin(), Origin);
        Assert.AreEqual(FCVField3.getSpacing(), Spacing);

        float[] OtherValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] OtherValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] OtherValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        for (int i = 0; i < ResolutionX.x * ResolutionX.y * ResolutionX.z; i++)
        {
            OtherValueX[i] = InitialValueX[i] + 1;
        }
        for (int i = 0; i < ResolutionY.x * ResolutionY.y * ResolutionY.z; i++)
        {
            OtherValueY[i] = InitialValueY[i] + 1;
        }
        for (int i = 0; i < ResolutionZ.x * ResolutionZ.y * ResolutionZ.z; i++)
        {
            OtherValueZ[i] = InitialValueZ[i] + 1;
        }
        GridDataX.SetData(OtherValueX);
        GridDataY.SetData(OtherValueY);
        GridDataZ.SetData(OtherValueZ);

        FCVField3.getGridDataX().GetData(DstGridValueX);
        FCVField3.getGridDataY().GetData(DstGridValueY);
        FCVField3.getGridDataZ().GetData(DstGridValueZ);
        for (int i = 0; i < ResolutionX.x * ResolutionX.y * ResolutionX.z; i++)
        {
            Assert.AreEqual(InitialValueX[i], DstGridValueX[i]);
        }
        for (int i = 0; i < ResolutionY.x * ResolutionY.y * ResolutionY.z; i++)
        {
            Assert.AreEqual(InitialValueY[i], DstGridValueY[i]);
        }
        for (int i = 0; i < ResolutionZ.x * ResolutionZ.y * ResolutionZ.z; i++)
        {
            Assert.AreEqual(InitialValueZ[i], DstGridValueZ[i]);
        }

        GridDataX.SetData(InitialValueX);
        GridDataY.SetData(InitialValueY);
        GridDataZ.SetData(InitialValueZ);
        #endregion

        #region Constructor4
        CFaceCenteredVectorField FCVField4 = new CFaceCenteredVectorField(FCVField3);

        Assert.AreEqual(FCVField4.getResolution(), Resolution);
        Assert.AreEqual(FCVField4.getOrigin(), Origin);
        Assert.AreEqual(FCVField4.getSpacing(), Spacing);

        FCVField4.getGridDataX().GetData(DstGridValueX);
        FCVField4.getGridDataY().GetData(DstGridValueY);
        FCVField4.getGridDataZ().GetData(DstGridValueZ);
        for (int i = 0; i < ResolutionX.x * ResolutionX.y * ResolutionX.z; i++)
        {
            Assert.AreEqual(InitialValueX[i], DstGridValueX[i]);
        }
        for (int i = 0; i < ResolutionY.x * ResolutionY.y * ResolutionY.z; i++)
        {
            Assert.AreEqual(InitialValueY[i], DstGridValueY[i]);
        }
        for (int i = 0; i < ResolutionZ.x * ResolutionZ.y * ResolutionZ.z; i++)
        {
            Assert.AreEqual(InitialValueZ[i], DstGridValueZ[i]);
        }
        #endregion

        #region Resize
        Vector3Int TempResolution = new Vector3Int(64, 12, 23);
        Vector3Int TempResolutionX = TempResolution + new Vector3Int(1, 0, 0);
        Vector3Int TempResolutionY = TempResolution + new Vector3Int(0, 1, 0);
        Vector3Int TempResolutionZ = TempResolution + new Vector3Int(0, 0, 1);
        Vector3 TempOrigin = new Vector3(0, 5, 4);
        Vector3 TempSpacing = new Vector3(4, 2.2f, 1);

        float[] TempInitialValueX = new float[TempResolutionX.x * TempResolutionX.y * TempResolutionX.z];
        float[] TempInitialValueY = new float[TempResolutionY.x * TempResolutionY.y * TempResolutionY.z];
        float[] TempInitialValueZ = new float[TempResolutionZ.x * TempResolutionZ.y * TempResolutionZ.z];
        float[] TempDstGridDataX = new float[TempResolutionX.x * TempResolutionX.y * TempResolutionX.z];
        float[] TempDstGridDataY = new float[TempResolutionY.x * TempResolutionY.y * TempResolutionY.z];
        float[] TempDstGridDataZ = new float[TempResolutionZ.x * TempResolutionZ.y * TempResolutionZ.z];
        for (int i = 0; i < ResolutionX.x * ResolutionX.y * ResolutionX.z; i++)
        {
            InitialValueX[i] = i * 0.7f;
        }
        for (int i = 0; i < ResolutionY.x * ResolutionY.y * ResolutionY.z; i++)
        {
            TempInitialValueY[i] = i * 2.7f;
        }
        for (int i = 0; i < ResolutionZ.x * ResolutionZ.y * ResolutionZ.z; i++)
        {
            TempInitialValueZ[i] = i * 4.7f;
        }

        FCVField4.resize(TempResolution, TempOrigin, TempSpacing, TempInitialValueX, TempInitialValueY, TempInitialValueZ);

        Assert.AreEqual(FCVField4.getResolution(), TempResolution);
        Assert.AreEqual(FCVField4.getOrigin(), TempOrigin);
        Assert.AreEqual(FCVField4.getSpacing(), TempSpacing);

        FCVField4.getGridDataX().GetData(TempDstGridDataX);
        FCVField4.getGridDataY().GetData(TempDstGridDataY);
        FCVField4.getGridDataZ().GetData(TempDstGridDataZ);
        for (int i = 0; i < ResolutionX.x * ResolutionX.y * ResolutionX.z; i++)
        {
            Assert.AreEqual(TempInitialValueX[i], TempDstGridDataX[i]);
        }
        for (int i = 0; i < ResolutionY.x * ResolutionY.y * ResolutionY.z; i++)
        {
            Assert.AreEqual(TempInitialValueY[i], TempDstGridDataY[i]);
        }
        for (int i = 0; i < ResolutionZ.x * ResolutionZ.y * ResolutionZ.z; i++)
        {
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
    public void FaceCenteredVectorFieldTest_SampleFieldLinear()
    {
        //简单测试采样
        #region SampleTest1
        Vector3Int Resolution = new Vector3Int(2, 2, 1);
        Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Vector3 Origin = new Vector3(0, 0, 0);
        Vector3 Spacing = new Vector3(1, 1, 1);

        float[] SrcVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] SrcVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] SrcVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        float[] DstVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] DstVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] DstVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        float[] PosVectorFieldValueXX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] PosVectorFieldValueXY = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] PosVectorFieldValueXZ = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] PosVectorFieldValueYX = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] PosVectorFieldValueYY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] PosVectorFieldValueYZ = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] PosVectorFieldValueZX = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        float[] PosVectorFieldValueZY = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        float[] PosVectorFieldValueZZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    SrcVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = (int)(x % 2);
                    PosVectorFieldValueXX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x;
                    PosVectorFieldValueXY[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = y + 0.5f;
                    PosVectorFieldValueXZ[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = z + 0.5f;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    SrcVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = (int)(y % 2);
                    PosVectorFieldValueYX[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = x + 0.5f;
                    PosVectorFieldValueYY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y;
                    PosVectorFieldValueYZ[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = z + 0.5f;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    SrcVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = (int)(z % 2);
                    PosVectorFieldValueZX[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = x + 0.5f;
                    PosVectorFieldValueZY[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = y + 0.5f;
                    PosVectorFieldValueZZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z;
                }
            }
        }

        CCellCenteredVectorField CCVPosFieldX = new CCellCenteredVectorField(ResolutionX, Origin, Spacing, PosVectorFieldValueXX, PosVectorFieldValueXY, PosVectorFieldValueXZ);
        CCellCenteredVectorField CCVPosFieldY = new CCellCenteredVectorField(ResolutionY, Origin, Spacing, PosVectorFieldValueYX, PosVectorFieldValueYY, PosVectorFieldValueYZ);
        CCellCenteredVectorField CCVPosFieldZ = new CCellCenteredVectorField(ResolutionZ, Origin, Spacing, PosVectorFieldValueZX, PosVectorFieldValueZY, PosVectorFieldValueZZ);
        CFaceCenteredVectorField FCVSrcField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CFaceCenteredVectorField FCVDstField = new CFaceCenteredVectorField(Resolution, Origin, Spacing);

        FCVSrcField.sampleField(CCVPosFieldX, CCVPosFieldY, CCVPosFieldZ, FCVDstField);

        FCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        FCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        FCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[0] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[1] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[2] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[3] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[4] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[5] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[0] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[1] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[2] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[3] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[4] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[5] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[0] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[1] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[2] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[3] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[4] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[5] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[6] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[7] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    PosVectorFieldValueXX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x;
                    PosVectorFieldValueXY[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = y + 0.4f;
                    PosVectorFieldValueXZ[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = z + 0.4f;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    PosVectorFieldValueYX[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = x + 0.4f;
                    PosVectorFieldValueYY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y;
                    PosVectorFieldValueYZ[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = z + 0.4f;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    PosVectorFieldValueZX[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = x + 0.4f;
                    PosVectorFieldValueZY[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = y + 0.4f;
                    PosVectorFieldValueZZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z;
                }
            }
        }

        CCVPosFieldX.resize(ResolutionX, Origin, Spacing, PosVectorFieldValueXX, PosVectorFieldValueXY, PosVectorFieldValueXZ);
        CCVPosFieldY.resize(ResolutionY, Origin, Spacing, PosVectorFieldValueYX, PosVectorFieldValueYY, PosVectorFieldValueYZ);
        CCVPosFieldZ.resize(ResolutionZ, Origin, Spacing, PosVectorFieldValueZX, PosVectorFieldValueZY, PosVectorFieldValueZZ);
        FCVDstField.resize(Resolution, Origin, Spacing);

        FCVSrcField.sampleField(CCVPosFieldX, CCVPosFieldY, CCVPosFieldZ, FCVDstField);

        FCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        FCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        FCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[0] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[1] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[2] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[3] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[4] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[5] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[0] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[1] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[2] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[3] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[4] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[5] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[0] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[1] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[2] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[3] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[4] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[5] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[6] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[7] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    PosVectorFieldValueXX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x;
                    PosVectorFieldValueXY[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = y + 0.6f;
                    PosVectorFieldValueXZ[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = z + 0.6f;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    PosVectorFieldValueYX[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = x + 0.6f;
                    PosVectorFieldValueYY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y;
                    PosVectorFieldValueYZ[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = z + 0.6f;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    PosVectorFieldValueZX[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = x + 0.6f;
                    PosVectorFieldValueZY[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = y + 0.6f;
                    PosVectorFieldValueZZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z;
                }
            }
        }

        CCVPosFieldX.resize(ResolutionX, Origin, Spacing, PosVectorFieldValueXX, PosVectorFieldValueXY, PosVectorFieldValueXZ);
        CCVPosFieldY.resize(ResolutionY, Origin, Spacing, PosVectorFieldValueYX, PosVectorFieldValueYY, PosVectorFieldValueYZ);
        CCVPosFieldZ.resize(ResolutionZ, Origin, Spacing, PosVectorFieldValueZX, PosVectorFieldValueZY, PosVectorFieldValueZZ);
        FCVDstField.resize(Resolution, Origin, Spacing);

        FCVSrcField.sampleField(CCVPosFieldX, CCVPosFieldY, CCVPosFieldZ, FCVDstField);

        FCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        FCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        FCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[0] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[1] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[2] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[3] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[4] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[5] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[0] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[1] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[2] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[3] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[4] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[5] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[0] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[1] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[2] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[3] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[4] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[5] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[6] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[7] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //测试FCV网格采样FCV网格的复杂情况
        #region SampleTest2
        Resolution = new Vector3Int(2, 10, 20);
        ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        SrcVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        SrcVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        DstVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        DstVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        DstVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueXX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueXY = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueXZ = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueYX = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueYY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueYZ = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueZX = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueZY = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueZZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    SrcVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = (int)(x % 3) * 10 + (int)(y % 3) * 10;
                    PosVectorFieldValueXX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x * 10 - 10 + 3;
                    PosVectorFieldValueXY[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueXZ[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = z * 30 - 50 + 18;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    SrcVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = (int)(y % 4) * 10 + (int)(z % 3) * 10;
                    PosVectorFieldValueYX[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = x * 10 - 10 + 6;
                    PosVectorFieldValueYY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y * 20 - 40 - 4;
                    PosVectorFieldValueYZ[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = z * 30 - 50 + 18;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    SrcVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = (int)(z % 5) * 10 + (int)(x % 3) * 10;
                    PosVectorFieldValueZX[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = x * 10 - 10 + 6;
                    PosVectorFieldValueZY[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueZZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 30 - 50 + 15;
                }
            }
        }

        CCVPosFieldX.resize(ResolutionX, Origin, Spacing, PosVectorFieldValueXX, PosVectorFieldValueXY, PosVectorFieldValueXZ);
        CCVPosFieldY.resize(ResolutionY, Origin, Spacing, PosVectorFieldValueYX, PosVectorFieldValueYY, PosVectorFieldValueYZ);
        CCVPosFieldZ.resize(ResolutionZ, Origin, Spacing, PosVectorFieldValueZX, PosVectorFieldValueZY, PosVectorFieldValueZZ);
        FCVSrcField.resize(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        FCVDstField.resize(Resolution, Origin, Spacing);

        FCVSrcField.sampleField(CCVPosFieldX, CCVPosFieldY, CCVPosFieldZ, FCVDstField);

        FCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        FCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        FCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[0] - 4) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[1] - 14) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[2] - 21) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[3] - 14) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[4] - 24) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[5] - 31) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[30] - 4) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[31] - 14) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[32] - 21) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[0] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[1] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[2] - 9) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[3] - 9) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[22] - 11) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[23] - 11) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[0] - 6) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[1] - 15) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[2] - 6) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[3] - 15) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[20] - 16) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[21] - 25) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[22] - 16) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[23] - 25) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //测试CCV网格采样FCV网格的复杂情况
        #region SampleTest3
        Resolution = new Vector3Int(2, 10, 20);
        ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        SrcVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        SrcVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        DstVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 10 - 10 + 3;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y * 20 - 40 + 8;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * 30 - 50 + 15;
                }
            }
        }

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    SrcVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x * 10;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    SrcVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y * 10;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    SrcVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 10;
                }
            }
        }

        CCellCenteredVectorField CCVPosField = new CCellCenteredVectorField(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        FCVSrcField.resize(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CCellCenteredVectorField CCVDstField = new CCellCenteredVectorField(Resolution, Origin, Spacing);

        FCVSrcField.sampleField(CCVPosField, CCVDstField);

        CCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        CCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        CCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] - x * 10 - 3) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] - y * 10 - 4) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] - z * 10 - 5) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }
        #endregion

        //测试只输入位置的情况下的采样
        #region SampleTest4
        Resolution = new Vector3Int(2, 10, 20);
        ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        SrcVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        SrcVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
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
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 10 - 10 + 3;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y * 20 - 40 + 8;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * 30 - 50 + 15;
                    PosVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x)] = PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                    PosVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 1] = PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                    PosVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 2] = PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                }
            }
        }

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    SrcVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x * 10;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    SrcVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y * 10;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    SrcVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 10;
                }
            }
        }

        ComputeBuffer SampleDstData = new ComputeBuffer(3 * Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        ComputeBuffer SamplePosData = new ComputeBuffer(3 * Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        SamplePosData.SetData(PosVectorFieldValue);

        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        FCVSrcField.resize(Resolution, new Vector3(-10, -40, -50), new Vector3(10, 20, 30), SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CCVDstField.resize(Resolution, Origin, Spacing);

        FCVSrcField.sampleField(CCVPosField, CCVDstField);
        FCVSrcField.sampleField(SamplePosData, SampleDstData);

        CCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        CCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        CCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);
        SampleDstData.GetData(DstVectorFieldValue);

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] - x * 10 - 3) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] - y * 10 - 4) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] - z * 10 - 5) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x)] - DstVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x]) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 1] - DstVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x]) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 2] - DstVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x]) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }

        SampleDstData.Release();
        SamplePosData.Release();
        #endregion
    }

    [Test]
    public void FaceCenteredVectorFieldTest_SampleFieldCatmullRom()
    {
        //CatmullRom手算测试
        #region SampleTest1
        Vector3Int Resolution = new Vector3Int(3, 4, 4);
        Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Vector3 Origin = new Vector3(-10, -40, -50);
        Vector3 Spacing = new Vector3(10, 20, 30);

        float[] SrcVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] SrcVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] SrcVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        float[] DstVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] DstVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] DstVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        float[] PosVectorFieldValueXX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] PosVectorFieldValueXY = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] PosVectorFieldValueXZ = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] PosVectorFieldValueYX = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] PosVectorFieldValueYY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] PosVectorFieldValueYZ = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] PosVectorFieldValueZX = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        float[] PosVectorFieldValueZY = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        float[] PosVectorFieldValueZZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    SrcVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueXX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x * 10 - 10 - 1;
                    PosVectorFieldValueXY[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueXZ[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    SrcVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y * 10;
                    PosVectorFieldValueYX[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = x * 10 - 10 + 6;
                    PosVectorFieldValueYY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y * 20 - 40 - 4;
                    PosVectorFieldValueYZ[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = z * 30 - 50 + 18;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    SrcVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 10;
                    PosVectorFieldValueZX[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = x * 10 - 10 + 6;
                    PosVectorFieldValueZY[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueZZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 30 - 50 + 15;
                }
            }
        }

        CCellCenteredVectorField CCVPosFieldX = new CCellCenteredVectorField(ResolutionX, Origin, Spacing, PosVectorFieldValueXX, PosVectorFieldValueXY, PosVectorFieldValueXZ);
        CCellCenteredVectorField CCVPosFieldY = new CCellCenteredVectorField(ResolutionY, Origin, Spacing, PosVectorFieldValueYX, PosVectorFieldValueYY, PosVectorFieldValueYZ);
        CCellCenteredVectorField CCVPosFieldZ = new CCellCenteredVectorField(ResolutionZ, Origin, Spacing, PosVectorFieldValueZX, PosVectorFieldValueZY, PosVectorFieldValueZZ);
        CFaceCenteredVectorField FCVSrcField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CFaceCenteredVectorField FCVDstField = new CFaceCenteredVectorField(Resolution, Origin, Spacing);

        FCVSrcField.sampleField(CCVPosFieldX, CCVPosFieldY, CCVPosFieldZ, FCVDstField, ESamplingAlgorithm.CATMULLROM);

        FCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        FCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        FCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[22] - 2.4415f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //CatmullRom手算测试
        #region SampleTest2
        Resolution = new Vector3Int(4, 3, 4);
        ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        SrcVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        SrcVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        DstVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        DstVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        DstVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueXX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueXY = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueXZ = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueYX = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueYY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueYZ = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueZX = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueZY = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueZZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    SrcVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueXX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x * 10 - 10 - 1;
                    PosVectorFieldValueXY[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueXZ[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    SrcVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueYX[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = x * 10 - 10 + 4;
                    PosVectorFieldValueYY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y * 20 - 40 + 2;
                    PosVectorFieldValueYZ[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    SrcVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 10;
                    PosVectorFieldValueZX[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = x * 10 - 10 + 6;
                    PosVectorFieldValueZY[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueZZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 30 - 50 + 15;
                }
            }
        }

        CCVPosFieldX.resize(ResolutionX, Origin, Spacing, PosVectorFieldValueXX, PosVectorFieldValueXY, PosVectorFieldValueXZ);
        CCVPosFieldY.resize(ResolutionY, Origin, Spacing, PosVectorFieldValueYX, PosVectorFieldValueYY, PosVectorFieldValueYZ);
        CCVPosFieldZ.resize(ResolutionZ, Origin, Spacing, PosVectorFieldValueZX, PosVectorFieldValueZY, PosVectorFieldValueZZ);
        FCVSrcField.resize(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        FCVDstField.resize(Resolution, Origin, Spacing);

        FCVSrcField.sampleField(CCVPosFieldX, CCVPosFieldY, CCVPosFieldZ, FCVDstField, ESamplingAlgorithm.CATMULLROM);

        FCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        FCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        FCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[22] - 2.4415f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //CatmullRom手算测试
        #region SampleTest3
        Resolution = new Vector3Int(4, 4, 3);
        ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        SrcVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        SrcVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        DstVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        DstVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        DstVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueXX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueXY = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueXZ = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueYX = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueYY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueYZ = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueZX = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueZY = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueZZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    SrcVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueXX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x * 10 - 10 - 1;
                    PosVectorFieldValueXY[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueXZ[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    SrcVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueYX[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = x * 10 - 10 + 4;
                    PosVectorFieldValueYY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y * 20 - 40 + 2;
                    PosVectorFieldValueYZ[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    SrcVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueZX[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = x * 10 - 10 + 4;
                    PosVectorFieldValueZY[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueZZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 30 - 50 + 9;
                }
            }
        }

        CCVPosFieldX.resize(ResolutionX, Origin, Spacing, PosVectorFieldValueXX, PosVectorFieldValueXY, PosVectorFieldValueXZ);
        CCVPosFieldY.resize(ResolutionY, Origin, Spacing, PosVectorFieldValueYX, PosVectorFieldValueYY, PosVectorFieldValueYZ);
        CCVPosFieldZ.resize(ResolutionZ, Origin, Spacing, PosVectorFieldValueZX, PosVectorFieldValueZY, PosVectorFieldValueZZ);
        FCVSrcField.resize(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        FCVDstField.resize(Resolution, Origin, Spacing);

        FCVSrcField.sampleField(CCVPosFieldX, CCVPosFieldY, CCVPosFieldZ, FCVDstField, ESamplingAlgorithm.CATMULLROM);

        FCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        FCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        FCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[22] - 2.4415f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //测试CatmullRom只输入位置的情况下的采样
        #region SampleTest4
        Resolution = new Vector3Int(2, 10, 20);
        ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        SrcVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        SrcVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        DstVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstVectorFieldValue = new float[3 * Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValue = new float[3 * Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 10 - 10 + 3;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y * 20 - 40 + 8;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * 30 - 50 + 15;
                    PosVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x)] = PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                    PosVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 1] = PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                    PosVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 2] = PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                }
            }
        }

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    SrcVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x * 10;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    SrcVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y * 10;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    SrcVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 10;
                }
            }
        }

        ComputeBuffer SampleDstData = new ComputeBuffer(3 * Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        ComputeBuffer SamplePosData = new ComputeBuffer(3 * Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        SamplePosData.SetData(PosVectorFieldValue);

        CCellCenteredVectorField CCVPosField = new CCellCenteredVectorField(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        FCVSrcField.resize(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CCellCenteredVectorField CCVDstField = new CCellCenteredVectorField(Resolution, Origin, Spacing);

        FCVSrcField.sampleField(CCVPosField, CCVDstField, ESamplingAlgorithm.CATMULLROM);
        FCVSrcField.sampleField(SamplePosData, SampleDstData, ESamplingAlgorithm.CATMULLROM);

        CCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        CCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        CCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);
        SampleDstData.GetData(DstVectorFieldValue);

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x)] - DstVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x]) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 1] - DstVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x]) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 2] - DstVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x]) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }

        SampleDstData.Release();
        SamplePosData.Release();
        #endregion

        //MonoCatmullRom手算测试
        #region SampleTest5
        Resolution = new Vector3Int(3, 4, 4);
        ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        SrcVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        SrcVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        DstVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        DstVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        DstVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueXX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueXY = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueXZ = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueYX = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueYY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueYZ = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueZX = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueZY = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueZZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    SrcVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueXX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x * 10 - 10 - 1;
                    PosVectorFieldValueXY[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueXZ[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    SrcVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y * 10;
                    PosVectorFieldValueYX[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = x * 10 - 10 + 4;
                    PosVectorFieldValueYY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y * 20 - 40 + 2;
                    PosVectorFieldValueYZ[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    SrcVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 10;
                    PosVectorFieldValueZX[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = x * 10 - 10 + 6;
                    PosVectorFieldValueZY[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueZZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 30 - 50 + 15;
                }
            }
        }

        CCVPosFieldX.resize(ResolutionX, Origin, Spacing, PosVectorFieldValueXX, PosVectorFieldValueXY, PosVectorFieldValueXZ);
        CCVPosFieldY.resize(ResolutionY, Origin, Spacing, PosVectorFieldValueYX, PosVectorFieldValueYY, PosVectorFieldValueYZ);
        CCVPosFieldZ.resize(ResolutionZ, Origin, Spacing, PosVectorFieldValueZX, PosVectorFieldValueZY, PosVectorFieldValueZZ);
        FCVSrcField.resize(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        FCVDstField.resize(Resolution, Origin, Spacing);

        FCVSrcField.sampleField(CCVPosFieldX, CCVPosFieldY, CCVPosFieldZ, FCVDstField, ESamplingAlgorithm.MONOCATMULLROM);

        FCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        FCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        FCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[22] - 2.437f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //MonoCatmullRom手算测试
        #region SampleTest6
        Resolution = new Vector3Int(4, 3, 4);
        ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        SrcVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        SrcVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        DstVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        DstVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        DstVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueXX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueXY = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueXZ = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueYX = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueYY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueYZ = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueZX = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueZY = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueZZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    SrcVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueXX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x * 10 - 10 - 1;
                    PosVectorFieldValueXY[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueXZ[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    SrcVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueYX[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = x * 10 - 10 + 4;
                    PosVectorFieldValueYY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y * 20 - 40 + 2;
                    PosVectorFieldValueYZ[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    SrcVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 10;
                    PosVectorFieldValueZX[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = x * 10 - 10 + 6;
                    PosVectorFieldValueZY[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueZZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 30 - 50 + 15;
                }
            }
        }

        CCVPosFieldX.resize(ResolutionX, Origin, Spacing, PosVectorFieldValueXX, PosVectorFieldValueXY, PosVectorFieldValueXZ);
        CCVPosFieldY.resize(ResolutionY, Origin, Spacing, PosVectorFieldValueYX, PosVectorFieldValueYY, PosVectorFieldValueYZ);
        CCVPosFieldZ.resize(ResolutionZ, Origin, Spacing, PosVectorFieldValueZX, PosVectorFieldValueZY, PosVectorFieldValueZZ);
        FCVSrcField.resize(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        FCVDstField.resize(Resolution, Origin, Spacing);

        FCVSrcField.sampleField(CCVPosFieldX, CCVPosFieldY, CCVPosFieldZ, FCVDstField, ESamplingAlgorithm.MONOCATMULLROM);

        FCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        FCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        FCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[22] - 2.437f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //MonoCatmullRom手算测试
        #region SampleTest7
        Resolution = new Vector3Int(4, 4, 3);
        ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        SrcVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        SrcVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        DstVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        DstVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        DstVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueXX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueXY = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueXZ = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueYX = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueYY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueYZ = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueZX = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueZY = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueZZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    SrcVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueXX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x * 10 - 10 - 1;
                    PosVectorFieldValueXY[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueXZ[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    SrcVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueYX[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = x * 10 - 10 + 4;
                    PosVectorFieldValueYY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y * 20 - 40 + 2;
                    PosVectorFieldValueYZ[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    SrcVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueZX[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = x * 10 - 10 + 4;
                    PosVectorFieldValueZY[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueZZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 30 - 50 + 9;
                }
            }
        }

        CCVPosFieldX.resize(ResolutionX, Origin, Spacing, PosVectorFieldValueXX, PosVectorFieldValueXY, PosVectorFieldValueXZ);
        CCVPosFieldY.resize(ResolutionY, Origin, Spacing, PosVectorFieldValueYX, PosVectorFieldValueYY, PosVectorFieldValueYZ);
        CCVPosFieldZ.resize(ResolutionZ, Origin, Spacing, PosVectorFieldValueZX, PosVectorFieldValueZY, PosVectorFieldValueZZ);
        FCVSrcField.resize(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        FCVDstField.resize(Resolution, Origin, Spacing);

        FCVSrcField.sampleField(CCVPosFieldX, CCVPosFieldY, CCVPosFieldZ, FCVDstField, ESamplingAlgorithm.MONOCATMULLROM);

        FCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        FCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        FCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[22] - 2.437f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //测试MonoCatmullRom只输入位置的情况下的采样
        #region SampleTest8
        Resolution = new Vector3Int(2, 10, 20);
        ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        SrcVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        SrcVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        DstVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValue = new float[3 * Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValue = new float[3 * Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 10 - 10 + 3;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y * 20 - 40 + 8;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * 30 - 50 + 15;
                    PosVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x)] = PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                    PosVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 1] = PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                    PosVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 2] = PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                }
            }
        }

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    SrcVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x * 10;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    SrcVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y * 10;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    SrcVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 10;
                }
            }
        }

        SampleDstData = new ComputeBuffer(3 * Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        SamplePosData = new ComputeBuffer(3 * Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        SamplePosData.SetData(PosVectorFieldValue);

        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        FCVSrcField.resize(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CCVDstField.resize(Resolution, Origin, Spacing);

        FCVSrcField.sampleField(CCVPosField, CCVDstField, ESamplingAlgorithm.MONOCATMULLROM);
        FCVSrcField.sampleField(SamplePosData, SampleDstData, ESamplingAlgorithm.MONOCATMULLROM);

        CCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        CCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        CCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);
        SampleDstData.GetData(DstVectorFieldValue);

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x)] - DstVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x]) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 1] - DstVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x]) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 2] - DstVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x]) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }

        SampleDstData.Release();
        SamplePosData.Release();
        #endregion
    }

    [Test]
    public void FaceCenteredVectorFieldTest_SampleFieldCubicBridson()
    {
        //CubicBridson手算测试
        #region SampleTest1
        Vector3Int Resolution = new Vector3Int(3, 4, 4);
        Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Vector3 Origin = new Vector3(-10, -40, -50);
        Vector3 Spacing = new Vector3(10, 20, 30);

        float[] SrcVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] SrcVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] SrcVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        float[] DstVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] DstVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] DstVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        float[] PosVectorFieldValueXX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] PosVectorFieldValueXY = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] PosVectorFieldValueXZ = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] PosVectorFieldValueYX = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] PosVectorFieldValueYY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] PosVectorFieldValueYZ = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] PosVectorFieldValueZX = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        float[] PosVectorFieldValueZY = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        float[] PosVectorFieldValueZZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    SrcVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueXX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x * 10 - 10 - 1;
                    PosVectorFieldValueXY[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueXZ[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    SrcVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y * 10;
                    PosVectorFieldValueYX[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = x * 10 - 10 + 6;
                    PosVectorFieldValueYY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y * 20 - 40 - 4;
                    PosVectorFieldValueYZ[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = z * 30 - 50 + 18;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    SrcVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 10;
                    PosVectorFieldValueZX[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = x * 10 - 10 + 6;
                    PosVectorFieldValueZY[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueZZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 30 - 50 + 15;
                }
            }
        }

        CCellCenteredVectorField CCVPosFieldX = new CCellCenteredVectorField(ResolutionX, Origin, Spacing, PosVectorFieldValueXX, PosVectorFieldValueXY, PosVectorFieldValueXZ);
        CCellCenteredVectorField CCVPosFieldY = new CCellCenteredVectorField(ResolutionY, Origin, Spacing, PosVectorFieldValueYX, PosVectorFieldValueYY, PosVectorFieldValueYZ);
        CCellCenteredVectorField CCVPosFieldZ = new CCellCenteredVectorField(ResolutionZ, Origin, Spacing, PosVectorFieldValueZX, PosVectorFieldValueZY, PosVectorFieldValueZZ);
        CFaceCenteredVectorField FCVSrcField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CFaceCenteredVectorField FCVDstField = new CFaceCenteredVectorField(Resolution, Origin, Spacing);

        FCVSrcField.sampleField(CCVPosFieldX, CCVPosFieldY, CCVPosFieldZ, FCVDstField, ESamplingAlgorithm.CUBICBRIDSON);

        FCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        FCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        FCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[22] - 2.5255f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //CubicBridson手算测试
        #region SampleTest2
        Resolution = new Vector3Int(4, 3, 4);
        ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        SrcVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        SrcVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        DstVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        DstVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        DstVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueXX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueXY = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueXZ = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueYX = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueYY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueYZ = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueZX = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueZY = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueZZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    SrcVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueXX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x * 10 - 10 - 1;
                    PosVectorFieldValueXY[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueXZ[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    SrcVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueYX[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = x * 10 - 10 + 4;
                    PosVectorFieldValueYY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y * 20 - 40 + 2;
                    PosVectorFieldValueYZ[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    SrcVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 10;
                    PosVectorFieldValueZX[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = x * 10 - 10 + 6;
                    PosVectorFieldValueZY[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueZZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 30 - 50 + 15;
                }
            }
        }

        CCVPosFieldX.resize(ResolutionX, Origin, Spacing, PosVectorFieldValueXX, PosVectorFieldValueXY, PosVectorFieldValueXZ);
        CCVPosFieldY.resize(ResolutionY, Origin, Spacing, PosVectorFieldValueYX, PosVectorFieldValueYY, PosVectorFieldValueYZ);
        CCVPosFieldZ.resize(ResolutionZ, Origin, Spacing, PosVectorFieldValueZX, PosVectorFieldValueZY, PosVectorFieldValueZZ);
        FCVSrcField.resize(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        FCVDstField.resize(Resolution, Origin, Spacing);

        FCVSrcField.sampleField(CCVPosFieldX, CCVPosFieldY, CCVPosFieldZ, FCVDstField, ESamplingAlgorithm.CUBICBRIDSON);

        FCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        FCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        FCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[22] - 2.5255f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //CubicBridson手算测试
        #region SampleTest3
        Resolution = new Vector3Int(4, 4, 3);
        ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        SrcVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        SrcVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        DstVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        DstVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        DstVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueXX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueXY = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueXZ = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueYX = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueYY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueYZ = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueZX = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueZY = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueZZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    SrcVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueXX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x * 10 - 10 - 1;
                    PosVectorFieldValueXY[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueXZ[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    SrcVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueYX[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = x * 10 - 10 + 4;
                    PosVectorFieldValueYY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y * 20 - 40 + 2;
                    PosVectorFieldValueYZ[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    SrcVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueZX[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = x * 10 - 10 + 4;
                    PosVectorFieldValueZY[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueZZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 30 - 50 + 9;
                }
            }
        }

        CCVPosFieldX.resize(ResolutionX, Origin, Spacing, PosVectorFieldValueXX, PosVectorFieldValueXY, PosVectorFieldValueXZ);
        CCVPosFieldY.resize(ResolutionY, Origin, Spacing, PosVectorFieldValueYX, PosVectorFieldValueYY, PosVectorFieldValueYZ);
        CCVPosFieldZ.resize(ResolutionZ, Origin, Spacing, PosVectorFieldValueZX, PosVectorFieldValueZY, PosVectorFieldValueZZ);
        FCVSrcField.resize(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        FCVDstField.resize(Resolution, Origin, Spacing);

        FCVSrcField.sampleField(CCVPosFieldX, CCVPosFieldY, CCVPosFieldZ, FCVDstField, ESamplingAlgorithm.CUBICBRIDSON);

        FCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        FCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        FCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[22] - 2.5255f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //测试CubicBridson只输入位置的情况下的采样
        #region SampleTest4
        Resolution = new Vector3Int(2, 10, 20);
        ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        SrcVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        SrcVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        DstVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstVectorFieldValue = new float[3 * Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValue = new float[3 * Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 10 - 10 + 3;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y * 20 - 40 + 8;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * 30 - 50 + 15;
                    PosVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x)] = PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                    PosVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 1] = PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                    PosVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 2] = PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                }
            }
        }

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    SrcVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x * 10;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    SrcVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y * 10;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    SrcVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 10;
                }
            }
        }

        ComputeBuffer SampleDstData = new ComputeBuffer(3 * Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        ComputeBuffer SamplePosData = new ComputeBuffer(3 * Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        SamplePosData.SetData(PosVectorFieldValue);

        CCellCenteredVectorField CCVPosField = new CCellCenteredVectorField(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        FCVSrcField.resize(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CCellCenteredVectorField CCVDstField = new CCellCenteredVectorField(Resolution, Origin, Spacing);

        FCVSrcField.sampleField(CCVPosField, CCVDstField, ESamplingAlgorithm.CUBICBRIDSON);
        FCVSrcField.sampleField(SamplePosData, SampleDstData, ESamplingAlgorithm.CUBICBRIDSON);

        CCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        CCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        CCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);
        SampleDstData.GetData(DstVectorFieldValue);

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x)] - DstVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x]) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 1] - DstVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x]) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 2] - DstVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x]) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }

        SampleDstData.Release();
        SamplePosData.Release();
        #endregion

        //ClampCubicBridson手算测试
        #region SampleTest5
        Resolution = new Vector3Int(3, 4, 4);
        ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        SrcVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        SrcVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        DstVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        DstVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        DstVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueXX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueXY = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueXZ = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueYX = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueYY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueYZ = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueZX = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueZY = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueZZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    SrcVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueXX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x * 10 - 10 - 1;
                    PosVectorFieldValueXY[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueXZ[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    SrcVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y * 10;
                    PosVectorFieldValueYX[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = x * 10 - 10 + 4;
                    PosVectorFieldValueYY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y * 20 - 40 + 2;
                    PosVectorFieldValueYZ[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    SrcVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 10;
                    PosVectorFieldValueZX[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = x * 10 - 10 + 6;
                    PosVectorFieldValueZY[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueZZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 30 - 50 + 15;
                }
            }
        }

        CCVPosFieldX.resize(ResolutionX, Origin, Spacing, PosVectorFieldValueXX, PosVectorFieldValueXY, PosVectorFieldValueXZ);
        CCVPosFieldY.resize(ResolutionY, Origin, Spacing, PosVectorFieldValueYX, PosVectorFieldValueYY, PosVectorFieldValueYZ);
        CCVPosFieldZ.resize(ResolutionZ, Origin, Spacing, PosVectorFieldValueZX, PosVectorFieldValueZY, PosVectorFieldValueZZ);
        FCVSrcField.resize(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        FCVDstField.resize(Resolution, Origin, Spacing);

        FCVSrcField.sampleField(CCVPosFieldX, CCVPosFieldY, CCVPosFieldZ, FCVDstField, ESamplingAlgorithm.CLAMPCUBICBRIDSON);

        FCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        FCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        FCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueX[22] - 2.5255f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //ClampCubicBridson手算测试
        #region SampleTest6
        Resolution = new Vector3Int(4, 3, 4);
        ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        SrcVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        SrcVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        DstVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        DstVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        DstVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueXX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueXY = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueXZ = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueYX = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueYY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueYZ = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueZX = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueZY = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueZZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    SrcVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueXX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x * 10 - 10 - 1;
                    PosVectorFieldValueXY[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueXZ[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    SrcVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueYX[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = x * 10 - 10 + 4;
                    PosVectorFieldValueYY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y * 20 - 40 + 2;
                    PosVectorFieldValueYZ[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    SrcVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 10;
                    PosVectorFieldValueZX[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = x * 10 - 10 + 6;
                    PosVectorFieldValueZY[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueZZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 30 - 50 + 15;
                }
            }
        }

        CCVPosFieldX.resize(ResolutionX, Origin, Spacing, PosVectorFieldValueXX, PosVectorFieldValueXY, PosVectorFieldValueXZ);
        CCVPosFieldY.resize(ResolutionY, Origin, Spacing, PosVectorFieldValueYX, PosVectorFieldValueYY, PosVectorFieldValueYZ);
        CCVPosFieldZ.resize(ResolutionZ, Origin, Spacing, PosVectorFieldValueZX, PosVectorFieldValueZY, PosVectorFieldValueZZ);
        FCVSrcField.resize(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        FCVDstField.resize(Resolution, Origin, Spacing);

        FCVSrcField.sampleField(CCVPosFieldX, CCVPosFieldY, CCVPosFieldZ, FCVDstField, ESamplingAlgorithm.CLAMPCUBICBRIDSON);

        FCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        FCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        FCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueY[22] - 2.5255f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //ClampCubicBridson手算测试
        #region SampleTest7
        Resolution = new Vector3Int(4, 4, 3);
        ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        SrcVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        SrcVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        DstVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        DstVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        DstVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueXX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueXY = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueXZ = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        PosVectorFieldValueYX = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueYY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueYZ = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        PosVectorFieldValueZX = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueZY = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        PosVectorFieldValueZZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    SrcVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueXX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x * 10 - 10 - 1;
                    PosVectorFieldValueXY[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueXZ[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    SrcVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueYX[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = x * 10 - 10 + 4;
                    PosVectorFieldValueYY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y * 20 - 40 + 2;
                    PosVectorFieldValueYZ[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = z * 30 - 50 + 24;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    SrcVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = (x % 2) + (y % 3) + z;
                    PosVectorFieldValueZX[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = x * 10 - 10 + 4;
                    PosVectorFieldValueZY[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = y * 20 - 40 + 12;
                    PosVectorFieldValueZZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 30 - 50 + 9;
                }
            }
        }

        CCVPosFieldX.resize(ResolutionX, Origin, Spacing, PosVectorFieldValueXX, PosVectorFieldValueXY, PosVectorFieldValueXZ);
        CCVPosFieldY.resize(ResolutionY, Origin, Spacing, PosVectorFieldValueYX, PosVectorFieldValueYY, PosVectorFieldValueYZ);
        CCVPosFieldZ.resize(ResolutionZ, Origin, Spacing, PosVectorFieldValueZX, PosVectorFieldValueZY, PosVectorFieldValueZZ);
        FCVSrcField.resize(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        FCVDstField.resize(Resolution, Origin, Spacing);

        FCVSrcField.sampleField(CCVPosFieldX, CCVPosFieldY, CCVPosFieldZ, FCVDstField, ESamplingAlgorithm.CLAMPCUBICBRIDSON);

        FCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        FCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        FCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);

        Assert.IsTrue(Mathf.Abs(DstVectorFieldValueZ[22] - 2.5255f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        //测试ClampCubicBridson只输入位置的情况下的采样
        #region SampleTest8
        Resolution = new Vector3Int(2, 10, 20);
        ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Origin = new Vector3(-10, -40, -50);
        Spacing = new Vector3(10, 20, 30);

        SrcVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        SrcVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        SrcVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        DstVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorFieldValue = new float[3 * Resolution.x * Resolution.y * Resolution.z];
        PosVectorFieldValue = new float[3 * Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 10 - 10 + 3;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y * 20 - 40 + 8;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * 30 - 50 + 15;
                    PosVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x)] = PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                    PosVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 1] = PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                    PosVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 2] = PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x];
                }
            }
        }

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    SrcVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x * 10;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    SrcVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y * 10;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    SrcVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 10;
                }
            }
        }

        SampleDstData = new ComputeBuffer(3 * Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        SamplePosData = new ComputeBuffer(3 * Resolution.x * Resolution.y * Resolution.z, sizeof(float));
        SamplePosData.SetData(PosVectorFieldValue);

        CCVPosField.resize(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        FCVSrcField.resize(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CCVDstField.resize(Resolution, Origin, Spacing);

        FCVSrcField.sampleField(CCVPosField, CCVDstField, ESamplingAlgorithm.CLAMPCUBICBRIDSON);
        FCVSrcField.sampleField(SamplePosData, SampleDstData, ESamplingAlgorithm.CLAMPCUBICBRIDSON);

        CCVDstField.getGridDataX().GetData(DstVectorFieldValueX);
        CCVDstField.getGridDataY().GetData(DstVectorFieldValueY);
        CCVDstField.getGridDataZ().GetData(DstVectorFieldValueZ);
        SampleDstData.GetData(DstVectorFieldValue);

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x)] - DstVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x]) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 1] - DstVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x]) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldValue[3 * (z * Resolution.x * Resolution.y + y * Resolution.x + x) + 2] - DstVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x]) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }

        SampleDstData.Release();
        SamplePosData.Release();
        #endregion
    }

    [Test]
    public void FaceCenteredVectorFieldTest_Divergence()
    {
        Vector3Int Resolution = new Vector3Int(2, 10, 20);
        Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Vector3 Origin = new Vector3(-10, -40, -50);
        Vector3 Spacing = new Vector3(10, 20, 30);

        float[] SrcVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] SrcVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] SrcVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        float[] DstScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    SrcVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = (int)(x % 2);
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    SrcVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = (int)(y % 3);
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    SrcVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = (int)(z % 4);
                }
            }
        }

        CFaceCenteredVectorField FCVSrcField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CCellCenteredScalarField CCSDstField = new CCellCenteredScalarField(Resolution, Origin, Spacing);

        FCVSrcField.divergence(CCSDstField);

        CCSDstField.getGridData().GetData(DstScalarFieldValue);

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    float TempX = (((int)(x + 1 < Resolution.x + 1 ? x + 1 : x) % 2) - (int)(x % 2)) / 10.0f;
                    float TempY = (((int)(y + 1 < Resolution.y + 1 ? y + 1 : y) % 3) - (int)(y % 3)) / 20.0f;
                    float TempZ = (((int)(z + 1 < Resolution.z + 1 ? z + 1 : z) % 4) - (int)(z % 4)) / 30.0f;
                    Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] - TempX - TempY - TempZ) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }
    }

    [Test]
    public void FaceCenteredVectorFieldTest_Curl()
    {
        Vector3Int Resolution = new Vector3Int(2, 10, 20);
        Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Vector3 Origin = new Vector3(-10, -40, -50);
        Vector3 Spacing = new Vector3(10, 20, 30);

        float[] SrcVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] SrcVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] SrcVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        float[] PosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] PosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstVectorFieldAValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstVectorFieldAValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstVectorFieldAValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstVectorFieldBValueX = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstVectorFieldBValueY = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstVectorFieldBValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    PosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 10 - 10 + 5;
                    PosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y * 20 - 40 + 10;
                    PosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * 30 - 50 + 15;
                }
            }
        }

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    SrcVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x + y;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    SrcVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y + z;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    SrcVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = x + y + z;
                }
            }
        }

        CFaceCenteredVectorField FCVSrcField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CCellCenteredVectorField CCVSrcField = new CCellCenteredVectorField(Resolution, Origin, Spacing);
        CCellCenteredVectorField CCVPosField = new CCellCenteredVectorField(Resolution, Origin, Spacing, PosVectorFieldValueX, PosVectorFieldValueY, PosVectorFieldValueZ);
        CCellCenteredVectorField CCVDstField1 = new CCellCenteredVectorField(Resolution, Origin, Spacing);
        CCellCenteredVectorField CCVDstField2 = new CCellCenteredVectorField(Resolution, Origin, Spacing);

        FCVSrcField.sampleField(CCVPosField, CCVSrcField);

        FCVSrcField.curl(CCVDstField1);
        CCVSrcField.curl(CCVDstField2);

        CCVDstField1.getGridDataX().GetData(DstVectorFieldAValueX);
        CCVDstField1.getGridDataY().GetData(DstVectorFieldAValueY);
        CCVDstField1.getGridDataZ().GetData(DstVectorFieldAValueZ);
        CCVDstField2.getGridDataX().GetData(DstVectorFieldBValueX);
        CCVDstField2.getGridDataY().GetData(DstVectorFieldBValueY);
        CCVDstField2.getGridDataZ().GetData(DstVectorFieldBValueZ);

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldAValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] - DstVectorFieldBValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x]) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldAValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] - DstVectorFieldBValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x]) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(DstVectorFieldAValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] - DstVectorFieldBValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x]) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }
    }

    [Test]
    public void FaceCenteredVectorFieldTest_Length()
    {
        Vector3Int Resolution = new Vector3Int(2, 10, 20);
        Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Vector3 Origin = new Vector3(-10, -40, -50);
        Vector3 Spacing = new Vector3(10, 20, 30);

        float[] SrcVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] SrcVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] SrcVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        float[] DstScalarFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    SrcVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x - 0.5f;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    SrcVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y - 0.5f;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    SrcVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z - 0.5f;
                }
            }
        }

        CFaceCenteredVectorField FCVSrcField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, SrcVectorFieldValueX, SrcVectorFieldValueY, SrcVectorFieldValueZ);
        CCellCenteredScalarField CCSDstField = new CCellCenteredScalarField(Resolution, Origin, Spacing);

        FCVSrcField.length(CCSDstField);

        CCSDstField.getGridData().GetData(DstScalarFieldValue);

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    Vector3 CurVector = new Vector3(x, y, z);
                    Assert.IsTrue(Mathf.Abs(DstScalarFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] - CurVector.magnitude) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }
    }
}
