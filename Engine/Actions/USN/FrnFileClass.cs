
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using TDSNET.Engine.Utils;

namespace TDSNET.Engine.Actions.USN
{

    public class AdditionInfo : IDisposable
    {
        public ulong? parentFileReferenceNumber;
        public bool orderFirst = false;

        public AdditionInfo(ulong? parentFileRefNum)
        {
            parentFileReferenceNumber = parentFileRefNum;
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    parentFileReferenceNumber = null;
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~AdditionInfo()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class FrnFileOrigin
    {
        public char VolumeName; //根目录名称

        public string fileName = "";

        public ulong fileReferenceNumber;

        public FrnFileOrigin parentFrn = null;

        public ulong keyindex;
        public int IcoIndex = -1;

        public AdditionInfo additionInfo;

        public static FrnFileOrigin Create(string filename, char vol, ulong fileRefNum, ulong? parentFileRefNum)
        {
            FrnFileOrigin f = new FrnFileOrigin(filename, vol, fileRefNum);

            f.additionInfo = new AdditionInfo(parentFileRefNum);

            return f;
        }

        private FrnFileOrigin(string filename, char vol, ulong fileRefNum)
        {
            fileName = string.Intern(filename);
            VolumeName = vol;
            fileReferenceNumber = fileRefNum;
        }



        public void DisposeAdditionInfo()
        {
            additionInfo.Dispose();
            additionInfo = null;
        }

    }


    public class FileSys
    {
        public static List<FrnFileOrigin> vlist = new List<FrnFileOrigin>();

        public const int SPLITFILENUMBER = 100000;
        public DriveInfo driveInfo;
        public NtfsUsnJournal ntfsUsnJournal;
        public Dictionary<ulong, FrnFileOrigin> files = new Dictionary<ulong, FrnFileOrigin>();
        public Win32Api.USN_JOURNAL_DATA usnStates;
        public Collection<ArrayList> SplitedFiles = new Collection<ArrayList>();
        public FileSys(DriveInfo dInfo)
        {
            driveInfo = dInfo;
        }


        public void Compress()
        {
            foreach (FrnFileOrigin f in files.Values)
            {
                f.DisposeAdditionInfo();
            }

            files.TrimExcess();
        }

        /// <summary>
        /// 查询并跟踪USN状态，更新后保存当前状态再继续跟踪
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool SaveJournalState()        //保存USN状态
        {

            Win32Api.USN_JOURNAL_DATA journalState = new Win32Api.USN_JOURNAL_DATA();
            NtfsUsnJournal.UsnJournalReturnCode rtn = ntfsUsnJournal.GetUsnJournalState(ref journalState);
            if (rtn == NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS)
            {
                usnStates = journalState;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 掩码
        /// </summary>
        uint reasonMask = Win32Api.USN_REASON_FILE_CREATE | Win32Api.USN_REASON_FILE_DELETE | Win32Api.USN_REASON_RENAME_NEW_NAME;

        public void DoWhileFileChanges()  //筛选USN状态改变
        {
            if (usnStates.UsnJournalID != 0)
            {


                _ = ntfsUsnJournal.GetUsnJournalEntries(usnStates, reasonMask, out List<Win32Api.UsnEntry> usnEntries, out Win32Api.USN_JOURNAL_DATA newUsnState);

                for (int i = 0; i < usnEntries.Count; i++)
                {
                    var f = usnEntries[i];
                    uint value = f.Reason & Win32Api.USN_REASON_RENAME_NEW_NAME;

                    if (0 != value && files.Count > 0)
                    {
                        if (files.ContainsKey(f.FileReferenceNumber) && files.ContainsKey(f.ParentFileReferenceNumber))
                        {
                            string nacn = SpellCN.GetSpellCode(f.Name.AsSpan());
                            FrnFileOrigin frn = files[f.FileReferenceNumber];
                            frn.keyindex = TBS(nacn.AsSpan());
                            if (!string.Equals(nacn, f.Name.ToUpperInvariant()))
                            {
                                frn.fileName = "|" + f.Name + "|" + nacn + "|";
                            }
                            else
                            {
                                frn.fileName = "|" + f.Name + "|";
                            }
                            frn.parentFrn = files[f.ParentFileReferenceNumber];
                            files[f.FileReferenceNumber] = frn;
                        }
                    }


                    value = f.Reason & Win32Api.USN_REASON_FILE_CREATE;
                    if (0 != value)
                    {
                        if (!files.ContainsKey(f.FileReferenceNumber) && !string.IsNullOrWhiteSpace(f.Name) && files.ContainsKey(f.ParentFileReferenceNumber))
                        {
                            string nacn = SpellCN.GetSpellCode(f.Name);
                            string name;
                            if (!string.Equals(nacn, f.Name.ToUpperInvariant()))
                            {
                                name = "|" + f.Name + "|" + nacn + "|";
                            }
                            else
                            {
                                name = "|" + f.Name + "|";
                            }

                            FrnFileOrigin frn = FrnFileOrigin.Create(name, driveInfo.Name[0], f.FileReferenceNumber, f.ParentFileReferenceNumber);
                            frn.keyindex = TBS(nacn.AsSpan());
                            frn.parentFrn = files[f.ParentFileReferenceNumber];
                            files.Add(frn.fileReferenceNumber, frn);
                            vlist.Add(null);
                        }
                    }

                    value = f.Reason & Win32Api.USN_REASON_FILE_DELETE;
                    if (0 != value && files.Count > 0)
                    {
                        if (files.ContainsKey(f.FileReferenceNumber))
                        {
                            files.Remove(f.FileReferenceNumber);
                        }
                    }
                    usnStates = newUsnState;   //更新状态                    
                }
            }
        }

        public void CreateFiles()
        {
            ntfsUsnJournal.GetNtfsVolumeAllentries(driveInfo.Name[0], out NtfsUsnJournal.UsnJournalReturnCode rtnCode, this);
        }

        const char POSITIVE = '1';

        const char NEGATIVE = '0';

        const int SCREENCHARNUM = 45;

        static readonly char[] alphbet = { '@', '.', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '-', '_', '[', ']', '(', ')', '/'};

        public static ulong TBS(ReadOnlySpan<char> txt)
        {
            char[] alph = new char[SCREENCHARNUM];

            for (int i = 0; i < SCREENCHARNUM; i++)
            {
                if (txt.Contains(alphbet[i])) { alph[i] = POSITIVE; } else { alph[i] = NEGATIVE; }
            }
            return Convert.ToUInt64(new string(alph), 2);
        }
    }




}
