using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Session_file_parser
{
    internal abstract class Variable
    {
        private char type;

        public char Type
        {
            get => type;
            set => type = value;
        }

        public abstract string GetValue();

        public virtual int GetLength()
        {
            throw new NotImplementedException();
        }
        public virtual void AddElement(ArrayElement arrayElement)
        {
            throw new NotImplementedException();
        }
        public virtual IEnumerable<ArrayElement> GetArrayValues()
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class IntegerVariable : Variable
    {
        private int value;

        public int Value
        {
            get => value;
            set => this.value = value;
        }
        public IntegerVariable(char type, int value)
        {
            if (type != 'i') { throw new InvalidDataException(); }
            Type = type;
            Value = value;
        }

        public override string GetValue()
        {
            return value.ToString();
        }
    }

    internal sealed class DecimalVariable : Variable
    {
        private decimal value;

        public decimal Value
        {
            get => value;
            set => this.value = value;
        }
        public DecimalVariable(char type, decimal value)
        {
            if (type != 'd') { throw new InvalidDataException(); }
            Type = type;
            Value = value;
        }

        public override string GetValue()
        {
            return value.ToString();
        }
    }

    internal sealed class StringVariable : Variable
    {
        private int length;
        private string value;

        public int Length
        {
            get => length;
            set => length = value;
        }
        public string Value
        {
            get => value;
            set => this.value = value;
        }
        public StringVariable(char type, int length, string value)
        {
            if (type != 's') { throw new InvalidDataException(); }
            if (length != value.Length) { throw new InvalidDataException(); }
            Type = type;
            Length = length;
            Value = value;
        }

        public override string GetValue()
        {
            return value.ToString();
        }
        public override int GetLength()
        {
            return Length;
        }
    }

    internal sealed class BooleanVariable : Variable
    {
        private bool value;
        public bool Value
        {
            get => value;
            set => this.value = value;
        }
        public BooleanVariable(char type, bool value)
        {
            if (type != 'b') { throw new InvalidDataException(); }
            Type = type;
            Value = value;
        }

        public override string GetValue()
        {
            return value.ToString();
        }
    }

    internal sealed class ArrayVariable : Variable
    {
        private int length;
        private readonly List<ArrayElement> value = new List<ArrayElement>();

        public int Length
        {
            get => length;
            set => length = value;
        }
        public List<ArrayElement> Value
        {
            get => value;
            set => throw new InvalidOperationException();
        }
        public ArrayVariable(char type, int length)
        {
            if (type != 'a') { throw new InvalidDataException(); }
            Type = type;
            Length = length;
        }

        public override void AddElement(ArrayElement el)
        {
            value.Add(el);
        }
        public override string GetValue()
        {
            return "Array[" + Length + "]";
        }
        public override int GetLength()
        {
            return Length;
        }
        public override IEnumerable<ArrayElement> GetArrayValues()
        {
            return Value;
        }
    }

    internal sealed class ArrayElement
    {
        private Variable index;
        private Variable value;

        public Variable Index
        {
            get => index;
            set => index = value;
        }
        public Variable Value
        {
            get => value;
            set => this.value = value;
        }

        public ArrayElement(Variable index, Variable value)
        {
            this.index = index;
            this.value = value;
        }
    }

    internal static class OutputManager
    {
        private const int outputColumnWidth = 16;

        public static string AdjustLength(string str)
        {
            //Adjusting the length string to make fit the column
            if (str.Length > outputColumnWidth)
            {
                str = str.Substring(0, outputColumnWidth - 3) + "...";
            }
            else if (str.Length < outputColumnWidth)
            {
                while (str.Length < outputColumnWidth)
                {
                    str += ' ';
                }
            }
            return str;
        }

        public static void OutputArray(int arrayLevel, IEnumerable<ArrayElement> arrayElements, StringBuilder output)
        {
            /**
             * arrayLevel represents the layer of array and is used for proper indenting of the text
             * 1 stands for the first dimension of array (intending two columns - name and type).
             */
            foreach (ArrayElement el in arrayElements)
            {
                string str;

                //Reading the index
                str = AdjustLength(el.Index.GetValue());
                str = GetIndentation(arrayLevel) + str;
                output.Append(str + "|");

                //Reading the type of value
                char elValType = el.Value.Type;
                switch (elValType)
                {
                    case 'i':
                        str = "INT";
                        break;
                    case 'd':
                        str = "DOUBLE";
                        break;
                    case 's':
                        str = "STRING";
                        str += "(" + el.Value.GetLength().ToString() + ")";
                        break;
                    case 'b':
                        str = "BOOL";
                        break;
                    case 'a':
                        str = "ARRAY";
                        str += "(" + el.Value.GetLength().ToString() + ")";
                        break;
                }
                str = AdjustLength(str);
                output.Append(str + "|");

                //Reading the value itself
                switch (elValType)
                {
                    //Int, decimal, string and boolean - just writing the raw value
                    case 'i':
                    case 'd':
                    case 's':
                    case 'b':
                        str = AdjustLength(el.Value.GetValue().ToString());
                        output.Append(str + "|\n");
                        break;
                    //Array - increase indentation and start sub-table
                    case 'a':
                        str = "Index           |Type            |Value           ";
                        output.Append(str + "|\n");
                        str = GetIndentation(arrayLevel + 1) + "----------------|----------------|----------------";
                        output.Append(str + "|\n");
                        OutputArray(arrayLevel + 1, el.Value.GetArrayValues(), output);
                        break;
                }
            }
            output.Append(GetIndentation(arrayLevel) + "________________|________________|________________|\n");
        }

        public static string GetIndentation(int indent)
        {
            indent *= 2;    //One level of indentation is two columns
            string ind = string.Empty;
            ind += "|";
            for (int i = 0; i < indent; i++)
            {
                for (int j = 0; j < outputColumnWidth; j++)
                {
                    ind += " ";
                }
                ind += "|";
            }
            return ind;
        }
    }

    internal class Program
    {
        private const char nameDelimiter = '|';
        private const char valueDelimiter = ':';
        private const char variableDelimiter = ';';
        private const char arrayStartSign = '{';
        private const char arrayEndSign = '}';

        private static IntegerVariable ReadIntegerVariable(string rawInput)
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

            string value_str = string.Empty;
            while (rawInput[i] != variableDelimiter)
            {
                value_str += rawInput[i];
                i++;
            }
            int value = Convert.ToInt32(value_str);
            return new IntegerVariable(type, value);
        }

        private static DecimalVariable ReadDecimalVariable(string rawInput)
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

            string value_str = string.Empty;
            while (rawInput[i] != variableDelimiter)
            {
                value_str += rawInput[i];
                i++;
            }
            decimal value = decimal.Parse(value_str, System.Globalization.CultureInfo.InvariantCulture);
            return new DecimalVariable(type, value);
        }

        private static StringVariable ReadStringVariable(string rawInput)
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
            string str = string.Empty;
            while (rawInput[i] != valueDelimiter)
            {
                str += rawInput[i];
                i++;
            }
            int length = Convert.ToInt32(str);
            i += 2; //Skipping the value delimiter and the opening "

            //Reading the content (X)
            str = string.Empty;
            while (rawInput[i] != variableDelimiter)
            {
                str += rawInput[i];
                i++;
            }
            str = str.Remove(str.Length - 1);   //Removing the closing "
            return new StringVariable(type, length, str);
        }

        private static BooleanVariable ReadBooleanVariable(string rawInput)
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

        private static ArrayVariable ReadArrayVariable(string rawInput)
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
            string str = string.Empty;
            while (rawInput[i] != valueDelimiter)
            {
                str += rawInput[i];
                i++;
            }
            int arrLength = Convert.ToInt32(str);
            i += 2;    //Skipping the delimiter and opening bracket

            ArrayVariable var = new ArrayVariable(type, arrLength);

            for (int j = 0; j < arrLength; j++)
            {
                Variable inx = null;
                Variable val = null;
                str = string.Empty;

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

                str = string.Empty;

                //Reading the value
                char value_type = rawInput[i];
                if (value_type == 'a')    //Array value is ending with }
                {
                    int arrayLevel = -1;
                    while (arrayLevel > 0 || rawInput[i] != arrayEndSign)
                    {
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

        private static void Main()
        {
            Console.WriteLine("Do you want to provide input as a file or as a text? (F/T)");
            char inputType = ' ';
            while (inputType != 'f' && inputType != 't')
            {
                Console.WriteLine("Press either F for file or T for text.");
                inputType = Console.ReadKey().KeyChar;
                Console.WriteLine();
            }

            string inputPath = string.Empty;
            string input = string.Empty;
            if (inputType == 'f')
            {
                Console.WriteLine("Enter the path to the session file.");
                while (!File.Exists(inputPath))
                {
                    if (inputPath != string.Empty)
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

            string outputPath = string.Empty;
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
            Dictionary<string, Variable> vars = new Dictionary<string, Variable>();

            while (i < inputLength)
            {
                varCount++;
                Console.WriteLine("Reading variable #{0}", varCount);

                //Getting the name of the variable
                string var_name = string.Empty;
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
                        if (input[i] == arrayStartSign) { arrayLevel++; }
                        if (input[i] == arrayEndSign) { arrayLevel--; }
                        value_str += input[i];
                        i++;
                    }
                    value_str += input[i];   //Adding the closing }
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

            //Constructing output
            StringBuilder resultBuilder = new StringBuilder();

            //The first two lines (always same)
            resultBuilder.Append("|Name            |Type            |Value           |\n");
            resultBuilder.Append("|----------------|----------------|----------------|\n");
            foreach (KeyValuePair<string, Variable> currentVar in vars)
            {
                //Reading the name of the variable (key) and its object (value)
                string varName = currentVar.Key;
                Variable var = currentVar.Value;

                //Outputting the name
                varName = OutputManager.AdjustLength(varName);
                resultBuilder.Append("|" + varName + "|");

                //Outputting the type
                char varType = var.Type;
                string varTypeStr = string.Empty;
                switch (varType)
                {
                    case 'i':
                        varTypeStr = "INT";
                        break;
                    case 'd':
                        varTypeStr = "DOUBLE";
                        break;
                    case 's':
                        varTypeStr = "STRING";
                        varTypeStr += "(" + var.GetLength().ToString() + ")";
                        break;
                    case 'b':
                        varTypeStr = "BOOL";
                        break;
                    case 'a':
                        varTypeStr = "ARRAY";
                        varTypeStr += "(" + var.GetLength().ToString() + ")";
                        break;
                }
                varTypeStr = OutputManager.AdjustLength(varTypeStr);
                resultBuilder.Append(varTypeStr + "|");

                //Outputting the value
                string varValueStr;
                switch (varType)
                {
                    //Int, decimal, string and boolean - just writing the raw value
                    case 'i':
                    case 'd':
                    case 's':
                    case 'b':
                        varValueStr = var.GetValue().ToString();
                        varValueStr = OutputManager.AdjustLength(varValueStr);
                        resultBuilder.Append(varValueStr + "|\n");
                        break;
                    //Array - increase indentation and start sub-table
                    case 'a':
                        varValueStr = "Index           |Type            |Value           ";
                        resultBuilder.Append(varValueStr + "|\n");
                        varValueStr = OutputManager.GetIndentation(1) + "----------------|----------------|----------------";
                        resultBuilder.Append(varValueStr + "|\n");
                        OutputManager.OutputArray(1, var.GetArrayValues(), resultBuilder);
                        break;
                }
            }
            string result = resultBuilder.ToString();

            Console.WriteLine("Done!");

            if (outputType == 't')
            {
                Console.WriteLine(result);
            }
            else
            {
                using (StreamWriter Writer = new StreamWriter(File.Create(outputPath)))
                {
                    Writer.WriteLine(result);
                    Console.WriteLine("Parsed text was saved into {0}.", outputPath);
                }
            }
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey(true);
        }
    }
}
