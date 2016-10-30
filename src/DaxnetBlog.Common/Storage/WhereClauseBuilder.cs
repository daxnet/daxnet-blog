using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DaxnetBlog.Common.Storage
{
    /// <summary>
    /// Represents the base class of all the where clause builders.
    /// </summary>
    /// <typeparam name="TDataObject">The type of the data object which would be mapped to
    /// a certain table in the relational database.</typeparam>
    internal sealed class WhereClauseBuilder<TEntity, TKey> : ExpressionVisitor, IWhereClauseBuilder<TEntity, TKey>
        where TKey : IEquatable<TKey>
        where TEntity : class, IEntity<TKey>, new()
    {
        #region Private Fields
        private readonly StringBuilder sb = new StringBuilder();
        private readonly Dictionary<string, object> parameterValues = new Dictionary<string, object>();
        private readonly IStoreMapping storeMapping;
        private readonly StorageDialectSettings dialectSettings;
        private readonly bool useTableAlias;
        private bool startsWith = false;
        private bool endsWith = false;
        private bool contains = false;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>WhereClauseBuilderBase&lt;T&gt;</c> class.
        /// </summary>
        /// <param name="storeMapping">The <c>Apworks.Storage.IStorageMappingResolver</c>
        /// instance which will be used for generating the mapped field names.</param>
        public WhereClauseBuilder(IStoreMapping storeMapping, StorageDialectSettings dialectSettings, bool useTableAlias = false)
        {
            this.storeMapping = storeMapping;
            this.dialectSettings = dialectSettings;
            this.useTableAlias = useTableAlias;
        }
        #endregion

        #region Private Methods
        private void Out(string s)
        {
            sb.Append(s);
        }

        //private void OutMember(Expression instance, MemberInfo member)
        //{
        //    string mappedFieldName = storeMapping.GetEscapedColumnName<TEntity, TKey>(this.dialectSettings, member.Name);
        //    Out(mappedFieldName);
        //}
        #endregion

        #region Protected Properties
        /// <summary>
        /// Gets a <c>System.String</c> value which represents the AND operation in the WHERE clause.
        /// </summary>
        private string And => dialectSettings.SqlAndStatement;
        /// <summary>
        /// Gets a <c>System.String</c> value which represents the OR operation in the WHERE clause.
        /// </summary>
        private string Or => dialectSettings.SqlOrStatement;
        /// <summary>
        /// Gets a <c>System.String</c> value which represents the EQUAL operation in the WHERE clause.
        /// </summary>
        private string Equal => dialectSettings.SqlEqualOperator;

        /// <summary>
        /// Gets a <c>System.String</c> value which represents the NOT operation in the WHERE clause.
        /// </summary>
        private string Not => dialectSettings.SqlNotStatement;

        /// <summary>
        /// Gets a <c>System.String</c> value which represents the NOT EQUAL operation in the WHERE clause.
        /// </summary>
        private string NotEqual => dialectSettings.SqlNotEqualOperator;

        private string Is => dialectSettings.SqlIsOperator;

        private string IsNot => dialectSettings.SqlIsNotOperator;

        /// <summary>
        /// Gets a <c>System.String</c> value which represents the LIKE operation in the WHERE clause.
        /// </summary>
        private string Like => dialectSettings.SqlLikeStatement;

        /// <summary>
        /// Gets a <c>System.Char</c> value which represents the place-holder for the wildcard in the LIKE operation.
        /// </summary>
        private char LikeSymbol => dialectSettings.SqlLikeSymbol;

        #endregion

        #region Protected Methods
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.BinaryExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            string str;
            switch (node.NodeType)
            {
                case ExpressionType.Add:
                    str = "+";
                    break;
                case ExpressionType.AddChecked:
                    str = "+";
                    break;
                case ExpressionType.AndAlso:
                    str = this.And;
                    break;
                case ExpressionType.Divide:
                    str = "/";
                    break;
                case ExpressionType.Equal:
                    if (node.Right.NodeType == ExpressionType.Constant &&
                        ((ConstantExpression)node.Right).Value == null)
                    {
                        str = this.Is;
                    }
                    else
                    {
                        str = this.Equal;
                    }
                    break;
                case ExpressionType.GreaterThan:
                    str = ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    str = ">=";
                    break;
                case ExpressionType.LessThan:
                    str = "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    str = "<=";
                    break;
                case ExpressionType.Modulo:
                    str = "%";
                    break;
                case ExpressionType.Multiply:
                    str = "*";
                    break;
                case ExpressionType.MultiplyChecked:
                    str = "*";
                    break;
                case ExpressionType.Not:
                    str = this.Not;
                    break;
                case ExpressionType.NotEqual:
                    if (node.Right.NodeType == ExpressionType.Constant &&
                        ((ConstantExpression)node.Right).Value == null)
                    {
                        str = this.IsNot;
                    }
                    else
                    {
                        str = this.NotEqual;
                    }
                    break;
                case ExpressionType.OrElse:
                    str = this.Or;
                    break;
                case ExpressionType.Subtract:
                    str = "-";
                    break;
                case ExpressionType.SubtractChecked:
                    str = "-";
                    break;
                default:
                    throw new NotSupportedException();
            }

            Out("(");
            Visit(node.Left);
            Out(" ");
            Out(str);
            Out(" ");
            Visit(node.Right);
            Out(")");
            return node;
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.MemberExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member.DeclaringType == typeof(TEntity) ||
                typeof(TEntity).GetTypeInfo().IsSubclassOf(node.Member.DeclaringType))
            {
                var mappedFieldName = "";
                if (this.useTableAlias)
                {
                    mappedFieldName = $"T_{storeMapping.GetTableName<TEntity, TKey>()}.{storeMapping.GetEscapedColumnName<TEntity, TKey>(this.dialectSettings, node.Member.Name)}";
                }
                else
                {
                    mappedFieldName = $"{storeMapping.GetEscapedTableName<TEntity, TKey>(dialectSettings)}.{storeMapping.GetEscapedColumnName<TEntity, TKey>(this.dialectSettings, node.Member.Name)}";
                }
                Out(mappedFieldName);
            }
            else
            {
                if (node.Member is FieldInfo)
                {
                    ConstantExpression ce = node.Expression as ConstantExpression;
                    FieldInfo fi = node.Member as FieldInfo;
                    object fieldValue = fi.GetValue(ce.Value);
                    Expression constantExpr = Expression.Constant(fieldValue);
                    Visit(constantExpr);
                }
                else if (node.Member is PropertyInfo)
                {
                    if (node.NodeType == ExpressionType.Constant)
                    {
                        ConstantExpression ce = node.Expression as ConstantExpression;
                        PropertyInfo pi = node.Member as PropertyInfo;
                        object fieldValue = pi.GetValue(ce.Value);
                        Expression constantExpr = Expression.Constant(fieldValue);
                        Visit(constantExpr);
                    }
                    else if (node.NodeType == ExpressionType.MemberAccess)
                    {
                        var memberExpression = node.Expression as MemberExpression;
                        VisitMember(memberExpression);
                    }
                }
                else
                    throw new NotSupportedException();
            }
            return node;
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.ConstantExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value == null)
            {
                Out("NULL");
            }
            else
            {
                string paramName = string.Format("{0}{1}", this.dialectSettings.ParameterChar, Utils.GetUniqueStringValue(8));
                Out(paramName);
                if (!parameterValues.ContainsKey(paramName))
                {
                    object v = null;
                    if (startsWith && node.Value is string)
                    {
                        startsWith = false;
                        v = node.Value.ToString() + LikeSymbol;
                    }
                    else if (endsWith && node.Value is string)
                    {
                        endsWith = false;
                        v = LikeSymbol + node.Value.ToString();
                    }
                    else if (contains && node.Value is string)
                    {
                        contains = false;
                        v = LikeSymbol + node.Value.ToString() + LikeSymbol;
                    }
                    else if (node.Type.GetTypeInfo().IsEnum)
                    {
                        v = Convert.ToInt32(node.Value);
                    }
                    else
                    {
                        v = node.Value;
                    }
                    parameterValues.Add(paramName, v);
                }
            }
            return node;
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.MethodCallExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            Out("(");
            Visit(node.Object);
            if (node.Arguments == null || node.Arguments.Count != 1)
                throw new NotSupportedException();
            Expression expr = node.Arguments[0];
            switch (node.Method.Name)
            {
                case "StartsWith":
                    startsWith = true;
                    Out(" ");
                    Out(Like);
                    Out(" ");
                    break;
                case "EndsWith":
                    endsWith = true;
                    Out(" ");
                    Out(Like);
                    Out(" ");
                    break;
                case "Equals":
                    Out(" ");
                    Out(Equal);
                    Out(" ");
                    break;
                case "Contains":
                    contains = true;
                    Out(" ");
                    Out(Like);
                    Out(" ");
                    break;
                default:
                    throw new NotSupportedException();
            }
            if (expr is ConstantExpression || expr is MemberExpression)
                Visit(expr);
            else
                throw new NotSupportedException();
            Out(")");
            return node;
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.BlockExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitBlock(BlockExpression node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.CatchBlock"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.ConditionalExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitConditional(ConditionalExpression node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.DebugInfoExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.DefaultExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitDefault(DefaultExpression node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }

        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.ElementInit"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override ElementInit VisitElementInit(ElementInit node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.GotoExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitGoto(GotoExpression node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.Expression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitExtension(Expression node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.IndexExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitIndex(IndexExpression node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.InvocationExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitInvocation(InvocationExpression node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.LabelExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitLabel(LabelExpression node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.LabelTarget"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override LabelTarget VisitLabelTarget(LabelTarget node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.Expression&lt;T&gt;"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.ListInitExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitListInit(ListInitExpression node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.LoopExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitLoop(LoopExpression node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.MemberAssignment"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.MemberBinding"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override MemberBinding VisitMemberBinding(MemberBinding node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.MemberInitExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.MemberListBinding"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.MemberMemberBinding"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.NewExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitNew(NewExpression node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.NewArrayExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.ParameterExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitParameter(ParameterExpression node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.RuntimeVariablesExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.SwitchExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitSwitch(SwitchExpression node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.SwitchCase"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override SwitchCase VisitSwitchCase(SwitchCase node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.TryExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitTry(TryExpression node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.TypeBinaryExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            throw new NotSupportedException("The current method is not implemented.");
        }
        /// <summary>
        /// Visits the children of <see cref="System.Linq.Expressions.UnaryExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise,
        /// returns the original expression.</returns>
        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Convert)
            {
                Visit(node.Operand);
                return node;
            }
            else
            {
                throw new NotSupportedException("The current method is not implemented.");
            }
        }
        #endregion

        #region IWhereClauseBuilder<T> Members
        /// <summary>
        /// Builds the WHERE clause from the given expression object.
        /// </summary>
        /// <param name="expression">The expression object.</param>
        /// <returns>The <c>Apworks.Storage.Builders.WhereClauseBuildResult</c> instance
        /// which contains the build result.</returns>
        public WhereClauseBuildResult BuildWhereClause(Expression<Func<TEntity, bool>> expression)
        {
            this.sb.Clear();
            this.parameterValues.Clear();
            this.Visit(expression.Body);
            WhereClauseBuildResult result = new WhereClauseBuildResult
            {
                ParameterValues = parameterValues,
                WhereClause = sb.ToString()
            };
            return result;
        }
        #endregion
    }
}
