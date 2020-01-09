using System;
using System.Collections.Generic;
using System.IO;

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
        public abstract string GetValue();
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
        public override string GetValue()
        {
            return this.value.ToString();
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
        public override string GetValue()
        {
            return this.value.ToString();
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
        public override string GetValue()
        {
            return this.value.ToString();
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
        public override string GetValue()
        {
            return this.value.ToString();
        }
    }
    class ArrayVariable : Variable
    {
        private int length;
        private List<ArrayElement> value = new List<ArrayElement>();

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

        public ArrayVariable(char type, int length)
        {
            if (type != 'a') { throw new InvalidDataException(); }
            this.Type = type;
            this.Length = length;
        }
        public override string GetValue()
        {
            return "Array[" + this.Length  + "]";
        }
    }

    class ArrayElement
    {
        private Variable index;
        private Variable value;

        public Variable Index
        {
            get { return this.index; }
            set { this.index = value; }
        }
        public Variable Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public ArrayElement(Variable index, Variable value)
        {
            this.index = index;
            this.value = value;
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
                i++;
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
                i++;
            }
            double value = double.Parse(value_str, System.Globalization.CultureInfo.InvariantCulture);
            return new DecimalVariable(type, value);
        }

        static StringVariable ReadStringVariable(string rawInput)
        {
            /**
             * Method to return an object of type StringVariable with data from input string
             * Format of the input string:
             *      s:A:"X";
             *      A = length
             *      X = content
             */
            int i = 0;
            char type = rawInput[i];
            if (type != 's') { throw new InvalidDataException(); }
            i += 2; //Skipping the value delimiter

            //Reading the length (A)
            string str = String.Empty;
            while (rawInput[i] != valueDelimiter)
            {
                str += rawInput[i];
                i++;
            }
            int length = Convert.ToInt32(str);
            i += 2; //Skipping the value delimiter and the opening "

            //Reading the content (X)
            str = String.Empty;
            while (rawInput[i] != variableDelimiter)
            {
                str += rawInput[i];
                i++;
            }
            str = str.Remove(str.Length - 1);   //Removing the closing "
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

        static ArrayVariable ReadArrayVariable(string rawInput)
        {
            /**
             * Method to return an object of type ArrayVariable with data from input string
             * Format of the input string:
             *      a:A:{t:l:x;T:L:X;...;};
             *      A = amount of index-value pairs in the brackets
             *      t = type of index
             *      l = length of the index (in case of associative array) - not obligatory
             *      x = value of the index
             *      T = type of the value
             *      L = length of the value (in case of string or array value) - not obligatory
             *      X = value itself
             *      ... = next elements
             */
            int i = 0;
            char type = rawInput[i];
            if (type != 'a') { throw new InvalidDataException(); }
            i += 2; //Skipping the value delimiter

            //Reading the length of the array
            string str = String.Empty;
            while (rawInput[i] != valueDelimiter)
            {
                str += rawInput[i];
                i++;
            }
            int arrLength = Convert.ToInt32(str);
            str = String.Empty;
            i += 2;    //Skipping the delimiter and opening bracket

            ArrayVariable var = new ArrayVariable(type, arrLength);

            for (int j = 0; j < arrLength; j++)
            {
                Variable inx = null;
                Variable val = null;
                str = String.Empty;

                //Reading the index
                char index_type = rawInput[i];
                while (rawInput[i] != variableDelimiter)
                {
                    str += rawInput[i];
                    i++;
                }
                str += rawInput[i];   //Adding the ending delimiter
                i++;
                switch (index_type)
                {
                    case 'i':
                        inx = ReadIntegerVariable(str);
                        break;
                    case 's':
                        inx = ReadStringVariable(str);
                        break;
                }

                str = String.Empty;

                //Reading the value
                char value_type = rawInput[i];
                if (value_type == 'a')    //Array value is ending with }
                {
                    int arrayLevel = -1;
                    while (arrayLevel > 0 || rawInput[i] != arrayEndSign)
                    {
                        //Console.WriteLine("arrayLevel: {0} \t input[i + 1]: {1} \t value_str: {2} \t i: {3}", arrayLevel, rawInput[i + 1], str, i);
                        if (rawInput[i] == arrayStartSign) { arrayLevel++; }
                        if (rawInput[i] == arrayEndSign) { arrayLevel--; }
                        str += rawInput[i];
                        i++;
                    }
                    str += rawInput[i];   //Adding the closing }
                    //Console.WriteLine(str);
                    i++;    //Skipping the closing }
                    val = ReadArrayVariable(str);

                }
                else    //Integer, double, boolean and string values are ending with ;
                {
                    while (rawInput[i] != variableDelimiter)
                    {
                        str += rawInput[i];
                        i++;
                    }
                    str += rawInput[i];   //Adding the ending delimiter
                    i++;
                    switch (value_type)
                    {
                        case 'i':
                            val = ReadIntegerVariable(str);
                            break;
                        case 'd':
                            val = ReadDecimalVariable(str);
                            break;
                        case 's':
                            val = ReadStringVariable(str);
                            break;
                        case 'b':
                            val = ReadBooleanVariable(str);
                            break;
                    }
                }
                //Console.WriteLine("Index: {0} \t Value: {1}", inx.GetValue(), val.GetValue());
                var.AddElement(new ArrayElement(inx, val));
            }
            return var;
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
                Console.WriteLine("Enter the path to the file (if it doesn't exist, it will be created, otherwise, it will be rewriten).");
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

                //Reading the type
                Variable var = null;
                char type = input[i];
                string value_str = type.ToString() + valueDelimiter.ToString();
                i += 2; //Skipping the delimiter

                if (type == 'a')    //Array value is ending with }
                {
                    int arrayLevel = -1;
                    while (arrayLevel > 0 || input[i] != arrayEndSign)
                    {
                        //Console.WriteLine("arrayLevel: {0} \t input[i + 1]: {1} \t value_str: {2} \t i: {3}", arrayLevel, input[i + 1], value_str, i);
                        if (input[i] == arrayStartSign) { arrayLevel++; }
                        if (input[i] == arrayEndSign) { arrayLevel--; }
                        value_str += input[i];
                        i++;
                    }
                    value_str += input[i];   //Adding the closing }
                    //Console.WriteLine(value_str);
                    i++;    //Skipping the closing }
                    var = ReadArrayVariable(value_str);
                }
                else    //Integer, double, boolean and string values are ending with ;
                {
                    while (input[i] != variableDelimiter)
                    {
                        value_str += input[i];
                        i++;
                    }
                    value_str += input[i];   //Adding the ending delimiter
                    i++;
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
