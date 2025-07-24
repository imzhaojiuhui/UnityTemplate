using System;
using System.Collections.Generic;

namespace HotUpdate
{
    public class StringConditionParser
    {
        private readonly Dictionary<string, Func<string, string, bool>> _comparisonOperators = new Dictionary<string, Func<string, string, bool>>
        {
            { "==", (a, b) => a == b },
            { ">", (a, b) => double.Parse(a) > double.Parse(b) },
            { "<", (a, b) => double.Parse(a) < double.Parse(b) },
            { ">=", (a, b) => double.Parse(a) >= double.Parse(b) },
            { "<=", (a, b) => double.Parse(a) <= double.Parse(b) }
        };

        private readonly Dictionary<string, Func<bool, bool, bool>> _logicalOperators = new Dictionary<string, Func<bool, bool, bool>>
        {
            { "&&", (a, b) => a && b },
            { "||", (a, b) => a || b }
        };

        private readonly Dictionary<string, Func<bool, bool>> _unaryOperators = new Dictionary<string, Func<bool, bool>>
        {
            { "!", a => !a }
        };

        private string[] _tokens;
        private int _position;

        // 解析并计算表达式
        public bool Evaluate(string expression, Dictionary<string, string> variables)
        {
            _tokens = Tokenize(expression);
            _position = 0;
            return ParseExpression(variables);
        }
        
        private Dictionary<string, string> _cacheVariables = new Dictionary<string, string>();
        
        /// <summary>
        /// e.g:expression = "(LevelId>=4 && CarLv>2)&&(TaskId:1001)";
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool Evaluate(string expression)
        {
            // 示例：游戏参数
            // _cacheVariables["LevelId"] = DataMgr.Instance.MapLevel.ToString();
            // _cacheVariables["CarLv"] = DataMgr.Instance.CarLv.ToString();
            // _cacheVariables["TaskId"] = TaskMgr.Instance.taskid
            return Evaluate(expression, _cacheVariables);
        }

        private Dictionary<string, string[]> _cacheTokens = new Dictionary<string, string[]>();
        // 词法分析
        private string[] Tokenize(string expression)
        {
            if (_cacheTokens.TryGetValue(expression,  out var result))
            {
                return result;
            }
            var tokens = new List<string>();
            var currentToken = "";

            for (int i = 0; i < expression.Length; i++)
            {
                var c = expression[i];

                if (char.IsWhiteSpace(c))
                {
                    if (!string.IsNullOrEmpty(currentToken))
                    {
                        tokens.Add(currentToken);
                        currentToken = "";
                    }
                    continue;
                }

                if (c == '(' || c == ')' || c == '!' || c == '&' || c == '|' || c == '=' || c == '>' || c == '<')
                {
                    if (!string.IsNullOrEmpty(currentToken))
                    {
                        tokens.Add(currentToken);
                        currentToken = "";
                    }

                    if (c == '&' && i + 1 < expression.Length && expression[i + 1] == '&')
                    {
                        tokens.Add("&&");
                        i++;
                    }
                    else if (c == '|' && i + 1 < expression.Length && expression[i + 1] == '|')
                    {
                        tokens.Add("||");
                        i++;
                    }
                    else if (c == '=' && i + 1 < expression.Length && expression[i + 1] == '=')
                    {
                        tokens.Add("==");
                        i++;
                    }
                    else if (c == '>' && i + 1 < expression.Length && expression[i + 1] == '=')
                    {
                        tokens.Add(">=");
                        i++;
                    }
                    else if (c == '<' && i + 1 < expression.Length && expression[i + 1] == '=')
                    {
                        tokens.Add("<=");
                        i++;
                    }
                    else
                    {
                        tokens.Add(c.ToString());
                    }
                }
                else
                {
                    currentToken += c;
                }
            }

            if (!string.IsNullOrEmpty(currentToken))
            {
                tokens.Add(currentToken);
            }

            result = tokens.ToArray();
            _cacheTokens.Add(expression, result);
            return result;
        }

        // 解析表达式
        private bool ParseExpression(Dictionary<string, string> variables)
        {
            var result = ParseTerm(variables);

            while (_position < _tokens.Length && (_tokens[_position] == "&&" || _tokens[_position] == "||"))
            {
                var op = _tokens[_position++];
                var right = ParseTerm(variables);
                result = _logicalOperators[op](result, right);
            }

            return result;
        }

        // 解析项
        private bool ParseTerm(Dictionary<string, string> variables)
        {
            if (_position >= _tokens.Length)
            {
                throw new ArgumentException("Unexpected end of expression");
            }

            var token = _tokens[_position];

            if (token == "!")
            {
                _position++;
                return _unaryOperators["!"](ParseTerm(variables));
            }
            else if (token == "(")
            {
                _position++;
                var result = ParseExpression(variables);

                if (_position >= _tokens.Length || _tokens[_position] != ")")
                {
                    throw new ArgumentException("Mismatched parentheses");
                }

                _position++;
                return result;
            }
            else
            {
                return ParseComparison(variables);
            }
        }

        // 解析比较表达式
        private bool ParseComparison(Dictionary<string, string> variables)
        {
            var left = _tokens[_position++];

            if (_position >= _tokens.Length)
            {
                return GetValue(left, variables) == "true";
            }

            var op = _tokens[_position];

            if (_comparisonOperators.ContainsKey(op))
            {
                _position++;

                if (_position >= _tokens.Length)
                {
                    throw new ArgumentException("Missing right operand");
                }

                var right = _tokens[_position++];
                return _comparisonOperators[op](GetValue(left, variables), GetValue(right, variables));
            }

            return GetValue(left, variables) == "true";
        }

        // 获取变量值
        private string GetValue(string token, Dictionary<string, string> variables)
        {
            if (variables.TryGetValue(token, out var value))
            {
                return value;
            }

            // 示例：自定义条件
            // if (token.StartsWith("TaskId:"))
            // {
            //     int taskId = int.Parse(token.AsSpan(7));
            //     return TaskMgr.Instance.IsTaskFinished(taskId)?"true":"false";
            // }
            
            return token;
        }
    }
}