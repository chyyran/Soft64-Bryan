using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.MipsR4300.Debugging.Dog
{
    internal sealed class DogQueryProvider : IQueryProvider
    {
        #region IQueryProvider Members

        public IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        public object Execute(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
