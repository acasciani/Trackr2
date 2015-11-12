using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using TrackrModels;
using Trackr;
using System.Web.Providers.Entities;

namespace TrackrProviders
{
    internal static class ProviderUtils
    {
        public static object GetConfigValue(NameValueCollection config, string configKey, object defaultValue)
        {
            object configValue;

            try
            {
                configValue = config[configKey];
                configValue = string.IsNullOrEmpty(configValue.ToString()) ? defaultValue : configValue;
            }
            catch
            {
                configValue = defaultValue;
            }

            return configValue;
        }

        public static Application EnsureApplication(string applicationName, TrackrModels.UserManagement context)
        {
            Application application = new Application()
            {
                ApplicationName = applicationName
            };
            return application;
        }

        /// <summary>
        /// Builds a contains expression.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="valueSelector">The value selector.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static Expression<Func<TElement, bool>> BuildContainsExpression<TElement, TValue>(Expression<Func<TElement, TValue>> valueSelector, IEnumerable<TValue> values)
        {
            if (null == valueSelector)
            {
                throw new ArgumentNullException("valueSelector");
            }

            if (null == values)
            {
                throw new ArgumentNullException("values");
            }

            ParameterExpression p = valueSelector.Parameters.Single();

            if (!values.Any())
            {
                return e => false;
            }

            IEnumerable<Expression> equals = values.Select(value => (Expression)Expression.Equal(valueSelector.Body, Expression.Constant(value, typeof(TValue))));
            Expression body = equals.Aggregate(Expression.Or);
            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }
    }
}