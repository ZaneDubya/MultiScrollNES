using System;
using System.Collections.Generic;

namespace eightbit.Data.SpriteData
{
    public class Sprite
    {
        public Sprite()
        {
            m_Data = 0x00;
        }

        private byte m_Data = 0x00;
        public byte DataByte { get { return m_Data; } }
        private SpriteMetaTileFrame[][] m_StandardFrames = null, m_ExtendedFrames = null, m_ExtraFrames = null;

        // ================================================================================
        // Frames are exposed
        // ================================================================================

        public SpriteMetaTileFrame[] GetMetaTileFrame(FrameTypeEnum FrameType, int index)
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

        public SpriteMetaTileFrame[] GetMetaTileFrame(int index)
        {
            if (HasStandardSprites)
            {
                if (index >= 8)
                    index -= 8;
                else
                    return m_StandardFrames[index];
            }

            if (HasExtendedSprites)
            {
                if (index >= 4)
                    index -= 4;
                else
                    return m_ExtendedFrames[index];
            }

            if (ExtraFrames == ExtraFramesEnum.Extra0)
                return null;
            if (index < 0 || index >= ExtraFramesInt)
                return null;
            return m_ExtraFrames[index];
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

        public int MetaDataSize
        {
            get
            {
                return FrameCount * TilesPerFrame * 2;
            }
        }

        public Tuple<byte, byte>[] TileTransformTable
        {
            get
            {
                List<Tuple<byte, byte>> tile_transforms = new List<Tuple<byte, byte>>();

                for (int j = 0; j < FrameCount; j++)
                {
                    SpriteMetaTileFrame[] frame = GetMetaTileFrame(j);
                    for (int k = 0; k < frame.Length; k++)
                    {
                        Tuple<byte, byte> tileAndPage = new Tuple<byte, byte>(frame[k].Tile, frame[k].TilePage);
                        bool match = false;
                        for (int l = 0; l < tile_transforms.Count; l++)
                        {
                            if (tile_transforms[l].Item1 == tileAndPage.Item1 && tile_transforms[l].Item2 == tileAndPage.Item2)
                            {
                                match = true;
                                break;
                            }
                        }
                        if (!match)
                        {
                            tile_transforms.Add(tileAndPage);
                        }
                    }
                }

                if (tile_transforms.Count > 64)
                    throw new Exception("TileGfx count for sprite " + Name + " exceed maximum count of 64.");

                return tile_transforms.ToArray();
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

        private void updateFrames(bool hasData, ref SpriteMetaTileFrame[][] frameData, int frame_length)
        {
            if (!hasData)
            {
                frameData = null;
            }
            else
            {
                SpriteMetaTileFrame[][] newData;
                newData = new SpriteMetaTileFrame[frame_length][];
                for (int i = 0; (i < newData.Length); i++)
                {
                    newData[i] = new SpriteMetaTileFrame[TilesPerFrame];
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
                            newData[i][j] = new SpriteMetaTileFrame();
                    }
                    for (; (i < newData.Length); i++) // i is set by the previous for loop.
                    {
                        for (int j = 0; (j < newData[i].Length); j++)
                            newData[i][j] = new SpriteMetaTileFrame();
                    }
                }
                else
                {
                    for (int i = 0; i < newData.Length; i++)
                        for (int j = 0; j < newData[i].Length; j++)
                            newData[i][j] = new SpriteMetaTileFrame();
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

        public void Unserialize(Core.BinaryFileReader reader, int version)
        {
            Name = reader.ReadString();
            m_Data = reader.ReadByte();
            if (HasStandardSprites)
            {
                m_StandardFrames = new SpriteMetaTileFrame[8][];
                for (int i = 0; i < 8; i++)
                {
                    m_StandardFrames[i] = new SpriteMetaTileFrame[TilesPerFrame];
                    for (int j = 0; j < m_StandardFrames[i].Length; j++)
                    {
                        m_StandardFrames[i][j] = new SpriteMetaTileFrame(reader, version);
                    }
                }
            }
            if (HasExtendedSprites)
            {
                m_ExtendedFrames = new SpriteMetaTileFrame[4][];
                for (int i = 0; i < 4; i++)
                {
                    m_ExtendedFrames[i] = new SpriteMetaTileFrame[TilesPerFrame];
                    for (int j = 0; j < m_ExtendedFrames[i].Length; j++)
                    {
                        m_ExtendedFrames[i][j] = new SpriteMetaTileFrame(reader, version);
                    }
                }
            }
            if (ExtraFrames != ExtraFramesEnum.Extra0)
            {
                m_ExtraFrames = new SpriteMetaTileFrame[ExtraFramesInt][];
                for (int i = 0; i < ExtraFramesInt; i++)
                {
                    m_ExtraFrames[i] = new SpriteMetaTileFrame[TilesPerFrame];
                    for (int j = 0; j < m_ExtraFrames[i].Length; j++)
                    {
                        m_ExtraFrames[i][j] = new SpriteMetaTileFrame(reader, version);
                    }
                }
            }
        }

        public void Serialize(Core.BinaryFileWriter writer, int version)
        {
            writer.Write((string)Name);
            writer.Write((byte)m_Data);
            if (m_StandardFrames != null)
                for (int i = 0; i < m_StandardFrames.Length; i++)
                    for (int j = 0; j < m_StandardFrames[i].Length; j++)
                        m_StandardFrames[i][j].Serialize(writer, version);
            if (m_ExtendedFrames != null)
                for (int i = 0; i < m_ExtendedFrames.Length; i++)
                    for (int j = 0; j < m_ExtendedFrames[i].Length; j++)
                        m_ExtendedFrames[i][j].Serialize(writer, version);
            if (m_ExtraFrames != null)
                for (int i = 0; i < m_ExtraFrames.Length; i++)
                    for (int j = 0; j < m_ExtraFrames[i].Length; j++)
                        m_ExtraFrames[i][j].Serialize(writer, version);
        }

        public void ExportMetaSprites(Core.BinaryFileWriter writer, Tuple<byte, byte>[] tile_transform_table)
        {
            if (m_StandardFrames != null)
                for (int i = 0; i < m_StandardFrames.Length; i++)
                    for (int j = 0; j < m_StandardFrames[i].Length; j++)
                        m_StandardFrames[i][j].Export(writer, tile_transform_table);
            if (m_ExtendedFrames != null)
                for (int i = 0; i < m_ExtendedFrames.Length; i++)
                    for (int j = 0; j < m_ExtendedFrames[i].Length; j++)
                        m_ExtendedFrames[i][j].Export(writer, tile_transform_table);
            if (m_ExtraFrames != null)
                for (int i = 0; i < m_ExtraFrames.Length; i++)
                    for (int j = 0; j < m_ExtraFrames[i].Length; j++)
                        m_ExtraFrames[i][j].Export(writer, tile_transform_table);
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
}
