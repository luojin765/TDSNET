using FileSync;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TDSNET.Engine.Actions.USN;
using TDSNET.Engine.Utils;

namespace TDSNET.Utils
{
    internal class PickSameFiles
    {
        class SameFileInfo
        {
            public long Size;
            readonly public IList<string> PathList=new List<string>();
        }

        internal static void Comparer(IList<FileSys> fileSystems,string formatFilter,long minimalSize)
        {


            ConcurrentDictionary<long, IList<FrnFileOrigin>> sameFile = new();
            int i = 0;

            Parallel.ForEach(fileSystems, fileSys =>
            {
                Parallel.ForEach(fileSys.files.Values, file =>
                {
                    var filePath = GetPath(file);
                    long fileSize = 0;

                    if (string.Equals(Path.GetExtension(filePath.ToString()),formatFilter, StringComparison.OrdinalIgnoreCase)
                    && File.Exists(filePath.ToString()))
                    {
                        try
                        {
                            fileSize = new FileInfo(filePath.ToString()).Length;
                        }
                        catch
                        {

                        }
                    }

                    if (fileSize < minimalSize) return;

                    if (sameFile.ContainsKey(fileSize))
                    {
                        sameFile[fileSize].Add(file);
                    }
                    else
                    {
                        sameFile.TryAdd(fileSize, new List<FrnFileOrigin> { file });
                    }
                    Interlocked.Increment(ref i);
                    if (i % 10000 == 0) Debug.WriteLine(i);
                });
            });

            Console.WriteLine("done");

            Parallel.ForEach(sameFile, sf =>
            {
                if (sf.Value.Count > 1)
                {
                    sameFile.TryRemove(sf.Key,out var val);
                }
            });
            Console.WriteLine("clean");


        }

        static IList<SameFileInfo> CompareFiles(long size, IList<FrnFileOrigin> files)
        {
            
        }
        static IList<SameFileInfo> CompareFilesByTwoTwo(IList<FrnFileOrigin> files)
        {           

            List<SameFileInfo> sameFiles = new List<SameFileInfo>(files.Count);

            int currentIndex = 0;

            for(int i=0;i<files.Count;i++)
            {
                
                if (files[i] == null)
                {
                    continue;
                }

                sameFiles.Add(new SameFileInfo());

                for (int j = 0; j < files.Count; j++)
                {
                    if (files[j] == null || files[i] == files[j])
                    {
                        continue;
                    }

                    SameFileInfo


                }
            }
        }
        //static IList<SameFileInfo> CompareFilesByMD5(IList<FrnFileOrigin> files)
        //{

        //}



        static ReadOnlySpan<char> GetPath(FrnFileOrigin f)
        {
            var path = StringUtils.GetPathStr(f, ReadOnlySpan<char>.Empty);
            if (path.EndsWith(":".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                var pathChar = new char[path.Length + 1];
                Array.Copy(path.ToArray(), pathChar, path.Length);
                pathChar[pathChar.Length - 1] = '\\';
                return pathChar.AsSpan();
            }
            else
            {
                return path;
            }
        }
    }
}
