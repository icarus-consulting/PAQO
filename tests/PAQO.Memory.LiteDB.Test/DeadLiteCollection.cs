// MIT License
//
// Copyright(c) 2022 ICARUS Consulting GmbH
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Test.PAQO.Memory.LiteDB
{
    public sealed class DeadLiteCollection : ILiteCollection<BsonDocument>
    {
        public string Name => throw new InvalidOperationException("This collection is dead.");

        public BsonAutoId AutoId => throw new InvalidOperationException("This collection is dead.");

        public EntityMapper EntityMapper => throw new InvalidOperationException("This collection is dead.");

        public int Count()
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public int Count(BsonExpression predicate)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public int Count(string predicate, BsonDocument parameters)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public int Count(string predicate, params BsonValue[] args)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public int Count(Expression<Func<BsonDocument, bool>> predicate)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public int Count(Query query)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public bool Delete(BsonValue id)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public int DeleteAll()
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public int DeleteMany(BsonExpression predicate)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public int DeleteMany(string predicate, BsonDocument parameters)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public int DeleteMany(string predicate, params BsonValue[] args)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public int DeleteMany(Expression<Func<BsonDocument, bool>> predicate)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public bool DropIndex(string name)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public bool EnsureIndex(string name, BsonExpression expression, bool unique = false)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public bool EnsureIndex(BsonExpression expression, bool unique = false)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public bool EnsureIndex<K>(Expression<Func<BsonDocument, K>> keySelector, bool unique = false)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public bool EnsureIndex<K>(string name, Expression<Func<BsonDocument, K>> keySelector, bool unique = false)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public bool Exists(BsonExpression predicate)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public bool Exists(string predicate, BsonDocument parameters)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public bool Exists(string predicate, params BsonValue[] args)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public bool Exists(Expression<Func<BsonDocument, bool>> predicate)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public bool Exists(Query query)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public IEnumerable<BsonDocument> Find(BsonExpression predicate, int skip = 0, int limit = int.MaxValue)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public IEnumerable<BsonDocument> Find(Query query, int skip = 0, int limit = int.MaxValue)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public IEnumerable<BsonDocument> Find(Expression<Func<BsonDocument, bool>> predicate, int skip = 0, int limit = int.MaxValue)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public IEnumerable<BsonDocument> FindAll()
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public BsonDocument FindById(BsonValue id)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public BsonDocument FindOne(BsonExpression predicate)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public BsonDocument FindOne(string predicate, BsonDocument parameters)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public BsonDocument FindOne(BsonExpression predicate, params BsonValue[] args)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public BsonDocument FindOne(Expression<Func<BsonDocument, bool>> predicate)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public BsonDocument FindOne(Query query)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public ILiteCollection<BsonDocument> Include<K>(Expression<Func<BsonDocument, K>> keySelector)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public ILiteCollection<BsonDocument> Include(BsonExpression keySelector)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public BsonValue Insert(BsonDocument entity)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public void Insert(BsonValue id, BsonDocument entity)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public int Insert(IEnumerable<BsonDocument> entities)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public int InsertBulk(IEnumerable<BsonDocument> entities, int batchSize = 5000)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public long LongCount()
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public long LongCount(BsonExpression predicate)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public long LongCount(string predicate, BsonDocument parameters)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public long LongCount(string predicate, params BsonValue[] args)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public long LongCount(Expression<Func<BsonDocument, bool>> predicate)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public long LongCount(Query query)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public BsonValue Max(BsonExpression keySelector)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public BsonValue Max()
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public K Max<K>(Expression<Func<BsonDocument, K>> keySelector)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public BsonValue Min(BsonExpression keySelector)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public BsonValue Min()
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public K Min<K>(Expression<Func<BsonDocument, K>> keySelector)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public ILiteQueryable<BsonDocument> Query()
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public bool Update(BsonDocument entity)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public bool Update(BsonValue id, BsonDocument entity)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public int Update(IEnumerable<BsonDocument> entities)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public int UpdateMany(BsonExpression transform, BsonExpression predicate)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public int UpdateMany(Expression<Func<BsonDocument, BsonDocument>> extend, Expression<Func<BsonDocument, bool>> predicate)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public bool Upsert(BsonDocument entity)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public int Upsert(IEnumerable<BsonDocument> entities)
        {
            throw new InvalidOperationException("This collection is dead.");
        }

        public bool Upsert(BsonValue id, BsonDocument entity)
        {
            throw new InvalidOperationException("This collection is dead.");
        }
    }
}
