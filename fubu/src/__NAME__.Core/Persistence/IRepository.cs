#region Using Directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FluentNHibernate.Utils;
using Kokugen.Core.Domain;
using NHibernate;
using NHibernate.Connection;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Linq;
using NHibernate.Mapping;
using NHibernate.Type;
using System.Linq.Expressions;

#endregion

namespace Kokugen.Core.Persistence
{

    public interface IRepository<ENTITY> where ENTITY : Domain.Entity
    {
        void Save(ENTITY entity);

        void SaveAndFlush(ENTITY entity);

        ENTITY Load(Guid id);

        ENTITY Get(Guid id);

        ENTITY FindBy<U>(Expression<Func<ENTITY, U>> expression, U search);

        IQueryable<ENTITY> Query();

        IQueryable<ENTITY> Query(IDomainQuery<ENTITY> whereQuery);

        void Delete(ENTITY entity);

        void DeleteAll();

        IEnumerable<ENTITY> FindAll(params ICriterion[] criteria);

        //PagedList<ENTITY> PagedQuery<ENTITY>(int start, int numItems) where ENTITY : Entity;

        /// <summary>
        /// Execute the specified stored procedure with the given parameters and then converts
        /// the results using the supplied delegate.
        /// </summary>
        /// <typeparam name="T2">The collection type to return.</typeparam>
        /// <param name="converter">The delegate which converts the raw results.</param>
        /// <param name="sp_name">The name of the stored procedure.</param>
        /// <param name="parameters">Parameters for the stored procedure.</param>
        /// <returns></returns>
        IEnumerable<T2> ExecuteStoredProcedure<T2>(Converter<SafeDataReader, T2> converter, string sp_name,
                                                   params Parameter[] parameters);

        IEnumerable<T2> ExecuteStoredProcedure2<T2>(Converter<IDataReader, T2> converter, string sp_name,
                                                    params Parameter[] parameters);

        IQuery CreateQuery(string hqlQuery);
        ISQLQuery CreateSQLQuery(string sqlQuery);

        void Evict(ENTITY entity);
    }

    public class Parameter
    {
        private readonly IType type;

        public Parameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public Parameter(string name, object value, IType type)
        {
            Name = name;
            Value = value;
            this.type = type;
        }

        public IType Type
        {
            get { return type; }
        }

        public string Name { get; set; }

        public object Value { get; set; }
    }

    public class NHibernateRepository<ENTITY>
        where ENTITY : Domain.Entity
    {
        private readonly ISession _session;

        public NHibernateRepository(ISession session)
        {
            _session = session;
        }

        #region IRepository Members

        public virtual void Save(ENTITY entity)
        {
            //if (entity.Id.IsEmpty())
            //    entity.Created = DateTime.Now;
            //entity.LastUpdated = DateTime.Now;

           _session.SaveOrUpdate(entity);
        }

        public virtual void SaveAndFlush(ENTITY entity)
        {
           Save(entity);
            _session.Flush();
        }

        public ENTITY Load(Guid id)
        {
            return _session.Load<ENTITY>(id);
        }

        public ENTITY Get(Guid id)
        {
            return _session.Get<ENTITY>(id);
        }

        public IQueryable<ENTITY> Query()
        {
            return _session.Linq<ENTITY>();
        }

        public IQueryable<ENTITY> Query(IDomainQuery<ENTITY> whereQuery)
        {
            return _session.Linq<ENTITY>().Where(whereQuery.Expression);
        }

        public virtual void Delete(ENTITY entity)
        {
           _session.Delete(entity);
        }

        public void DeleteAll()
        {
            var query = String.Format("from {0}", typeof (ENTITY).Name);
           _session.Delete(query);
        }

        public IEnumerable<ENTITY> FindAll(params ICriterion[] criteria)
        {
            var crit =_session.CreateCriteria(typeof (ENTITY));
            foreach (var criterion in criteria)
            {
                if (criterion == null) continue;
                crit.Add(criterion);
            }
            return crit.Future<ENTITY>();
        }

        public ENTITY FindBy<TU>(Expression<Func<ENTITY, TU>> expression, TU search) 
        {
            string propertyName = ReflectionHelper.GetAccessor(expression).FieldName;
            ICriteria criteria =
                _session.CreateCriteria(typeof(ENTITY)).Add(
                    Restrictions.Eq(propertyName, search));
            return criteria.UniqueResult() as ENTITY;
        }
        

        /// <summary>
        /// Execute the specified stored procedure with the given parameters and then converts
        /// the results using the supplied delegate.
        /// </summary>
        /// <typeparam name="T2">The collection type to return.</typeparam>
        /// <param name="converter">The delegate which converts the raw results.</param>
        /// <param name="sp_name">The name of the stored procedure.</param>
        /// <param name="parameters">Parameters for the stored procedure.</param>
        /// <returns></returns>
        public IEnumerable<T2> ExecuteStoredProcedure<T2>(Converter<SafeDataReader, T2> converter, string sp_name,
                                                          params Parameter[] parameters)
        {
            IConnectionProvider connectionProvider = ((ISessionFactoryImplementor)_session.SessionFactory).ConnectionProvider;
            IDbConnection connection = connectionProvider.GetConnection();

            try
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = sp_name;
                    command.CommandType = CommandType.StoredProcedure;

                    CreateDbDataParameters(command, parameters);
                    var reader = new SafeDataReader(command.ExecuteReader());
                    var results = new List<T2>();

                    while (reader.Read())
                        results.Add(converter(reader));

                    reader.Close();

                    return results;
                }
            }
            finally
            {
                connectionProvider.CloseConnection(connection);
            }
        }

        public IEnumerable<T2> ExecuteStoredProcedure2<T2>(Converter<IDataReader, T2> converter, string sp_name,
                                                           params Parameter[] parameters)
        {
            IConnectionProvider connectionProvider = ((ISessionFactoryImplementor)_session.SessionFactory).ConnectionProvider;
            IDbConnection connection = connectionProvider.GetConnection();

            try
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = sp_name;
                    command.CommandType = CommandType.StoredProcedure;

                    CreateDbDataParameters(command, parameters);
                    var reader = command.ExecuteReader();
                    var results = new List<T2>();

                    while (reader.Read())
                        results.Add(converter(reader));

                    reader.Close();

                    return results;
                }
            }
            finally
            {
                connectionProvider.CloseConnection(connection);
            }
        }

        

        public IQuery CreateQuery(string hqlQuery)
        {
            return _session.CreateQuery(hqlQuery);
        }

        public ISQLQuery CreateSQLQuery(string sqlQuery)
        {
            return _session.CreateSQLQuery(sqlQuery);
        }

        #endregion

        public static void CreateDbDataParameters(IDbCommand command, Parameter[] parameters)
        {
            foreach (Parameter parameter in parameters)
            {
                IDbDataParameter sp_arg = command.CreateParameter();
                sp_arg.ParameterName = parameter.Name;
                sp_arg.Value = parameter.Value;
                command.Parameters.Add(sp_arg);
            }
        }

        

        public void Evict(ENTITY entity)
        {
           _session.Evict(entity);
        }
    }
}