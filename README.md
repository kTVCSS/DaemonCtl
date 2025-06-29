# DaemonCtl
## A little tool that autorestarts daemons from a appconfig written with dotnet
### How it works
The program gets daemon list from `daemonctl.json` and monitors their statuses by cmd `systemctl status {DaemonName}`. If it's not active, it tries to start daemon by similar cmd. The interval of checking is 1 min.
### How to use
1. Upload binaries to your host
2. Edit `daemonctl.json` config - it pretty simple, cuz you have to edit just daemon names that you want to control
3. Execute `chmod +x DaemonCtl`
4. Run it `./DaemonCtl`
