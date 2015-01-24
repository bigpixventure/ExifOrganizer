﻿//
// DirectoryParser.cs: Directory meta data parser class.
//
// Copyright (C) 2014 Rikard Johansson
//
// This program is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option) any
// later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// this program. If not, see http://www.gnu.org/licenses/.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ExifOrganizer.Meta.Parsers
{
    internal class DirectoryParser : Parser
    {
        public static MetaData Parse(string path)
        {
            Task<MetaData> task = ParseAsync(path);
            task.ConfigureAwait(false); // Prevent deadlock of caller
            return task.Result;
        }

        public static async Task<MetaData> ParseAsync(string path)
        {
            return await Task.Run(() => ParseThread(path));
        }

        private static MetaData ParseThread(string path)
        {
            if (!Directory.Exists(path))
                throw new MetaParseException("Directory not found: {0}", path);

            MetaData meta = new MetaData();
            meta.Type = MetaType.Directory;
            meta.Path = path;
            //meta.Data = new Dictionary<MetaKey, object>();
            //meta.Data[MetaKey.FileName] = Path.GetFileName(path);
            //meta.Data[MetaKey.OriginalName] = meta.Data[MetaKey.FileName];
            //meta.Data[MetaKey.Size] = GetFileSize(path);
            //meta.Data[MetaKey.Date] = File.GetCreationTime(path);
            //meta.Data[MetaKey.Tags] = new string[0];
            return meta;
        }
    }
}
