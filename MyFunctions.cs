using QueryEngine;
using System;
namespace DoActions
{
    class MyFunctions
    {
        protected const UInt64 ROOT_FILE_REFERENCE_NUMBER = 0x5000000000005L;

        private static string BuildPath(FrnFilePath currentNode, FrnFilePath parentNode)
        {
            
            return  parentNode.path+"\\"+ tdsCshapu.Form1.getfile(currentNode.fileName);
        }

        /// <summary>
        /// GetPath从Diction中递归获取文件夹目录，返回string类型
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        public static string GetPathLink(int index, UInt64 key)
        {
           
            FrnFilePath currentValue;
            if (tdsCshapu.Form1.fileList[index].TryGetValue(key, out currentValue))
            {
                if (currentValue.parentFileReferenceNumber == ROOT_FILE_REFERENCE_NUMBER) { return ""; }


                currentValue.path = "";  //强制刷新所有路径
                if (string.IsNullOrWhiteSpace(currentValue.path)
                    && currentValue.parentFileReferenceNumber.HasValue
                    && tdsCshapu.Form1.fileList[index].ContainsKey(currentValue.parentFileReferenceNumber.Value))
                {
                    FrnFilePath parentValue = tdsCshapu.Form1.fileList[index][currentValue.parentFileReferenceNumber.Value];
                    parentValue.path = "";//强制刷新所有路径
                    while (string.IsNullOrWhiteSpace(parentValue.path)
                        && parentValue.parentFileReferenceNumber.HasValue
                        && tdsCshapu.Form1.fileList[index].ContainsKey(parentValue.parentFileReferenceNumber.Value))
                    {
                        currentValue = parentValue;

                        if (currentValue.parentFileReferenceNumber.HasValue
                            && tdsCshapu.Form1.fileList[index].ContainsKey(currentValue.parentFileReferenceNumber.Value))
                        {
                            //     treeWalkStack.Push(key);
                            parentValue = tdsCshapu.Form1.fileList[index][currentValue.parentFileReferenceNumber.Value];
                        }
                        else
                        {
                            parentValue = null;
                            break;
                        }
                    }

                    if (parentValue != null)
                    {

                        currentValue.path = BuildPath(currentValue, parentValue);

                     
                    }
                    else
                    {
                      
                        currentValue.path = string.Empty ;
                    }

                }

                return currentValue.path;

            }
            

            return "路径错误，目标可能已不存在.";

        }



    }
}