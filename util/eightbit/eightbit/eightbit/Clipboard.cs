using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eightbit
{
    class Clipboard
    {


        public bool HasData
        {
            get { return m_DataType != ClipboardData.Nothing; }
        }

        public ClipboardData DataType
        {
            get { return m_DataType; }
        }

        public void Clear()
        {
            m_DataType = ClipboardData.Nothing;
            m_ByteData = null;
        }

        public void Cut(ClipboardData dataType, byte[] data)
        {
            m_ClipType = ClipboardType.Cut;
            m_DataType = dataType;
            m_ByteData = data;
        }

        public void Copy(ClipboardData dataType, byte[] data)
        {
            m_ClipType = ClipboardType.Copy;
            m_DataType = dataType;
            m_ByteData = data;
        }

        public byte[] Paste()
        {
            byte[] value = (HasData) ? m_ByteData : null;
            if (m_ClipType == ClipboardType.Cut)
                Clear();
            return value;
        }

        private ClipboardData m_DataType;
        private ClipboardType m_ClipType;
        private byte[] m_ByteData;

        enum ClipboardType
        {
            Cut,
            Copy
        }
    }

    enum ClipboardData
    {
        Nothing,
        Tile,

    }
}
