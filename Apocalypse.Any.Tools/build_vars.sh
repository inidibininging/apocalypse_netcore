#!/bin/bash
# rsync built content with the executable
cd ..

APOCALYPSE_NETCORE_FRAMEWORK=net5.0

APOCALYPSE_NETCORE_DIR=$(pwd)
echo 'apocalypse_netcore_dir:' $APOCALYPSE_NETCORE_DIR

APOCALYPSE_NETCORE_TOOLS_DIR=$APOCALYPSE_NETCORE_DIR/Apocalypse.Any.Tools/
echo 'tools:' $APOCALYPSE_NETCORE_TOOLS_DIR

CONTENT_DIR=$APOCALYPSE_NETCORE_DIR/Apocalypse.Any.Client/Content
echo 'content_dir:' $APOCALYPSE_NETCORE_DIR

OUTPUT_DIR=$CONTENT_DIR/bin/DesktopGL/Apocalypse.Any.Client/Content
echo 'output client dir:' $APOCALYPSE_NETCORE_DIR

DEST_DIR=$APOCALYPSE_NETCORE_DIR/Apocalypse.Any.Client/bin/Debug/$APOCALYPSE_NETCORE_FRAMEWORK/Content/

echo 'destination dir:' $APOCALYPSE_NETCORE_DIR

