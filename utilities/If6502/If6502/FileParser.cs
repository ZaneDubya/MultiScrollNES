using System.Collections.Generic;
using System.IO;

namespace If6502
{
    class FileParser
    {
        private int m_LineIndex = 0;
        private List<StringTokenized> m_Lines;
        public List<StringTokenized> Lines { get { return m_Lines; } }

        public int Index
        {
            get
            {
                return m_LineIndex;
            }
            set
            {
                m_LineIndex = value;
            }
        }

        public string this[int index]
        {
            get
            {
                return m_Lines[index].ToString();
            }
        }

        public bool EOF
        {
            get
            {
                return m_LineIndex >= m_Lines.Count;
            }
        }

        public string ReadLine()
        {
            return m_Lines[m_LineIndex++].ToString();
        }

        public FileParser(string path)
        {
            m_Lines = new List<StringTokenized>();
            readFile(path);
        }

        private void readFile(string path)
        {
            string line;
            StreamReader file = new StreamReader(path);
            while ((line = file.ReadLine()) != null)
            {
                    StringTokenized ps = new StringTokenized(line);
                    m_Lines.Add(ps);
            }
            file.Close();
        }
    }
}
