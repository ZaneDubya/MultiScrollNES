using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace If6502
{
    class Program
    {
        static void Main(string[] args)
        {
            string path_in, path_out;

            /*args = new string[3];
            args[0] = "code";
            args[1] = "obj/code";
            args[2] = "-all";*/

            if (ReadArgs(args, out path_in, out path_out))
            {
                if (Option_CopyAll)
                {
                    foreach (string dirPath in Directory.GetDirectories(path_in, "*", SearchOption.AllDirectories))
                        if (!dirPath.Contains(path_out))
                            Directory.CreateDirectory(dirPath.Replace(path_in, path_out));
                    foreach (string file_in in Directory.GetFiles(path_in, "*.asm", SearchOption.AllDirectories))
                    {
                        string file_out = file_in.Replace(path_in, path_out);
                        DoFile(file_in, file_out);
                    }
                }
                else
                {
                    DoFile(path_in, path_out);
                }
            }
            else
            {

            }
        }

        static bool DoFile(string path_in, string path_out)
        {
            Console.Write("If6502 > " + Path.GetFileName(path_in) + "... ");
            FileParser reader = new FileParser(path_in);
            List<StringTokenized> lines = reader.Lines;

            Interpreter interpreter = new Interpreter();
            Console.WriteLine(interpreter.Interpret(lines));
            if (interpreter.Success)
            {
                WriteFile(lines, path_out);
                return true;
            }
            else
                return false;
        }

        static bool ReadArgs(string[] args, out string path_in, out string path_out)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(msg_usage);
                Console.WriteLine(string.Format(msg_error_file, "input or output"));
            }
            else if (args.Length == 1)
            {
                Console.WriteLine(msg_usage);
                Console.WriteLine(string.Format(msg_error_file, "output"));
            }
            else // args length >= 2
            {
                if (args.Length > 2)
                {
                    for (int i = 2; i < args.Length; i++)
                        switch (args[i])
                        {
                            case "-ophis":
                                Option_Ophis = true;
                                break;
                            case "-all":
                                Option_CopyAll = true;
                                break;
                            default:
                                Console.WriteLine(string.Format(msg_error_badoption, args[i]));
                                path_in = null;
                                path_out = null;
                                return false;
                        }
                }

                path_in = args[0];
                path_out = args[1];

                if (Option_CopyAll)
                {
                    if (path_in == string.Empty)
                    {
                        path_in = Path.GetDirectoryName(System.Environment.CurrentDirectory);
                    }
                    if (!Directory.Exists(path_in))
                    {
                        Console.WriteLine(string.Format(msg_error_doesnotexist, string.Format("input directory '{0}'", path_in)));
                        return false;
                    }
                    else
                    {
                        path_in = Path.GetFullPath(path_in);
                    }

                    if (Directory.Exists(path_out))
                    {
                        Console.WriteLine("Error: output directory cannot exist if option -all is used.");
                        return false;
                    }
                    else
                    {
                        Directory.CreateDirectory(path_out);
                        path_out = Path.GetFullPath(path_out);
                    }
                }
                else
                {
                    if (!File.Exists(path_in))
                    {
                        Console.WriteLine(string.Format(msg_error_doesnotexist, "input file"));
                        return false;
                    }
                    path_out = Path.GetFullPath(path_out);
                    if (!Directory.Exists(Path.GetDirectoryName(path_out)))
                    {
                        Console.WriteLine(string.Format(msg_error_doesnotexist, "output directory"));
                        return false;
                    }
                    if (Path.GetFileName(path_out) == string.Empty)
                    {
                        Console.WriteLine(msg_error_no_outputfilename);
                        return false;
                    }
                }

                return true;
            }

            path_in = null;
            path_out = null;
            return false;
        }

        public static bool Option_Ophis = false;
        public static bool Option_CopyAll = false;

        static string msg_usage = "Usage: if6502 srcfile outfile [options]";
        static string msg_error_file = "Error: no {0} file specified.";
        static string msg_error_doesnotexist = "Error: {0} does not exist.";
        static string msg_error_no_outputfilename = "Error: output has no filename.";
        static string msg_error_badoption = "Error: unknown option '{0}'.";

        static void WriteFile(List<StringTokenized> lines, string path)
        {
            using (StreamWriter file = new StreamWriter(path))
            {
                file.WriteLine(".scope");
                foreach (StringTokenized line in lines)
                {
                    if (line.CompiledString != null)
                    {
                        string tabs = new String('\t', tabCount(line.Line));
                        string compiled = line.CompiledString.Replace("\n", "\n" + tabs);
                        compiled = tabs + compiled;
                        file.WriteLine(compiled);
                    }
                    else
                    {
                        file.WriteLine(line.Line);
                    }
                }
                file.WriteLine(".scend");
            }
        }

        static int tabCount(string line)
        {
            int whitespace = 0;
            foreach (char ch in line)
            {
                if (ch != '\t')
                    break;
                whitespace++;
            }
            return whitespace;
        }
    }
}
