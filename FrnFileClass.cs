using CNChar;
using PInvoke;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UsnJournal;

namespace QueryEngine
{
    
   public class AdditionInfo
    {
        public  UInt64? parentFileReferenceNumber;
        public bool orderFirst=false;

        public AdditionInfo(ulong? parentFileRefNum)
        {
            parentFileReferenceNumber = parentFileRefNum;
        }
    }        
     
    public class FrnFileOrigin
    {
        public char VolumeName; //根目录名称
        public string FileName = "";

        public UInt64 fileReferenceNumber;

        public FrnFileOrigin parentFrn = null;

        public UInt64 keyindex;
        public int IcoIndex=-1;

        public AdditionInfo additionInfo;

        public static FrnFileOrigin Create(string filename, char vol, ulong fileRefNum, ulong? parentFileRefNum)
        {
            FrnFileOrigin f = new FrnFileOrigin(filename, vol, fileRefNum);

            f.additionInfo = new AdditionInfo(parentFileRefNum);

            return f;
        }

        private FrnFileOrigin(string filename,char vol,ulong fileRefNum)
        {            
            FileName = string.Intern(filename);
            VolumeName = vol;
            fileReferenceNumber = fileRefNum;
        }

    

        public void DisposeAdditionInfo()
        {
            additionInfo = null;
        }
       
   }


    public class FileSys
    {
        public static List<FrnFileOrigin> vlist = new List<FrnFileOrigin>();

        public const int SPLITFILENUMBER =100000;
        public DriveInfo driveInfo;
        public NtfsUsnJournal ntfsUsnJournal;
        public Dictionary<ulong, FrnFileOrigin> files=new Dictionary<ulong, FrnFileOrigin>();
        public Win32Api.USN_JOURNAL_DATA usnStates;
        public Collection<ArrayList> SplitedFiles = new Collection<ArrayList>();
        public FileSys(DriveInfo dInfo)
        {
            driveInfo = dInfo;
        }


        public void Compress()
        {            
            foreach(FrnFileOrigin f in files.Values)
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

        public void DoWhileFileChanges()  //筛选USN状态改变
        {
            if (usnStates.UsnJournalID != 0)
            {

                uint reasonMask = Win32Api.USN_REASON_FILE_CREATE | Win32Api.USN_REASON_FILE_DELETE | Win32Api.USN_REASON_RENAME_NEW_NAME;
                _ = ntfsUsnJournal.GetUsnJournalEntries(usnStates, reasonMask, out List<Win32Api.UsnEntry> usnEntries, out Win32Api.USN_JOURNAL_DATA newUsnState);


                foreach (Win32Api.UsnEntry f in usnEntries)
                {
                    uint value;
                    value = f.Reason & Win32Api.USN_REASON_RENAME_NEW_NAME;
                                       
                        if (0 != value && files.Count > 0)
                        {
                            if (files.ContainsKey(f.FileReferenceNumber) && files.ContainsKey(f.ParentFileReferenceNumber))
                            {
                                string nacn = SpellCN.GetSpellCode(f.Name.ToUpper());
                                FrnFileOrigin frn = files[f.FileReferenceNumber];
                                frn.keyindex = TBS(nacn);
                                if (!string.Equals(nacn, f.Name.ToUpper()))
                                {
                                    frn.FileName = "|" + f.Name + "|" + nacn + "|";
                                }
                                else
                                {
                                    frn.FileName = "|" + f.Name + "|";
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
                                string nacn = SpellCN.GetSpellCode(f.Name.ToUpper());
                                string name;
                                if (!string.Equals(nacn, f.Name.ToUpper()))
                                {
                                    name = "|" + f.Name + "|" + nacn + "|";
                                }
                                else
                                {
                                    name = "|" + f.Name + "|";
                                }

                                FrnFileOrigin frn = FrnFileOrigin.Create(name, driveInfo.Name[0], f.FileReferenceNumber, f.ParentFileReferenceNumber);
                                frn.keyindex = TBS(nacn);
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

       static readonly char[] alphbet = { '@', '.', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '-', '_', '[', ']', '(', ')', '/' };

        public static UInt64 TBS(string txt)
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
