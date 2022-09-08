using System;
using System.IO;
using UnityEngine;

namespace EulerFluidEngine
{
    public class CDataFile
    {
        private string m_FilePath;
        private long m_ReadPosition = 0;
        private FileStream m_FileStream = null;

        #region Constructor & Resize
        public CDataFile(string vFileFolder, string vFileName, bool vClearFile = false)
        {
            resize(vFileFolder,vFileName,vClearFile);
        }

        ~CDataFile()
        {
            m_FileStream.Close();
        }

        public void resize(string vFileFolder, string vFileName, bool vClearFile = false)
        {
            if(!Directory.Exists(vFileFolder))
            {
                Directory.CreateDirectory(vFileFolder);
            }

            m_FilePath = vFileFolder + "/" + vFileName + ".dat";
            m_ReadPosition = 0;
            if (m_FileStream != null) 
            {
                m_FileStream.Close();
            }

            m_FileStream = new FileStream(m_FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            if (vClearFile)
                clear();
        }
        #endregion

        #region Public Methods
        public void resetReadPosition()
        {
            m_ReadPosition = 0;
        }

        public void clear()
        {
            m_FileStream.Seek(0, SeekOrigin.Begin);
            m_FileStream.SetLength(0);
        }

        public void appendData(int vParticlesNum, ComputeBuffer vParticlesPos, ComputeBuffer vParticlesVel)
        {
            byte[] BytesToWrite;

            m_FileStream.Position = m_FileStream.Length;

            BytesToWrite = BitConverter.GetBytes(vParticlesNum);
            m_FileStream.Write(BytesToWrite, 0, sizeof(float));

            int ArraySize = vParticlesNum * 3 * sizeof(float);
            BytesToWrite = new byte[ArraySize];

            vParticlesPos.GetData(BytesToWrite, 0, 0, ArraySize);
            m_FileStream.Write(BytesToWrite, 0, BytesToWrite.Length);

            vParticlesPos.GetData(BytesToWrite, 0, 0, ArraySize);
            m_FileStream.Write(BytesToWrite, 0, BytesToWrite.Length);

            m_FileStream.Flush();
        }

        public bool readData(out int voParticlesNum, ComputeBuffer voParticlesPos, ComputeBuffer voParticlesVel)
        {
            byte[] BytesToRead=new byte[sizeof(float)];

            m_FileStream.Position = m_ReadPosition;

            m_FileStream.Read(BytesToRead, 0, sizeof(float));
            voParticlesNum = BitConverter.ToInt32(BytesToRead, 0);

            int ArraySize = voParticlesNum * 3 * sizeof(float);
            BytesToRead = new byte[ArraySize];

            m_FileStream.Read(BytesToRead, 0, ArraySize);
            voParticlesPos.SetData(BytesToRead);

            m_FileStream.Read(BytesToRead, 0, ArraySize);
            voParticlesPos.SetData(BytesToRead);

            m_FileStream.Flush();

            m_ReadPosition += 4 + 2 * ArraySize;
            if (m_ReadPosition >= m_FileStream.Length)
            {
                m_ReadPosition = 0;
                Debug.Log("文件" + m_FilePath + "已读取完成！");
                return true;
            }
            return false;
        }
        #endregion
    }
}