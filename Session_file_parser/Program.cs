using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Session_file_parser
{
    abstract class Variable
    {
        private string name;
        private char type;
        private int value;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public char Type
        {
            get { return type; }
            set { type = value; }
        }
    }
    class IntegerVariable : Variable
    {
        private int value;

        public int Value
        {
            get { return value; }
            set { this.value = value; }
        }
        public IntegerVariable(string name, char type, int value)
        {
            if (type != 'i') { throw new InvalidDataException(); }
            this.Name = name;
            this.Type = type;
            this.Value = value;
        }
    }
    class DecimalVariable : Variable
    {
        private double value;

        public double Value
        {
            get { return value; }
            set { this.value = value; }
        }
        public DecimalVariable(string name, char type, double value)
        {
            if (type != 'd') { throw new InvalidDataException(); }
            this.Name = name;
            this.Type = type;
            this.Value = value;
        }
    }
    class StringVariable : Variable
    {
        private int length;
        private string value;

        public int Length
        {
            get { return length; }
            set { length = value; }
        }
        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }
        public StringVariable(string name, char type, string value)
        {
            if (type != 's') { throw new InvalidDataException(); }
            this.Name = name;
            this.Type = type;
            this.Value = value;
        }
    }
    class BooleanVariable : Variable
    {
        private bool value;
        public bool Value
        {
            get { return value; }
            set { this.value = value; }
        }
        public BooleanVariable(string name, char type, bool value)
        {
            if (type != 'b') { throw new InvalidDataException(); }if (type != 'i' && type != 'd') { throw new InvalidDataException(); }
            this.Name = name;
            this.Type = type;
            this.Value = value;
        }
    }
    class ArrayVariable : Variable
    {
        private int length;
        private List<Variable> value;

        public int Length
        {
            get { return length; }
            set { length = value; }
        }
        public List<Variable> Value
        {
            get { return value; }
            set
            {
                int cnt = value.Count;
                for (int i = 0; i < cnt; i++)
                {
                    this.value.Add(value[i]);
                }
            }
        }
        public ArrayVariable(string name, char type)
        {
            if (type != 'a') { throw new InvalidDataException(); }
            this.Name = name;
            this.Type = type;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Do you want to provide input as a file or as a text? (F/T)");
            char inputType = ' ';
            while (inputType != 'f' && inputType != 't')
            {
                Console.WriteLine("Press either F for file or T for text.");
                inputType = Console.ReadKey().KeyChar;
                Console.WriteLine();
            }

            string inputPath = String.Empty;
            if (inputType == 'f')
            {
                Console.WriteLine("Enter the path to the session file.");
                while (!File.Exists(inputPath))
                {
                    if (inputPath != String.Empty)
                    {
                        Console.WriteLine("File not found.");
                    }
                    inputPath = Console.ReadLine();
                    Console.WriteLine();
                }
            }

            Console.WriteLine("Do you want to save the output into a textfile or display it as a text in this console?");
            char outputType = ' ';
            while (outputType != 'f' && outputType != 't')
            {
                Console.WriteLine("Press either F for file or T for text.");
                outputType = Console.ReadKey().KeyChar;
                Console.WriteLine();
            }

            string outputPath = String.Empty;
            if (outputType == 'f')
            {
                Console.WriteLine("Enter the path to the file (if it doesn't exist, it will be created, otherwise, it will be rewriteen).");
                outputPath = Console.ReadLine();
                Console.WriteLine();
            }

            Console.WriteLine("Press any key to start parsing of your input");
            Console.ReadKey(true);

            string result = String.Empty;
            
            /*
             TODO Program
             */

            Console.WriteLine("Done!");

            if (outputType == 't')
            {
                string[] resultLines = result.Split('¶');
                foreach (string line in resultLines)
                {
                    Console.WriteLine(line);
                }
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey(true);
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(File.Create(outputPath)))
                {
                    string[] resultLines = result.Split('¶');
                    foreach (string line in resultLines)
                    {
                        writer.WriteLine(line);
                    }
                    Console.WriteLine("Parsed text was saved into {0}.",outputPath);
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadKey(true);
                }
            }
        }
    }
}
