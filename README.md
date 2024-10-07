### CheckSEP v1.0
CheckSEP is a NRPE plugin that allows you to check the status of the 
**Symantec EndPoint Protection** virus definitions on Windows Server. 
It can be used in Nagios or any other monitoring tool that supports Nagios 
plugins like LibreNMS with "check_nrpe" and NSClient++ installed on the 
Windows Server.  
The code is based on "check_av.vbs" script by Matt White and has been 
rewritten to .NET 8 with small enhancements.

#### Requirements
- .NET 8 Runtime installed on the Windows Server

#### Installation
1. Compile the code or use the already compiled version (CheckSEP v1.0.zip).  
2. Copy the compiled executables to the NSClient++ "scripts/CheckSEP" folder  
Usually it should be at:  
```C:\Program Files\NSClient++\scripts\CheckSEP```
3. Add the following line to the nsclient.ini file under
[/settings/external scripts/scripts]:  
```
check_sep = scripts/CheckSEP/CheckSEP.exe -w:$ARG1$ -c:$ARG2$
```
4. Restart the NSClient++ service.
5. You can test it with check_nrpe:
```
./check_nrpe -H <hostname> -2 -c check_sep -a 2 3
```
6. Add the check command to your monitoring tool accordingly.

### Credits
Matt White (for check_av.vbs)  
Łukasz Chryk
