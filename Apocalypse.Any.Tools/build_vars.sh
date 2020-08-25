#!/bin/bash
# rsync built content with the executable
cd ..
APOCALYPSE_NETCORE_DIR=$(pwd)
echo 'apocalypse_netcore_dir:' $APOCALYPSE_NETCORE_DIR
CONTENT_DIR=$APOCALYPSE_NETCORE_DIR/Apocalypse.Any.Client/Content
OUTPUT_DIR=$CONTENT_DIR/bin/DesktopGL/Apocalypse.Any.Client/Content
DEST_DIR=$APOCALYPSE_NETCORE_DIR/Apocalypse.Any.Client/bin/Debug/netcoreapp2.1/Content/