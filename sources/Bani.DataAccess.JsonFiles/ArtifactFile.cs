// Bani
// Copyright (C) 2022 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using Newtonsoft.Json;

namespace DustInTheWind.Bani.DataAccess.JsonFiles;

public class ArtifactFile<T>
    where T : JArtifact
{
    public string FilePath { get; }

    public T Artifact { get; set; }

    public ArtifactFile(string filePath)
    {
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public void Open()
    {
        string json = File.ReadAllText(FilePath);
        Artifact = JsonConvert.DeserializeObject<T>(json);

        if (Artifact != null)
            Artifact.Location = FilePath;
    }
}