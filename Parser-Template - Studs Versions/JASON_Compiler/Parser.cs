using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASON_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        Node Program()
        {
            Node program = new Node("Begin_Of_Program");

            program.Children.Add(Functions());
            MessageBox.Show("Success");
            return program;
        }

        Node Main_Fun()
        {
            Node Main = new Node("Main_Fun");
            Main.Children.Add(match(Token_Class.Main));
            Main.Children.Add(match(Token_Class.OpenParen));
            Main.Children.Add(match(Token_Class.CloseParen));
            Main.Children.Add(Function_Body());
            return Main;
        }

        Node Functions()
        {
            Node functions = new Node("Functions");
            functions.Children.Add(DataType());
            functions.Children.Add(Fun());
            return functions;
        }
        Node Fun()
        {
            Node Function = new Node("Fun");
            bool underCount = InputPointer < TokenStream.Count;
            bool identifierToken = false;
            bool mainToken = false;
          

            if (underCount)
            {
                identifierToken = TokenStream[InputPointer].token_type == Token_Class.Identfire;
                mainToken = TokenStream[InputPointer].token_type == Token_Class.Main;
                
            }
            if (mainToken)
            {
                Function.Children.Add(Main_Fun());
                return Function;
            }
            else if (identifierToken)
            {
                Function.Children.Add(Function_Statement());
                Function.Children.Add(Functions());
                return Function;
            }
            if (InputPointer + 1 <= TokenStream.Count)
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                               + Token_Class.Identfire + " or " + "Expected "
                               + Token_Class.Main + " and " +
                               TokenStream[InputPointer].token_type.ToString() +
                               "  found\r\n");
                
                InputPointer++;
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                               + Token_Class.Main + " Function in program and " +
                               " WE NOT found them \r\n");
            }

            return null;
        }
        Node Function_Statement()
        {
            Node functionStatement = new Node("Function_Statement");
            bool underCount = InputPointer < TokenStream.Count;

            bool identifierToken = false;
          

            if (underCount)
            {
                identifierToken = TokenStream[InputPointer].token_type == Token_Class.Identfire;
            }
            if (identifierToken)
            {
                functionStatement.Children.Add(Function_Declaration());
                functionStatement.Children.Add(Function_Body());
                return functionStatement;
            }
            if (InputPointer + 1 <= TokenStream.Count)
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                + Token_Class.Identfire + " or " + "Expected "
                + " and " +
                TokenStream[InputPointer].token_type.ToString() +
                "  found\r\n");
               
                InputPointer++;
            }

            return null;

        }
        Node Function_Declaration()
        {
            Node functionDeclaration = new Node("Function_Declaration");

            
            functionDeclaration.Children.Add(Function_Identifier());
            functionDeclaration.Children.Add(match(Token_Class.OpenParen));
            functionDeclaration.Children.Add(Parameter_List());
            functionDeclaration.Children.Add(match(Token_Class.CloseParen));

            return functionDeclaration;
        }
        Node DataType()
        {
            Node dataType = new Node("DataType");
            bool underCount = InputPointer < TokenStream.Count;
            bool intToken = false;
            bool floatToken = false;
            bool stringToken = false;

            if (underCount)
            {
                intToken = TokenStream[InputPointer].token_type == Token_Class.Int;
                floatToken = TokenStream[InputPointer].token_type == Token_Class.Float;
                stringToken = TokenStream[InputPointer].token_type == Token_Class.Data_type_String;
            }
            if (intToken)
            {
                dataType.Children.Add(match(Token_Class.Int));
                return dataType;
            }
            else if (floatToken)
            {
                dataType.Children.Add(match(Token_Class.Float));
                return dataType;
            }
            else if (stringToken)
            {
                dataType.Children.Add(match(Token_Class.Data_type_String));
                return dataType;
            }

            if (InputPointer + 1 <= TokenStream.Count)
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                 + Token_Class.Int + " or " + "Expected "
                 + Token_Class.Float + " or " + "Expected "
                 + Token_Class.Data_type_String + " and " +
                 TokenStream[InputPointer].token_type.ToString() +
                 "  found\r\n");
                
                InputPointer++;
            }

            return null;

        }
        Node Function_Identifier()
        {
            Node functionName = new Node("Function_Identifier");
            bool underCount = InputPointer < TokenStream.Count;
            bool identifierToken = false;

            if (underCount)
            {
                identifierToken = TokenStream[InputPointer].token_type == Token_Class.Identfire;
            }
            if (identifierToken)
            {
                functionName.Children.Add(match(Token_Class.Identfire));
                return functionName;
            }
            else
            {
                if (InputPointer + 1 < TokenStream.Count)
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                   + Token_Class.Identfire + " and " +
                   TokenStream[InputPointer].token_type.ToString() +
                   "  found\r\n");
                   
                    InputPointer++;
                }

                return null;
            }
        }
        Node Parameter_List()
        {
            Node parameterList = new Node("Parameter_List");
            bool underCount = InputPointer < TokenStream.Count;
            bool intToken = false;
            bool floatToken = false;
            bool stringToken = false;

            if (underCount)
            {
                intToken = TokenStream[InputPointer].token_type == Token_Class.Int;
                floatToken = TokenStream[InputPointer].token_type == Token_Class.Float;
                stringToken = TokenStream[InputPointer].token_type == Token_Class.Data_type_String;
            }

            if (intToken || floatToken || stringToken)
            {
                parameterList.Children.Add(Parameter());
                parameterList.Children.Add(Parameter_Lists());
                return parameterList;
            }
            else
            { return null; }


        }
        Node Parameter_Lists()
        {
            Node parameterLists = new Node("Parameter_Lists");
            bool underCount = InputPointer < TokenStream.Count;
            bool token_Comma = false;
            if (underCount)
                token_Comma = TokenStream[InputPointer].token_type == Token_Class.Comma;

            if (token_Comma)
            {
                parameterLists.Children.Add(match(Token_Class.Comma));
                parameterLists.Children.Add(Parameter());
                parameterLists.Children.Add(Parameter_Lists());
                return parameterLists;
            }
            else
            { return null; }


        }
        Node Parameter()
        {
            Node parameter = new Node("Parameter");

            parameter.Children.Add(DataType());
            parameter.Children.Add(match(Token_Class.Identfire));
            return parameter;
        }
        Node Function_Body()
        {
            Node functionBody = new Node("Function_Body");

            functionBody.Children.Add(match(Token_Class.OpenBrac));
            functionBody.Children.Add(Statements());
            functionBody.Children.Add(Return());
            functionBody.Children.Add(match(Token_Class.CloseBrac));
            return functionBody;
        }
        Node Statement()
        {
            Node statement = new Node("Statement");
            bool underCount = InputPointer < TokenStream.Count;
            bool intToken = false, floatToken = false, stringToken = false;
            bool identifierToken = false;
            bool writeToken = false;
            bool readToken = false;
            bool ifToken = false;
            bool repeatToken = false;
           

            if (underCount)
            {
                intToken = TokenStream[InputPointer].token_type == Token_Class.Int;
                floatToken = TokenStream[InputPointer].token_type == Token_Class.Float;
                stringToken = TokenStream[InputPointer].token_type == Token_Class.Data_type_String;
                identifierToken = TokenStream[InputPointer].token_type == Token_Class.Identfire;
                writeToken = TokenStream[InputPointer].token_type == Token_Class.Write;
                readToken = TokenStream[InputPointer].token_type == Token_Class.Read;
                ifToken = TokenStream[InputPointer].token_type == Token_Class.If;
                repeatToken = TokenStream[InputPointer].token_type == Token_Class.Repeat;
               
            }
            if (floatToken || intToken || stringToken)
            {
                statement.Children.Add(Declaration());
                return statement;
            }
            else if (identifierToken)
            {

                statement.Children.Add(assign());
                return statement;
            }
            else if (writeToken)
            {
                statement.Children.Add(Write());
                return statement;
            }
            else if (readToken)
            {
                statement.Children.Add(Read());
                return statement;
            }
            else if (ifToken)
            {
                statement.Children.Add(If());
                return statement;
            }
            else if (repeatToken)
            {
                statement.Children.Add(Repeat());
                return statement;
            }

            if (InputPointer + 1 <= TokenStream.Count)
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                 + Token_Class.Identfire + " or " + "Expected "
                 + Token_Class.Number + " or " + "Expected "
                 + Token_Class.String + " or " + "Expected "
                 + Token_Class.Int + " or "
                 + Token_Class.Write + " or " + "Expected "
                 + Token_Class.Read + " or " + "Expected "
                 + Token_Class.If + " or " + "Expected "
                 + Token_Class.Repeat + " and " +

                TokenStream[InputPointer].token_type.ToString() +
                "  found\r\n");
                
                InputPointer++;

            }


            return null;
        }
        Node Statements()
        {
            Node statements = new Node("Set_Statement");
            bool underCount = InputPointer < TokenStream.Count;
            bool intToken = false, floatToken = false, stringToken = false;
            bool identifierToken = false;
            bool writeToken = false;
            bool readToken = false;
            bool ifToken = false;
            bool repeatToken = false;
            

            if (underCount)
            {
                intToken = TokenStream[InputPointer].token_type == Token_Class.Int;
                floatToken = TokenStream[InputPointer].token_type == Token_Class.Float;
                stringToken = TokenStream[InputPointer].token_type == Token_Class.Data_type_String;
                identifierToken = TokenStream[InputPointer].token_type == Token_Class.Identfire;
                writeToken = TokenStream[InputPointer].token_type == Token_Class.Write;
                readToken = TokenStream[InputPointer].token_type == Token_Class.Read;
                ifToken = TokenStream[InputPointer].token_type == Token_Class.If;
                repeatToken = TokenStream[InputPointer].token_type == Token_Class.Repeat;
                
            }
            if (floatToken || intToken
                || stringToken || identifierToken
                || writeToken || readToken
                || ifToken || repeatToken)
            {
                statements.Children.Add(Statement());
                statements.Children.Add(Statements());
                return statements;
            }

            return null;
        }
        Node Read()
        {
            Node Read_State = new Node("Read");
            Read_State.Children.Add(match(Token_Class.Read));
            Read_State.Children.Add(match(Token_Class.Identfire));
            Read_State.Children.Add(match(Token_Class.Semicolon));
            return Read_State;
        }
        
        Node Write()
        {
            Node Write_State = new Node("Write");
            Write_State.Children.Add(match(Token_Class.Write));
            Write_State.Children.Add(EndL());
            Write_State.Children.Add(match(Token_Class.Semicolon));
            return Write_State;
        }
        Node EndL()
        {
            Node EndL = new Node("EndL");
            bool underCount = InputPointer < TokenStream.Count;
            bool token_endl = false;
            bool stringToken = false;
            bool identifierToken = false;
            bool numberToken = false;

            if (underCount)
            {
                stringToken = TokenStream[InputPointer].token_type == Token_Class.String;
                identifierToken = TokenStream[InputPointer].token_type == Token_Class.Identfire;
                numberToken = TokenStream[InputPointer].token_type == Token_Class.Number;
                token_endl = TokenStream[InputPointer].token_type == Token_Class.Endl;
            }
            if (token_endl)
            {
                EndL.Children.Add(match(Token_Class.Endl));
                return EndL;
            }
            else if (stringToken || numberToken || identifierToken)
            {
                EndL.Children.Add(Expression());
                return EndL;
            }
            Errors.Error_List.Add("Parsing Error: Expected "
            + Token_Class.Identfire + " or " + "Expected "
            + Token_Class.Number + " or " + "Expected "
            + Token_Class.String + " or " + "Expected "
            + Token_Class.Endl + " and " +

            TokenStream[InputPointer].token_type.ToString() +
            "  found\r\n");
            if (InputPointer + 1 < TokenStream.Count)
                InputPointer++;
            return null;
        }

        Node Return()
        {
            Node Return_State = new Node("Return");
            Return_State.Children.Add(match(Token_Class.Return));
            Return_State.Children.Add(Expression());
            Return_State.Children.Add(match(Token_Class.Semicolon));
            return Return_State;
        }
        Node assign()
        {
            Node assign_state = new Node("assign");
            assign_state.Children.Add(match(Token_Class.Identfire));
            assign_state.Children.Add(match(Token_Class.AssignmentOP));
            assign_state.Children.Add(Expression());
            assign_state.Children.Add(match(Token_Class.Semicolon));
            return assign_state;
        }

        Node Expression()
        {
            Node expression = new Node("Expression");
            bool underCount = InputPointer < TokenStream.Count;
            bool stringToken = false;
            bool identifierToken = false;
            bool numberToken = false;
            bool leftParenthesesToken = false;

            if (underCount)
            {
                stringToken = TokenStream[InputPointer].token_type == Token_Class.String;
                identifierToken = TokenStream[InputPointer].token_type == Token_Class.Identfire;
                numberToken = TokenStream[InputPointer].token_type == Token_Class.Number;
                leftParenthesesToken = TokenStream[InputPointer].token_type == Token_Class.OpenParen;
            }
            if (stringToken)
            {
                expression.Children.Add(match(Token_Class.String));
                return expression;
            }
            else if (identifierToken || numberToken)
            {
                expression.Children.Add(Term());
                expression.Children.Add(Arithmatic_Part());
                return expression;
            }
            else if (leftParenthesesToken)
            {
                expression.Children.Add(Equation());
                return expression;
            }
            if (InputPointer + 1 <= TokenStream.Count)
            {
                Errors.Error_List.Add("Parsing Error: Expected "
              + Token_Class.Identfire + " or " + "Expected "
              + Token_Class.Number + " or " + "Expected "
              + Token_Class.String + " or " + "Expected "
              + Token_Class.CloseParen + " and " +

              TokenStream[InputPointer].token_type.ToString() +
              "  found\r\n");
                if (InputPointer + 1 < TokenStream.Count)
                    InputPointer++;
            }

            return null;
        }

        Node Equation()
        {
            Node equation = new Node("Equation");
            bool underCount = InputPointer < TokenStream.Count;
            bool identifierToken = false;
            bool numberToken = false;
            bool leftParenthesesToken = false;

            if (underCount)
            {
                identifierToken = TokenStream[InputPointer].token_type == Token_Class.Identfire;
                numberToken = TokenStream[InputPointer].token_type == Token_Class.Number;
                leftParenthesesToken = TokenStream[InputPointer].token_type == Token_Class.OpenParen;
            }

            if (identifierToken || numberToken)
            {
                equation.Children.Add(Term());
                equation.Children.Add(Arithmatic_Term());
                return equation;
            }
            else if (leftParenthesesToken)
            {
                equation.Children.Add(match(Token_Class.OpenParen));
                equation.Children.Add(Term());
                equation.Children.Add(Arithmatic_Term());
                equation.Children.Add(match(Token_Class.CloseParen));
                equation.Children.Add(Arithmatic_Terms());
                return equation;
            }
            if (InputPointer + 1 <= TokenStream.Count)
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                + Token_Class.Identfire + " or " + "Expected "
                + Token_Class.Number + " or " + "Expected "
                + Token_Class.CloseParen + " and " +

                TokenStream[InputPointer].token_type.ToString() +
                "  found\r\n");
              
                InputPointer++;
            }

            return null;
        }

        Node Arithmatic_Part()
        {
            Node arithmaticPart = new Node("Arithmatic_Part");
            bool underCount = InputPointer < TokenStream.Count;
            bool plusToken = false;
            bool minusToken = false;
            bool multToken = false;
            bool divToken = false;

            if (underCount)
            {
                plusToken = TokenStream[InputPointer].token_type == Token_Class.PlusOp;
                minusToken = TokenStream[InputPointer].token_type == Token_Class.MinusOp;
                multToken = TokenStream[InputPointer].token_type == Token_Class.MultiplyOp;
                divToken = TokenStream[InputPointer].token_type == Token_Class.DivideOp;
            }
            if (plusToken || minusToken || multToken || divToken)
            {
                arithmaticPart.Children.Add(Arithmatic_Term());
                return arithmaticPart;
            }

            return null;
        }

        Node Arithmatic_Term()
        {
            Node arithmaticTerm = new Node("Arithmatic_Term");


            arithmaticTerm.Children.Add(Arithmatic_Op());
            arithmaticTerm.Children.Add(Equation_Part());
            return arithmaticTerm;

        }

        Node Equation_Part()
        {
            Node equation = new Node("Equation_Part");
            bool underCount = InputPointer < TokenStream.Count;
            bool identifierToken = false;
            bool numberToken = false;
            bool leftParenthesesToken = false;

            if (underCount)
            {
                identifierToken = TokenStream[InputPointer].token_type == Token_Class.Identfire;
                numberToken = TokenStream[InputPointer].token_type == Token_Class.Number;
                leftParenthesesToken = TokenStream[InputPointer].token_type == Token_Class.OpenParen;
            }

            if (identifierToken || numberToken)
            {
                equation.Children.Add(Term());
                equation.Children.Add(Arithmatic_Terms());
                return equation;
            }
            else if (leftParenthesesToken)
            {
                equation.Children.Add(Equation());
                return equation;
            }
            if (InputPointer + 1 <= TokenStream.Count)
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                + Token_Class.Identfire + " or " + "Expected "
                + Token_Class.Number + " or " + "Expected "
                + Token_Class.CloseParen + " and " +

                TokenStream[InputPointer].token_type.ToString() +
                "  found\r\n");
                
                InputPointer++;
            }

            return null;
        }

        Node Arithmatic_Terms()
        {
            Node arithmaticTerm = new Node("Arithmatic_Terms");
            bool underCount = InputPointer < TokenStream.Count;
            bool plusToken = false;
            bool minusToken = false;
            bool multToken = false;
            bool divToken = false;

            if (underCount)
            {
                plusToken = TokenStream[InputPointer].token_type == Token_Class.PlusOp;
                minusToken = TokenStream[InputPointer].token_type == Token_Class.MinusOp;
                multToken = TokenStream[InputPointer].token_type == Token_Class.MultiplyOp;
                divToken = TokenStream[InputPointer].token_type == Token_Class.DivideOp;
            }
            if (plusToken || minusToken || divToken || multToken)
            {
                arithmaticTerm.Children.Add(Arithmatic_Term());
                return arithmaticTerm;
            }

            return null;
        }

        Node Arithmatic_Op()
        {
            Node arithmaticOperation = new Node("Arithmatic_Op");
            bool underCount = InputPointer < TokenStream.Count;
            bool plusToken = false;
            bool minusToken = false;
            bool multToken = false;
            bool divToken = false;

            if (underCount)
            {
                plusToken = TokenStream[InputPointer].token_type == Token_Class.PlusOp;
                minusToken = TokenStream[InputPointer].token_type == Token_Class.MinusOp;
                multToken = TokenStream[InputPointer].token_type == Token_Class.MultiplyOp;
                divToken = TokenStream[InputPointer].token_type == Token_Class.DivideOp;
            }
            if (plusToken)
            {
                arithmaticOperation.Children.Add(match(Token_Class.PlusOp));
                return arithmaticOperation;
            }
            else if (minusToken)
            {
                arithmaticOperation.Children.Add(match(Token_Class.MinusOp));
                return arithmaticOperation;
            }
            else if (multToken)
            {
                arithmaticOperation.Children.Add(match(Token_Class.MultiplyOp));
                return arithmaticOperation;
            }
            else if (divToken)
            {
                arithmaticOperation.Children.Add(match(Token_Class.DivideOp));
                return arithmaticOperation;
            }
            if (InputPointer + 1 <= TokenStream.Count)
            {
                Errors.Error_List.Add("Parsing Error: Expected "
               + Token_Class.PlusOp + " or " + "Expected "
               + Token_Class.MinusOp + " or " + "Expected "
               + Token_Class.DivideOp + " or " + "Expected "
               + Token_Class.MultiplyOp + " and " +
               TokenStream[InputPointer].token_type.ToString() +
               "  found\r\n");
                
                InputPointer++;
            }

            return null;
        }

        Node Term()
        {
            Node term = new Node("Term");
            bool underCount = InputPointer < TokenStream.Count;
            bool identifierToken = false;
            bool numberToken = false;

            if (underCount)
            {
                identifierToken = TokenStream[InputPointer].token_type == Token_Class.Identfire;
                numberToken = TokenStream[InputPointer].token_type == Token_Class.Number;
            }
            if (numberToken)
            {
                term.Children.Add(match(Token_Class.Number));
                return term;
            }
            else if (identifierToken)
            {
                term.Children.Add(match(Token_Class.Identfire));
                term.Children.Add(Function_call());
                return term;
            }
            if (InputPointer + 1 <= TokenStream.Count)
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                 + Token_Class.Identfire + " or " + "Expected "
                 + Token_Class.Number
                 + " and " +

                 TokenStream[InputPointer].token_type.ToString() +
                 "  found\r\n");
                
                InputPointer++;
            }

            return null;

        }

        Node Function_call()
        {
            Node functionCall = new Node("Function_call");
            bool underCount = InputPointer < TokenStream.Count;
            bool leftParenthesesToken = false;

            if (underCount)
            {
                leftParenthesesToken = TokenStream[InputPointer].token_type == Token_Class.OpenParen;
            }
            if (leftParenthesesToken)
            {
                functionCall.Children.Add(match(Token_Class.OpenParen));
                functionCall.Children.Add(Identifier_List());
                functionCall.Children.Add(match(Token_Class.CloseParen));
                return functionCall;
            }


            return null;
        }

        Node Identifier_List()
        {

            Node id_list = new Node("Identifier_List");
            bool underCount = InputPointer < TokenStream.Count;
            bool identifierToken = false;
            bool stringToken = false;
            bool numberToken = false;

            if (underCount)
            {
                identifierToken = TokenStream[InputPointer].token_type == Token_Class.Identfire;
                stringToken = TokenStream[InputPointer].token_type == Token_Class.String;
                numberToken = TokenStream[InputPointer].token_type == Token_Class.Number;
            }
            if (identifierToken)
            {
                id_list.Children.Add(match(Token_Class.Identfire));
                id_list.Children.Add(Identifier_Lists());

                return id_list;
            }
            else if (stringToken)
            {
                id_list.Children.Add(match(Token_Class.String));
                id_list.Children.Add(Identifier_Lists());

                return id_list;
            }
            else if (numberToken)
            {
                id_list.Children.Add(match(Token_Class.Number));
                id_list.Children.Add(Identifier_Lists());

                return id_list;
            }
            return null;
        }

        Node Identifier_Lists()
        {
            Node identifierLists = new Node("Identifier_Lists");
            bool underCount = InputPointer < TokenStream.Count;
            bool commaToken = false;

            bool identifierToken = false;
            bool stringToken = false;
            bool numberToken = false;

            if (underCount)
            {
                identifierToken = TokenStream[InputPointer].token_type == Token_Class.Identfire;
                stringToken = TokenStream[InputPointer].token_type == Token_Class.String;
                numberToken = TokenStream[InputPointer].token_type == Token_Class.Number;
                commaToken = TokenStream[InputPointer].token_type == Token_Class.Comma;
            }
            if (commaToken)
            {
                identifierLists.Children.Add(match(Token_Class.Comma));
                {
                    underCount = InputPointer < TokenStream.Count;
                    if (underCount)
                    {
                        identifierToken = TokenStream[InputPointer].token_type == Token_Class.Identfire;
                        stringToken = TokenStream[InputPointer].token_type == Token_Class.String;
                        numberToken = TokenStream[InputPointer].token_type == Token_Class.Number;
                        commaToken = TokenStream[InputPointer].token_type == Token_Class.Comma;
                    }
                }
                if (identifierToken)
                {
                    identifierLists.Children.Add(match(Token_Class.Identfire));
                }
                else if (stringToken)
                {
                    identifierLists.Children.Add(match(Token_Class.String));
                }
                else if (numberToken)
                {
                    identifierLists.Children.Add(match(Token_Class.Number));
                }
                identifierLists.Children.Add(Identifier_Lists());

                return identifierLists;
            }
            return null;
        }

        Node Repeat()
        {
            Node Repeat = new Node("Repeat");
            Repeat.Children.Add(match(Token_Class.Repeat));
            Repeat.Children.Add(Statements());
            Repeat.Children.Add(match(Token_Class.Until));
            Repeat.Children.Add(Condition_Statement());

            return Repeat;
        }

        Node Condition_Statement()
        {
            Node conditionStatement = new Node("Condition_Statement");
            conditionStatement.Children.Add(Condition());
            conditionStatement.Children.Add(Bool_Condition());

            return conditionStatement;
        }
        Node Condition()
        {
            Node condition = new Node("Condition");
            condition.Children.Add(match(Token_Class.Identfire));
            condition.Children.Add(Condition_Op());
            condition.Children.Add(Term());

            return condition;
        }
        Node Condition_Op()
        {
            Node conditionOperation = new Node("Condition_Op");
            bool underCount = InputPointer < TokenStream.Count;
            bool lessThanToken = false;
            bool greaterThanToken = false;
            bool isEqualToken = false;
            bool notEqualToken = false;
            if (underCount)
            {
                lessThanToken = TokenStream[InputPointer].token_type == Token_Class.LessThan;
                greaterThanToken = TokenStream[InputPointer].token_type == Token_Class.GreaterThan;
                isEqualToken = TokenStream[InputPointer].token_type == Token_Class.IsEqual;
                notEqualToken = TokenStream[InputPointer].token_type == Token_Class.NotEqual;
            }
            if (greaterThanToken)
            {
                conditionOperation.Children.Add(match(Token_Class.GreaterThan));
                return conditionOperation;
            }
            else if (isEqualToken)
            {
                conditionOperation.Children.Add(match(Token_Class.IsEqual));
                return conditionOperation;
            }
            else if (lessThanToken)
            {
                conditionOperation.Children.Add(match(Token_Class.LessThan));
                return conditionOperation;
            }
            else if (notEqualToken)
            {
                conditionOperation.Children.Add(match(Token_Class.NotEqual));
                return conditionOperation;
            }

            if (InputPointer + 1 <= TokenStream.Count)
            {
                Errors.Error_List.Add("Parsing Error: Expected "
            + Token_Class.LessThan + " or " + "Expected "
            + Token_Class.GreaterThan + " or " + "Expected "
            + Token_Class.IsEqual + " or " + "Expected "
            + Token_Class.NotEqual + " and " +

            TokenStream[InputPointer].token_type.ToString() +
            "  found\r\n");
                InputPointer++;
            }


            return null;
        }
        Node Bool_Condition()
        {
            Node boolCondition = new Node("Bool_Condition");
            bool underCount = InputPointer < TokenStream.Count;
            bool andToken = false;
            bool orToken = false;

            if (underCount)
            {
                andToken = TokenStream[InputPointer].token_type == Token_Class.And;
                orToken = TokenStream[InputPointer].token_type == Token_Class.Or;
            }
            if (andToken || orToken)
            {
                boolCondition.Children.Add(Boolean_Op());
                boolCondition.Children.Add(Condition());
                boolCondition.Children.Add(Bool_Condition());

                return boolCondition;

            }
            return null;
        }
        Node Boolean_Op()
        {
            Node booleanOperation = new Node("Boolean_Operator");
            bool underCount = InputPointer < TokenStream.Count;
            bool andToken = false;
            bool orToken = false;

            if (underCount)
            {
                andToken = TokenStream[InputPointer].token_type == Token_Class.And;
                orToken = TokenStream[InputPointer].token_type == Token_Class.Or;
            }
            if (andToken)
            {
                booleanOperation.Children.Add(match(Token_Class.And));
                return booleanOperation;

            }
            else if (orToken)
            {
                booleanOperation.Children.Add(match(Token_Class.Or));
                return booleanOperation;
            }
            if (InputPointer + 1 <= TokenStream.Count)
            {
                Errors.Error_List.Add("Parsing Error: Expected "
             + Token_Class.And + "Operator " + " or " + "Expected "
             + Token_Class.Or + "Operator" + " and " +
             TokenStream[InputPointer].token_type.ToString() +
             "  found\r\n");
                //if (InputPointer + 1 < TokenStream.Count)
                InputPointer++;
            }


            return null;
        }
        Node If()
        {
            Node If_State = new Node("If");
            If_State.Children.Add(match(Token_Class.If));
            If_State.Children.Add(Condition_Statement());
            If_State.Children.Add(match(Token_Class.Then));
            If_State.Children.Add(Statements());
            If_State.Children.Add(Close());

            return If_State;
        }
        Node Close()
        {
            Node closeIf = new Node("Close");
            bool underCount = InputPointer < TokenStream.Count;
            bool elseToken = false;
            bool elseIfToken = false;
            bool endToken = false;
            if (underCount)
            {
                elseToken = TokenStream[InputPointer].token_type == Token_Class.Else;
                elseIfToken = TokenStream[InputPointer].token_type == Token_Class.Elseif;
                endToken = TokenStream[InputPointer].token_type == Token_Class.End;
            }
            if (elseIfToken)
            {
                closeIf.Children.Add(ElseIf());
                return closeIf;
            }
            else if (endToken)
            {
                closeIf.Children.Add(match(Token_Class.End));
                return closeIf;
            }
            else if (elseToken)
            {
                closeIf.Children.Add(Else());
                return closeIf;
            }
            if (InputPointer + 1 <= TokenStream.Count)
            {
                Errors.Error_List.Add("Parsing Error: Expected "
             + Token_Class.Elseif + " or " + "Expected "
            + Token_Class.End + " or " + "Expected "
             + Token_Class.Else + " and " +
             TokenStream[InputPointer].token_type.ToString() +
             "  found\r\n");
                InputPointer++;
            }


            return null;
        }
        Node ElseIf()
        {
            Node elseIf = new Node("ElseIf");
            elseIf.Children.Add(match(Token_Class.Elseif));
            elseIf.Children.Add(Condition_Statement());
            elseIf.Children.Add(match(Token_Class.Then));
            elseIf.Children.Add(Statements());
            elseIf.Children.Add(Close());

            return elseIf;
        }
        Node Else()
        {
            Node Else_Statement = new Node("Else");
            Else_Statement.Children.Add(match(Token_Class.Else));
            Else_Statement.Children.Add(Statements());
            Else_Statement.Children.Add(match(Token_Class.End));
            return Else_Statement;
        }
        Node Declaration()
        {
            Node declaration = new Node("Declaration");
            declaration.Children.Add(DataType());
            declaration.Children.Add(Decl());
            declaration.Children.Add(Declaration_List());
            declaration.Children.Add(match(Token_Class.Semicolon));
            return declaration;
        }
        Node Decl()
        {
            Node declaration = new Node("Decl");

            declaration.Children.Add(match(Token_Class.Identfire));
            declaration.Children.Add(Assign());
            return declaration;
        }
        Node Assign()
        {
            Node assign = new Node("Assign");
            bool underCount = InputPointer < TokenStream.Count;
            bool assignToken = false;


            if (underCount)
            {
                assignToken = TokenStream[InputPointer].token_type == Token_Class.AssignmentOP;

            }
            if (assignToken)
            {
                assign.Children.Add(match(Token_Class.AssignmentOP));
                assign.Children.Add(Expression());
                return assign;
            }


            return null;
        }
        Node Declaration_List()
        {
            Node declarationList = new Node("Declaration_List");

            bool underCount = InputPointer < TokenStream.Count;
            bool commaToken = false;


            if (underCount)
            {
                commaToken = TokenStream[InputPointer].token_type == Token_Class.Comma;

            }
            if (commaToken)
            {
                declarationList.Children.Add(match(Token_Class.Comma));
                declarationList.Children.Add(match(Token_Class.Identfire));
                declarationList.Children.Add(Assign());
                declarationList.Children.Add(Declaration_List());
                return declarationList;
            }

            return null;
        }



        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }
        
        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
