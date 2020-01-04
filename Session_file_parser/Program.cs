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
        private char type;

        public char Type
        {
            get { return type; }
            set { type = value; }
        }

        public abstract void AddElement(ArrayElement arrayElement);
    }
    class IntegerVariable : Variable
    {
        private int value;

        public int Value
        {
            get { return value; }
            set { this.value = value; }
        }
        public IntegerVariable(char type, int value)
        {
            if (type != 'i') { throw new InvalidDataException(); }
            this.Type = type;
            this.Value = value;
        }

        public override void AddElement(ArrayElement arrayElement)
        {
            throw new NotImplementedException();
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
        public DecimalVariable(char type, double value)
        {
            if (type != 'd') { throw new InvalidDataException(); }
            this.Type = type;
            this.Value = value;
        }

        public override void AddElement(ArrayElement arrayElement)
        {
            throw new NotImplementedException();
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
        public StringVariable(char type, int length, string value)
        {
            if (type != 's') { throw new InvalidDataException(); }
            if (length != value.Length) { throw new InvalidDataException(); }
            this.Type = type;
            this.Length = length;
            this.Value = value;
        }

        public override void AddElement(ArrayElement arrayElement)
        {
            throw new NotImplementedException();
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
        public BooleanVariable(char type, bool value)
        {
            if (type != 'b') { throw new InvalidDataException(); }
            this.Type = type;
            this.Value = value;
        }

        public override void AddElement(ArrayElement arrayElement)
        {
            throw new NotImplementedException();
        }
    }
    class ArrayVariable : Variable
    {
        private int length;
        private List<ArrayElement> value;

        public int Length
        {
            get { return length; }
            set { length = value; }
        }
        public List<ArrayElement> Value
        {
            get { return this.value; }
            set { throw new InvalidOperationException(); }
        }
        public override void AddElement(ArrayElement el)
        {
            this.value.Add(el);
        }

        public ArrayVariable(char type)
        {
            if (type != 'a') { throw new InvalidDataException(); }
            this.Type = type;
        }
    }

    abstract class ArrayElement
    {
        private char index_type;
        private char value_type;
        private Variable value;

        public char Index_type
        {
            get { return index_type; }
            set { index_type = value; }
        }
        public char Value_type
        {
            get { return value_type; }
            set { value_type = value; }
        }
        public Variable Value
        {
            get { return this.Value; }
            set { this.value = value; }
        }
    }
    class NumericArrayElement : ArrayElement
    {
        private IntegerVariable index;
        public IntegerVariable Index
        {
            get { return index; }
            set { index = value; }
        }
    }
    class AssociativeArrayElement : ArrayElement
    {
        private StringVariable index;
        public StringVariable Index
        {
            get { return index; }
            set { index = value; }
        }
    }

    class Program
    {
        const char nameDelimiter = '|';
        const char valueDelimiter = ':';
        const char variableDelimiter = ';';
        const char stringSigns = '"';
        const char arrayStartSign = '{';
        const char arrayEndSign = '}';

        static Variable GetVariable(char type, int length, string rawValue)
        {
            Variable var = null;
            switch (type)   //Parsing the raw value and saving into object
            {
                case 'i':   //Integer
                    var = new IntegerVariable(type, Convert.ToInt32(rawValue));
                    break;
                case 'd':   //Double
                    var = new DecimalVariable(type, double.Parse(rawValue, System.Globalization.CultureInfo.InvariantCulture));
                    break;
                case 'b':   //Boolean
                    var = new BooleanVariable(type, (rawValue == "1") ? true : false);
                    break;
                case 's':   //String
                    var = new StringVariable(type, length, rawValue);
                    break;
                case 'a':   //Array
                    var = new ArrayVariable(type);
                    //Split the raw input into single array elements
                    //Numeric arrays:           i:?;i:?;        i:?;s:?:"?";
                    //Associative arrays:       s:?:"?";i:?;    s:?:"?";s:?:"?";
                    //Multidimensional arrays   i:?;a:?:{...}   s:?:"?";a:?:{...}
                    List<String> rawArrayElements = new List<string>();
                    //TODO

                    int cnt = rawArrayElements.Count;
                    for (int i = 0; i < cnt; i++)
                    {
                        var.AddElement(ReadArrayVariable(rawArrayElements[i]));
                    }
                    break;
            }
            return var;
        }

        static IntegerVariable ReadIntegerVariable(string rawInput)
        {
            /**
             * Method to return an object of type IntegerVariable with data from input string
             * Format of the input string:
             *      i:X;
             *      X = value
             */
            int i = 0;
            char type = rawInput[i];
            if (type != 'i') { throw new InvalidDataException(); }
            i += 2; //Skipping the value delimiter

            string value_str = String.Empty;
            while (rawInput[i] != variableDelimiter)
            {
                value_str += rawInput[i];
            }
            int value = Convert.ToInt32(value_str);
            return new IntegerVariable(type, value);
        }

        static DecimalVariable ReadDecimalVariable(string rawInput)
        {
            /**
             * Method to return an object of type DecimalVariable with data from input string
             * Format of the input string:
             *      d:X;
             *      X = value
             */
            int i = 0;
            char type = rawInput[i];
            if (type != 'd') { throw new InvalidDataException(); }
            i += 2; //Skipping the value delimiter

            string value_str = String.Empty;
            while (rawInput[i] != variableDelimiter)
            {
                value_str += rawInput[i];
            }
            double value = double.Parse(value_str, System.Globalization.CultureInfo.InvariantCulture);
            return new DecimalVariable(type, value);
        }

        static StringVariable ReadStringVariable(string rawInput)
        {
            /**
             * Method to return an object of type StringVariable with data from input string
             * Format of the input string:
             *      s:A:X;
             *      A = length
             *      X = content
             */
            int i = 0;
            char type = rawInput[i];
            if (type != 's') { throw new InvalidDataException(); }
            i += 2; //Skipping the value delimiter

            //Reading the length (A)
            string str = String.Empty;
            while (rawInput[i] != variableDelimiter)
            {
                str += rawInput[i];
            }
            int length = Convert.ToInt32(str);
            i += 2; //Skipping the value delimiter

            //Reading the content (X)
            str = String.Empty;
            while (rawInput[i] != variableDelimiter)
            {
                str += rawInput[i];
            }
            return new StringVariable(type, length, str);
        }
        static BooleanVariable ReadBooleanVariable(string rawInput)
        {
            /**
             * Method to return an object of type BooleanVariable with data from input string
             * Format of the input string:
             *      b:X;
             *      X = value (0 for false, 1 for true)
             */
            int i = 0;
            char type = rawInput[i];
            if (type != 'b') { throw new InvalidDataException(); }
            i += 2; //Skipping the value delimiter

            bool val = (rawInput[i] == '1') ? true : false;
            return new BooleanVariable(type, val);
        }

        static ArrayVariable ReadArrayVariable(string input)
        {
            /**
             * Method to return an object of type ArrayVariable with data from input string
             * Format of the input string:
             *      a:A:{t:l:x;T:L:X;...;};
             *      A = amount of index-value pairs in the bracklets
             *      t = type of index
             *      l = length of the index (in case of associative array) - not obligatory
             *      x = value of the index
             *      T = type of the value
             *      L = length of the value (in case of string or array value) - not obligatory
             *      X = value itself
             *      ... = next elements
             */
            //TODO
            //char index_type = input[0];

            throw new NotImplementedException();
            //return new ArrayElement();
        }

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
            Dictionary<string, Variable> vars = new Dictionary<string, Variable>();

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

                Variable var = null;
                char type = input[i];
                i += 2; //Skipping the delimiter

                if (type == 'a')    //Array value is ending with }
                {
                    int arrayLevel = 0;
                    string value_str = String.Empty;
                    while (arrayLevel > 0 || input[i] != variableDelimiter)
                    {
                        if (input[i] == arrayStartSign) { arrayLevel++; }
                        if (input[i] == arrayEndSign) { arrayLevel--; }
                        value_str += input[i];
                        i++;
                    }
                    value_str += input[i];   //Adding the closing }
                    var = ReadArrayVariable(value_str);
                }
                else    //Integer, double, boolean and string values are ending with ;
                {
                    string value_str = String.Empty;
                    while (input[i] != variableDelimiter)
                    {
                        value_str += input[i];
                        i++;
                    }
                    switch (type)
                    {
                        case 'i':
                            var = ReadIntegerVariable(value_str);
                            break;
                        case 'd':
                            var = ReadDecimalVariable(value_str);
                            break;
                        case 's':
                            var = ReadStringVariable(value_str);
                            break;
                        case 'b':
                            var = ReadBooleanVariable(value_str);
                            break;
                    }
                }
                vars.Add(var_name, var);  //Addition of the formated variable to the dictionary
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
