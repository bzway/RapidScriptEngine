﻿using JScript.Lexers;
using JScript.Script;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JScript.Parsers
{
    public class Parser
    {
        private static Regex regexIdentifier = new Regex("^[_a-zA-Z][_a-zA-Z0-9]*");

        private readonly Lexer lexer;
        public Parser(string code)
        {
            this.lexer = new Lexer(code);
        }
        public SyntaxTree Parse()
        {
            SyntaxTree tree = new SyntaxTree();
            while (this.lexer.NextToken())
            {
                var token = this.lexer.Current;
                switch (token.Type)
                {

                    #region Assign
                    case TokenType.Word:
                        if (!regexIdentifier.IsMatch(token.Fragment.Text))
                        {
                            throw new ScriptException("", token);
                        }
                        var operate = this.StepOperate();
                        var exprssion = this.StepExpression();
                        tree.Children.Add(new AssignNode(token.Fragment.Text, OperateType.Add, exprssion));
                        break;
                    #endregion

                    #region Return
                    case TokenType.Return:
                        var returnNode = this.StepReturn();

                        tree.Children.Add(returnNode);
                        break;
                    #endregion

                    #region Function

                    case TokenType.Function:
                        tree.Children.Add(this.Function(ScriptTypes.Null));
                        break;

                    #endregion

                    #region While
                    case TokenType.While:
                        var whileNode = this.StepWhile();
                        tree.Children.Add(whileNode);
                        break;
                    #endregion

                    #region For
                    case TokenType.For:
                        var forNode = this.StepFor();
                        tree.Children.Add(forNode);
                        break;
                    #endregion

                    #region Foreach
                    case TokenType.Foreach:
                        break;
                    #endregion

                    #region If
                    case TokenType.If:
                        var ifNode = this.StepIf();
                        tree.Children.Add(ifNode);
                        break;
                    #endregion
                    default:
                        break;
                }
            }
            return tree;
        }

        private ISyntaxNode[] StepParameter()
        {
            throw new NotImplementedException();
        }

        private ISyntaxNode StepAssgin()
        {
            var left = this.lexer.Current.Fragment.Text;
            OperateType opt = this.StepOperate();
            ISyntaxNode right = this.StepExpression();
            return new AssignNode(left, opt, right);
        }

        private ISyntaxNode StepExpression()
        {
            throw new NotImplementedException();
        }

        private OperateType StepOperate()
        {
            OperateType operate = OperateType.None;
            while (this.lexer.NextToken())
            {
                switch (this.lexer.Current.Type)
                {
                    case TokenType.OpreationAdd:
                        var next = this.StepOperate();
                        switch (next)
                        {
                            case OperateType.Add:
                                return OperateType.AddAdd;
                            case OperateType.None:
                                return OperateType.Add;
                            case OperateType.Assign:
                                break;
                            case OperateType.Invoke:
                                break;
                            default:
                                break;
                        }
                        
                        break;
                    case TokenType.OpreationSub:
                        break;
                    case TokenType.OpreationMul:
                        break;
                    case TokenType.OpreationDiv:
                        break;
                    case TokenType.OpreationMod:
                        break;
                    case TokenType.Not:
                        break;
                    case TokenType.And:
                        break;
                    case TokenType.Or:
                        break;
                    case TokenType.Xor:
                        break;
                    case TokenType.OpenParen:
                        operate = OperateType.Invoke;
                        break;
                    case TokenType.CloseParen:
                        break;
                    case TokenType.End:
                        break;
                    case TokenType.Equal:
                        break;
                    case TokenType.OpenSquare:
                        break;
                    case TokenType.CloseSquare:
                        break;
                    case TokenType.Dot:
                        break;
                    case TokenType.Comma:
                        break;
                    default:
                        break;
                }
            }
            throw new NotImplementedException();
        }

        private ISyntaxNode StepReturn()
        {
            var expressionNode = this.StepExpression();
            ReturnNode returnNode = new ReturnNode(expressionNode);
            return returnNode;
        }

        private ISyntaxNode Function(ScriptTypes type)
        {
            var token = this.lexer.NextToken();
            return null;
        }

        private ISyntaxNode Invoke(Token token, Token token2)
        {
            return null;
        }

        private ISyntaxNode StepIf()
        {
            var condition = (AbstractSyntaxNode<BooleanScriptType>)this.StepExpression();
            var blockTrue = this.Parse();

            var blockFalse = this.Parse();

            return new IfNode(condition, new SequenceNode(blockTrue.Children.ToArray()), new SequenceNode(blockFalse.Children.ToArray()));
        }
        private ISyntaxNode StepWhile()
        {
            var condition = (AbstractSyntaxNode<BooleanScriptType>)this.StepExpression();
            var block = this.Parse();
            return new WhileNode(condition, new SequenceNode(block.Children.ToArray()));
        }
        private ISyntaxNode StepFor()
        {
            return new WhileNode(null, null);
        }
    }
}