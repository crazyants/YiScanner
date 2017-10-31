# Yi Camera FTP Scanner and Video Clips Downloaded


## Monitoring and downloading latest clips

```
Wikiled.YiScanner Monitor -Cameras=1080i -Hosts=192.168.0.202 [-Compress] -Out=c:\out -Scan=10 [-Archive=2]
```

## Download once files, which haven't been downloaded yet

```
Wikiled.YiScanner Download -Cameras=1080i -Hosts=192.168.0.202 [-Compress] -Out=c:\out [-Archive=2]
```

Options:
- **Camera** - list of cameras
- **Hosts** - list of hosts. 
- **Compress** - do you want to compress files
- **Out** - location of downloaded files
- **Scan** - frequency of FTP scan (in seconds)
- **Archive** - delete previously downloaded old files. Number specifies how many days you want to keep history


# FTP configuration 
FTP configuration can be modified in file **appsettings.json**