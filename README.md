# DllVersionScanner

This tool get arguments version , days , path 
and search for DLL files . 
Create output Text fiel with found DLL file list in the following condissions :
1. DLL invalid by Version and Days if DLL version is higer than in given argument and Creation Day is older then Days in given argument.
2. DLL invalid  only by Version
3. DLL invalid only by Cretation Day
4. Valid DLL have Version lower then in given argument and Creted in a last X days ( X as an argument)



DllVersionScanner.exe 

DllVersionScanner 1.0.0
Copyright (C) 2023 DllVersionScanner


  Required option 'v, version' is missing.
  Required option 'a, age' is missing.
  Required option 'p, path' is missing.

  -v, --version    Required. Input DLL version for validation.

  -a, --age        Required. Input how old DLL file can be in days.

  -p, --path       Required. Input path or multiple pathes separeted by comma .

  --help           Display this help screen.

  --version        Display version information.
