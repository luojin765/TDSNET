# TDSNET - A fast file search program with UI on Win
---
1. filename match in less than 30ms from 3 million files on my laptop.
2. supports multi key words search seperated by space.
3. fuzzy match with first letter of chinese Pinyin.
4. simple UI with all files illustrated and supports select, drag etc.
5. simple hotkey: "esc key" return, selected all, clear input; "space key" open the file directory.
6. UI theme, transparency, wake up hotkey and other settings could be modified in the setting.ini file.
7. App icon coubld be found in the right bottom windows notification area after program start, and coubld be wake up by double click or wake up hotkey  (ctrl + . or ctrl + ~ in default).
---
# A search example:

"d,c: hb\ a |12 .pdf|"

list the the files start with "12", end with ".pdf", contains the string "a", located in disks d: or c:, under the directory with the directoryName contains string "hb"

--- 

The search process is based on USN and supports windows os only (e.g. win10 win11).

The program was written using 100% csharp language with no spacial extern libs.

The program was written with dotNet 8, Winform (net framework could also be supported with little modification since it was once upgraded from net framework 4.7.2).

All fileinfo was cached in memory, and would be updated automatically.

