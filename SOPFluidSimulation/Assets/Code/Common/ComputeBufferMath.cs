using UnityEngine;

namespace EulerFluidEngine
{
    public static class CMathTool
    {
        private static bool m_Initialized = false;

        private static ComputeShader m_ComputeBufferMathTool = null;

        private static int m_CopyKernel = -1;
        private static int m_ScaleKernel = -1;
        private static int m_PlusKernel = -1;
        private static int m_PlusAlphaXKernel = -1;
        private static int m_SumKernel = -1;
        private static int m_DotKernel = -1;
        private static int m_GetMaxValueKernel = -1;
        private static int m_GetMinValueKernel = -1;
        private static int m_GetAbsMaxValueKernel = -1;
        private static int m_DetectInvalidValueKernel = -1;
        private static int m_TransferFloats2IntsKernel = -1;
        private static int m_TransferInts2FloatsKernel = -1;

        private static ComputeBuffer m_DotBuffer = new ComputeBuffer(CGlobalMacroAndFunc.MAX_NUM_OF_THREADS, sizeof(float));
        private static ComputeBuffer m_ReduceBufferA = new ComputeBuffer(65536, sizeof(float));
        private static ComputeBuffer m_ReduceBufferB = new ComputeBuffer(65536, sizeof(float));
        private static ComputeBuffer m_FlagBuffer = new ComputeBuffer(1, sizeof(float));

        #region Init&Free
        public static void init()
        {
            if (m_Initialized) return;

            free();

            m_ComputeBufferMathTool = Resources.Load("Shaders/ComputeBufferMathTool") as ComputeShader;

            m_CopyKernel = m_ComputeBufferMathTool.FindKernel("copy");
            m_ScaleKernel = m_ComputeBufferMathTool.FindKernel("scale");
            m_PlusKernel = m_ComputeBufferMathTool.FindKernel("plus");
            m_PlusAlphaXKernel = m_ComputeBufferMathTool.FindKernel("plusAlphaX");
            m_SumKernel = m_ComputeBufferMathTool.FindKernel("sum");
            m_DotKernel = m_ComputeBufferMathTool.FindKernel("dot");
            m_GetMaxValueKernel = m_ComputeBufferMathTool.FindKernel("getMaxValue");
            m_GetMinValueKernel = m_ComputeBufferMathTool.FindKernel("getMinValue");
            m_GetAbsMaxValueKernel = m_ComputeBufferMathTool.FindKernel("getAbsMaxValue");
            m_DetectInvalidValueKernel = m_ComputeBufferMathTool.FindKernel("detectInvalidValue");
            m_TransferFloats2IntsKernel = m_ComputeBufferMathTool.FindKernel("transferFloats2Ints");
            m_TransferInts2FloatsKernel = m_ComputeBufferMathTool.FindKernel("transferInts2Floats");

            Shader.SetGlobalFloat("ATMOICCOFF", CGlobalMacroAndFunc.ATMOICCOFF);
            Shader.SetGlobalFloat("MINATOMICCOFF", CGlobalMacroAndFunc.MINATOMICCOFF);

            m_DotBuffer = new ComputeBuffer(CGlobalMacroAndFunc.MAX_NUM_OF_THREADS, sizeof(float));
            m_ReduceBufferA = new ComputeBuffer(65536, sizeof(float));
            m_ReduceBufferB = new ComputeBuffer(65536, sizeof(float));
            m_FlagBuffer = new ComputeBuffer(1, sizeof(float));

            m_Initialized = true;
        }
        public static void free()
        {
            m_DotBuffer.Release();
            m_ReduceBufferA.Release();
            m_ReduceBufferB.Release();
            m_FlagBuffer.Release();

            m_Initialized = false;
        }
        #endregion

        #region MathMethods
        public static void copy(ComputeBuffer vVector, ComputeBuffer voVector)
        {
            if (!m_Initialized) init();

            CGlobalMacroAndFunc._ASSERTE(vVector.count == voVector.count, "拷贝Buffer时输入和输出维度不同！");

            m_ComputeBufferMathTool.SetBuffer(m_CopyKernel, "vVector_Copy", vVector);
            m_ComputeBufferMathTool.SetBuffer(m_CopyKernel, "voVector_Copy", voVector);

            CGlobalMacroAndFunc.dispatchKernel(m_ComputeBufferMathTool, m_CopyKernel, vVector.count, CGlobalMacroAndFunc.MAX_NUM_OF_THREADS_PER_BLOCK);
        }
        public static void scale(ComputeBuffer vioVector, float vScaleCoefficient)
        {
            if (!m_Initialized) init();

            m_ComputeBufferMathTool.SetFloat("vScaleCoefficient_Scale", vScaleCoefficient);
            m_ComputeBufferMathTool.SetBuffer(m_ScaleKernel, "vioVector_Scale", vioVector);

            CGlobalMacroAndFunc.dispatchKernel(m_ComputeBufferMathTool, m_ScaleKernel, vioVector.count, CGlobalMacroAndFunc.MAX_NUM_OF_THREADS_PER_BLOCK);
        }
        public static void plus(ComputeBuffer vioVector, float vScalarValue)
        {
            if (!m_Initialized) init();

            m_ComputeBufferMathTool.SetFloat("vScalarValue_Plus", vScalarValue);
            m_ComputeBufferMathTool.SetBuffer(m_PlusKernel, "vioVector_Plus", vioVector);

            CGlobalMacroAndFunc.dispatchKernel(m_ComputeBufferMathTool, m_PlusKernel, vioVector.count, CGlobalMacroAndFunc.MAX_NUM_OF_THREADS_PER_BLOCK);
        }
        public static void plusAlphaX(ComputeBuffer vioVector, ComputeBuffer vOther, float vAlpha)
        {
            if (!m_Initialized) init();

            CGlobalMacroAndFunc._ASSERTE(vioVector.count == vOther.count, "向量相加时输入向量维度不同！");

            m_ComputeBufferMathTool.SetFloat("vAlpha_PlusAlphaX", vAlpha);
            m_ComputeBufferMathTool.SetBuffer(m_PlusAlphaXKernel, "vOther_PlusAlphaX", vOther);
            m_ComputeBufferMathTool.SetBuffer(m_PlusAlphaXKernel, "vioVector_PlusAlphaX", vioVector);

            CGlobalMacroAndFunc.dispatchKernel(m_ComputeBufferMathTool, m_PlusAlphaXKernel, vioVector.count, CGlobalMacroAndFunc.MAX_NUM_OF_THREADS_PER_BLOCK);
        }
        //TODO:GPU2CPU
        public static float sum(ComputeBuffer vVector, int vSize)
        {
            if (!m_Initialized) init();

            CGlobalMacroAndFunc._ASSERTE(vVector.count != 0 && vSize != 0, "要求和的Buffer长度为零！");

            int TotalThreadNum = Mathf.Min(vVector.count, vSize);
            int NumOfThreadsPerBlock = 0, NumOfBlocksPerGrid = 0;

            ComputeBuffer Input = vVector;
            ComputeBuffer Output = m_ReduceBufferA;

            while (true)
            {
                CGlobalMacroAndFunc.fetchPropBlockGridSize1D(TotalThreadNum, CGlobalMacroAndFunc.MAX_NUM_OF_THREADS_PER_BLOCK, out NumOfThreadsPerBlock, out NumOfBlocksPerGrid);

                m_ComputeBufferMathTool.SetInt("TotalThreadNum", TotalThreadNum);
                m_ComputeBufferMathTool.SetBuffer(m_SumKernel, "vVector_Sum", Input);
                m_ComputeBufferMathTool.SetBuffer(m_SumKernel, "voVector_Sum", Output);

                m_ComputeBufferMathTool.Dispatch(m_SumKernel, NumOfBlocksPerGrid, 1, 1);

                TotalThreadNum = NumOfBlocksPerGrid;

                if (TotalThreadNum > 1)
                {
                    if (Input == vVector)
                    {
                        Input = Output;
                        Output = m_ReduceBufferB;
                    }
                    else
                    {
                        ComputeBuffer Temp = Input;
                        Input = Output;
                        Output = Temp;
                    }
                }
                else
                {
                    break;
                }
            }

            float[] ResultArray = new float[1];
            Output.GetData(ResultArray, 0, 0, 1);
            return ResultArray[0];
        }
        //GPU2CPU
        public static float dot(ComputeBuffer vVectorA, ComputeBuffer vVectorB)
        {
            if (!m_Initialized) init();

            CGlobalMacroAndFunc._ASSERTE(vVectorA.count == vVectorB.count, "Vector点乘时输入的Vector维度不同！");

            m_ComputeBufferMathTool.SetBuffer(m_DotKernel, "vVectorA_Dot", vVectorA);
            m_ComputeBufferMathTool.SetBuffer(m_DotKernel, "vVectorB_Dot", vVectorB);
            m_ComputeBufferMathTool.SetBuffer(m_DotKernel, "voVector_Dot", m_DotBuffer);

            CGlobalMacroAndFunc.dispatchKernel(m_ComputeBufferMathTool, m_DotKernel, vVectorA.count, CGlobalMacroAndFunc.MAX_NUM_OF_THREADS_PER_BLOCK);

            float Result = sum(m_DotBuffer, vVectorA.count);

            return Result;
        }
        //GPU2CPU
        public static float norm2(ComputeBuffer vVector)
        {
            if (!m_Initialized) init();

            m_ComputeBufferMathTool.SetBuffer(m_DotKernel, "vVectorA_Dot", vVector);
            m_ComputeBufferMathTool.SetBuffer(m_DotKernel, "vVectorB_Dot", vVector);
            m_ComputeBufferMathTool.SetBuffer(m_DotKernel, "voVector_Dot", m_DotBuffer);

            CGlobalMacroAndFunc.dispatchKernel(m_ComputeBufferMathTool, m_DotKernel, vVector.count, CGlobalMacroAndFunc.MAX_NUM_OF_THREADS_PER_BLOCK);

            float Result = sum(m_DotBuffer, vVector.count);

            return Result;
        }
        //GPU2CPU
        public static float getMaxValue(ComputeBuffer vVector)
        {
            if (!m_Initialized) init();

            int TotalThreadNum = vVector.count;
            int NumOfThreadsPerBlock = 0, NumOfBlocksPerGrid = 0;

            ComputeBuffer Input = vVector;
            ComputeBuffer Output = m_ReduceBufferA;

            while (true)
            {
                CGlobalMacroAndFunc.fetchPropBlockGridSize1D(TotalThreadNum, CGlobalMacroAndFunc.MAX_NUM_OF_THREADS_PER_BLOCK, out NumOfThreadsPerBlock, out NumOfBlocksPerGrid);

                m_ComputeBufferMathTool.SetInt("TotalThreadNum", TotalThreadNum);
                m_ComputeBufferMathTool.SetBuffer(m_GetMaxValueKernel, "vVector_GetMaxValue", Input);
                m_ComputeBufferMathTool.SetBuffer(m_GetMaxValueKernel, "voVector_GetMaxValue", Output);

                m_ComputeBufferMathTool.Dispatch(m_GetMaxValueKernel, NumOfBlocksPerGrid, 1, 1);

                TotalThreadNum = NumOfBlocksPerGrid;

                if (TotalThreadNum > 1)
                {
                    if (Input == vVector)
                    {
                        Input = Output;
                        Output = m_ReduceBufferB;
                    }
                    else
                    {
                        ComputeBuffer Temp = Input;
                        Input = Output;
                        Output = Temp;
                    }
                }
                else
                {
                    break;
                }
            }

            float[] ResultArray = new float[1];
            Output.GetData(ResultArray, 0, 0, 1);
            return ResultArray[0];
        }
        //GPU2CPU
        public static float getMinValue(ComputeBuffer vVector)
        {
            if (!m_Initialized) init();

            int TotalThreadNum = vVector.count;
            int NumOfThreadsPerBlock = 0, NumOfBlocksPerGrid = 0;

            ComputeBuffer Input = vVector;
            ComputeBuffer Output = m_ReduceBufferA;

            while (true)
            {
                CGlobalMacroAndFunc.fetchPropBlockGridSize1D(TotalThreadNum, CGlobalMacroAndFunc.MAX_NUM_OF_THREADS_PER_BLOCK, out NumOfThreadsPerBlock, out NumOfBlocksPerGrid);

                m_ComputeBufferMathTool.SetInt("TotalThreadNum", TotalThreadNum);
                m_ComputeBufferMathTool.SetBuffer(m_GetMinValueKernel, "vVector_GetMinValue", Input);
                m_ComputeBufferMathTool.SetBuffer(m_GetMinValueKernel, "voVector_GetMinValue", Output);

                m_ComputeBufferMathTool.Dispatch(m_GetMinValueKernel, NumOfBlocksPerGrid, 1, 1);

                TotalThreadNum = NumOfBlocksPerGrid;

                if (TotalThreadNum > 1)
                {
                    if (Input == vVector)
                    {
                        Input = Output;
                        Output = m_ReduceBufferB;
                    }
                    else
                    {
                        ComputeBuffer Temp = Input;
                        Input = Output;
                        Output = Temp;
                    }
                }
                else
                {
                    break;
                }
            }

            float[] ResultArray = new float[1];
            Output.GetData(ResultArray, 0, 0, 1);
            return ResultArray[0];
        }
        //GPU2CPU
        public static float getAbsMaxValue(ComputeBuffer vVector)
        {
            if (!m_Initialized) init();

            int TotalThreadNum = vVector.count;
            int NumOfThreadsPerBlock = 0, NumOfBlocksPerGrid = 0;

            ComputeBuffer Input = vVector;
            ComputeBuffer Output = m_ReduceBufferA;

            while (true)
            {
                CGlobalMacroAndFunc.fetchPropBlockGridSize1D(TotalThreadNum, CGlobalMacroAndFunc.MAX_NUM_OF_THREADS_PER_BLOCK, out NumOfThreadsPerBlock, out NumOfBlocksPerGrid);

                m_ComputeBufferMathTool.SetInt("TotalThreadNum", TotalThreadNum);
                m_ComputeBufferMathTool.SetBuffer(m_GetAbsMaxValueKernel, "vVector_GetAbsMaxValue", Input);
                m_ComputeBufferMathTool.SetBuffer(m_GetAbsMaxValueKernel, "voVector_GetAbsMaxValue", Output);

                m_ComputeBufferMathTool.Dispatch(m_GetAbsMaxValueKernel, NumOfBlocksPerGrid, 1, 1);

                TotalThreadNum = NumOfBlocksPerGrid;

                if (TotalThreadNum > 1)
                {
                    if (Input == vVector)
                    {
                        Input = Output;
                        Output = m_ReduceBufferB;
                    }
                    else
                    {
                        ComputeBuffer Temp = Input;
                        Input = Output;
                        Output = Temp;
                    }
                }
                else
                {
                    break;
                }
            }

            float[] ResultArray = new float[1];
            Output.GetData(ResultArray, 0, 0, 1);
            return ResultArray[0];
        }
        //CPU2GPU&GPU2CPU
        public static bool detectInvalidValue(ComputeBuffer vVector)
        {
            float[] ResultArray = new float[1];
            m_FlagBuffer.SetData(ResultArray);

            m_ComputeBufferMathTool.SetBuffer(m_DetectInvalidValueKernel, "vVector_DetectInvalidValue", vVector);
            m_ComputeBufferMathTool.SetBuffer(m_DetectInvalidValueKernel, "voVector_DetectInvalidValue", m_FlagBuffer);

            CGlobalMacroAndFunc.dispatchKernel(m_ComputeBufferMathTool, m_DetectInvalidValueKernel, vVector.count, CGlobalMacroAndFunc.MAX_NUM_OF_THREADS_PER_BLOCK);

            m_FlagBuffer.GetData(ResultArray);

            if (ResultArray[0] == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static void transferFloats2Ints(ComputeBuffer vFloats, ComputeBuffer voInts)
        {
            if (!m_Initialized) init();

            CGlobalMacroAndFunc._ASSERTE(vFloats.count <= voInts.count, "源float数组过大！");

            m_ComputeBufferMathTool.SetBuffer(m_TransferFloats2IntsKernel, "vFloats_TransferFloats2Ints", vFloats);
            m_ComputeBufferMathTool.SetBuffer(m_TransferFloats2IntsKernel, "voInts_TransferFloats2Ints", voInts);

            CGlobalMacroAndFunc.dispatchKernel(m_ComputeBufferMathTool, m_TransferFloats2IntsKernel, vFloats.count, CGlobalMacroAndFunc.MAX_NUM_OF_THREADS_PER_BLOCK);
        }
        public static void transferInts2Floats(ComputeBuffer vInts, ComputeBuffer voFloats)
        {
            if (!m_Initialized) init();

            CGlobalMacroAndFunc._ASSERTE(voFloats.count <= vInts.count, "目的float数组过大！");

            m_ComputeBufferMathTool.SetBuffer(m_TransferInts2FloatsKernel, "vInts_TransferInts2Floats", vInts);
            m_ComputeBufferMathTool.SetBuffer(m_TransferInts2FloatsKernel, "voFloats_TransferInts2Floats", voFloats);

            CGlobalMacroAndFunc.dispatchKernel(m_ComputeBufferMathTool, m_TransferInts2FloatsKernel, voFloats.count, CGlobalMacroAndFunc.MAX_NUM_OF_THREADS_PER_BLOCK);
        }
        #endregion
    }
}