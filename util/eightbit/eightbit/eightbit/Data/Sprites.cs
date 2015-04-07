using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eightbit.Data
{
    class Sprites
    {
        private const string c_SerializeIdentifier = "Sprites";
        private List<Sprite> m_Sprites = new List<Sprite>();
        public static int SpriteOffsetSize;

        public Sprites()
        {

        }

        public int Count
        {
            get { return m_Sprites.Count; }
        }

        public Sprite this[int index]
        {
            get
            {
                if (index < 0 || index >= m_Sprites.Count)
                    return null;
                return m_Sprites[index];
            }
        }

        public void AddSprite()
        {
            Sprite s = new Sprite();
            m_Sprites.Add(s);
        }

        public void RemoveLastSprite()
        {
            if (m_Sprites.Count > 0)
                m_Sprites.RemoveAt(m_Sprites.Count - 1);
        }

        public bool Unserialize(Core.BinaryFileReader reader)
        {
            string id = reader.ReadString();
            if (id != c_SerializeIdentifier)
                return false;
            int version = reader.ReadInt();
            if (version >= 1)
            {
                SpriteOffsetSize = reader.ReadInt();
                if (SpriteOffsetSize == 0)
                    SpriteOffsetSize = 8;
            }
            else
            {
                SpriteOffsetSize = 8;
            }
            if (version >= 0)
            {
                // version 0
                int count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    Sprite sprite = new Sprite();
                    sprite.Unserialize(reader);
                    m_Sprites.Add(sprite);
                }
            }
            return true;
        }

        public void Serialize(Core.BinaryFileWriter writer)
        {
            writer.Write(c_SerializeIdentifier);
            writer.Write((int)1); // version
            writer.Write((int)SpriteOffsetSize);
            writer.Write(Count);
            for (int i = 0; i < Count; i++)
            {
                m_Sprites[i].Serialize(writer);
            }
        }

        public void Export(Core.BinaryFileWriter header_writer, Core.BinaryFileWriter data_writer)
        {
            int max_offset = 256 * SpriteOffsetSize;
            int offset_int = 0;
            int wasted = 0;
            header_writer.Write((byte)Count);

            for (int i = 0; i < Count; i++)
            {
                byte offset_byte = CreateOffset(ref offset_int, m_Sprites[i], max_offset, out wasted);
                header_writer.Write(offset_byte);
                header_writer.Write(m_Sprites[i].DataByte);
                m_Sprites[i].ExportFrames(data_writer);
                for (int j = 0; j < wasted; j++)
                    data_writer.Write((byte)0xFF);
            }
        }

        private byte CreateOffset(ref int offset, Sprite sprite, int max_offset, out int wastedbytes)
        {
            if (offset >= max_offset)
                throw new Exception(string.Format("Bad SpriteData offset size. Too large for specified max_offset size of {0}.", max_offset));

            wastedbytes = 0;

            int log2 = (int)Math.Log(SpriteOffsetSize, 2);
            int lo_mask = ((0xFF) << log2) & 0x00FF;
            int hi_mask = ((0xFF) << log2) & 0xFF00;

            byte data = (byte)((offset & lo_mask) | ((offset & hi_mask) >> 8));
            offset += sprite.ExportSize;
            if ((offset % SpriteOffsetSize) != 0)
            {
                // bytes wasted!
                wastedbytes = SpriteOffsetSize - (offset % SpriteOffsetSize);
                offset += wastedbytes;
            }
            return data;
        }

        public Sprite[] ToArray()
        {
            return m_Sprites.ToArray();
        }
    }

    public class Sprite
    {
        public Sprite()
        {
            m_Data = 0x00;
        }

        private byte m_Data = 0x00;
        public byte DataByte { get { return m_Data; } }
        private SpriteTile[][] m_StandardFrames = null, m_ExtendedFrames = null, m_ExtraFrames = null;

        // ================================================================================
        // Frames are exposed
        // ================================================================================

        public SpriteTile[] GetFrame(FrameTypeEnum FrameType, int index)
        {
            switch (FrameType)
            {
                case FrameTypeEnum.Standard:
                    if (!HasStandardSprites)
                        return null;
                    if (index < 0 || index >= 8)
                        return null;
                    return m_StandardFrames[index];
                case FrameTypeEnum.Extended:
                    if (!HasExtendedSprites)
                        return null;
                    if (index < 0 || index >= 4)
                        return null;
                    return m_ExtendedFrames[index];
                case FrameTypeEnum.Extra:
                    if (ExtraFrames == ExtraFramesEnum.Extra0)
                        return null;
                    if (index < 0 || index >= ExtraFramesInt)
                        return null;
                    return m_ExtraFrames[index];
                default:
                    return null;
            }
        }

        // ================================================================================
        // External properties
        // ================================================================================

        public ExtraFramesEnum ExtraFrames
        {
            get
            {
                return (ExtraFramesEnum)(m_Data & 0x07);
            }
            set
            {
                m_Data = (byte)((m_Data & 0xF8) + ((byte)(value) & 0x07));
                updateFrameData();
            }
        }

        public SpriteSizeEnum SpriteSize
        {
            get
            {
                return (SpriteSizeEnum)((m_Data & 0xC0) >> 6);
            }
            set
            {
                m_Data = (byte)((m_Data & 0x3F) + (((byte)(value) & 0x03) << 6));
                updateFrameData();
            }
        }

        public int ExtraFramesInt
        {
            get
            {
                switch (ExtraFrames)
                {
                    case ExtraFramesEnum.Extra0:
                        return 0;
                    case ExtraFramesEnum.Extra1:
                        return 1;
                    case ExtraFramesEnum.Extra2:
                        return 2;
                    case ExtraFramesEnum.Extra3:
                        return 3;
                    case ExtraFramesEnum.Extra4:
                        return 4;
                    case ExtraFramesEnum.Extra8:
                        return 8;
                    case ExtraFramesEnum.Extra16:
                        return 16;
                    case ExtraFramesEnum.Extra32:
                        return 32;
                    default:
                        throw new Exception("Unhandled ExtraFramesInt value");
                }
            }
        }

        public int ExportSize
        {
            get
            {
                return FrameCount * TilesPerFrame * 2;
            }
        }

        public int FrameCount
        {
            get
            {
                return (HasStandardSprites ? 8 : 0) + (HasExtendedSprites ? 4 : 0) + (ExtraFramesInt);
            }
        }

        private int TilesPerFrame
        {
            get
            {
                switch (SpriteSize)
                {
                    case SpriteSizeEnum.Size8x8:
                        return 1;
                    case SpriteSizeEnum.Size16x16:
                        return 4;
                    case SpriteSizeEnum.Size24x24:
                        return 9;
                    case SpriteSizeEnum.Size32x32:
                        return 16;
                    default:
                        throw new Exception("Unhandled BytesPerFrame size.");
                }
            }
        }

        public bool HasStandardSprites
        {
            get
            {
                return (m_Data & 0x08) != 0 ? true : false;
            }
            set
            {
                m_Data = (byte)((m_Data & 0xF7) + (value ? 0x08 : 0x00));
                updateFrameData();
            }
        }

        public bool HasExtendedSprites
        {
            get
            {
                return (m_Data & 0x10) != 0 ? true : false;
            }
            set
            {
                m_Data = (byte)((m_Data & 0xEF) + (value ? 0x10 : 0x00));
                updateFrameData();
            }
        }

        // ================================================================================
        // Internal update for frame data
        // ================================================================================

        private void updateFrameData()
        {
            updateFrames(HasStandardSprites, ref m_StandardFrames, 8);
            updateFrames(HasExtendedSprites, ref m_ExtendedFrames, 4);
            updateFrames(ExtraFrames != ExtraFramesEnum.Extra0, ref m_ExtraFrames, ExtraFramesInt);
        }

        private void updateFrames(bool hasData, ref SpriteTile[][] frameData, int frame_length)
        {
            if (!hasData)
            {
                frameData = null;
            }
            else
            {
                SpriteTile[][] newData;
                newData = new SpriteTile[frame_length][];
                for (int i = 0; (i < newData.Length); i++)
                {
                    newData[i] = new SpriteTile[TilesPerFrame];
                }
                if (frameData != null)
                {
                    int i;
                    for (i = 0; (i < newData.Length) && (i < frameData.Length); i++)
                    {
                        int j;
                        for (j = 0; (j < newData[i].Length) && (j < frameData[i].Length); j++)
                            newData[i][j] = frameData[i][j];
                        for (; (j < newData[i].Length); j++)  // j is set by the previous for loop.
                            newData[i][j] = new SpriteTile();
                    }
                    for (; (i < newData.Length); i++) // i is set by the previous for loop.
                    {
                        for (int j = 0; (j < newData[i].Length); j++)
                            newData[i][j] = new SpriteTile();
                    }
                }
                else
                {
                    for (int i = 0; i < newData.Length; i++)
                        for (int j = 0; j < newData[i].Length; j++)
                            newData[i][j] = new SpriteTile();
                }
                frameData = newData;
            }
        }

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        private string m_Name = string.Empty;

        public override string ToString()
        {
            if (m_Name == null || m_Name == string.Empty)
                return "Unnamed Sprite";
            return m_Name;
        }

        // ================================================================================
        // Import/Export data
        // ================================================================================

        public void Unserialize(Core.BinaryFileReader reader)
        {
            Name = reader.ReadString();
            m_Data = reader.ReadByte();
            if (HasStandardSprites)
            {
                m_StandardFrames = new SpriteTile[8][];
                for (int i = 0; i < 8; i++)
                {
                    m_StandardFrames[i] = new SpriteTile[TilesPerFrame];
                    for (int j = 0; j < m_StandardFrames[i].Length; j++)
                    {
                        m_StandardFrames[i][j] = new SpriteTile(reader);
                    }
                }
            }
            if (HasExtendedSprites)
            {
                m_ExtendedFrames = new SpriteTile[4][];
                for (int i = 0; i < 4; i++)
                {
                    m_ExtendedFrames[i] = new SpriteTile[TilesPerFrame];
                    for (int j = 0; j < m_ExtendedFrames[i].Length; j++)
                    {
                        m_ExtendedFrames[i][j] = new SpriteTile(reader);
                    }
                }
            }
            if (ExtraFrames != ExtraFramesEnum.Extra0)
            {
                m_ExtraFrames = new SpriteTile[ExtraFramesInt][];
                for (int i = 0; i < ExtraFramesInt; i++)
                {
                    m_ExtraFrames[i] = new SpriteTile[TilesPerFrame];
                    for (int j = 0; j < m_ExtraFrames[i].Length; j++)
                    {
                        m_ExtraFrames[i][j] = new SpriteTile(reader);
                    }
                }
            }
        }

        public void Serialize(Core.BinaryFileWriter writer)
        {
            writer.Write((string)Name);
            writer.Write((byte)m_Data);
            if (m_StandardFrames != null)
                for (int i = 0; i < m_StandardFrames.Length; i++)
                    for (int j = 0; j < m_StandardFrames[i].Length; j++)
                        m_StandardFrames[i][j].Serialize(writer);
            if (m_ExtendedFrames != null)
                for (int i = 0; i < m_ExtendedFrames.Length; i++)
                    for (int j = 0; j < m_ExtendedFrames[i].Length; j++)
                        m_ExtendedFrames[i][j].Serialize(writer);
            if (m_ExtraFrames != null)
                for (int i = 0; i < m_ExtraFrames.Length; i++)
                    for (int j = 0; j < m_ExtraFrames[i].Length; j++)
                        m_ExtraFrames[i][j].Serialize(writer);
        }

        public void ExportFrames(Core.BinaryFileWriter writer)
        {
            if (m_StandardFrames != null)
                for (int i = 0; i < m_StandardFrames.Length; i++)
                    for (int j = 0; j < m_StandardFrames[i].Length; j++)
                        m_StandardFrames[i][j].Serialize(writer);
            if (m_ExtendedFrames != null)
                for (int i = 0; i < m_ExtendedFrames.Length; i++)
                    for (int j = 0; j < m_ExtendedFrames[i].Length; j++)
                        m_ExtendedFrames[i][j].Serialize(writer);
            if (m_ExtraFrames != null)
                for (int i = 0; i < m_ExtraFrames.Length; i++)
                    for (int j = 0; j < m_ExtraFrames[i].Length; j++)
                        m_ExtraFrames[i][j].Serialize(writer);
        }

        // ================================================================================
        // Enums
        // ================================================================================

        public enum SpriteSizeEnum
        {
            Size8x8 = 0,
            Size16x16 = 1,
            Size24x24 = 2,
            Size32x32 = 3
        }

        public enum ExtraFramesEnum
        {
            Extra0 = 0,
            Extra1 = 1,
            Extra2 = 2,
            Extra3 = 3,
            Extra4 = 4,
            Extra8 = 5,
            Extra16 = 6,
            Extra32 = 7
        }

        public enum FrameTypeEnum
        {
            Standard,
            Extended,
            Extra
        }
    }

    public class SpriteTile
    {
        public SpriteTile()
        {
            m_Byte1 = m_Byte2 = 0;
        }

        public SpriteTile(Core.BinaryFileReader reader)
        {
            m_Byte1 = reader.ReadByte();
            m_Byte2 = reader.ReadByte();
        }

        // http://wiki.nesdev.com/w/index.php/PPU_OAM
        private byte m_Byte1 = 0x00, m_Byte2 = 0x00;
        public byte Tile
        {
            get { return m_Byte1; }
            set { m_Byte1 = value; }
        }

        public bool FlipH
        {
            get
            {
                return (m_Byte2 & 0x40) != 0 ? true : false;
            }
            set
            {
                m_Byte2 = (byte)((m_Byte2 & 0xBF) + (value ? 0x40 : 0x00));
            }
        }

        public bool FlipV
        {
            get
            {
                return (m_Byte2 & 0x80) != 0 ? true : false;
            }
            set
            {
                m_Byte2 = (byte)((m_Byte2 & 0x7F) + (value ? 0x80 : 0x00));
            }
        }

        public void Flip()
        {
            if (FlipH)
            {
                FlipH = false;
                FlipV = !FlipV;
            }
            else
            {
                FlipH = true;
            }
        }

        public void Serialize(Core.BinaryFileWriter writer)
        {
            writer.Write(m_Byte1);
            writer.Write(m_Byte2);
        }
    }
}
