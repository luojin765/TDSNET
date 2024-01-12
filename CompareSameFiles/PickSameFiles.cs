using DocumentFormat.OpenXml.Drawing.Diagrams;
using FileSync;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TDSNET.Engine.Actions.USN;
using TDSNET.Engine.Utils;

namespace TDSNET.CompareSameFiles
{
    internal class PickSameFiles
    {

        internal class SameFileInfo
        {
            public class FrnFileWithPath
            {
                public string Path;
                public FrnFileOrigin Origin;

                public FrnFileWithPath(string path, FrnFileOrigin origin)
                {
                    Path = path;
                    Origin = origin;
                }
            }

            public long Size;
            readonly public IList<FrnFileWithPath> FileList;

            public SameFileInfo(long size)
            {
                Size = size;
                FileList = new List<FrnFileWithPath>();
            }

            internal void Add(FrnFileOrigin file,string path)
            {
                FileList.Add(new FrnFileWithPath(path, file));
            }
        }

      
        internal static ConcurrentBag<SameFileInfo> Comparer(IList<FileSys> fileSystems,string formatFilter,long minimalSize,Action<string> action)
        {
            if (minimalSize < 1)
            {
                minimalSize = 1024;
            }

            formatFilter =formatFilter.TrimStart('.');
            var filters = formatFilter.Replace(" ", "").Split('|');
            for(int k = 0; k < filters.Length; k++)
            {
                filters[k] = '.' + filters[k].TrimStart('.');
            }

            ConcurrentDictionary<long, ConcurrentBag<FrnFileOrigin>> sameFile = new();
            int currentFileCount = 0;
            int total = fileSystems.Sum(o => o.files.Count);

            Parallel.ForEach(fileSystems, fileSys =>
            { 
                    Parallel.ForEach(fileSys.files.Values, file =>
                        {
                            Interlocked.Increment(ref currentFileCount);

                            var filePath = GetPath(file);
                            long fileSize = 0;

                            var extension = Path.GetExtension(filePath.ToString());
                            foreach (var filter in filters)
                            {
                                if (string.Equals(extension, filter, StringComparison.OrdinalIgnoreCase))
                                {
                                    try
                                    {
                                        if (File.Exists(filePath.ToString()))
                                        {
                                            fileSize = new FileInfo(filePath.ToString()).Length;
                                        }
                                    }
                                    catch
                                    {
                                    }
                                    break;
                                }
                            }


                            if (fileSize < minimalSize) return;

                            if (sameFile.ContainsKey(fileSize))
                            {
                              sameFile[fileSize].Add(file);
                            }
                            else
                            {
                                sameFile.TryAdd(fileSize, new ConcurrentBag<FrnFileOrigin> { file });
                            }


                            if (currentFileCount % 100 == 0)
                            {
                                action?.Invoke((currentFileCount/(double)total*100).ToString("F2")+"%");
                                //processMessage.Text = currentFileCount.ToString();
                            }
                        });
            });
            action?.Invoke("100%");

            ConcurrentBag<SameFileInfo> sameFiles = new ConcurrentBag<SameFileInfo>();
            action?.Invoke("分析中");

            //Parallel.ForEach(sameFile, sf =>
            foreach (var sf in sameFile)
            {
                if (sf.Value.Count < 2)
                {
                    sameFile.TryRemove(sf.Key,out var val);
                }
                else
                {
                    foreach(var f in CompareFilesByTwoTwo(sf.Key,sf.Value.ToArray()))
                    {
                        sameFiles.Add(f);
                    }
                }
            }
            //);

            action?.Invoke("完成");
            return sameFiles;
        }
      
        static IList<SameFileInfo> CompareFilesByTwoTwo(long size,IList<FrnFileOrigin> files)
        {

            List<SameFileInfo> sameFiles = new List<SameFileInfo>(files.Count);

            for (int i = 0; i < files.Count; i++)
            {
                
                if (files[i] == null)
                {
                    continue;
                }

                try
                {
                    var path1 = GetPath(files[i]).ToString();
           
                    using var fs1 = new FileStream(path1, FileMode.Open);

                    var samplefile = new SameFileInfo(size);
                    samplefile.Add(files[i], path1);

                    for (int j = i + 1; j < files.Count; j++)
                    {
                        if(files[j] == null)
                        {
                            continue;
                        }
                        
                        var path2 = GetPath(files[j]).ToString();

                        if (files[i] == files[j] || path1 == path2)
                        {
                            files[j] = null;
                            continue;
                        }

                        try
                        {
                            using var fs2 = new FileStream(path2.ToString(), FileMode.Open);
                            var task = Task.Run(()=>FileCompare.CompareByReadOnlySpan(fs1, fs2)).GetAwaiter().GetResult();
                            if (task)
                            {
                                samplefile.Add(files[j], path2);
                                files[j] = null;
                            }
                        }
                        catch (Exception)
                        {
                            files[j] = null;
                        }
                    }
                    sameFiles.Add(samplefile);
                }
                catch (Exception)
                {
                    files[i] = null;
                }
            }

            for (int i= sameFiles.Count-1; i>=0; i--)
            {
                if (sameFiles[i].FileList.Count <= 1)
                {
                    sameFiles.RemoveAt(i);
                }
            }

            return sameFiles;

        }

        private static string getMD5ByHashAlgorithm(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException(string.Format("<{0}>, 不存在", path));
            int bufferSize = 1024 * 16;//自定义缓冲区大小16K
            byte[] buffer = new byte[bufferSize];
            Stream inputStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            HashAlgorithm hashAlgorithm = MD5.Create();
            int readLength = 0;//每次读取长度
            var output = new byte[bufferSize];
            while ((readLength = inputStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                //计算MD5
                hashAlgorithm.TransformBlock(buffer, 0, readLength, output, 0);
            }
            //完成最后计算，必须调用(由于上一部循环已经完成所有运算，所以调用此方法时后面的两个参数都为0)
            hashAlgorithm.TransformFinalBlock(buffer, 0, 0);
            string md5 = BitConverter.ToString(hashAlgorithm.Hash);
            hashAlgorithm.Clear();
            inputStream.Close();
            md5 = md5.Replace("-", "");
            return md5;
        }

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
