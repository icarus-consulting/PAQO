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
using System.IO;
using Yaapii.Atoms;
using Yaapii.Atoms.Scalar;

namespace PAQO.Memory.LiteDB.Facets
{
    /// <summary>
    /// Envelope for LiteDatabase (sticky).
    /// </summary>
    public abstract class LiteDatabaseEnvelope : ILiteDatabase
    {
        private readonly IScalar<ILiteDatabase> origin;

        /// <summary>
        /// Envelope for LiteDatabase (sticky).
        /// </summary>
        public LiteDatabaseEnvelope(Func<ILiteDatabase> origin)
        {
            this.origin = new ScalarOf<ILiteDatabase>(origin);
        }

        public BsonMapper Mapper => this.origin.Value().Mapper;

        public ILiteStorage<string> FileStorage => this.origin.Value().FileStorage;

        public int UserVersion { get => this.origin.Value().UserVersion; set => this.origin.Value().UserVersion = value; }
        public TimeSpan Timeout { get => this.origin.Value().Timeout; set => this.origin.Value().Timeout = value; }
        public bool UtcDate { get => this.origin.Value().UtcDate; set => this.origin.Value().UtcDate = value; }
        public long LimitSize { get => this.origin.Value().LimitSize; set => this.origin.Value().LimitSize = value; }
        public int CheckpointSize { get => this.origin.Value().CheckpointSize; set => this.origin.Value().CheckpointSize = value; }

        public Collation Collation => this.origin.Value().Collation;

        public bool BeginTrans()
        {
            return this.origin.Value().BeginTrans();
        }

        public void Checkpoint()
        {
            this.origin.Value().Checkpoint();
        }

        public bool CollectionExists(string name)
        {
            return this.origin.Value().CollectionExists(name);
        }

        public bool Commit()
        {
            return this.origin.Value().Commit();
        }

        public void Dispose()
        {
            this.origin.Value().Dispose();
        }

        public bool DropCollection(string name)
        {
            return this.origin.Value().DropCollection(name);
        }

        public IBsonDataReader Execute(TextReader commandReader, BsonDocument parameters = null)
        {
            return this.origin.Value().Execute(commandReader, parameters);
        }

        public IBsonDataReader Execute(string command, BsonDocument parameters = null)
        {
            return this.origin.Value().Execute(command, parameters);
        }

        public IBsonDataReader Execute(string command, params BsonValue[] args)
        {
            return this.origin.Value().Execute(command, args);
        }

        public ILiteCollection<T> GetCollection<T>(string name, BsonAutoId autoId = BsonAutoId.ObjectId)
        {
            return this.origin.Value().GetCollection<T>(name, autoId);
        }

        public ILiteCollection<T> GetCollection<T>()
        {
            return this.origin.Value().GetCollection<T>();
        }

        public ILiteCollection<T> GetCollection<T>(BsonAutoId autoId)
        {
            return this.origin.Value().GetCollection<T>(autoId);
        }

        public ILiteCollection<BsonDocument> GetCollection(string name, BsonAutoId autoId = BsonAutoId.ObjectId)
        {
            return this.origin.Value().GetCollection(name, autoId);
        }

        public IEnumerable<string> GetCollectionNames()
        {
            return this.origin.Value().GetCollectionNames();
        }

        public ILiteStorage<TFileId> GetStorage<TFileId>(string filesCollection = "_files", string chunksCollection = "_chunks")
        {
            return this.origin.Value().GetStorage<TFileId>(filesCollection, chunksCollection);
        }

        public BsonValue Pragma(string name)
        {
            return this.origin.Value().Pragma(name);
        }

        public BsonValue Pragma(string name, BsonValue value)
        {
            return this.origin.Value().Pragma(name);
        }

        public long Rebuild(RebuildOptions options = null)
        {
            return this.origin.Value().Rebuild(options);
        }

        public bool RenameCollection(string oldName, string newName)
        {
            return this.origin.Value().RenameCollection(oldName, newName);
        }

        public bool Rollback()
        {
            return this.origin.Value().Rollback();
        }
    }
}
