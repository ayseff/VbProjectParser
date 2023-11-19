﻿using OpenMcdf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data._PROJECTINFORMATION;
using VbProjectParserCore.Data._PROJECTMODULES._MODULE;

namespace VbProjectParserCore.Data
{
    public class VbaStorage : IDisposable
    {
        private IDisposable m_disposable;

        public _VBA_PROJECTStream _VBA_PROJECTStream { get; private set; }
        public ProjectStream ProjectStream { get; private set; }

        public readonly DirStream DirStream;

        /// <summary>
        /// Key: Name of the stream
        /// Value: Stream
        /// </summary>
        public readonly IReadOnlyDictionary<string, ModuleStream> ModuleStreams;/*
        {
            get
            {
                return new ReadOnlyDictionary<string, ModuleStream>(this._ModuleStreams);
            }
        }*/
        protected readonly IDictionary<string, ModuleStream> _ModuleStreams;

        public VbaStorage(CompoundFile VbaBinFile)
        {
            m_disposable = VbaBinFile;

            // _VBA_PROJECT stream
            var VBAStorage = VbaBinFile.RootStorage.GetStorage("VBA");
            _VBA_PROJECTStream = ReadVbaProjectStream(VBAStorage);

            // DIR STREAM -------------------------
            CFStream thisWorkbookStream = VBAStorage.GetStream("dir");
            byte[] compressedData = thisWorkbookStream.GetData();
            byte[] uncompressed = XlCompressionAlgorithm.Decompress(compressedData);

            var uncompressedDataReader = new XlBinaryReader(ref uncompressed);
            DirStream = new DirStream(uncompressedDataReader);

            // MODULE STREAMS ----------------------------------------
            _ModuleStreams = new Dictionary<string, ModuleStream>(DirStream.ModulesRecord.Modules.Length);
            ModuleStreams = new ReadOnlyDictionary<string, ModuleStream>(_ModuleStreams);

            foreach (var module in DirStream.ModulesRecord.Modules)
            {
                var streamName = module.StreamNameRecord.GetStreamNameAsString();
                var stream = VBAStorage.GetStream(streamName).GetData();
                var localreader = new XlBinaryReader(ref stream);

                var moduleStream = new ModuleStream(DirStream.InformationRecord, module, localreader);

                _ModuleStreams.Add(streamName, moduleStream);
            }

            // PROJECT stream
            CFStream ProjectStorage = VbaBinFile.RootStorage.GetStream("PROJECT");
            ProjectStream = ReadProjectStream(ProjectStorage, DirStream.InformationRecord.CodePageRecord);
        }

        private _VBA_PROJECTStream ReadVbaProjectStream(CFStorage VBAStorage)
        {
            CFStream stream = VBAStorage.GetStream("_VBA_PROJECT");
            byte[] uncompressedData = stream.GetData();

            var dataReader = new XlBinaryReader(ref uncompressedData);
            var result = new _VBA_PROJECTStream(dataReader);
            return result;
        }

        private ProjectStream ReadProjectStream(CFStream PROJECTStorage, PROJECTCODEPAGE Codepage)
        {/*
            var y = new List<string>();
            PROJECTStorage.VisitEntries(x => y.Add(x.Name), false);*/
            byte[] uncompressedData = PROJECTStorage.GetData();

            var stream = new ProjectStream(Codepage, uncompressedData);
            return stream;
        }

        internal VbaStorage(IDictionary<string, ModuleStream> ModuleStreams, DirStream DirStream)
        {
            _ModuleStreams = ModuleStreams;
            this.ModuleStreams = new ReadOnlyDictionary<string, ModuleStream>(ModuleStreams);
        }


        public void Add(MODULE module, string SourceCode)
        {
            string moduleName = module.NameRecord.GetModuleNameAsString();
            string streamName = moduleName;

            if (_ModuleStreams.ContainsKey(streamName))
                throw new ArgumentException($"Already contains a module stream named {streamName}", "moduleStream");

            var stream = new ModuleStream(DirStream.InformationRecord, module, SourceCode);
            throw new NotImplementedException();
            /*
            this._ModuleStreams.Add(streamName, moduleStream);
            ModuleStreamAdded(streamName, moduleStream);*/
        }

        // Called when an item has been added to this._ModuleStreams
        private void ModuleStreamAdded(string streamName, ModuleStream moduleStream)
        {
            if (DirStream == null)
                throw new NullReferenceException("DirStream was null");

            ++DirStream.ModulesRecord.Count;

        }

        public virtual void Dispose()
        {
            if (m_disposable != null)
                m_disposable.Dispose();
        }

        public VbaStorage(string PathToVbaBinFile)
            : this(new CompoundFile(PathToVbaBinFile))
        {
        }

        public VbaStorage(Stream VbaBinFileStream)
            : this(new CompoundFile(VbaBinFileStream))
        {
        }
    }
}
