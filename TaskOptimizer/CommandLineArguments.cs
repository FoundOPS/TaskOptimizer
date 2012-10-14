using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TaskOptimizer
{
    public sealed class CommandLineArgument
    {
        public enum ArgumentType
        {
            Argument,
            Option
        }

        private String m_name;
        private String[] m_args;
        private ArgumentType m_type;

        public CommandLineArgument(String name, String[] args, ArgumentType type)
        {
            m_name = name;
            m_args = args;
            m_type = type;
        }

        public String Name
        { get { return m_name; } }
        public String[] Arguments
        { get { return m_args; } }
        public ArgumentType Type
        { get { return m_type; } }
    }

    public class CommandLineArguments
    {
        private CommandLineArgument[] m_args;

        public Int32 Count
        { get { return m_args.Length; } }
        public CommandLineArgument this[int i]
        { get { return m_args[i]; } }

        public CommandLineArguments()
            : this(System.Environment.CommandLine) { }
        public CommandLineArguments(String cmdLine)
        {
            List<String> argv = new List<string>();
            Int32 nextStart = 0;
            Boolean suppressFirst = false;

            foreach (Match m in Regex.Matches(cmdLine, "(\"[^\"]*\"|([^\" ])+)+", RegexOptions.ExplicitCapture))
            {
                if (m.Index != nextStart && !cmdLine.Substring(nextStart, m.Index - nextStart).All((char c) => { return c == ' '; }))
                    throw new InvalidOperationException();

                nextStart = m.Index + m.Length;

                if (!suppressFirst)
                {
                    suppressFirst = true;
                    continue;
                }

                if (m.Success)
                    argv.Add(DescapeQuotes(m.Value));
                else
                    throw new InvalidOperationException();

            }

            if (cmdLine.Length != nextStart && !cmdLine.Substring(nextStart).All((char c) => { return c == ' '; }))
                throw new InvalidOperationException();

            List<CommandLineArgument> args = new List<CommandLineArgument>();

            foreach (string arg in argv)
            {
                if (arg.StartsWith("/") || arg.StartsWith("-"))
                {
                    String narg = arg.Substring(1);
                    String name;
                    String opt;
                    Int32 index = narg.IndexOf(":");
                    if (index >= 0)
                    {
                        name = DescapeQuotes(narg.Substring(0, index));
                        opt = DescapeQuotes(narg.Substring(index + 1));
                    }
                    else
                    {
                        name = DescapeQuotes(narg);
                        opt = "";
                    }

                    args.Add(new CommandLineArgument(name, (from s in opt.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                            select s.Trim()).ToArray(), CommandLineArgument.ArgumentType.Option));
                }
                else
                    args.Add(new CommandLineArgument(arg, new string[0], CommandLineArgument.ArgumentType.Argument));
            }

            m_args = args.ToArray();
        }

        private String DescapeQuotes(String str)
        {
            if (str == "") return "";
            if (str.StartsWith("\"") && str.EndsWith("\""))
                return str.Substring(1, str.Length - 2).Replace("\"\"", "\"");
            else
                return str;
        }
    }
}
