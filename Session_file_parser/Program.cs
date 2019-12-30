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
        public StringVariable(string name, char type, int length, string value)
        {
            if (type != 's') { throw new InvalidDataException(); }
            this.Name = name;
            this.Type = type;
            this.Length = length;
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
            if (type != 'b') { throw new InvalidDataException(); }
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
        const char nameDelimiter = '|';
        const char valueDelimiter = ':';
        const char variableDelimiter = ';';
        const char stringSigns = '"';
        const char arrayEndSign = '}';

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
            string input = String.Empty;
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
            else
            {
                Console.WriteLine("Please, Enter the input (on one line).");
                input = Console.ReadLine();
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

            if (inputType == 'f')
            {
                input = File.ReadAllText(inputPath);
            }

            int i = 0;
            int varCount = 0;
            int inputLength = input.Length;
            string result = String.Empty;
            Dictionary<int, Variable> vars = new Dictionary<int, Variable>();

            while (i < inputLength)
            {
                varCount++;
                Console.WriteLine("Reading variable #{0}", varCount);

                //Getting the name of the variable
                string var_name = String.Empty;
                while (input[i] != nameDelimiter)
                {
                    var_name += input[i];
                    i++;
                }
                i++;    //Skipping the delimiter
                Console.WriteLine("\tName: {0}", var_name);

                //Getting the type of the variable
                char var_type = input[i];
                i += 2;    //Skipping the type and the delimiter
                Console.WriteLine("\tType: {0}", var_type.ToString());

                //Getting the length if array or string
                string var_length_str = String.Empty;
                int var_length = 0;
                if (var_type == 'a' || var_type == 's')
                {
                    while (input[i] != valueDelimiter)
                    {
                        var_length_str += input[i];
                        i++;
                    }
                    var_length = Convert.ToInt32(var_length_str);
                    i++;    //Skipping the delimiter
                    Console.WriteLine("Length: {0}", var_length);
                }

                //Getting the value of the variable
                string var_value_str = String.Empty;
                switch (var_type)   //Getting the raw value
                {
                    case 'i':   //Integer   \
                    case 'd':   //Double     --> Ending with ;
                    case 'b':   //Boolean   /
                        while (input[i] != variableDelimiter)
                        {
                            var_value_str += input[i];
                            i++;
                        }
                        i++;    //Skipping the delimiter
                        break;
                    case 's':   //String    --> Surronded with " "
                        i++;    //Skipping the starting "
                        while (input[i] != stringSigns)
                        {
                            var_value_str += input[i];
                            i++;
                        }
                        i += 2;    //Skipping the ending " and the delimiter
                        break;
                    case 'a':   //Array     --> Surronded with { }
                        i++;    //Skipping the {
                        while (input[i] != arrayEndSign)
                        {
                            var_value_str += input[i];
                            i++;
                        }
                        i++;    //Skipping the }
                        break;
                }
                switch (var_type)   //Parsing the raw value and saving into object
                {
                    case 'i':   //Integer
                        int var_value_i = Convert.ToInt32(var_value_str);
                        vars.Add(varCount, new IntegerVariable(var_name, var_type, var_value_i));
                        break;
                    case 'd':   //Double
                        double var_value_d = double.Parse(var_value_str, System.Globalization.CultureInfo.InvariantCulture);
                        vars.Add(varCount, new DecimalVariable(var_name, var_type, var_value_d));
                        break;
                    case 'b':   //Boolean
                        bool var_value_b = (var_value_str == "1") ? true : false;
                        vars.Add(varCount, new BooleanVariable(var_name, var_type, var_value_b));
                        break;
                    case 's':   //String
                        string var_value_s = var_value_str;
                        vars.Add(varCount, new StringVariable(var_name, var_type, var_length, var_value_s));
                        break;
                    case 'a':   //Array
                        //TODO
                        break;
                }

            }
            
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
