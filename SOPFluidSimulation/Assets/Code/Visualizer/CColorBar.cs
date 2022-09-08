using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace EulerFluidEngine
{
    public class CColorBar
    {
        public CColorBar(float vMaxValue)
        {
            resizeColorBar(vMaxValue);
        }

        public void resizeColorBar(float vMaxValue)
        {
            m_MaxValue = vMaxValue;
            if(m_ColorBar == null)
            {
                m_ColorBar = new Vector3[11];
                m_ColorBar[0] = new Vector3(0.0f, 0.007195f, 0.2590f);
                m_ColorBar[1] = new Vector3(0.0f, 0.0f, 0.5f);
                m_ColorBar[2] = new Vector3(0.0f, 0.3375f, 0.9f);
                m_ColorBar[3] = new Vector3(0.0f, 0.57f, 0.9f);
                m_ColorBar[4] = new Vector3(0.0032514f, 0.735f, 0.181f);
                m_ColorBar[5] = new Vector3(0.0065028f, 0.9f, 0.100473f);
                m_ColorBar[6] = new Vector3(0.228251f, 0.9f, 0.0502f);
                m_ColorBar[7] = new Vector3(0.45f, 0.9f, 0.0f);
                m_ColorBar[8] = new Vector3(0.9f, 0.45f, 0.0f);
                m_ColorBar[9] = new Vector3(0.9f, 0.0f, 0.0f);
                m_ColorBar[10] = new Vector3(0.3f, 0.0f, 0.0f);
            }     
        }

        public void setVisualizedCoff(float vVisualizedCoff)
        {
            m_VisualizedCoff = vVisualizedCoff;
        }

        public void Write2File(string path, CCellCenteredScalarField vOutputField)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    Vector3Int Resolution = vOutputField.getResolution();
                    Vector3 Origin = vOutputField.getOrigin();
                    Vector3 Spacing = vOutputField.getSpacing();
                    int TotalDim = Resolution.x * Resolution.y * Resolution.z;
                    float[] FieldValue = new float[TotalDim];
                    vOutputField.getGridData().GetData(FieldValue);

                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    //for (int z = 0; z < Resolution.z; z++)
                    //{
                    //    for (int y = 0; y < Resolution.y; y++)
                    //    {
                    //        for (int x = 0; x < Resolution.x; x++)
                    //        {
                    //            int CurLinearIndex = z * Resolution.x * Resolution.y + y * Resolution.x + x;
                    //            Vector3 CurPos = Origin + new Vector3((x + 0.5f) * Spacing.x, (y + 0.5f) * Spacing.y, (z + 0.5f) * Spacing.z);
                    //            if (CurPos.x >= 0.0f && CurPos.x <= 2.0f * CGlobalMacroAndFunc.M_PI && CurPos.y >= 0.0f && CurPos.y <= 2.0f * CGlobalMacroAndFunc.M_PI)
                    //            {
                    //                Vector3 Color = toRGB(Mathf.Abs(FieldValue[CurLinearIndex]));
                    //                sw.WriteLine("{0:G} {1:G} {2:G}", Color.x, Color.y, Color.z);
                    //            }
                    //        }
                    //    }
                    //}
                    for (int i = 0; i < TotalDim; i++)
                    {
                        Vector3 Color = toRGB(Mathf.Abs(FieldValue[i]));
                        sw.WriteLine("{0:G} {1:G} {2:G}", Color.x, Color.y, Color.z);
                    }
                    sw.Flush();
                }
            }
        } 

        public Vector3 toRGB(float vValue)
        {
            return __color(vValue / m_VisualizedCoff);
        }

        private Vector3 __lerp(Vector3 vVectorA, Vector3 vVectorB, float vOffset)
        {
            return (1.0f - vOffset) * vVectorA + vOffset * vVectorB;
        }

        private Vector3 __color(float vValue)
        {
            float X = Mathf.Min(Mathf.Max(vValue, 0.0f), 0.99f);
            int I = (int)(X * 10.0f);
            float FX = (X * 10.0f) - I;
            Vector3 Color = __lerp(m_ColorBar[I], m_ColorBar[I + 1], FX);
            return Color * 255.0f;
        }

        private float m_MaxValue;
        private Vector3[] m_ColorBar;
        private float m_VisualizedCoff = 10.0f;
    }
}