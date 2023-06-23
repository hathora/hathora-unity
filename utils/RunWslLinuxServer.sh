#!/bin/bash
##################################################################################
# ABOUT: Creates 1 LinuxServer (via bash shell) for Hathora-Unity
##################################################################################
# ARG LIST: https://docs.unity3d.com/Manual/PlayerCommandLineArguments.html 
# -batchmode is useful, but it's hard to know when to -quit (or need to end task)
# -nographics (combined with -batchmode) runs as headless dedicated server
##################################################################################
# HATHORA-UNITY CMDS
# -mode {server|client|host} // host == both client *and* server
# -memo {str} // If your host is !headless, this can add a memo. Eg: "Server".
# -scene {sceneName} // HathoraDemoScene-FishNet
#                    //	HathoraDemoScene-Mirror
#		     //	HathoraDemoScene-Menu // (!) Don't use menu for servers
#
# -> See HathoraArgHandlerBase.cs
##################################################################################
# Clear the console
clear

# print the output of the last command
echo "Starting dedicated server: $(./RevealWslVmIp.sh)"
echo "-----------------------------"
echo ""
##################################################################################
# START LINUX SERVER (from Windows -> via wsl2)
##################################################################################
exe_name="Hathora-Unity-LinuxServer.x86_64"
path_to_linux_server="../src/Build-Server/$exe_name"
echo "Starting $exe_name instance at:"
echo "$path_to_linux_server"

scene_name="HathoraDemoScene-FishNet"
unity_args="-batchmode -nographics"
hathora_args="-mode server -scene $scene_name"
args="$unity_args $hathora_args"

linux_cmd="$path_to_linux_server $args"
echo ""
echo "unity_args: $unity_args"
echo "hathora_args: $hathora_args"
echo "--------------------------"
echo ""

$linux_cmd
