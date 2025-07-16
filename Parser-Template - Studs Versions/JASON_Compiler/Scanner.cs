using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

public enum Token_Class
{
    //Begin, Call, Declare, End, Do, Else, EndIf, EndUntil, EndWhile, If, Integer,
    //Parameters, Procedure, Program, Read, Real, Set, Then, Until, While, Write,
    //Dot, Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,
    //GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,
    //Idenifier, Constant


    Int, Float, Data_type_String, String, Read, Write, Repeat, Until, If, Elseif, Else, Then,
    Return, Endl, End, PlusOp, MinusOp, MultiplyOp, DivideOp, Identfire, Number, LessThan,
    GreaterThan, IsEqual, NotEqual, And, Or, AssignmentOP,Main,
    OpenBracket,//  [
    CloseBracket,// ]
    OpenParen,//    (
    CloseParen, //  )
    OpenBrac,// {
    CloseBrac,//    }
    Semicolon,
    Comma,
   // Comment

}
namespace JASON_Compiler
{


    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> ConditionOperators = new Dictionary<string, Token_Class>();

        Dictionary<string, Token_Class> BooleanOperators = new Dictionary<string, Token_Class>();
        public Scanner()
        {
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("int", Token_Class.Int);
            ReservedWords.Add("float", Token_Class.Float);
            ReservedWords.Add("string", Token_Class.Data_type_String);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("elseif", Token_Class.Elseif);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("endl", Token_Class.Endl);
            ReservedWords.Add("Identfire", Token_Class.Identfire);
            ReservedWords.Add("end", Token_Class.End);
            ReservedWords.Add("main", Token_Class.Main);

            ReservedWords.Add("[", Token_Class.OpenBracket);
            ReservedWords.Add("]", Token_Class.CloseBracket);
            ReservedWords.Add("(", Token_Class.OpenParen);
            ReservedWords.Add(")", Token_Class.CloseParen);
            ReservedWords.Add("{", Token_Class.OpenBrac);
            ReservedWords.Add("}", Token_Class.CloseBrac);
            ReservedWords.Add(";", Token_Class.Semicolon);
            ReservedWords.Add(",", Token_Class.Comma);



            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);



            ConditionOperators.Add("=", Token_Class.IsEqual);
            ConditionOperators.Add("<>", Token_Class.NotEqual);
            ConditionOperators.Add("<", Token_Class.LessThan);
            ConditionOperators.Add(">", Token_Class.GreaterThan);


            BooleanOperators.Add("&&", Token_Class.And);
            BooleanOperators.Add("||", Token_Class.Or);
        }

        public void StartScanning(string SourceCode)
        {
            Tokens.Clear();
            Errors.Error_List.Clear();
            int lastIndex = -1;
            for (int i = 0; i < SourceCode.Length - 1;)
            {
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();
                int j = i + 1;
                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n' || CurrentChar == '\t')
                {
                    i = j;
                    lastIndex = j;
                    continue;
                }
                if (char.IsLetter(CurrentChar))
                {
                    CurrentChar = SourceCode[j];
                    while (char.IsLetter(CurrentChar) || char.IsDigit(CurrentChar))
                    {
                        CurrentLexeme += CurrentChar.ToString();
                        j++;
                        if (j < SourceCode.Length)
                            CurrentChar = SourceCode[j];
                        else
                            break;
                    }
                }
                else if (char.IsDigit(CurrentChar))
                {
                    CurrentChar = SourceCode[j];
                    while (char.IsDigit(CurrentChar) || CurrentChar == '.')
                    {
                        CurrentLexeme += CurrentChar.ToString();
                        j++;
                        if (j < SourceCode.Length)
                            CurrentChar = SourceCode[j];
                        else
                            break;
                    }
                }
                else if (CurrentChar == '/')
                {
                    CurrentChar = SourceCode[j];
                    if (CurrentChar == '*')
                    {
                        CurrentLexeme += CurrentChar.ToString();
                        j++;
                        CurrentChar = SourceCode[j];
                        int k = j + 1;
                        char NextChar;
                        while (j < SourceCode.Length && k < SourceCode.Length)
                        {
                            NextChar = SourceCode[k];
                            if (CurrentChar == '*' && NextChar == '/')
                            {
                                j += 2;
                                CurrentLexeme += "*/";
                                break;
                            }
                            else
                            {
                                CurrentLexeme += CurrentChar.ToString();
                                j++;
                                k++;
                                CurrentChar = NextChar;
                            }
                        }
                    }
                }
                else if (CurrentChar == '"')
                {
                    CurrentChar = SourceCode[j];
                    while (CurrentChar != '"')
                    {
                        CurrentLexeme += CurrentChar.ToString();
                        j++;
                        if (j < SourceCode.Length)
                            CurrentChar = SourceCode[j];
                        else
                            break;
                    }
                    if (j < SourceCode.Length && CurrentChar == '"')
                    {
                        CurrentLexeme += CurrentChar.ToString();
                        j++;
                    }
                }
                else
                {
                    // checking the operators
                    char NextChar = SourceCode[j];
                    if (CurrentChar == '&' && NextChar == '&' ||
                       CurrentChar == '|' && NextChar == '|' ||
                       CurrentChar == '<' && NextChar == '>' ||
                       CurrentChar == ':' && NextChar == '=')
                    {
                        CurrentLexeme += NextChar.ToString();
                        j++;
                    }
                }
                FindTokenClass(CurrentLexeme);
                i = j;
                lastIndex = j;
            }
            if (lastIndex == SourceCode.Length - 1)
                FindTokenClass(SourceCode[lastIndex].ToString());
            JASON_Compiler.TokenStream = Tokens;

        }
        void FindTokenClass(string Lex)
        {

            Token Tok = new Token();
            Tok.lex = Lex;
            char slash = '/';
            char star = '*';

            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }
            else if (ConditionOperators.ContainsKey(Lex))
            {
                Tok.token_type = ConditionOperators[Lex];
                Tokens.Add(Tok);
            }
            else if (BooleanOperators.ContainsKey(Lex))
            {
                Tok.token_type = BooleanOperators[Lex];
                Tokens.Add(Tok);
            }
            else if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Identfire;
                Tokens.Add(Tok);
            }
            else if (isString(Lex))
            {
                Tok.token_type = Token_Class.String;
                Tokens.Add(Tok);
            }
            else if (isNumber(Lex))
            {
                Tok.token_type = Token_Class.Number;
                Tokens.Add(Tok);
            }
            else if (isAssignmentOp(Lex))
            {
                Tok.token_type = Token_Class.AssignmentOP;
                Tokens.Add(Tok);
            }
            else if ((Lex[0].Equals(slash) && Lex[1].Equals(star)))
            {
                
                if (isComment(Lex))
                    return;
                else
                {
                    Errors.Error_List.Add("comment not closed " + Lex);
                    return;
                }
            }
            //Is it an undefined?
            else
            {
                Errors.Error_List.Add("undefined token: " + Lex);
            }
        }
        bool isIdentifier(string lex)
        {
            Regex reg = new Regex(@"^([a-zA-Z])([0-9a-zA-Z])*$", RegexOptions.Compiled);
          
            if (reg.IsMatch(lex))
            {
                return true;
            }

            return false;
        }

        bool isString(string lex)
        {
            Regex String = new Regex(@"^"".*""$");
            if (String.IsMatch(lex))
            {
                return true;
            }

            return false;
        }


        bool isNumber(string lex)
        {
            Regex Number = new Regex(@"^[0-9]+(\.[0-9]+)?$");
            if (Number.IsMatch(lex))
            {
                return true;
            }
            return false;
        }

        bool isComment(string lex)
        {
            bool isValid = false;
            var rex = new Regex(@"^/\*(.|\n)*\*/$");

            if (rex.IsMatch(lex))
            {
                isValid = true;
            }
            return isValid;
        }

        bool isAssignmentOp(string lex)
        {

            if (lex == ":=")
            {
                return true;
            }
            return false;
        }



    }
}