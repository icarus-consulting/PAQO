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
using LiteDB.Engine;
using System;
using System.Collections.Generic;
using Yaapii.Atoms;
using Yaapii.Atoms.Scalar;

namespace PAQO.Memory.LiteDB.Facets
{
    /// <summary>
    /// Envelope for LiteEngine (sticky).
    /// </summary>
    public abstract class LiteEngineEnvelope : ILiteEngine
    {
        private readonly IScalar<ILiteEngine> liteEngine;

        /// <summary>
        /// Envelope for LiteEngine (sticky).
        /// </summary>
        public LiteEngineEnvelope(Func<ILiteEngine> liteEngine)
        {
            this.liteEngine = new ScalarOf<ILiteEngine>(liteEngine);
        }

        public bool BeginTrans()
        {
            return this.liteEngine.Value().BeginTrans();
        }

        public int Checkpoint()
        {
            return this.liteEngine.Value().Checkpoint();
        }

        public bool Commit()
        {
            return this.liteEngine.Value().Commit();
        }

        public int Delete(string collection, IEnumerable<BsonValue> ids)
        {
            return this.liteEngine.Value().Delete(collection, ids);
        }

        public int DeleteMany(string collection, BsonExpression predicate)
        {
            return this.liteEngine.Value().DeleteMany(collection, predicate);
        }

        public void Dispose()
        {
            this.liteEngine.Value().Dispose();
        }

        public bool DropCollection(string name)
        {
            return this.liteEngine.Value().DropCollection(name);
        }

        public bool DropIndex(string collection, string name)
        {
            return this.liteEngine.Value().DropIndex(collection, name);
        }

        public bool EnsureIndex(string collection, string name, BsonExpression expression, bool unique)
        {
            return this.liteEngine.Value().EnsureIndex(collection, name, expression, unique);
        }

        public int Insert(string collection, IEnumerable<BsonDocument> docs, BsonAutoId autoId)
        {
            return this.liteEngine.Value().Insert(collection, docs, autoId);
        }

        public BsonValue Pragma(string name)
        {
            return this.liteEngine.Value().Pragma(name);
        }

        public bool Pragma(string name, BsonValue value)
        {
            return this.liteEngine.Value().Pragma(name, value);
        }

        public IBsonDataReader Query(string collection, Query query)
        {
            return this.liteEngine.Value().Query(collection, query);
        }

        public long Rebuild(RebuildOptions options)
        {
            return this.liteEngine.Value().Rebuild(options);
        }

        public bool RenameCollection(string name, string newName)
        {
            return this.liteEngine.Value().RenameCollection(name, newName);
        }

        public bool Rollback()
        {
            return this.liteEngine.Value().Rollback();
        }

        public int Update(string collection, IEnumerable<BsonDocument> docs)
        {
            return this.liteEngine.Value().Update(collection, docs);
        }

        public int UpdateMany(string collection, BsonExpression transform, BsonExpression predicate)
        {
            return this.liteEngine.Value().UpdateMany(collection, transform, predicate);
        }

        public int Upsert(string collection, IEnumerable<BsonDocument> docs, BsonAutoId autoId)
        {
            return this.liteEngine.Value().Upsert(collection, docs, autoId);
        }
    }
}
