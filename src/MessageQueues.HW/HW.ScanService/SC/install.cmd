for %%A in (..\HW.ScanService.exe) DO set P=%%~fA
sc create HW.ScanService binPath="%P% -props:inputFolders=C:\winserv\inputs\1;C:\winserv\inputs\2|scanInterval=5000|logPath=C:\winserv\scanner.log|Endpoint=sb://epbygrow0257t3.grodno.epam.com/ServiceBusDefaultNamespace|StsEndpoint=https://epbygrow0257t3.grodno.epam.com:9355/ServiceBusDefaultNamespace|RuntimePort=9354|ManagementPort=9355"
