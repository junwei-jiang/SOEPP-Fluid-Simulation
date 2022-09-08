using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EulerFluidEngine
{
    public class CSmokeEmitterInvoker
    {
        private static bool m_Initialized = false;

        private static ComputeShader m_SmokeEmitterTool = null;

        private static int m_EmitteVortexCollisionDensityAndTemperatureKernel = -1;
        private static int m_EmitteVortexCollisionVelXKernel = -1;
        private static int m_EmitteVortexCollisionVelYKernel = -1;
        private static int m_EmitteVortexCollisionVelZKernel = -1;
        private static int m_EmitteVortexLeapFroggingDensityAndTemperatureKernel = -1;
        private static int m_EmitteVortexLeapFroggingVelXKernel = -1;
        private static int m_EmitteVortexLeapFroggingVelYKernel = -1;
        private static int m_EmitteVortexLeapFroggingVelZKernel = -1;
        private static int m_Emitte2DSmokeDensityAndTemperatureKernel = -1;
        private static int m_Emitte3DSmokeDensityAndTemperatureKernel = -1;
        private static int m_Emitte2DSmokeVelXKernel = -1;
        private static int m_Emitte2DSmokeVelYKernel = -1;
        private static int m_Emitte2DSmokeVelZKernel = -1;

        #region Init
        public static void init()
        {
            if (m_Initialized) return;

            m_SmokeEmitterTool = Resources.Load("Shaders/SmokeEmitter") as ComputeShader;

            m_EmitteVortexCollisionDensityAndTemperatureKernel = m_SmokeEmitterTool.FindKernel("emitteVortexCollisionDensityAndTemperature");
            m_EmitteVortexCollisionVelXKernel = m_SmokeEmitterTool.FindKernel("emitteVortexCollisionVelX");
            m_EmitteVortexCollisionVelYKernel = m_SmokeEmitterTool.FindKernel("emitteVortexCollisionVelY");
            m_EmitteVortexCollisionVelZKernel = m_SmokeEmitterTool.FindKernel("emitteVortexCollisionVelZ");
            m_EmitteVortexLeapFroggingDensityAndTemperatureKernel = m_SmokeEmitterTool.FindKernel("emitteVortexLeapFroggingDensityAndTemperature");
            m_EmitteVortexLeapFroggingVelXKernel = m_SmokeEmitterTool.FindKernel("emitteVortexLeapFroggingVelX");
            m_EmitteVortexLeapFroggingVelYKernel = m_SmokeEmitterTool.FindKernel("emitteVortexLeapFroggingVelY");
            m_EmitteVortexLeapFroggingVelZKernel = m_SmokeEmitterTool.FindKernel("emitteVortexLeapFroggingVelZ");
            m_Emitte2DSmokeDensityAndTemperatureKernel = m_SmokeEmitterTool.FindKernel("emitte2DSmokeDensityAndTemperature");
            m_Emitte3DSmokeDensityAndTemperatureKernel = m_SmokeEmitterTool.FindKernel("emitte3DSmokeDensityAndTemperature");
            m_Emitte2DSmokeVelXKernel = m_SmokeEmitterTool.FindKernel("emitte2DSmokeVelX");
            m_Emitte2DSmokeVelYKernel = m_SmokeEmitterTool.FindKernel("emitte2DSmokeVelY");
            m_Emitte2DSmokeVelZKernel = m_SmokeEmitterTool.FindKernel("emitte2DSmokeVelZ");

            m_Initialized = true;
        }
        #endregion

        public static void emitteVortexCollisionDensityAndTemperatureInvoker(float vEmitteDensity, float vEmitteTemperature, CCellCenteredScalarField voDensityField, CCellCenteredScalarField voTemperatureField, bool vEmitterFlag = true)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = voDensityField.getResolution();
            Vector3 Origin = voDensityField.getOrigin();
            Vector3 Spacing = voDensityField.getSpacing();

            int TotalThreadNum = (int)(Resolution.x * Resolution.y * Resolution.z);

            m_SmokeEmitterTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_SmokeEmitterTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_SmokeEmitterTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);

            m_SmokeEmitterTool.SetBool("EmitterFlag_emitteVortexCollisionDensityAndTemperature", vEmitterFlag);
            m_SmokeEmitterTool.SetFloat("EmitteDensity_emitteVortexCollisionDensityAndTemperature", vEmitteDensity);
            m_SmokeEmitterTool.SetFloat("EmitteTemperature_emitteVortexCollisionDensityAndTemperature", vEmitteTemperature);
            m_SmokeEmitterTool.SetBuffer(m_EmitteVortexCollisionDensityAndTemperatureKernel, "voDensityFieldData_emitteVortexCollisionDensityAndTemperature", voDensityField.getGridData());
            m_SmokeEmitterTool.SetBuffer(m_EmitteVortexCollisionDensityAndTemperatureKernel, "voTemperatureFieldData_emitteVortexCollisionDensityAndTemperature", voTemperatureField.getGridData());

            CGlobalMacroAndFunc.dispatchKernel(m_SmokeEmitterTool, m_EmitteVortexCollisionDensityAndTemperatureKernel, TotalThreadNum);
        }

        public static void emitteVortexCollisionVelInvoker(CFaceCenteredVectorField voFluidVelField)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = voFluidVelField.getResolution();
            Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
            Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
            Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
            Vector3 Origin = voFluidVelField.getOrigin();
            Vector3 Spacing = voFluidVelField.getSpacing();

            int TotalThreadNumX = (int)(ResolutionX.x * ResolutionX.y * ResolutionX.z);
            m_SmokeEmitterTool.SetInts("GridResolution", ResolutionX.x, ResolutionX.y, ResolutionX.z);
            m_SmokeEmitterTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_SmokeEmitterTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_SmokeEmitterTool.SetBuffer(m_EmitteVortexCollisionVelXKernel, "voFluidVelFieldDataX_emitteVortexCollisionVelX", voFluidVelField.getGridDataX());
            CGlobalMacroAndFunc.dispatchKernel(m_SmokeEmitterTool, m_EmitteVortexCollisionVelXKernel, TotalThreadNumX);

            int TotalThreadNumY = (int)(ResolutionY.x * ResolutionY.y * ResolutionY.z);
            m_SmokeEmitterTool.SetInts("GridResolution", ResolutionY.x, ResolutionY.y, ResolutionY.z);
            m_SmokeEmitterTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_SmokeEmitterTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_SmokeEmitterTool.SetBuffer(m_EmitteVortexCollisionVelYKernel, "voFluidVelFieldDataY_emitteVortexCollisionVelY", voFluidVelField.getGridDataY());
            CGlobalMacroAndFunc.dispatchKernel(m_SmokeEmitterTool, m_EmitteVortexCollisionVelYKernel, TotalThreadNumY);

            int TotalThreadNumZ = (int)(ResolutionZ.x * ResolutionZ.y * ResolutionZ.z);
            m_SmokeEmitterTool.SetInts("GridResolution", ResolutionZ.x, ResolutionZ.y, ResolutionZ.z);
            m_SmokeEmitterTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_SmokeEmitterTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_SmokeEmitterTool.SetBuffer(m_EmitteVortexCollisionVelZKernel, "voFluidVelFieldDataZ_emitteVortexCollisionVelZ", voFluidVelField.getGridDataZ());
            CGlobalMacroAndFunc.dispatchKernel(m_SmokeEmitterTool, m_EmitteVortexCollisionVelZKernel, TotalThreadNumZ);
        }

        public static void emitteVortexLeapFroggingDensityAndTemperatureInvoker(float vEmitteDensity, float vEmitteTemperature, CCellCenteredScalarField voDensityField, CCellCenteredScalarField voTemperatureField, bool vEmitterFlag = true)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = voDensityField.getResolution();
            Vector3 Origin = voDensityField.getOrigin();
            Vector3 Spacing = voDensityField.getSpacing();

            int TotalThreadNum = (int)(Resolution.x * Resolution.y * Resolution.z);

            m_SmokeEmitterTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_SmokeEmitterTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_SmokeEmitterTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);

            m_SmokeEmitterTool.SetBool("EmitterFlag_emitteVortexLeapFroggingDensityAndTemperature", vEmitterFlag);
            m_SmokeEmitterTool.SetFloat("EmitteDensity_emitteVortexLeapFroggingDensityAndTemperature", vEmitteDensity);
            m_SmokeEmitterTool.SetFloat("EmitteTemperature_emitteVortexLeapFroggingDensityAndTemperature", vEmitteTemperature);
            m_SmokeEmitterTool.SetBuffer(m_EmitteVortexLeapFroggingDensityAndTemperatureKernel, "voDensityFieldData_emitteVortexLeapFroggingDensityAndTemperature", voDensityField.getGridData());
            m_SmokeEmitterTool.SetBuffer(m_EmitteVortexLeapFroggingDensityAndTemperatureKernel, "voTemperatureFieldData_emitteVortexLeapFroggingDensityAndTemperature", voTemperatureField.getGridData());

            CGlobalMacroAndFunc.dispatchKernel(m_SmokeEmitterTool, m_EmitteVortexLeapFroggingDensityAndTemperatureKernel, TotalThreadNum);
        }

        public static void emitteVortexLeapFroggingVelInvoker(CFaceCenteredVectorField voFluidVelField)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = voFluidVelField.getResolution();
            Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
            Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
            Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
            Vector3 Origin = voFluidVelField.getOrigin();
            Vector3 Spacing = voFluidVelField.getSpacing();

            int TotalThreadNumX = (int)(ResolutionX.x * ResolutionX.y * ResolutionX.z);
            m_SmokeEmitterTool.SetInts("GridResolution", ResolutionX.x, ResolutionX.y, ResolutionX.z);
            m_SmokeEmitterTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_SmokeEmitterTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_SmokeEmitterTool.SetBuffer(m_EmitteVortexLeapFroggingVelXKernel, "voFluidVelFieldDataX_emitteVortexLeapFroggingVelX", voFluidVelField.getGridDataX());
            CGlobalMacroAndFunc.dispatchKernel(m_SmokeEmitterTool, m_EmitteVortexLeapFroggingVelXKernel, TotalThreadNumX);

            int TotalThreadNumY = (int)(ResolutionY.x * ResolutionY.y * ResolutionY.z);
            m_SmokeEmitterTool.SetInts("GridResolution", ResolutionY.x, ResolutionY.y, ResolutionY.z);
            m_SmokeEmitterTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_SmokeEmitterTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_SmokeEmitterTool.SetBuffer(m_EmitteVortexLeapFroggingVelYKernel, "voFluidVelFieldDataY_emitteVortexLeapFroggingVelY", voFluidVelField.getGridDataY());
            CGlobalMacroAndFunc.dispatchKernel(m_SmokeEmitterTool, m_EmitteVortexLeapFroggingVelYKernel, TotalThreadNumY);

            int TotalThreadNumZ = (int)(ResolutionZ.x * ResolutionZ.y * ResolutionZ.z);
            m_SmokeEmitterTool.SetInts("GridResolution", ResolutionZ.x, ResolutionZ.y, ResolutionZ.z);
            m_SmokeEmitterTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_SmokeEmitterTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_SmokeEmitterTool.SetBuffer(m_EmitteVortexLeapFroggingVelZKernel, "voFluidVelFieldDataZ_emitteVortexLeapFroggingVelZ", voFluidVelField.getGridDataZ());
            CGlobalMacroAndFunc.dispatchKernel(m_SmokeEmitterTool, m_EmitteVortexLeapFroggingVelZKernel, TotalThreadNumZ);
        }

        public static void emitte2DSmokeDensityAndTemperatureInvoker(float vEmitteDensity, float vEmitteTemperature, CCellCenteredScalarField voDensityField, CCellCenteredScalarField voTemperatureField)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = voDensityField.getResolution();
            Vector3 Origin = voDensityField.getOrigin();
            Vector3 Spacing = voDensityField.getSpacing();

            int TotalThreadNum = (int)(Resolution.x * Resolution.y * Resolution.z);

            m_SmokeEmitterTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_SmokeEmitterTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_SmokeEmitterTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);

            m_SmokeEmitterTool.SetFloat("EmitteDensity_emitte2DSmokeDensityAndTemperature", vEmitteDensity);
            m_SmokeEmitterTool.SetFloat("EmitteTemperature_emitte2DSmokeDensityAndTemperature", vEmitteTemperature);
            m_SmokeEmitterTool.SetBuffer(m_Emitte2DSmokeDensityAndTemperatureKernel, "voDensityFieldData_emitte2DSmokeDensityAndTemperature", voDensityField.getGridData());
            m_SmokeEmitterTool.SetBuffer(m_Emitte2DSmokeDensityAndTemperatureKernel, "voTemperatureFieldData_emitte2DSmokeDensityAndTemperature", voTemperatureField.getGridData());

            CGlobalMacroAndFunc.dispatchKernel(m_SmokeEmitterTool, m_Emitte2DSmokeDensityAndTemperatureKernel, TotalThreadNum);
        }

        public static void emitte3DSmokeDensityAndTemperatureInvoker(float vEmitteDensity, float vEmitteTemperature, CCellCenteredScalarField voDensityField, CCellCenteredScalarField voTemperatureField)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = voDensityField.getResolution();
            Vector3 Origin = voDensityField.getOrigin();
            Vector3 Spacing = voDensityField.getSpacing();

            int TotalThreadNum = (int)(Resolution.x * Resolution.y * Resolution.z);

            m_SmokeEmitterTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_SmokeEmitterTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_SmokeEmitterTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);

            m_SmokeEmitterTool.SetFloat("EmitteDensity_emitte3DSmokeDensityAndTemperature", vEmitteDensity);
            m_SmokeEmitterTool.SetFloat("EmitteTemperature_emitte3DSmokeDensityAndTemperature", vEmitteTemperature);
            m_SmokeEmitterTool.SetBuffer(m_Emitte3DSmokeDensityAndTemperatureKernel, "voDensityFieldData_emitte3DSmokeDensityAndTemperature", voDensityField.getGridData());
            m_SmokeEmitterTool.SetBuffer(m_Emitte3DSmokeDensityAndTemperatureKernel, "voTemperatureFieldData_emitte3DSmokeDensityAndTemperature", voTemperatureField.getGridData());

            CGlobalMacroAndFunc.dispatchKernel(m_SmokeEmitterTool, m_Emitte3DSmokeDensityAndTemperatureKernel, TotalThreadNum);
        }

        public static void emitte2DSmokeVelInvoker(CFaceCenteredVectorField voFluidVelField)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = voFluidVelField.getResolution();
            Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
            Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
            Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
            Vector3 Origin = voFluidVelField.getOrigin();
            Vector3 Spacing = voFluidVelField.getSpacing();

            int TotalThreadNumX = (int)(ResolutionX.x * ResolutionX.y * ResolutionX.z);
            m_SmokeEmitterTool.SetInts("GridResolution", ResolutionX.x, ResolutionX.y, ResolutionX.z);
            m_SmokeEmitterTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_SmokeEmitterTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_SmokeEmitterTool.SetBuffer(m_Emitte2DSmokeVelXKernel, "voFluidVelFieldDataX_emitte2DSmokeVelX", voFluidVelField.getGridDataX());
            CGlobalMacroAndFunc.dispatchKernel(m_SmokeEmitterTool, m_Emitte2DSmokeVelXKernel, TotalThreadNumX);

            int TotalThreadNumY = (int)(ResolutionY.x * ResolutionY.y * ResolutionY.z);
            m_SmokeEmitterTool.SetInts("GridResolution", ResolutionY.x, ResolutionY.y, ResolutionY.z);
            m_SmokeEmitterTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_SmokeEmitterTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_SmokeEmitterTool.SetBuffer(m_Emitte2DSmokeVelYKernel, "voFluidVelFieldDataY_emitte2DSmokeVelY", voFluidVelField.getGridDataY());
            CGlobalMacroAndFunc.dispatchKernel(m_SmokeEmitterTool, m_Emitte2DSmokeVelYKernel, TotalThreadNumY);

            int TotalThreadNumZ = (int)(ResolutionZ.x * ResolutionZ.y * ResolutionZ.z);
            m_SmokeEmitterTool.SetInts("GridResolution", ResolutionZ.x, ResolutionZ.y, ResolutionZ.z);
            m_SmokeEmitterTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_SmokeEmitterTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_SmokeEmitterTool.SetBuffer(m_Emitte2DSmokeVelZKernel, "voFluidVelFieldDataZ_emitte2DSmokeVelZ", voFluidVelField.getGridDataZ());
            CGlobalMacroAndFunc.dispatchKernel(m_SmokeEmitterTool, m_Emitte2DSmokeVelZKernel, TotalThreadNumZ);
        }
    }
}